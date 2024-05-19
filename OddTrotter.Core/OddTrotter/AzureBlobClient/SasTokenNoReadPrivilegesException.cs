namespace OddTrotter.AzureBlobClient
{
    using System;

    public sealed class SasTokenNoReadPrivilegesException : Exception
    {
        public SasTokenNoReadPrivilegesException(string token, string blobUrl, string message)
            : base(message)
        {
            this.Token = token;
            this.BlobUrl = blobUrl;
        }
        
        public string BlobUrl { get; }

        public string Token { get; }
    }
}
