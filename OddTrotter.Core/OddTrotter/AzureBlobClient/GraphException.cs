namespace OddTrotter.AzureBlobClient
{
    using System;
    using System.Net.Http;

    public sealed class GraphException : Exception
    {
        public GraphException(string message, HttpRequestException exception)
            : base(message, exception)
        {
        }
    }
}
