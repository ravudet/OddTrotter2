namespace OddTrotter.AzureBlobClient
{
    using System;

    public sealed class SasTokenNoWritePrivilegesException : Exception
    {
        public SasTokenNoWritePrivilegesException(string token, string blobUrl, string message)
            : base(message)
        {
            this.Token = token;
            this.BlobUrl = blobUrl;
        }

        public string Token { get; }

        public string BlobUrl { get; }
    }
}
