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
            this.sasToken = sasToken;
        }

        public string BlobContainerUrl { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>read, write, and create permissions are required</remarks>
        public string sasToken { get; }

        public sealed class Builder
        {
            [JsonRequired]
            [JsonPropertyName("blobContainerUrl")]
            public string? BlobContainerUrl { get; set; }

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
