namespace OddTrotter.AzureBlobClient
{
    using System;

    public sealed class MalformedExtensionException : Exception
    {
        public MalformedExtensionException(string url, string message)
            : base(message)
        {
            Url = url;
        }

        public MalformedExtensionException(string url, string message, Exception innerException)
            : base(message, innerException)
        {
            Url = url;
        }

        public string Url { get; }
    }
}
