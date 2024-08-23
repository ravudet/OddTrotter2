namespace OddTrotter.TokenIssuance
{
    using System;

    public sealed class TokenIssuanceException : Exception
    {
        public TokenIssuanceException(string message)
            : base(message)
        {
        }

        public TokenIssuanceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
