namespace OddTrotter.Encryptor
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using System;

    /// <summary>
    /// Unit tests for <see cref="EncryptorSettings"/>
    /// </summary>
    [TestClass]
    public sealed class EncryptorSettingsUnitTests
    {
        /// <summary>
        /// Creates default <see cref="EncryptorSettings"/>
        /// </summary>
        [TestMethod]
        public void DefaultSettings()
        {
            new EncryptorSettings.Builder()
            {
            }.Build();
        }

        /// <summary>
        /// Creates <see cref="EncryptorSettings"/> with a <see langword="null"/> <see cref="EncryptorSettings.Encoding"/>
        /// </summary>
        [TestMethod]
        public void NullEncoding()
        {
            var builder = new EncryptorSettings.Builder()
            {
                Encoding =
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                    ,
            };
            Assert.ThrowsException<ArgumentNullException>(() => builder.Build());
        }

        /// <summary>
        /// Creates <see cref="EncryptorSettings"/> with a <see langword="null"/> <see cref="EncryptorSettings.Password"/>
        /// </summary>
        [TestMethod]
        public void NullPassword()
        {
            var builder = new EncryptorSettings.Builder()
            {
                Password =
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                    ,
            };
            Assert.ThrowsException<ArgumentNullException>(() => builder.Build());
        }
    }
}
