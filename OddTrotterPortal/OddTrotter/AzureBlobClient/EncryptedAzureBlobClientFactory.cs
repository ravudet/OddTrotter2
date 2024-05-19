namespace OddTrotter.AzureBlobClient
{
    using System;

    using global::OddTrotter.Encryptor;

    public static class EncryptedAzureBlobClientFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="azureBlobClient"></param>
        /// <param name="encryptor"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="azureBlobClient"/> or <paramref name="encryptor"/> is <see langword="null"/></exception>
        public static EncryptedAzureBlobClient Create(IAzureBlobClient azureBlobClient, Encryptor encryptor)
        {
            if (azureBlobClient == null)
            {
                throw new ArgumentNullException(nameof(azureBlobClient));
            }

            if (encryptor == null)
            {
                throw new ArgumentNullException(nameof(encryptor));
            }

            return new EncryptedAzureBlobClient(azureBlobClient, encryptor);
        }
    }
}
