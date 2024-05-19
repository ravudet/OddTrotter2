namespace OddTrotter.Encryptor
{
    using System;

    public sealed class EncryptionException : Exception
    {
        public EncryptionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
