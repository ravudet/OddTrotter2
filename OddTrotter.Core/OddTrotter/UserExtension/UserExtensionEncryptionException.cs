namespace OddTrotter.UserExtension
{
    using System;

    public sealed class UserExtensionEncryptionException : Exception
    {
        public UserExtensionEncryptionException(string message, string dataToEncrypt, Exception exception)
            : base(message, exception)
        {
            DataToEncrypt = dataToEncrypt;
        }

        public string DataToEncrypt { get; }
    }
}
