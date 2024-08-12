namespace OddTrotter.GraphClient
{
    using System;

    public sealed class InvalidAccessTokenException : Exception
    {
        public InvalidAccessTokenException(string accessToken, string message, Exception innerException)
            : base(message, innerException)
        {
            AccessToken = accessToken;
        }

        public string AccessToken { get; }
    }
}
