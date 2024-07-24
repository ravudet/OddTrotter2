namespace OddTrotter
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    /// <summary>
    /// Unit tests for <see cref="OddTrotterBlobSettings"/>
    /// </summary>
    [TestCategory(TestCategories.Unit)]
    [TestClass]
    public sealed class OddTrotterBlobSettingsUnitTests
    {
        /// <summary>
        /// Creates default <see cref="OddTrotterBlobSettings"/>
        /// </summary>
        [TestMethod]
        public void DefaultSettings()
        {
            var builder = new OddTrotterBlobSettings.Builder()
            {
                BlobContainerUrl = "https://a.container",
                SasToken = "sometoken",
            };
            var settings = builder.Build();

            Assert.AreEqual(builder.BlobContainerUrl, settings.BlobContainerUrl);
            Assert.AreEqual(builder.SasToken, settings.SasToken);
        }

        /// <summary>
        /// Creates <see cref="OddTrotterBlobSettings"/> with a <see langword="null"/> blob container URL
        /// </summary>
        [TestMethod]
        public void NullBlobContainerUrl()
        {
            var builder = new OddTrotterBlobSettings.Builder()
            {
                BlobContainerUrl = null,
                SasToken = "sometoken",
            };

            Assert.ThrowsException<ArgumentNullException>(() => builder.Build());
        }

        /// <summary>
        /// Creates <see cref="OddTrotterBlobSettings"/> with a <see langword="null"/> blob container URL
        /// </summary>
        [TestMethod]
        public void NullSasToken()
        {
            var builder = new OddTrotterBlobSettings.Builder()
            {
                BlobContainerUrl = "https://a.container",
                SasToken = null,
            };

            Assert.ThrowsException<ArgumentNullException>(() => builder.Build());
        }
    }
}
