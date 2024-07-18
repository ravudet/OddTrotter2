namespace OddTrotter.UserExtension
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for <see cref="UserExtensionService"/>
    /// </summary>
    public static class UserExtensionServiceExtensions
    {
        private const string oddTrotterBlobContainerUrlFormFieldName = "oddTrotterBlobContainerUrl";

        private const string oddTrotterBlobContainerSasTokenFormFieldName = "oddTrotterBlobContainerSasToken";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpRequestData"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="userExtensionService"/> or <paramref name="httpRequestData"/> is <see langword="null"/></exception>
        /// <exception cref="MissingFormDataException">Thrown if the form data in <paramref name="httpRequestData"/> contained missing fields</exception>
        /// <exception cref="InvalidFormDataException">Thrown if the form data in <paramref name="httpRequestData"/> contained fields with invalid data</exception>
        /// <exception cref="UserExtensionEncryptionException">Thrown if the <see cref="OddTrotterBlobSettings"/> generated from <paramref name="httpRequestData"/> are too long to encrypt</exception>
        /// <exception cref="EncryptionException">Thrown if an error occurred encrypting the <see cref="OddTrotterBlobSettings"/> generated from <paramref name="httpRequestData"/></exception>
        /// <exception cref="HttpRequestException">
        /// Thrown if the request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        /// <exception cref="InvalidAccessTokenException">
        /// Thrown if the access token configured on the provided <see cref="IGraphClient"/> is invalid or provides insufficient privileges for the requests</exception>
        /// <exception cref="GraphException">Thrown if graph produced an error when writing the oddtrotter user extension</exception>
        public static async Task ConfigureUserExtension(this UserExtensionService userExtensionService, HttpRequestData httpRequestData)
        {
            if (userExtensionService == null)
            {
                throw new ArgumentNullException(nameof(userExtensionService));
            }

            if (httpRequestData == null)
            {
                throw new ArgumentNullException(nameof(httpRequestData));
            }

            if (!httpRequestData.Form.TryGetValue(oddTrotterBlobContainerUrlFormFieldName, out var blobContainerUrls))
            {
                throw new MissingFormDataException(new[] { oddTrotterBlobContainerUrlFormFieldName });
            }

            if (blobContainerUrls.Count == 0)
            {
                throw new MissingFormDataException(new[] { oddTrotterBlobContainerUrlFormFieldName });
            }

            string? blobContainerUrl;
            try
            {
                blobContainerUrl = blobContainerUrls.Single();
            }
            catch (InvalidOperationException)
            {
                throw new InvalidFormDataException(oddTrotterBlobContainerUrlFormFieldName);
            }

            if (string.IsNullOrEmpty(blobContainerUrl))
            {
                throw new InvalidFormDataException(oddTrotterBlobContainerUrlFormFieldName);
            }

            if (!httpRequestData.Form.TryGetValue(oddTrotterBlobContainerSasTokenFormFieldName, out var blobContainerSasTokens))
            {
                throw new MissingFormDataException(new[] { oddTrotterBlobContainerSasTokenFormFieldName });
            }

            if (blobContainerSasTokens.Count == 0)
            {
                throw new MissingFormDataException(new[] { oddTrotterBlobContainerSasTokenFormFieldName });
            }

            string? blobContainerSasToken;
            try
            {
                blobContainerSasToken = blobContainerSasTokens.Single();
            }
            catch (InvalidOperationException)
            {
                throw new InvalidFormDataException(oddTrotterBlobContainerSasTokenFormFieldName);
            }

            if (string.IsNullOrEmpty(blobContainerSasToken))
            {
                throw new InvalidFormDataException(oddTrotterBlobContainerSasTokenFormFieldName);
            }

            var settings = new OddTrotterBlobSettings.Builder()
            {
                BlobContainerUrl = blobContainerUrl,
                SasToken = blobContainerSasToken,
            }.Build();
            await userExtensionService.ConfigureUserExtension(settings).ConfigureAwait(false);
        }
    }
}
