namespace OddTrotter.UserExtension
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Primitives;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using global::OddTrotter.Encryptor;
    using global::OddTrotter.GraphClient;

    /// <summary>
    /// Unit tests for <see cref="UserExtensionServiceExtensions"/>
    /// </summary>
    [TestClass]
    public sealed class UserExtensionServiceExtensionsUnitTests
    {
        /// <summary>
        /// Configures the odd trotter user extension with a <see langword="null"/> user extension service
        /// </summary>
        [TestMethod]
        public async Task ConfigureUserExtensionNullUserExtensionService()
        {
            UserExtensionService userExtensionService =
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            var httpRequestData = new HttpRequestData(new MockHttpRequest());

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
#pragma warning disable CS8604 // Possible null reference argument.
                userExtensionService
#pragma warning restore CS8604 // Possible null reference argument.
                .ConfigureUserExtension(httpRequestData)).ConfigureAwait(false);
        }

        private sealed class MockHttpRequest : HttpRequest
        {
            public override HttpContext HttpContext => throw new NotImplementedException();

            public override string Method { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public override string Scheme { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public override bool IsHttps { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public override HostString Host { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public override PathString PathBase { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public override PathString Path { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public override QueryString QueryString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public override IQueryCollection Query { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public override string Protocol { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public override IHeaderDictionary Headers => throw new NotImplementedException();

            public override IRequestCookieCollection Cookies { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public override long? ContentLength { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public override string? ContentType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public override Stream Body { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public override bool HasFormContentType => throw new NotImplementedException();

            public override IFormCollection Form { get; set; } = new FormCollection(new Dictionary<string, StringValues>());

            public override Task<IFormCollection> ReadFormAsync(CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Configures the odd trotter user extension with <see langword="null"/> request data
        /// </summary>
        [TestMethod]
        public async Task ConfigureUserExtensionNullRequestData()
        {
            var userExtensionService = new UserExtensionService(new MockGraphClient(), new Encryptor());

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => userExtensionService.ConfigureUserExtension(
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                (HttpRequestData)null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                )).ConfigureAwait(false);
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

            public async Task<HttpResponseMessage> PatchAsync(RelativeUri relativeUri, HttpContent httpContent)
            {
                return await Task.FromResult(new HttpResponseMessage()).ConfigureAwait(false);
            }

            public Task<HttpResponseMessage> PostAsync(RelativeUri relativeUri, HttpContent httpContent)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Configures the odd trotter user extension when no container URL is provided
        /// </summary>
        [TestMethod]
        public async Task ConfigureUserExtensionMissingContainerUrlField()
        {
            var userExtensionService = new UserExtensionService(new MockGraphClient(), new Encryptor());
            var httpRequestData = new HttpRequestData(new MockHttpRequest());

            var missingFormDataException = await Assert.ThrowsExceptionAsync<MissingFormDataException>(() => userExtensionService.ConfigureUserExtension(httpRequestData)).ConfigureAwait(false);
            Assert.IsTrue(missingFormDataException.MissingFormFieldNames.Contains("oddTrotterBlobContainerUrl"));
        }

        /// <summary>
        /// Configures the odd trotter user extension when no container URL is provided
        /// </summary>
        [TestMethod]
        public async Task ConfigureUserExtensionNoContainerUrls()
        {
            var userExtensionService = new UserExtensionService(new MockGraphClient(), new Encryptor());
            var httpRequestData = new HttpRequestData(new MockHttpRequest()
            {
                Form = new FormCollection(new Dictionary<string, StringValues>()
                {
                    { "oddTrotterBlobContainerUrl", new StringValues() },
                }),
            });

            var missingFormDataException = await Assert.ThrowsExceptionAsync<MissingFormDataException>(() => userExtensionService.ConfigureUserExtension(httpRequestData)).ConfigureAwait(false);
            Assert.IsTrue(missingFormDataException.MissingFormFieldNames.Contains("oddTrotterBlobContainerUrl"));
        }

        /// <summary>
        /// Configures the odd trotter user extension when many container URLs are provided
        /// </summary>
        [TestMethod]
        public async Task ConfigureUserExtensionManyContainerUrls()
        {
            var userExtensionService = new UserExtensionService(new MockGraphClient(), new Encryptor());
            var httpRequestData = new HttpRequestData(new MockHttpRequest()
            {
                Form = new FormCollection(new Dictionary<string, StringValues>()
                {
                    { "oddTrotterBlobContainerUrl", new StringValues(new[] { "first", "second" }) },
                }),
            });

            var invalidFormDataException = await Assert.ThrowsExceptionAsync<InvalidFormDataException>(() => userExtensionService.ConfigureUserExtension(httpRequestData)).ConfigureAwait(false);
            Assert.AreEqual("oddTrotterBlobContainerUrl", invalidFormDataException.FormFieldName);
        }

        /// <summary>
        /// Configures the odd trotter user extension when the container URL is empty
        /// </summary>
        [TestMethod]
        public async Task ConfigureUserExtensionEmptyContainerUrl()
        {
            var userExtensionService = new UserExtensionService(new MockGraphClient(), new Encryptor());
            var httpRequestData = new HttpRequestData(new MockHttpRequest()
            {
                Form = new FormCollection(new Dictionary<string, StringValues>()
                {
                    { "oddTrotterBlobContainerUrl", new StringValues(new[] { string.Empty }) },
                }),
            });

            var invalidFormDataException = await Assert.ThrowsExceptionAsync<InvalidFormDataException>(() => userExtensionService.ConfigureUserExtension(httpRequestData)).ConfigureAwait(false);
            Assert.AreEqual("oddTrotterBlobContainerUrl", invalidFormDataException.FormFieldName);
        }

        /// <summary>
        /// Configures the odd trotter user extension when no SAS token`` is provided
        /// </summary>
        [TestMethod]
        public async Task ConfigureUserExtensionMissingSasTokenField()
        {
            var userExtensionService = new UserExtensionService(new MockGraphClient(), new Encryptor());
            var httpRequestData = new HttpRequestData(new MockHttpRequest()
            {
                Form = new FormCollection(new Dictionary<string, StringValues>()
                {
                    { "oddTrotterBlobContainerUrl", new StringValues(new[] { "https://blob.container.url" }) },
                }),
            });

            var missingFormDataException = await Assert.ThrowsExceptionAsync<MissingFormDataException>(() => userExtensionService.ConfigureUserExtension(httpRequestData)).ConfigureAwait(false);
            Assert.IsTrue(missingFormDataException.MissingFormFieldNames.Contains("oddTrotterBlobContainerSasToken"));
        }

        /// <summary>
        /// Configures the odd trotter user extension when no SAS token is provided
        /// </summary>
        [TestMethod]
        public async Task ConfigureUserExtensionNoSasTokens()
        {
            var userExtensionService = new UserExtensionService(new MockGraphClient(), new Encryptor());
            var httpRequestData = new HttpRequestData(new MockHttpRequest()
            {
                Form = new FormCollection(new Dictionary<string, StringValues>()
                {
                    { "oddTrotterBlobContainerUrl", new StringValues(new[] { "https://blob.container.url" }) },
                    { "oddTrotterBlobContainerSasToken", new StringValues() },
                }),
            });

            var missingFormDataException = await Assert.ThrowsExceptionAsync<MissingFormDataException>(() => userExtensionService.ConfigureUserExtension(httpRequestData)).ConfigureAwait(false);
            Assert.IsTrue(missingFormDataException.MissingFormFieldNames.Contains("oddTrotterBlobContainerSasToken"));
        }

        /// <summary>
        /// Configures the odd trotter user extension when many SAS tokens are provided
        /// </summary>
        [TestMethod]
        public async Task ConfigureUserExtensionManySasTokens()
        {
            var userExtensionService = new UserExtensionService(new MockGraphClient(), new Encryptor());
            var httpRequestData = new HttpRequestData(new MockHttpRequest()
            {
                Form = new FormCollection(new Dictionary<string, StringValues>()
                {
                    { "oddTrotterBlobContainerUrl", new StringValues(new[] { "https://blob.container.url" }) },
                    { "oddTrotterBlobContainerSasToken", new StringValues(new[] { "first", "second" }) },
                }),
            });

            var invalidFormDataException = await Assert.ThrowsExceptionAsync<InvalidFormDataException>(() => userExtensionService.ConfigureUserExtension(httpRequestData)).ConfigureAwait(false);
            Assert.AreEqual("oddTrotterBlobContainerSasToken", invalidFormDataException.FormFieldName);
        }

        /// <summary>
        /// Configures the odd trotter user extension when the SAS token is empty
        /// </summary>
        [TestMethod]
        public async Task ConfigureUserExtensionEmptySasToken()
        {
            var userExtensionService = new UserExtensionService(new MockGraphClient(), new Encryptor());
            var httpRequestData = new HttpRequestData(new MockHttpRequest()
            {
                Form = new FormCollection(new Dictionary<string, StringValues>()
                {
                    { "oddTrotterBlobContainerUrl", new StringValues(new[] { "https://blob.container.url" }) },
                    { "oddTrotterBlobContainerSasToken", new StringValues(new[] { string.Empty }) },
                }),
            });

            var invalidFormDataException = await Assert.ThrowsExceptionAsync<InvalidFormDataException>(() => userExtensionService.ConfigureUserExtension(httpRequestData)).ConfigureAwait(false);
            Assert.AreEqual("oddTrotterBlobContainerSasToken", invalidFormDataException.FormFieldName);
        }

        /// <summary>
        /// Configures the odd trotter user extension
        /// </summary>
        [TestMethod]
        public async Task ConfigureUserExtension()
        {
            var userExtensionService = new UserExtensionService(new MockGraphClient(), new Encryptor());
            var httpRequestData = new HttpRequestData(new MockHttpRequest()
            {
                Form = new FormCollection(new Dictionary<string, StringValues>()
                {
                    { "oddTrotterBlobContainerUrl", new StringValues(new[] { "https://blob.container.url" }) },
                    { "oddTrotterBlobContainerSasToken", new StringValues(new[] { "sometoken" }) },
                }),
            });

            await userExtensionService.ConfigureUserExtension(httpRequestData).ConfigureAwait(false);
        }
    }
}