namespace OddTrotter.AzureBlobClient
{
    using System;

    public sealed class MalformedSettingsException : Exception
    {
        public MalformedSettingsException(string url, string message)
            : base(message)
        {
            Url = url;
        }

        public MalformedSettingsException(string url, string propertyName, string message)
            : base(message)
        {
            Url = url;
            PropertyName = propertyName;
        }

        public MalformedSettingsException(string url, string message, Exception innerException)
            : base(message, innerException)
        {
            Url = url;
        }

        public string Url { get; }

        public string? PropertyName { get; }
    }
}
