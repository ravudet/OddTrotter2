namespace OddTrotter.Encryptor
{
    using System;
    using System.Linq;

    public static class EncryptorFactory
    {
        private const string extensionEncryptionPasswordFormFieldName = "oddTrotterEncryptionPassword";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpRequestData"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="httpRequestData"/> is <see langword="null"/></exception>
        /// <exception cref="MissingFormDataException">Thrown if the form data in <paramref name="httpRequestData"/> contained missing fields</exception>
        /// <exception cref="InvalidFormDataException">Thrown if the form data in <paramref name="httpRequestData"/> contained fields with invalid data</exception>
        public static Encryptor Create(HttpRequestData httpRequestData)
        {
            if (httpRequestData == null)
            {
                throw new ArgumentNullException(nameof(httpRequestData));
            }

            var extensionEncryptionPassword = GetExtensionEncryptionPassword(httpRequestData);
            var encryptorSettings = new EncryptorSettings.Builder()
            {
                Password = extensionEncryptionPassword,
            }.Build();
            return new Encryptor(encryptorSettings);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpRequestData"></param>
        /// <returns></returns>
        /// <exception cref="MissingFormDataException">Thrown if the form data in <paramref name="httpRequestData"/> contained missing fields</exception>
        /// <exception cref="InvalidFormDataException">Thrown if the form data in <paramref name="httpRequestData"/> contained fields with invalid data</exception>
        private static string GetExtensionEncryptionPassword(HttpRequestData httpRequestData)
        {
            if (!httpRequestData.Form.TryGetValue(extensionEncryptionPasswordFormFieldName, out var extensionEncryptionPasswords))
            {
                throw new MissingFormDataException(new[] { extensionEncryptionPasswordFormFieldName });
            }

            if (extensionEncryptionPasswords.Count == 0)
            {
                throw new MissingFormDataException(new[] { extensionEncryptionPasswordFormFieldName });
            }

            string? extensionEncryptionPassword;
            try
            {
                extensionEncryptionPassword = extensionEncryptionPasswords.Single();
            }
            catch (InvalidOperationException)
            {
                throw new InvalidFormDataException(extensionEncryptionPasswordFormFieldName);
            }

            if (string.IsNullOrWhiteSpace(extensionEncryptionPassword))
            {
                throw new MissingFormDataException(new[] { extensionEncryptionPasswordFormFieldName });
            }

            return extensionEncryptionPassword;
        }
    }
}
