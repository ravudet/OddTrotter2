namespace System
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for <see cref="AbsoluteUri"/>
    /// </summary>
    [TestClass]
    public sealed class AbsoluteUriUnitTests
    {
        /// <summary>
        /// Creates a <see cref="AbsoluteUri"/> for a <see langword="null"/> <see cref="Uri"/>
        /// </summary>
        [TestMethod]
        public void NullUri()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsException<ArgumentNullException>(() => new AbsoluteUri(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        /// <summary>
        /// Creates a <see cref="AbsoluteUri"/> from a <see cref="Uri"/> with a relative URI
        /// </summary>
        [TestMethod]
        public void RelativeUri()
        {
            var uri = new Uri("/index.html", UriKind.Relative);
            Assert.ThrowsException<ArgumentException>(() => new AbsoluteUri(uri));

            uri = new Uri("/index.html", UriKind.RelativeOrAbsolute);
            Assert.ThrowsException<ArgumentException>(() => new AbsoluteUri(uri));
        }

        /// <summary>
        /// Creates a <see cref="AbsoluteUri"/> from a <see cref="Uri"/> with an absolute URI
        /// </summary>
        [TestMethod]
        public void AbsoluteUri()
        {
            var uri = new Uri("https://www.test.com/index.html", UriKind.Absolute);
            var absoluteUri = new AbsoluteUri(uri);
            Assert.IsTrue(absoluteUri.IsAbsoluteUri);

            uri = new Uri("https://www.test.com/index.html", UriKind.RelativeOrAbsolute);
            absoluteUri = new AbsoluteUri(uri);
            Assert.IsTrue(absoluteUri.IsAbsoluteUri);
        }
    }
}