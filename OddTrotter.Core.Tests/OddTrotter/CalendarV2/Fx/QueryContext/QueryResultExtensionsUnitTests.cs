namespace Fx.QueryContext
{
    using System;

    using Fx.Either;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class QueryResultExtensionsUnitTests
    {
        [TestMethod]
        public void WhereNullSource()
        {
            IQueryResult<string, Exception> queryResult =
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                null
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                ;

            Assert.ThrowsException<ArgumentNullException>(() =>
#pragma warning disable CS8604 // Possible null reference argument.
                queryResult
#pragma warning restore CS8604 // Possible null reference argument.
                .Where(_ => true));
        }

        private sealed class MockQueryResult : IQueryResult<string, Exception>
        {
            public MockQueryResult(IQueryResultNode<string, Exception> nodes)
            {
                Nodes = nodes;
            }

            public IQueryResultNode<string, Exception> Nodes { get; }
        }

        [TestMethod]
        public void WhereNullPredicate()
        {
            var value = "asdf";
            var node = Either.Left(new MockElement(value)).Right<IEither<IError<Exception>, IEmpty>>().ToQueryResultNode();
        }
    }
}
