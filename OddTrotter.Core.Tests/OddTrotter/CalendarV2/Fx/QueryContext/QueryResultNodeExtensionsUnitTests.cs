namespace Fx.QueryContext
{
    using System;

    using Fx.Either;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class QueryResultNodeExtensionsUnitTests
    {
        [TestMethod]
        public void ToQueryResultNodeNullNode()
        {
            IEither<IElement<string, Exception>, IEither<IError<Exception>, IEmpty>> either =
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                null
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                ;

            Assert.ThrowsException<ArgumentNullException>(() =>
#pragma warning disable CS8604 // Possible null reference argument.
                either
#pragma warning restore CS8604 // Possible null reference argument.
                .ToQueryResultNode());
        }

        [TestMethod]
        public void ToQueryResultNode()
        {
            var value = "asdf";
            var node = Either.Left(new MockElement(value)).Right<IEither<IError<Exception>, IEmpty>>().ToQueryResultNode();

            var result = node.Apply(
                (element, context) => string.Concat(element.Value, element.Value),
                (terminal, context) => terminal.Apply(
                    (error, context) => error.Value.Message,
                    (empty, context) => string.Empty,
                    new Nothing()),
                new Nothing());

            Assert.AreEqual(value + value, result);

            node = Either.Left<MockElement>().Right(Either.Left(new MockError(new Exception(value))).Right<IEmpty>()).ToQueryResultNode();

            result = node.Apply(
                (element, context) => string.Concat(element.Value, element.Value),
                (terminal, context) => terminal.Apply(
                    (error, context) => error.Value.Message,
                    (empty, context) => string.Empty,
                    new Nothing()),
                new Nothing());

            Assert.AreEqual(value, result);

            node = Either.Left<MockElement>().Right(Either.Left<MockError>().Right(MockEmpty.Instance)).ToQueryResultNode();

            result = node.Apply(
                (element, context) => string.Concat(element.Value, element.Value),
                (terminal, context) => terminal.Apply(
                    (error, context) => error.Value.Message,
                    (empty, context) => string.Empty,
                    new Nothing()),
                new Nothing());

            Assert.AreEqual(string.Empty, result);
        }

        private sealed class MockElement : IElement<string, Exception>
        {
            /// <summary>
            /// placeholder
            /// </summary>
            /// <param name="value"></param>
            public MockElement(string value)
            {
                this.Value = value;
            }

            /// <inheritdoc/>
            public string Value { get; }

            /// <inheritdoc/>
            public IQueryResultNode<string, Exception> Next()
            {
                return
                    Either
                        .Left<IElement<string, Exception>>()
                        .Right(
                            Either
                                .Left<IError<Exception>>()
                                .Right(
                                    MockEmpty.Instance))
                        .ToQueryResultNode();
            }
        }

        private sealed class MockEmpty : IEmpty
        {
            /// <summary>
            /// placeholder
            /// </summary>
            private MockEmpty()
            {
            }

            /// <summary>
            /// placeholder
            /// </summary>
            public static MockEmpty Instance { get; } = new MockEmpty();
        }

        private sealed class MockError : IError<Exception>
        {
            /// <summary>
            /// placeholder
            /// </summary>
            /// <param name="value"></param>
            public MockError(Exception value)
            {
                this.Value = value;
            }

            /// <inheritdoc/>
            public Exception Value { get; }
        }

        [TestMethod]
        public void WhereNullSource()
        {
            IQueryResultNode<string, Exception> node =
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                null
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                ;

            Assert.ThrowsException<ArgumentNullException>(() =>
#pragma warning disable CS8604 // Possible null reference argument.
                node
#pragma warning restore CS8604 // Possible null reference argument.
                .Where(val => val.Length % 2 == 0));
        }

        [TestMethod]
        public void WhereNullPredicate()
        {
            var value = "asdf";
            var node = Either.Left(new MockElement(value)).Right<IEither<IError<Exception>, IEmpty>>().ToQueryResultNode();

            Assert.ThrowsException<ArgumentNullException>(() => node.Where(
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                ));
        }

        [TestMethod]
        public void WhereNoElements()
        {
            var node = Either.Left<MockElement>().Right(Either.Left<MockError>().Right(MockEmpty.Instance)).ToQueryResultNode();

            var result = node.Where(_ => true);

            Assert.IsFalse(result.TryGetLeft(out var element));
            Assert.IsTrue(result.TryGetRight(out var terminal));
            Assert.IsFalse(terminal.TryGetLeft(out var error));
            Assert.IsTrue(terminal.TryGetRight(out var empty));
        }

        [TestMethod]
        public void WhereNoElementsError()
        {
            var invalidOperationException = new InvalidOperationException();
            var node = Either.Left<MockElement>().Right(Either.Left(new MockError(invalidOperationException)).Right<MockEmpty>()).ToQueryResultNode();

            var result = node.Where(_ => true);

            Assert.IsFalse(result.TryGetLeft(out var element));
            Assert.IsTrue(result.TryGetRight(out var terminal));
            Assert.IsTrue(terminal.TryGetLeft(out var error));
            Assert.AreEqual(invalidOperationException, error.Value);
            Assert.IsFalse(terminal.TryGetRight(out var empty));
        }

        [TestMethod]
        public void WhereNoError()
        {
            var value = "asdf";
            var node = Either.Left(new MockElement(value)).Right<IEither<MockError, MockEmpty>>().ToQueryResultNode();

            var result = node.Where(_ => true);

            Assert.IsTrue(result.TryGetLeft(out var element));
            Assert.AreEqual(value, element.Value);
            Assert.IsFalse(result.TryGetRight(out var terminal));
            
            result = node.Where(_ => false);

            Assert.IsFalse(result.TryGetLeft(out element));
            Assert.IsTrue(result.TryGetRight(out var terminal2));
            Assert.IsFalse(terminal2.TryGetLeft(out var error));
            Assert.IsTrue(terminal2.TryGetRight(out var empty));
        }
    }
}
