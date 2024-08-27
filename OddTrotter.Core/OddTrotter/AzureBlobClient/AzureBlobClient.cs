namespace OddTrotter.AzureBlobClient
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// A client for azure blob storage. The azure storage account that the blob container is in can have the following "known-good" configuration:
    /// - region: west us
    /// - primary service: unselected
    /// - performance: standard
    /// - redundancy: geo-redundant storage with "make read access to data available in the event of regional unavailability"
    /// - require secure transfer for rest api operations: checked
    /// - allow enabling anonymous access on individual containers: unchecked
    /// - enable storage account key access: checked
    /// - default to microsoft entra authorization in azure portal: unchecked
    /// - minimum tls version: version 1.2
    /// - permitted scope for copy operaetions: from any storage account
    /// - enable hierarchical namespaces: unchecked
    /// - enable sftp: unchecked
    /// - enable network file system v3: unchecked
    /// - network access: enable public access from all networks
    /// - routing preference: microsoft network routing
    /// - enable point-in-time restore for containers: unchecked
    /// - enable soft delete for blobs: unchecked
    /// - enable soft delete for containers: unchecked
    /// - enable soft delete for file shares: checked
    /// - enable versioning for blobs: unchecked
    /// - enable blob change feed: unchecked
    /// - enable version-level immutability support: unchecked
    /// - encryption type: microsoft-managed keys
    /// - enable support for customer-managed keys: blobs and files only
    /// - enable infrastructure encryption: unchecked
    /// </summary>
    public sealed class AzureBlobClient : IAzureBlobClient
    {
        private readonly string containerUri;

        private readonly string sasToken;

        private readonly string apiVersion;

        private readonly string blobType;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="containerUri">
        /// The URI for an azure blob container that is in a storage account with the following configuration:
        /// - region: west us
        /// - primary service: unselected
        /// - performance: standard
        /// - redundancy: geo-redundant storage with "make read access to data available in the event of regional unavailability"
        /// - require secure transfer for rest api operations: checked
        /// - allow enabling anonymous access on individual containers: unchecked
        /// - enable storage account key access: checked
        /// - default to microsoft entra authorization in azure portal: unchecked
        /// - minimum tls version: version 1.2
        /// - permitted scope for copy operaetions: from any storage account
        /// - enable hierarchical namespaces: unchecked
        /// - enable sftp: unchecked
        /// - enable network file system v3: unchecked
        /// - network access: enable public access from all networks
        /// - routing preference: microsoft network routing
        /// - enable point-in-time restore for containers: unchecked
        /// - enable soft delete for blobs: unchecked
        /// - enable soft delete for containers: unchecked
        /// - enable soft delete for file shares: checked
        /// - enable versioning for blobs: unchecked
        /// - enable blob change feed: unchecked
        /// - enable version-level immutability support: unchecked
        /// - encryption type: microsoft-managed keys
        /// - enable support for customer-managed keys: blobs and files only
        /// - enable infrastructure encryption: unchecked
        /// </param>
        /// <param name="sasToken">A SAS token that has read, create, and write permissions for the azure storage account at <paramref name="containerUri"/></param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="containerUri"/> or <paramref name="sasToken"/> or <paramref name="apiVersion"/> is <see langword="null"/>
        /// </exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="apiVersion"/> is empty</exception>
        public AzureBlobClient(AbsoluteUri containerUri, string sasToken, string apiVersion)
            : this(containerUri, sasToken, apiVersion, AzureBlobClientSettings.Default)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="containerUri">
        /// The URI for an azure blob container that is in a storage account with the following configuration:
        /// - region: west us
        /// - primary service: unselected
        /// - performance: standard
        /// - redundancy: geo-redundant storage with "make read access to data available in the event of regional unavailability"
        /// - require secure transfer for rest api operations: checked
        /// - allow enabling anonymous access on individual containers: unchecked
        /// - enable storage account key access: checked
        /// - default to microsoft entra authorization in azure portal: unchecked
        /// - minimum tls version: version 1.2
        /// - permitted scope for copy operaetions: from any storage account
        /// - enable hierarchical namespaces: unchecked
        /// - enable sftp: unchecked
        /// - enable network file system v3: unchecked
        /// - network access: enable public access from all networks
        /// - routing preference: microsoft network routing
        /// - enable point-in-time restore for containers: unchecked
        /// - enable soft delete for blobs: unchecked
        /// - enable soft delete for containers: unchecked
        /// - enable soft delete for file shares: checked
        /// - enable versioning for blobs: unchecked
        /// - enable blob change feed: unchecked
        /// - enable version-level immutability support: unchecked
        /// - encryption type: microsoft-managed keys
        /// - enable support for customer-managed keys: blobs and files only
        /// - enable infrastructure encryption: unchecked
        /// </param>
        /// <param name="sasToken">A SAS token that has read, create, and write permissions for the azure storage account at <paramref name="containerUri"/></param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="containerUri"/> or <paramref name="sasToken"/> or <paramref name="apiVersion"/> or <paramref name="settings"/> is <see langword="null"/>
        /// </exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="apiVersion"/> is empty</exception>
        public AzureBlobClient(AbsoluteUri containerUri, string sasToken, string apiVersion, AzureBlobClientSettings settings)
        {
            if (containerUri == null)
            {
                throw new ArgumentNullException(nameof(containerUri));
            }

            if (sasToken == null)
            {
                throw new ArgumentNullException(nameof(sasToken));
            }

            if (apiVersion == null)
            {
                throw new ArgumentNullException(nameof(apiVersion));
            }

            if (string.IsNullOrEmpty(apiVersion))
            {
                throw new ArgumentException($"'{nameof(apiVersion)}' cannot be empty");
            }

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            this.containerUri = containerUri.OriginalString.TrimEnd('/');
            this.sasToken = sasToken;

            this.apiVersion = apiVersion;
            this.blobType = settings.BlobType;
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> GetAsync(string blobName)
        {
            if (blobName == null)
            {
                throw new ArgumentNullException(nameof(blobName));
            }

            if (string.IsNullOrEmpty(blobName))
            {
                throw new ArgumentException($"'{nameof(blobName)}' cannot be empty");
            }

            var blobUrl = GenerateBlobUrl(blobName);
            using (var httpClient = new HttpClient()) //// TODO a new client should not be created for every request
            {
                httpClient.DefaultRequestHeaders.Add("x-ms-date", DateTime.UtcNow.ToString());
                httpClient.DefaultRequestHeaders.Add("x-ms-version", this.apiVersion);

                HttpResponseMessage? httpResponseMessage = null;
                try
                {
                    httpResponseMessage = await httpClient.GetAsync(blobUrl).ConfigureAwait(false);
                    if (httpResponseMessage.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        var responseContent = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new InvalidSasTokenException(this.sasToken, responseContent);
                    }
                    else if (httpResponseMessage.StatusCode == HttpStatusCode.Forbidden)
                    {
                        var responseContent = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new SasTokenNoReadPrivilegesException(this.sasToken, blobUrl.OriginalString, responseContent);
                    }
                }
                catch
                {
                    httpResponseMessage?.Dispose();
                    throw;
                }

                return httpResponseMessage;
            }
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> PutAsync(string blobName, HttpContent httpContent)
        {
            if (blobName == null)
            {
                throw new ArgumentNullException(nameof(blobName));
            }

            if (string.IsNullOrEmpty(blobName))
            {
                throw new ArgumentException($"'{nameof(blobName)}' cannot be empty");
            }

            if (httpContent == null)
            {
                throw new ArgumentNullException(nameof(httpContent));
            }

            var blobUrl = GenerateBlobUrl(blobName);
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("x-ms-date", DateTime.UtcNow.ToString());
                httpClient.DefaultRequestHeaders.Add("x-ms-version", this.apiVersion);
                httpClient.DefaultRequestHeaders.Add("x-ms-blob-type", this.blobType);

                HttpResponseMessage? httpResponseMessage = null;
                try
                {
                    httpResponseMessage = await httpClient.PutAsync(blobUrl, httpContent).ConfigureAwait(false);
                    if (httpResponseMessage.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        var responseContent = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new InvalidSasTokenException(this.sasToken, responseContent);
                    }
                    else if (httpResponseMessage.StatusCode == HttpStatusCode.Forbidden)
                    {
                        var responseContent = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new SasTokenNoWritePrivilegesException(this.sasToken, blobUrl.OriginalString, responseContent);
                    }
                }
                catch
                {
                    httpResponseMessage?.Dispose();
                    throw;
                }

                return httpResponseMessage;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blobName"></param>
        /// <returns></returns>
        private Uri GenerateBlobUrl(string blobName)
        {
            // this may be the source of a bug, but i can't figure out any case that UriFormatException is thrown, so we are not catching it and handling it in any way; if we discover a case where the exception is thrown, we should use that exception to validate blob names and SAS tokens
            return new Uri($"{this.containerUri}/{blobName}?{this.sasToken}");
        }
    }
}
