namespace OddTrotter.GraphClient
{
    using System;

    public sealed class InvalidAccessTokenException : Exception
    {
        public InvalidAccessTokenException(string url, string accessToken, string message)
            : base(message)
        {
            Url = url;
            AccessToken = accessToken;
        }

        public string Url { get; }

        public string AccessToken { get; }
    }
}
