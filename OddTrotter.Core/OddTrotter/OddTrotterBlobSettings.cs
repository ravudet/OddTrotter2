namespace OddTrotter
{
    using System;
    using System.Text.Json.Serialization;

    public sealed class OddTrotterBlobSettings
    {
        private OddTrotterBlobSettings(string blobContainerUrl, string sasToken)
        {
            //// TODO version this
            this.BlobContainerUrl = blobContainerUrl;
            this.SasToken = sasToken;
        }

        /// <summary>
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
        /// </summary>
        [JsonPropertyName("blobContainerUrl")]
        public string BlobContainerUrl { get; }

        /// <summary>
        /// A SAS token that has read, create, and write permissions for the azure storage account at <see cref="BlobContainerUrl"/>
        /// </summary>
        [JsonPropertyName("sasToken")]
        public string SasToken { get; }

        public sealed class Builder
        {
            /// <summary>
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
            /// </summary>
            [JsonRequired]
            [JsonPropertyName("blobContainerUrl")]
            public string? BlobContainerUrl { get; set; }

            /// <summary>
            /// A SAS token that has read, create, and write permissions for the azure storage account at <see cref="BlobContainerUrl"/>
            /// </summary>
            [JsonRequired]
            [JsonPropertyName("sasToken")]
            public string? SasToken { get; set; }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown if <see cref="BlobContainerUrl"/> or <see cref="SasToken"/> is <see langword="null"/></exception>
            public OddTrotterBlobSettings Build()
            {
                if (this.BlobContainerUrl == null)
                {
                    throw new ArgumentNullException(nameof(this.BlobContainerUrl));
                }

                if (this.SasToken == null)
                {
                    throw new ArgumentNullException(nameof(this.SasToken));
                }

                return new OddTrotterBlobSettings(this.BlobContainerUrl, this.SasToken);
            }
        }
    }
}
