namespace OddTrotter
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Configuration;
    using global::OddTrotter.AzureBlobClient;
    using global::OddTrotter.Encryptor;
    using global::OddTrotter.GraphClient;
    using global::OddTrotter.TodoList;

    public static class OddTrotterFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oddTrotterData"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="memoryCache"/> or <paramref name="httpRequestData"/> or <paramref name="configuration"/> is <see langword="null"/>
        /// </exception>
        /// <exception cref="GraphUriException">
        /// Thrown if the graph root URL specified in <paramref name="configuration"/> is not a valid URL or it is not an absolute URL
        /// </exception>
        /// <exception cref="MissingFormDataException">Thrown if the form data in <paramref name="httpRequestData"/> contained missing fields</exception>
        /// <exception cref="InvalidFormDataException">Thrown if the form data in <paramref name="httpRequestData"/> contained fields with invalid data</exception>
        /// <exception cref="InvalidAccessTokenException">Thrown if the access token in <paramref name="httpRequestData"/> is not a valid HTTP authorization header (for example, if it is a Bearer token, but is not prefixed with 'Bearer')</exception>
        /// <exception cref="HttpRequestException">
        /// Thrown if retrieving the oddtrotter user extension from graph failed due to an underlying issue such as network connectivity, DNS failure, server certificate
        /// validation or timeout
        /// </exception>
        /// <exception cref="UnauthorizedAccessTokenException">
        /// Thrown if the access token provided in the form data in <paramref name="httpRequestData"/> is invalid or provides insufficient privileges to retrieve the oddtrotter
        /// user extension
        /// </exception>
        /// <exception cref="ExtensionNotConfiguredException">Thrown if the oddtrotter user extension has not been configured for the current user</exception>
        /// <exception cref="GraphException">Thrown if graph produced an error while reading the oddtrotter user extension</exception>
        /// <exception cref="MalformedExtensionException">
        /// Thrown if the oddtrotter user extension is a malformed <see cref="AzureBlobClientFactory.OddTrotterUserExtension"/>
        /// </exception>
        /// <exception cref="MalformedExtensionDataException">
        /// Thrown if the <see cref="AzureBlobClientFactory.OddTrotterUserExtension.Data"/> in the oddtrotter user extension is malformed or <see langword="null"/>
        /// </exception>
        /// <exception cref="EncryptionException">
        /// Thrown if the password provided in the form data in <paramref name="httpRequestData"/> was not used to encrypt the
        /// <see cref="AzureBlobClientFactory.OddTrotterUserExtension.Data"/>
        /// </exception>
        /// <exception cref="MalformedSettingsException">
        /// Thrown if the decrypted <see cref="AzureBlobClientFactory.OddTrotterUserExtension.Data"/> is a malformed <see cref="AzureBlobClientFactory.OddTrotterSettings"/>
        /// </exception>
        public static async Task<OddTrotter> Create(IMemoryCache memoryCache, HttpRequestData httpRequestData, IConfiguration configuration)
        {
            if (memoryCache == null)
            {
                throw new ArgumentNullException(nameof(memoryCache));
            }

            if (httpRequestData == null)
            {
                throw new ArgumentNullException(nameof(httpRequestData));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var graphClient = GraphClientFactory.Create(httpRequestData, configuration);
            var encryptor = EncryptorFactory.Create(httpRequestData);
            var azureBlobClient = await AzureBlobClientFactory.Create(graphClient, encryptor).ConfigureAwait(false);
            var encryptedAzureBlobClient = EncryptedAzureBlobClientFactory.Create(azureBlobClient, encryptor);

            var partitionedMemoryCache = PartitionedMemoryCacheFactory.Create(httpRequestData.Id, memoryCache);
            var todoListService = new TodoListService(partitionedMemoryCache, graphClient, encryptedAzureBlobClient);

            return new OddTrotter(todoListService);
        }
    }
}
