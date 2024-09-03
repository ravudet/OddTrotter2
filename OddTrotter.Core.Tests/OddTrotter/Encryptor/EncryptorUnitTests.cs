namespace OddTrotter.Encryptor
{
    using System;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for <see cref="Encryptor"/>
    /// </summary>
    [TestCategory(TestCategories.Unit)]
    [TestClass]
    public sealed class EncryptorUnitTests
    {
        /// <summary>
        /// Creates a <see cref="Encryptor"/> with <see langword="null"/> settings
        /// </summary>
        [TestMethod]
        public void NullSettings()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new Encryptor(
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                ));
        }

        /// <summary>
        /// Encrypts data and then decrypts it using default settings
        /// </summary>
        [TestMethod]
        public void EncryptAndDecrypt()
        {
            var encryptor = new Encryptor();

            var plainText = "some data";
            var cipherText = encryptor.Encrypt(plainText);
            var decryptedPlaintext = encryptor.Decrypt(cipherText);
            Assert.AreEqual(plainText, decryptedPlaintext);
        }

        /// <summary>
        /// Encrypts data and then decrypts it using custom settings
        /// </summary>
        [TestMethod]
        public void EncryptAndDecryptWithPassword()
        {
            var defaultEncryptor = new Encryptor();

            var settings = new EncryptorSettings.Builder()
            {
                Password = "password",
            }.Build();
            var encryptor = new Encryptor(settings);

            var plainText = "some data";
            var cipherText = encryptor.Encrypt(plainText);
            var defaultCipherText = defaultEncryptor.Encrypt(plainText);
            Assert.AreNotEqual(defaultCipherText, cipherText);

            var decryptedPlaintext = encryptor.Decrypt(cipherText);
            Assert.AreEqual(plainText, decryptedPlaintext);
        }

        /// <summary>
        /// Encrypts <see langword="null"/> data
        /// </summary>
        [TestMethod]
        public void EncryptNullData()
        {
            var encryptor = new Encryptor();
            Assert.ThrowsException<ArgumentNullException>(() => encryptor.Encrypt(
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                (string?)null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                ));
        }

        /// <summary>
        /// Encrypts a very long data string
        /// </summary>
        [TestMethod]
        public void EncryptLengthData()
        {
            var encryptor = new Encryptor();
            var plainText = new string(' ', StringUtilities.MaxLength);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => encryptor.Encrypt(plainText));
        }

        /// <summary>
        /// Decrypts <see langword="null"/> data
        /// </summary>
        [TestMethod]
        public void DecryptNullData()
        {
            var encryptor = new Encryptor();
            Assert.ThrowsException<ArgumentNullException>(() => encryptor.Decrypt(
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                ));
        }

        /// <summary>
        /// Decrypts a very short ciphertext
        /// </summary>
        [TestMethod]
        public void DecryptShortData()
        {
            var encryptor = new Encryptor();
            var cipherText = new byte[2];
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => encryptor.Decrypt(cipherText));
        }

        /// <summary>
        /// Decrypts ciphertext using a password that is different from the one that encrypted it
        /// </summary>
        [TestMethod]
        public void DecryptWithWrongPassword()
        {
            var settings = new EncryptorSettings.Builder()
            {
                Password = "password",
            }.Build();
            var encryptor = new Encryptor(settings);

            var defaultEncryptor = new Encryptor();

            var plaintext = "some data";
            var cipherText = encryptor.Encrypt(plaintext);
            Assert.ThrowsException<EncryptionException>(() => defaultEncryptor.Decrypt(cipherText));
        }

        [TestMethod]
        public void EncryptLongData()
        {
            using (var memoryStream = new ChunkedMemoryStream(new byte[int.MaxValue], true))
            {
                memoryStream.Write()
            }
        }
    }
}
