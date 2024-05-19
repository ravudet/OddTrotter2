namespace OddTrotter.AzureBlobClient
{
    using System;
    using System.Net.Http.Headers;

    public sealed class AzureBlobClientSettings
    {
        private AzureBlobClientSettings(string blobType)
        {
            this.BlobType = blobType;
        }

        public string BlobType { get; }

        public sealed class Builder
        {
            public string BlobType { get; set; } = "BlockBlob";

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown if <see cref="BlobType"/> is <see langword="null"/></exception>
            /// <exception cref="ArgumentException">Thrown if <see cref="BlobType"/> contains only whitespace characters or is not a valid header value</exception>
            public AzureBlobClientSettings Build()
            {
                if (this.BlobType == null)
                {
                    throw new ArgumentNullException(nameof(this.BlobType));
                }

                if (string.IsNullOrWhiteSpace(this.BlobType))
                {
                    throw new ArgumentException($"'{nameof(this.BlobType)}' cannot be whitespace-only");
                }

                try
                {
                    new FakeHeaders().Add("x-ms-blob-type", this.BlobType);
                }
                catch (FormatException)
                {
                    throw new ArgumentException($"'{nameof(this.BlobType)}' is not a valid header value");
                }

                return new AzureBlobClientSettings(this.BlobType);
            }

            private sealed class FakeHeaders : HttpHeaders
            {
                public FakeHeaders()
                {
                }
            }
        }
    }
}
