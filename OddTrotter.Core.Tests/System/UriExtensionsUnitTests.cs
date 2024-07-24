namespace System
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OddTrotter;

    /// <summary>
    /// Unit tests for <see cref="UriExtensions"/>
    /// </summary>
    [TestCategory(TestCategories.Unit)]
    [TestClass]
    public sealed class UriExtensionsUnitTests
    {
        /// <summary>
        /// Creates a <see cref="RelativeUri"/> for a <see langword="null"/> <see cref="Uri"/>
        /// </summary>
        [TestMethod]
        public void NullUriToRelativeUri()
        {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            var uri = (Uri)null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8604 // Possible null reference argument.
            Assert.ThrowsException<ArgumentNullException>(() => uri.ToRelativeUri());
#pragma warning restore CS8604 // Possible null reference argument.
        }

        /// <summary>
        /// Creates a <see cref="RelativeUri"/> from a <see cref="Uri"/> with a absolute URI
        /// </summary>
        [TestMethod]
        public void AbsoluteUriToRelativeUri()
        {
            var uri = new Uri("https://www.test.com/index.html", UriKind.Absolute);
            Assert.ThrowsException<ArgumentException>(() => uri.ToRelativeUri());

            uri = new Uri("https://www.test.com/index.html", UriKind.RelativeOrAbsolute);
            Assert.ThrowsException<ArgumentException>(() => uri.ToRelativeUri());
        }

        /// <summary>
        /// Creates a <see cref="AbsoluteUri"/> from a <see cref="Uri"/> with a relative URI
        /// </summary>
        [TestMethod]
        public void RelativeUriToRelativeUri()
        {
            var uri = new Uri("/index.html", UriKind.Relative);
            var absoluteUri = uri.ToRelativeUri();
            Assert.IsFalse(absoluteUri.IsAbsoluteUri);

            uri = new Uri("/index.html", UriKind.RelativeOrAbsolute);
            absoluteUri = uri.ToRelativeUri();
            Assert.IsFalse(absoluteUri.IsAbsoluteUri);
        }

        /// <summary>
        /// Creates a <see cref="AbsoluteUri"/> for a <see langword="null"/> <see cref="Uri"/>
        /// </summary>
        [TestMethod]
        public void NullUriToAbsoluteUri()
        {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            var uri = (Uri)null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8604 // Possible null reference argument.
            Assert.ThrowsException<ArgumentNullException>(() => uri.ToAbsoluteUri());
#pragma warning restore CS8604 // Possible null reference argument.
        }

        /// <summary>
        /// Creates a <see cref="AbsoluteUri"/> from a <see cref="Uri"/> with a relative URI
        /// </summary>
        [TestMethod]
        public void AbsoluteUriToAbsoluteUri()
        {
            var uri = new Uri("/index.html", UriKind.Relative);
            Assert.ThrowsException<ArgumentException>(() => uri.ToAbsoluteUri());

            uri = new Uri("/index.html", UriKind.RelativeOrAbsolute);
            Assert.ThrowsException<ArgumentException>(() => uri.ToAbsoluteUri());
        }

        /// <summary>
        /// Creates a <see cref="AbsoluteUri"/> from a <see cref="Uri"/> with an absolute URI
        /// </summary>
        [TestMethod]
        public void RelativeUriToAbsoluteUri()
        {
            var uri = new Uri("https://www.test.com/index.html", UriKind.Absolute);
            var absoluteUri = uri.ToAbsoluteUri();
            Assert.IsTrue(absoluteUri.IsAbsoluteUri);

            uri = new Uri("https://www.test.com/index.html", UriKind.RelativeOrAbsolute);
            absoluteUri = uri.ToAbsoluteUri();
            Assert.IsTrue(absoluteUri.IsAbsoluteUri);
        }
    }
}
