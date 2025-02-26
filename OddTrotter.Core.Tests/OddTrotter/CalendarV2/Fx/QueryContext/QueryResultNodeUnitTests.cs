/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.QueryContext
{
    using System;

    using Fx.Either;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class QueryResultNodeUnitTests
    {
        [TestMethod]
        public void InitializeNullNode()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new QueryResultNode<string, Exception>(
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                ));
        }

        [TestMethod]
        public void ApplyNullLeftMap()
        {
            var value = "asdf";
            var node = new QueryResultNode<string, Exception>(
                Either.Left(new MockElement(value)).Right<IEither<IError<Exception>, IEmpty>>());

            Assert.ThrowsException<ArgumentNullException>(() => node.Apply(
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                ,
                (terminal, context) => 
                    terminal.Apply(
                        (error, context) => error.Value.Message, 
                        (empty, context) => string.Empty, 
                        new Nothing()), 
                new Nothing()));
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

        [TestMethod]
        public void ApplyNullRightMap()
        {
            var value = "asdf";
            var node = new QueryResultNode<string, Exception>(
                Either.Left(new MockElement(value)).Right<IEither<IError<Exception>, IEmpty>>());

            Assert.ThrowsException<ArgumentNullException>(() => node.Apply(
                (element, context) => string.Concat(element.Value, element.Value),
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                , new Nothing()));
        }

        [TestMethod]
        public void ApplyLeftMapException()
        {
            var value = "asdf";
            var invalidOperationException = new InvalidOperationException();
            var node = new QueryResultNode<string, Exception>(
                Either.Left(new MockElement(value)).Right<IEither<IError<Exception>, IEmpty>>());

            var leftMapException = Assert.ThrowsException<LeftMapException>(() => node.Apply(
                (element, context) => throw invalidOperationException,
                (terminal, context) => terminal.Apply(
                    (error, context) => error.Value.Message,
                    (empty, context) => string.Empty,
                    new Nothing()),
                new Nothing()));

            Assert.AreEqual(invalidOperationException, leftMapException.InnerException);

            node = new QueryResultNode<string, Exception>(
                Either.Left<MockElement>().Right(Either.Left(new MockError(new Exception(value))).Right<IEmpty>()));

            var result = node.Apply(
                (element, context) => throw invalidOperationException,
                (terminal, context) => terminal.Apply(
                    (error, context) => error.Value.Message,
                    (empty, context) => string.Empty,
                    new Nothing()),
                new Nothing());

            Assert.AreEqual(value, result);
        }

        [TestMethod]
        public void ApplyRightMapException()
        {
            var value = "asdf";
            var invalidOperationException = new InvalidOperationException();
            var node = new QueryResultNode<string, Exception>(
                Either.Left(new MockElement(value)).Right<IEither<IError<Exception>, IEmpty>>());

            var result = node.Apply(
                (element, context) => string.Concat(element.Value, element.Value),
                (terminal, context) => throw invalidOperationException,
                new Nothing());

            Assert.AreEqual(value + value, result);
            
            node = new QueryResultNode<string, Exception>(
                Either.Left<MockElement>().Right(Either.Left(new MockError(new Exception(value))).Right<IEmpty>()));

            var rightMapException = Assert.ThrowsException<RightMapException>(() => node.Apply(
                (element, context) => string.Concat(element.Value, element.Value),
                (terminal, context) => throw invalidOperationException,
                new Nothing()));

            Assert.AreEqual(invalidOperationException, rightMapException.InnerException);
        }

        [TestMethod]
        public void Apply()
        {
            var value = "asdf";
            var node = new QueryResultNode<string, Exception>(
                Either.Left(new MockElement(value)).Right<IEither<IError<Exception>, IEmpty>>());

            var result = node.Apply(
                (element, context) => string.Concat(element.Value, element.Value),
                (terminal, context) => terminal.Apply(
                    (error, context) => error.Value.Message, 
                    (empty, context) => string.Empty, 
                    new Nothing()), 
                new Nothing());

            Assert.AreEqual(value + value, result);

            node = new QueryResultNode<string, Exception>(
                Either.Left<MockElement>().Right(Either.Left(new MockError(new Exception(value))).Right<IEmpty>()));

            result = node.Apply(
                (element, context) => string.Concat(element.Value, element.Value),
                (terminal, context) => terminal.Apply(
                    (error, context) => error.Value.Message,
                    (empty, context) => string.Empty,
                    new Nothing()), 
                new Nothing());

            Assert.AreEqual(value, result);

            node = new QueryResultNode<string, Exception>(
                Either.Left<MockElement>().Right(Either.Left<MockError>().Right(MockEmpty.Instance)));

            result = node.Apply(
                (element, context) => string.Concat(element.Value, element.Value),
                (terminal, context) => terminal.Apply(
                    (error, context) => error.Value.Message,
                    (empty, context) => string.Empty,
                    new Nothing()),
                new Nothing());

            Assert.AreEqual(string.Empty, result);
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
    }
}
