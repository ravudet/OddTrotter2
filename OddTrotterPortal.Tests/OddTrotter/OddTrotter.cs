namespace OddTrotter
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using global::OddTrotter.AzureBlobClient;
    using global::OddTrotter.GraphClient;
    using global::OddTrotter.TodoList;

    /// <summary>
    /// Unit tests for <see cref="OddTrotter"/>
    /// </summary>
    [TestClass]
    public sealed class OddTrotterUnitTests
    {
        /// <summary>
        /// Creates an OddTrotter
        /// </summary>
        [TestMethod]
        public void CreateOddTrotter()
        {
            var memoryCacheOptions = new MemoryCacheOptions();
            var memoryCache = new MemoryCache(memoryCacheOptions);
            var graphClient = new MockGraphClient();
            var blobClient = new MockBlobClient();
            var todoListService = new TodoListService(memoryCache, graphClient, blobClient);
            new OddTrotter(todoListService);
        }

        /// <summary>
        /// Creates an OddTrotter with a <see langword="null"/> todo list
        /// </summary>
        [TestMethod]
        public void NullTodoList()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new OddTrotter(
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                ));
        }

        private sealed class MockGraphClient : IGraphClient
        {
            public Task<HttpResponseMessage> GetAsync(RelativeUri relativeUri)
            {
                throw new NotImplementedException();
            }

            public Task<HttpResponseMessage> GetAsync(AbsoluteUri absoluteUri)
            {
                throw new NotImplementedException();
            }

            public Task<HttpResponseMessage> PatchAsync(RelativeUri relativeUri, HttpContent httpContent)
            {
                throw new NotImplementedException();
            }

            public Task<HttpResponseMessage> PostAsync(RelativeUri relativeUri, HttpContent httpContent)
            {
                throw new NotImplementedException();
            }
        }

        private sealed class MockBlobClient : IAzureBlobClient
        {
            public Task<HttpResponseMessage> GetAsync(string blobName)
            {
                throw new NotImplementedException();
            }

            public Task<HttpResponseMessage> PutAsync(string blobName, HttpContent httpContent)
            {
                throw new NotImplementedException();
            }
        }
    }
}
