namespace OddTrotter.AzureBlobClient
{
    using System;

    public sealed class MalformedExtensionDataException : Exception
    {
        public MalformedExtensionDataException(string url, string message, Exception innerException)
            : base(message, innerException)
        {
            Url = url;
        }

        public MalformedExtensionDataException(string url, string message)
            : base(message)
        {
            Url = url;
        }

        public string Url { get; }
    }
}
