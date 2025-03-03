/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.QueryContext
{
    using System;

    using Fx.Either;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class QueryResultNodeExtensionsUnitTests
    {
        private static class NullablePlayground
        {
            private sealed class MockQueryResult : IQueryResult<string, int?>
            {
                /// <summary>
                /// placeholder
                /// </summary>
                /// <param name="nodes"></param>
                /// <exception cref="ArgumentNullException">Thrown if <paramref name="nodes"/> is <see langword="null"/></exception>
                public MockQueryResult(IQueryResultNode<string, int?> nodes)
                {
                    //// TODO clean this up as a real test
                    ArgumentNullException.ThrowIfNull(nodes);

                    this.Nodes = nodes;
                }

                /// <inheritdoc/>
                public IQueryResultNode<string, int?> Nodes { get; }
            }
            internal sealed class MockError : IError<int?>
            {
                /// <summary>
                /// placeholder
                /// </summary>
                /// <param name="value"></param>
                public MockError(int? value)
                {
                    this.Value = value;
                }

                /// <inheritdoc/>
                public int? Value { get; }
            }


            internal sealed class MockEmpty : IEmpty
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
            internal sealed class MockElement : IElement<string, int?>
            {
                private readonly IQueryResultNode<string, int?> next;

                /// <summary>
                /// placeholder
                /// </summary>
                /// <param name="value"></param>
                public MockElement(string value)
                    : this(
                        value,
                        Either
                            .Left<IElement<string, int?>>()
                            .Right(
                                Either
                                    .Left<IError<int?>>()
                                    .Right(
                                        MockEmpty.Instance))
                            .ToQueryResultNode())
                {
                }

                /// <summary>
                /// placeholder
                /// </summary>
                /// <param name="value"></param>
                /// <param name="next"></param>
                /// <exception cref="ArgumentNullException">Thrown if <paramref name="next"/> is <see langword="null"/></exception>
                public MockElement(string value, IQueryResultNode<string, int?> next)
                {
                    ArgumentNullException.ThrowIfNull(next);

                    this.Value = value;
                    this.next = next;
                }

                /// <inheritdoc/>
                public string Value { get; }

                /// <inheritdoc/>
                public IQueryResultNode<string, int?> Next()
                {
                    return this.next;
                }
            }

            public static void DoWork()
            {
                var first =
                    new MockQueryResult(
                        Either
                            .Left<MockElement>()
                            .Right(
                                Either
                                    .Left(new MockError(null))
                                    .Right<MockEmpty>())
                            .ToQueryResultNode());
                var second =
                    new MockQueryResult(
                        Either
                            .Left<MockElement>()
                            .Right(
                                Either
                                    .Left(new MockError(5))
                                    .Right<MockEmpty>())
                            .ToQueryResultNode());

                var concated = first
                    .Concat(
                        second,
                        firstError => new[] { firstError },
                        secondError => new[] { secondError },
                        (firstError, secondError) => new[] { firstError, secondError });

                Assert.IsTrue(concated.Nodes.TryGetRight(out var terminal));
                Assert.IsTrue(terminal.TryGetLeft(out var error));
                Assert.AreEqual(2, error.Value.Length);
            }
        }

        [TestMethod]
        public void Play()
        {
            NullablePlayground.DoWork();
        }

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

            node =
                Either
                    .Left<MockElement>()
                    .Right(
                        Either
                            .Left(
                                new MockError(
                                    new Exception(value)))
                            .Right<IEmpty>())
                    .ToQueryResultNode();

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
            var node =
                Either
                    .Left<MockElement>()
                    .Right(
                        Either
                            .Left(
                                new MockError(invalidOperationException))
                            .Right<MockEmpty>())
                    .ToQueryResultNode();

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
            var next = element.Next();
            Assert.IsFalse(next.TryGetLeft(out var secondElement));
            Assert.IsTrue(next.TryGetRight(out var secondTerminal));
            Assert.IsFalse(secondTerminal.TryGetLeft(out var secondError));
            Assert.IsTrue(secondTerminal.TryGetRight(out var secondEmpty));
            Assert.IsFalse(result.TryGetRight(out var terminal));

            result = node.Where(_ => false);

            Assert.IsFalse(result.TryGetLeft(out element));
            Assert.IsTrue(result.TryGetRight(out var terminal2));
            Assert.IsFalse(terminal2.TryGetLeft(out var error));
            Assert.IsTrue(terminal2.TryGetRight(out var empty));
        }

        [TestMethod]
        public void WhereElementFollowedByError()
        {
            var value = "asdf";
            var invalidOperationException = new InvalidOperationException();
            var node =
                Either
                    .Left(
                        new MockElement(
                            value,
                            Either
                                .Left<MockElement>()
                                .Right(
                                    Either
                                        .Left(
                                            new MockError(invalidOperationException))
                                        .Right<MockEmpty>())
                                .ToQueryResultNode()))
                    .Right<IEither<MockError, MockEmpty>>()
                    .ToQueryResultNode();

            var result = node.Where(_ => true);

            Assert.IsTrue(result.TryGetLeft(out var element));
            Assert.AreEqual(value, element.Value);
            var next = element.Next();
            Assert.IsFalse(next.TryGetLeft(out var secondElement));
            Assert.IsTrue(next.TryGetRight(out var secondTerminal));
            Assert.IsTrue(secondTerminal.TryGetLeft(out var secondError));
            Assert.AreEqual(invalidOperationException, secondError.Value);
            Assert.IsFalse(secondTerminal.TryGetRight(out var secondEmpty));
            Assert.IsFalse(result.TryGetRight(out var terminal));

            result = node.Where(_ => false);

            Assert.IsFalse(result.TryGetLeft(out element));
            Assert.IsTrue(result.TryGetRight(out var terminal2));
            Assert.IsTrue(terminal2.TryGetLeft(out var error));
            Assert.AreEqual(invalidOperationException, error.Value);
            Assert.IsFalse(terminal2.TryGetRight(out var empty));
        }

        [TestMethod]
        public void SelectNullSource()
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
                .Select(val => val.Length));
        }

        [TestMethod]
        public void SelectNullSelector()
        {
            var value = "asdf";
            var node = Either.Left(new MockElement(value)).Right<IEither<IError<Exception>, IEmpty>>().ToQueryResultNode();

            Assert.ThrowsException<ArgumentNullException>(() => node.Select(
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                (Func<string, int>)null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                ));
        }

        [TestMethod]
        public void SelectNoElements()
        {
            var node = Either.Left<MockElement>().Right(Either.Left<MockError>().Right(MockEmpty.Instance)).ToQueryResultNode();

            var result = node.Select(val => val.Length);

            Assert.IsFalse(result.TryGetLeft(out var element));
            Assert.IsTrue(result.TryGetRight(out var terminal));
            Assert.IsFalse(terminal.TryGetLeft(out var error));
            Assert.IsTrue(terminal.TryGetRight(out var empty));
        }

        [TestMethod]
        public void SelectNoElementsError()
        {
            var invalidOperationException = new InvalidOperationException();
            var node =
                Either
                    .Left<MockElement>()
                    .Right(
                        Either
                            .Left(
                                new MockError(invalidOperationException))
                            .Right<MockEmpty>())
                    .ToQueryResultNode();

            var result = node.Select(val => val.Length);

            Assert.IsFalse(result.TryGetLeft(out var element));
            Assert.IsTrue(result.TryGetRight(out var terminal));
            Assert.IsTrue(terminal.TryGetLeft(out var error));
            Assert.AreEqual(invalidOperationException, error.Value);
            Assert.IsFalse(terminal.TryGetRight(out var empty));
        }

        [TestMethod]
        public void SelectNoError()
        {
            var value = "asdf";
            var node = Either.Left(new MockElement(value)).Right<IEither<MockError, MockEmpty>>().ToQueryResultNode();

            var result = node.Select(val => val.Length);

            Assert.IsTrue(result.TryGetLeft(out var element));
            Assert.AreEqual(4, element.Value);
            var next = element.Next();
            Assert.IsFalse(next.TryGetLeft(out var secondElement));
            Assert.IsTrue(next.TryGetRight(out var secondTerminal));
            Assert.IsFalse(secondTerminal.TryGetLeft(out var secondError));
            Assert.IsTrue(secondTerminal.TryGetRight(out var secondEmpty));
            Assert.IsFalse(result.TryGetRight(out var terminal));
        }

        [TestMethod]
        public void SelectElementFollowedByError()
        {
            var value = "asdf";
            var invalidOperationException = new InvalidOperationException();
            var node =
                Either
                    .Left(
                        new MockElement(
                            value,
                            Either
                                .Left<MockElement>()
                                .Right(
                                    Either
                                        .Left(
                                            new MockError(invalidOperationException))
                                        .Right<MockEmpty>())
                                .ToQueryResultNode()))
                    .Right<IEither<MockError, MockEmpty>>()
                    .ToQueryResultNode();

            var result = node.Select(val => val.Length);

            Assert.IsTrue(result.TryGetLeft(out var element));
            Assert.AreEqual(4, element.Value);
            var next = element.Next();
            Assert.IsFalse(next.TryGetLeft(out var secondElement));
            Assert.IsTrue(next.TryGetRight(out var secondTerminal));
            Assert.IsTrue(secondTerminal.TryGetLeft(out var secondError));
            Assert.AreEqual(invalidOperationException, secondError.Value);
            Assert.IsFalse(secondTerminal.TryGetRight(out var secondEmpty));
            Assert.IsFalse(result.TryGetRight(out var terminal));
        }
    }
}
