namespace OddTrotter.AzureBlobClient
{
    using System;

    public sealed class InvalidSasTokenException : Exception
    {
        public InvalidSasTokenException(string token, Exception exception)
            : base(token, exception)
        {
            this.Token = token;
        }

        public InvalidSasTokenException(string token, string message)
            : base(message)
        {
            this.Token = token;
        }

        public string Token { get; }
    }
}
