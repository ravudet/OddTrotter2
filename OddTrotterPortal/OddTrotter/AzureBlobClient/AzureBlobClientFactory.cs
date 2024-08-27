namespace OddTrotter.AzureBlobClient
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;

    using global::OddTrotter.Encryptor;
    using global::OddTrotter.GraphClient;
    using global::OddTrotter.UserExtension;

    public static class AzureBlobClientFactory
    {
        /// <summary>
        /// Reads the OddTrotter user extension data from Microsoft Graph using <paramref name="graphClient"/> and creates an azure blob client using the <see cref="OddTrotterBlobSettings"/> found in the user extension.
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="encryptor"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="graphClient"/> or <paramref name="encryptor"/> is <see langword="null"/></exception>
        /// <exception cref="HttpRequestException">
        /// Thrown if retrieving the oddtrotter user extension from graph failed due to an underlying issue such as network connectivity, DNS failure, server certificate
        /// validation or timeout
        /// </exception>
        /// <exception cref="UnauthorizedAccessTokenException">
        /// Thrown if the access token configured in <paramref name="graphClient"/> is invalid or provides insufficient privileges to retrieve the oddtrotter user extension
        /// </exception>
        /// <exception cref="ExtensionNotConfiguredException">Thrown if the oddtrotter user extension has not been configured for the current user</exception>
        /// <exception cref="GraphException">Thrown if graph produced an error while reading the oddtrotter user extension</exception>
        /// <exception cref="MalformedExtensionException">Thrown if the oddtrotter user extension is a malformed <see cref="OddTrotterUserExtension"/></exception>
        /// <exception cref="MalformedExtensionDataException">
        /// Thrown if the <see cref="OddTrotterUserExtension.Data"/> in the oddtrotter user extension is <see langword="null"/> or malformed
        /// </exception>
        /// <exception cref="EncryptionException">
        /// Thrown if the password configured in <paramref name="encryptor"/> was not used to encrypt the <see cref="OddTrotterUserExtension.Data"/>
        /// </exception>
        /// <exception cref="MalformedSettingsException">
        /// Thrown if the decrypted <see cref="OddTrotterUserExtension.Data"/> is a malformed <see cref="OddTrotterBlobSettings"/>
        /// </exception>
        public static async Task<AzureBlobClient> Create(IGraphClient graphClient, Encryptor encryptor)
        {
            if (graphClient == null)
            {
                throw new ArgumentNullException(nameof(graphClient));
            }

            if (encryptor == null)
            {
                throw new ArgumentNullException(nameof(encryptor));
            }

            //// TODO don't use "me", use the userid instead
            var extensionPath = "/me/extensions/microsoft.oddTrotter";
            OddTrotterBlobSettings oddTrotterSettings;
            using (var extensionHttpResponse = await graphClient.GetAsync(new Uri(extensionPath, UriKind.Relative).ToRelativeUri()).ConfigureAwait(false))
            {
                var extensionHttpResponseContent = await extensionHttpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (extensionHttpResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new ExtensionNotConfiguredException(extensionPath);
                }
                else
                {
                    try
                    {
                        extensionHttpResponse.EnsureSuccessStatusCode();
                    }
                    catch (HttpRequestException e)
                    {
                        throw new GraphException(extensionHttpResponseContent, e);
                    }
                }

                OddTrotterUserExtension? oddTrotterExtension;
                try
                {
                    oddTrotterExtension = JsonSerializer.Deserialize<OddTrotterUserExtension>(extensionHttpResponseContent);
                }
                catch (JsonException e)
                {
                    throw new MalformedExtensionException(extensionPath, extensionHttpResponseContent, e);
                }

                if (oddTrotterExtension == null)
                {
                    throw new MalformedExtensionException(extensionPath, extensionHttpResponseContent);
                }

                if (oddTrotterExtension.Data == null)
                {
                    throw new MalformedExtensionDataException(extensionPath, "The odd trotter extension data had a null value for data");
                }

                byte[] encryptedOddTrotterSettings;
                try
                {
                    encryptedOddTrotterSettings = Convert.FromBase64String(oddTrotterExtension.Data);
                }
                catch (FormatException e)
                {
                    throw new MalformedExtensionDataException(extensionPath, oddTrotterExtension.Data, e);
                }

                string serializedOddTrotterSettings;
                try
                {
                    serializedOddTrotterSettings = encryptor.Decrypt(encryptedOddTrotterSettings);
                }
                catch (ArgumentOutOfRangeException e)
                {
                    throw new EncryptionException(e.Message, e);
                }

                OddTrotterBlobSettings.Builder? oddTrotterSettingsBuilder;
                try
                {
                    oddTrotterSettingsBuilder = JsonSerializer.Deserialize<OddTrotterBlobSettings.Builder>(serializedOddTrotterSettings);
                }
                catch (JsonException e)
                {
                    throw new MalformedSettingsException(extensionPath, serializedOddTrotterSettings, e);
                }

                if (oddTrotterSettingsBuilder == null)
                {
                    throw new MalformedSettingsException(extensionPath, serializedOddTrotterSettings);
                }
                else if (oddTrotterSettingsBuilder.BlobContainerUrl == null)
                {
                    throw new MalformedSettingsException(extensionPath, nameof(oddTrotterSettingsBuilder.BlobContainerUrl), serializedOddTrotterSettings);
                }
                else if (oddTrotterSettingsBuilder.SasToken == null)
                {
                    throw new MalformedSettingsException(extensionPath, nameof(oddTrotterSettingsBuilder.SasToken), serializedOddTrotterSettings);
                }

                try
                {
                    new Uri(oddTrotterSettingsBuilder.BlobContainerUrl, UriKind.Absolute);
                }
                catch (Exception e) when (e is UriFormatException || e is ArgumentException)
                {
                    throw new MalformedSettingsException(extensionPath, nameof(oddTrotterSettings.BlobContainerUrl), serializedOddTrotterSettings);
                }

                oddTrotterSettings = oddTrotterSettingsBuilder.Build();
            }

            return new AzureBlobClient(
                new Uri(oddTrotterSettings.BlobContainerUrl, UriKind.Absolute).ToAbsoluteUri(),
                oddTrotterSettings.SasToken,
                "2020-02-10" // the api version is hard-coded instead of configured because callers of the class need to know the api version in order to call it correctly in the
                             // first place; this means that the api version is already known by those callers, so any need to change the api version must arise from net-new code,
                             // and so recompilation will be rqeuired anyway, making the configuration a moot point
                );
        }
    }
}
