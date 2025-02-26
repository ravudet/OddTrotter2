namespace Fx.QueryContext
{
    using Fx.Either;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

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
            var node = new QueryResultNode<string, Exception>(Either.Left(new MockElement(value)).Right<IEither<IError<Exception>, IEmpty>>());

            Assert.ThrowsException<ArgumentNullException>(() => node.Apply(
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                (Func<IElement<string, Exception>, Nothing, string>)null
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                ,
                (terminal, context) => terminal.Apply((error, context) => error.ToString(),
                (empty, context) => string.Empty, new Nothing()), new Nothing()));
        }

        private sealed class MockElement : IElement<string, Exception>
        {
            public MockElement(string value)
            {
                Value = value;
            }

            public string Value { get; }

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
            private MockEmpty()
            {
            }

            public static MockEmpty Instance { get; } = new MockEmpty();
        }

        [TestMethod]
        public void ApplyNullRightMap()
        {
            var value = "asdf";
            var node = new QueryResultNode<string, Exception>(Either.Left(new MockElement(value)).Right<IEither<IError<Exception>, IEmpty>>());

            Assert.ThrowsException<ArgumentNullException>(() => node.Apply(
                (element, context) => string.Concat(element.Value, element.Value),
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                , new Nothing()));
        }

        [TestMethod]
        public void 
    }
}
