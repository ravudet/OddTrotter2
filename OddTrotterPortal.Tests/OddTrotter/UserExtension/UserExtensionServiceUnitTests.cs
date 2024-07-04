namespace OddTrotter.UserExtension
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using global::OddTrotter.Encryptor;
    using global::OddTrotter.GraphClient;
    using global::OddTrotter.AzureBlobClient;

    /// <summary>
    /// Unit tests for <see cref="UserExtensionService"/>
    /// </summary>
    [TestClass]
    public sealed class UserExtensionServiceUnitTests
    {
        /// <summary>
        /// Creates a <see cref="UserExtensionService"/> with a <see langword="null"/> graph client
        /// </summary>
        [TestMethod]
        public void NullGraphClient()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new UserExtensionService(
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                null,
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                new Encryptor()));
        }

        /// <summary>
        /// Creates a <see cref="UserExtensionService"/> with a <see langword="null"/> encryptor
        /// </summary>
        [TestMethod]
        public void NullEncryptor()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new UserExtensionService(
                new MockGraphClient(),
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

        /// <summary>
        /// Configures the oddtrotter user extension using <see langword="null"/> blob settings
        /// </summary>
        [TestMethod]
        public async Task ConfigureUserExtensionNullBlobSettings()
        {
            var userExtensionService = new UserExtensionService(new MockGraphClient(), new Encryptor());
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => userExtensionService.ConfigureUserExtension(
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                )).ConfigureAwait(false);
        }

        /// <summary>
        /// Configures the oddtrotter user extension using extremely long blob settings
        /// </summary>
        /// <remarks>
        /// TODO this test *should* assert that payloads that are too long to encrypt throw the correct exception; however, the JSON serializer actually throws for smaller string lengths, so we aren't able to repro; this serializer exception isn't documented, so we have filed a bug: https://github.com/dotnet/runtime/issues/104168
        /// </remarks>
        [Ignore]
        [TestMethod]
        public async Task ConfigureUserExtensionLongBlobSettings()
        {
            var userExtensionService = new UserExtensionService(new MockGraphClient(), new Encryptor());
            var blobSettings = new OddTrotterBlobSettings.Builder()
            {
                BlobContainerUrl = "https://someurl.com",
                SasToken = new string(' ', StringUtilities.MaxLength),
            }.Build();
            await Assert.ThrowsExceptionAsync<UserExtensionEncryptionException>(() => userExtensionService.ConfigureUserExtension(blobSettings)).ConfigureAwait(false);
        }

        /// <summary>
        /// Configures an existing oddtrotter user extension
        /// </summary>
        [TestMethod]
        public async Task ConfigureExistingUserExtension()
        {
            var graphClient = new MockConfigureExistingUserExtensionGraphClient();
            var userExtensionService = new UserExtensionService(graphClient, new Encryptor());
            var blobSettings = new OddTrotterBlobSettings.Builder()
            {
                BlobContainerUrl = "https://someurl.com",
                SasToken = "sometoken",
            }.Build();
            await userExtensionService.ConfigureUserExtension(blobSettings).ConfigureAwait(false);
            Assert.AreEqual(1, graphClient.PatchCalled);
        }

        private sealed class MockConfigureExistingUserExtensionGraphClient : IGraphClient
        {
            public int PatchCalled { get; private set; } = 0;

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
                ++this.PatchCalled;
                StringContent? stringContent = null;
                try
                {
                    stringContent = new StringContent(nameof(ConfigureExistingUserExtension));
                    HttpResponseMessage? responseMessage = null;
                    try
                    {
                        responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
                        responseMessage.Content = stringContent;
                        return await Task.FromResult(responseMessage).ConfigureAwait(false);
                    }
                    catch
                    {
                        responseMessage?.Dispose();
                        throw;
                    }
                }
                catch
                {
                    stringContent?.Dispose();
                    throw;
                }
            }

            public Task<HttpResponseMessage> PostAsync(RelativeUri relativeUri, HttpContent httpContent)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Configures an existing oddtrotter user extension with no network
        /// </summary>
        [TestMethod]
        public async Task ConfigureExistingUserExtensionNoNetwork()
        {
            var graphClient = new MockConfigureExistingUserExtensionNoNetworkGraphClient();
            var userExtensionService = new UserExtensionService(graphClient, new Encryptor());
            var blobSettings = new OddTrotterBlobSettings.Builder()
            {
                BlobContainerUrl = "https://someurl.com",
                SasToken = "sometoken",
            }.Build();
            var httpRequestException = await Assert.ThrowsExceptionAsync<HttpRequestException>(() => userExtensionService.ConfigureUserExtension(blobSettings)).ConfigureAwait(false);
            Assert.AreEqual(MockConfigureExistingUserExtensionNoNetworkGraphClient.ExceptionMessage, httpRequestException.Message);
        }

        private sealed class MockConfigureExistingUserExtensionNoNetworkGraphClient : IGraphClient
        {
            public static string ExceptionMessage { get; } = nameof(ConfigureExistingUserExtensionNoNetwork);

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
                throw new HttpRequestException(ExceptionMessage);
            }

            public Task<HttpResponseMessage> PostAsync(RelativeUri relativeUri, HttpContent httpContent)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Configures an existing oddtrotter user extension with an invalid access token
        /// </summary>
        [TestMethod]
        public async Task ConfigureExistingUserExtensionInvalidAccessToken()
        {
            var graphClient = new MockConfigureExistingUserExtensionInvalidAccessTokenGraphClient();
            var userExtensionService = new UserExtensionService(graphClient, new Encryptor());
            var blobSettings = new OddTrotterBlobSettings.Builder()
            {
                BlobContainerUrl = "https://someurl.com",
                SasToken = "sometoken",
            }.Build();
            var invalidAccessTokenException = await Assert.ThrowsExceptionAsync<InvalidAccessTokenException>(() => userExtensionService.ConfigureUserExtension(blobSettings)).ConfigureAwait(false);
            Assert.AreEqual(MockConfigureExistingUserExtensionInvalidAccessTokenGraphClient.ExceptionMessage, invalidAccessTokenException.Message);
        }

        private sealed class MockConfigureExistingUserExtensionInvalidAccessTokenGraphClient : IGraphClient
        {
            public static string ExceptionMessage { get; } = nameof(ConfigureExistingUserExtensionInvalidAccessToken);

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
                throw new InvalidAccessTokenException("the url", "the accesstoken", ExceptionMessage);
            }

            public Task<HttpResponseMessage> PostAsync(RelativeUri relativeUri, HttpContent httpContent)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Configures an existing oddtrotter user extension with a graph failure
        /// </summary>
        [TestMethod]
        public async Task ConfigureExistingUserExtensionGraphFailure()
        {
            var graphClient = new MockConfigureExistingUserExtensionGraphFailureGraphClient();
            var userExtensionService = new UserExtensionService(graphClient, new Encryptor());
            var blobSettings = new OddTrotterBlobSettings.Builder()
            {
                BlobContainerUrl = "https://someurl.com",
                SasToken = "sometoken",
            }.Build();
            var graphException = await Assert.ThrowsExceptionAsync<GraphException>(() => userExtensionService.ConfigureUserExtension(blobSettings)).ConfigureAwait(false);
            Assert.AreEqual(MockConfigureExistingUserExtensionGraphFailureGraphClient.ExceptionMessage, graphException.Message);
        }

        private sealed class MockConfigureExistingUserExtensionGraphFailureGraphClient : IGraphClient
        {
            public static string ExceptionMessage { get; } = nameof(ConfigureExistingUserExtensionGraphFailure);

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
                StringContent? stringContent = null;
                try
                {
                    stringContent = new StringContent(ExceptionMessage);
                    HttpResponseMessage? responseMessage = null;
                    try
                    {
                        responseMessage = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
                        responseMessage.Content = stringContent;
                        return await Task.FromResult(responseMessage).ConfigureAwait(false);
                    }
                    catch
                    {
                        responseMessage?.Dispose();
                        throw;
                    }
                }
                catch
                {
                    stringContent?.Dispose();
                    throw;
                }
            }

            public Task<HttpResponseMessage> PostAsync(RelativeUri relativeUri, HttpContent httpContent)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Configures a new oddtrotter user extension
        /// </summary>
        [TestMethod]
        public async Task ConfigureNewUserExtension()
        {
            var graphClient = new MockConfigureNewUserExtensionGraphClient();
            var userExtensionService = new UserExtensionService(graphClient, new Encryptor());
            var blobSettings = new OddTrotterBlobSettings.Builder()
            {
                BlobContainerUrl = "https://someurl.com",
                SasToken = "sometoken",
            }.Build();
            await userExtensionService.ConfigureUserExtension(blobSettings).ConfigureAwait(false);
            Assert.AreEqual(1, graphClient.PatchCalled);
            Assert.AreEqual(1, graphClient.PostCalled);
        }

        private sealed class MockConfigureNewUserExtensionGraphClient : IGraphClient
        {
            public int PatchCalled { get; private set; } = 0;
            public int PostCalled { get; private set; } = 0;

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
                ++this.PatchCalled;
                StringContent? stringContent = null;
                try
                {
                    stringContent = new StringContent(string.Empty);
                    HttpResponseMessage? responseMessage = null;
                    try
                    {
                        responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
                        responseMessage.Content = stringContent;
                        return await Task.FromResult(responseMessage).ConfigureAwait(false);
                    }
                    catch
                    {
                        responseMessage?.Dispose();
                        throw;
                    }
                }
                catch
                {
                    stringContent?.Dispose();
                    throw;
                }
            }

            public async Task<HttpResponseMessage> PostAsync(RelativeUri relativeUri, HttpContent httpContent)
            {
                ++this.PostCalled;
                StringContent? stringContent = null;
                try
                {
                    stringContent = new StringContent(nameof(ConfigureExistingUserExtension));
                    HttpResponseMessage? responseMessage = null;
                    try
                    {
                        responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
                        responseMessage.Content = stringContent;
                        return await Task.FromResult(responseMessage).ConfigureAwait(false);
                    }
                    catch
                    {
                        responseMessage?.Dispose();
                        throw;
                    }
                }
                catch
                {
                    stringContent?.Dispose();
                    throw;
                }
            }
        }

        /// <summary>
        /// Configures a new oddtrotter user extension with no network
        /// </summary>
        [TestMethod]
        public async Task ConfigureNewUserExtensionNoNetwork()
        {
            var graphClient = new MockConfigureNewUserExtensionNoNetworkGraphClient();
            var userExtensionService = new UserExtensionService(graphClient, new Encryptor());
            var blobSettings = new OddTrotterBlobSettings.Builder()
            {
                BlobContainerUrl = "https://someurl.com",
                SasToken = "sometoken",
            }.Build();
            var httpRequestException = await Assert.ThrowsExceptionAsync<HttpRequestException>(() => userExtensionService.ConfigureUserExtension(blobSettings)).ConfigureAwait(false);
            Assert.AreEqual(MockConfigureNewUserExtensionNoNetworkGraphClient.ExceptionMessage, httpRequestException.Message);
        }

        private sealed class MockConfigureNewUserExtensionNoNetworkGraphClient : IGraphClient
        {
            public static string ExceptionMessage { get; } = nameof(ConfigureNewUserExtensionNoNetwork);

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
                StringContent? stringContent = null;
                try
                {
                    stringContent = new StringContent(string.Empty);
                    HttpResponseMessage? responseMessage = null;
                    try
                    {
                        responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
                        responseMessage.Content = stringContent;
                        return await Task.FromResult(responseMessage).ConfigureAwait(false);
                    }
                    catch
                    {
                        responseMessage?.Dispose();
                        throw;
                    }
                }
                catch
                {
                    stringContent?.Dispose();
                    throw;
                }
            }

            public Task<HttpResponseMessage> PostAsync(RelativeUri relativeUri, HttpContent httpContent)
            {
                throw new HttpRequestException(ExceptionMessage);
            }
        }

        /// <summary>
        /// Configures a new oddtrotter user extension with no network
        /// </summary>
        [TestMethod]
        public async Task ConfigureNewUserExtensionInvalidAccessToken()
        {
            var graphClient = new MockConfigureNewUserExtensionInvalidAccessTokenGraphClient();
            var userExtensionService = new UserExtensionService(graphClient, new Encryptor());
            var blobSettings = new OddTrotterBlobSettings.Builder()
            {
                BlobContainerUrl = "https://someurl.com",
                SasToken = "sometoken",
            }.Build();
            var invalidAccessTokenException = await Assert.ThrowsExceptionAsync<InvalidAccessTokenException>(() => userExtensionService.ConfigureUserExtension(blobSettings)).ConfigureAwait(false);
            Assert.AreEqual(MockConfigureNewUserExtensionInvalidAccessTokenGraphClient.ExceptionMessage, invalidAccessTokenException.Message);
        }

        private sealed class MockConfigureNewUserExtensionInvalidAccessTokenGraphClient : IGraphClient
        {
            public static string ExceptionMessage { get; } = nameof(ConfigureNewUserExtensionNoNetwork);

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
                StringContent? stringContent = null;
                try
                {
                    stringContent = new StringContent(string.Empty);
                    HttpResponseMessage? responseMessage = null;
                    try
                    {
                        responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
                        responseMessage.Content = stringContent;
                        return await Task.FromResult(responseMessage).ConfigureAwait(false);
                    }
                    catch
                    {
                        responseMessage?.Dispose();
                        throw;
                    }
                }
                catch
                {
                    stringContent?.Dispose();
                    throw;
                }
            }

            public Task<HttpResponseMessage> PostAsync(RelativeUri relativeUri, HttpContent httpContent)
            {
                throw new InvalidAccessTokenException("the url", "the token", ExceptionMessage);
            }
        }

        /// <summary>
        /// Configures a new oddtrotter user extension with a graph failure
        /// </summary>
        [TestMethod]
        public async Task ConfigureNewUserExtensionGraphFailure()
        {
            var graphClient = new MockConfigureNewUserExtensionGraphFailureGraphClient();
            var userExtensionService = new UserExtensionService(graphClient, new Encryptor());
            var blobSettings = new OddTrotterBlobSettings.Builder()
            {
                BlobContainerUrl = "https://someurl.com",
                SasToken = "sometoken",
            }.Build();
            var graphException = await Assert.ThrowsExceptionAsync<GraphException>(() => userExtensionService.ConfigureUserExtension(blobSettings)).ConfigureAwait(false);
            Assert.AreEqual(MockConfigureNewUserExtensionGraphFailureGraphClient.ExceptionMessage, graphException.Message);
        }

        private sealed class MockConfigureNewUserExtensionGraphFailureGraphClient : IGraphClient
        {
            public static string ExceptionMessage { get; } = nameof(ConfigureNewUserExtensionGraphFailure);

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
                StringContent? stringContent = null;
                try
                {
                    stringContent = new StringContent(string.Empty);
                    HttpResponseMessage? responseMessage = null;
                    try
                    {
                        responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
                        responseMessage.Content = stringContent;
                        return await Task.FromResult(responseMessage).ConfigureAwait(false);
                    }
                    catch
                    {
                        responseMessage?.Dispose();
                        throw;
                    }
                }
                catch
                {
                    stringContent?.Dispose();
                    throw;
                }
            }

            public async Task<HttpResponseMessage> PostAsync(RelativeUri relativeUri, HttpContent httpContent)
            {
                StringContent? stringContent = null;
                try
                {
                    stringContent = new StringContent(ExceptionMessage);
                    HttpResponseMessage? responseMessage = null;
                    try
                    {
                        responseMessage = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
                        responseMessage.Content = stringContent;
                        return await Task.FromResult(responseMessage).ConfigureAwait(false);
                    }
                    catch
                    {
                        responseMessage?.Dispose();
                        throw;
                    }
                }
                catch
                {
                    stringContent?.Dispose();
                    throw;
                }
            }
        }

        /// <summary>
        /// Configures an oddtrotter user extension that is being mucked with during the configuration process
        /// </summary>
        [TestMethod]
        public async Task ConfigureEventuallyConsistentUserExtension()
        {
            var graphClient = new MockConfigureEventuallyConsistentUserExtensionGraphClient();
            var userExtensionService = new UserExtensionService(graphClient, new Encryptor());
            var blobSettings = new OddTrotterBlobSettings.Builder()
            {
                BlobContainerUrl = "https://someurl.com",
                SasToken = "sometoken",
            }.Build();
            await userExtensionService.ConfigureUserExtension(blobSettings).ConfigureAwait(false);
            Assert.AreEqual(2, graphClient.PatchCalled);
            Assert.AreEqual(2, graphClient.PostCalled);
        }

        private sealed class MockConfigureEventuallyConsistentUserExtensionGraphClient : IGraphClient
        {
            public int PatchCalled { get; private set; } = 0;
            public int PostCalled { get; private set; } = 0;

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
                ++this.PatchCalled;
                StringContent? stringContent = null;
                try
                {
                    stringContent = new StringContent(string.Empty);
                    HttpResponseMessage? responseMessage = null;
                    try
                    {
                        responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
                        responseMessage.Content = stringContent;
                        return await Task.FromResult(responseMessage).ConfigureAwait(false);
                    }
                    catch
                    {
                        responseMessage?.Dispose();
                        throw;
                    }
                }
                catch
                {
                    stringContent?.Dispose();
                    throw;
                }
            }

            public async Task<HttpResponseMessage> PostAsync(RelativeUri relativeUri, HttpContent httpContent)
            {
                ++this.PostCalled;
                StringContent? stringContent = null;
                try
                {
                    stringContent = new StringContent(nameof(ConfigureExistingUserExtension));
                    HttpResponseMessage? responseMessage = null;
                    try
                    {
                        responseMessage = new HttpResponseMessage(this.PostCalled == 1 ? HttpStatusCode.Conflict : HttpStatusCode.OK);
                        responseMessage.Content = stringContent;
                        return await Task.FromResult(responseMessage).ConfigureAwait(false);
                    }
                    catch
                    {
                        responseMessage?.Dispose();
                        throw;
                    }
                }
                catch
                {
                    stringContent?.Dispose();
                    throw;
                }
            }
        }
    }
}