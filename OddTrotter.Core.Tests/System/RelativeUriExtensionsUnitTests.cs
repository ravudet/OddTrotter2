namespace System
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OddTrotter;

    /// <summary>
    /// Unit test for <see cref="RelativeUriExtensions"/>
    /// </summary>
    [TestCategory(TestCategories.Unit)]
    [TestClass]
    public sealed class RelativeUriExtensionsUnitTests
    {
        /// <summary>
        /// Gets the components of a <see langword="null"/> URI
        /// </summary>
        [TestMethod]
        public void NullUri()
        {
            RelativeUri relativeUri =
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

            Assert.ThrowsException<ArgumentNullException>(() =>
#pragma warning disable CS8604 // Possible null reference argument.
                relativeUri
#pragma warning restore CS8604 // Possible null reference argument.
                    .GetComponents(RelativeUriComponents.Query, UriFormat.Unescaped));
        }

        /// <summary>
        /// Gets the components of a <see langword="null"/> URI
        /// </summary>
        [TestMethod]
        public void InvalidComponents()
        {
            var relativeUri = new Uri("/test/uri?some=query#fragment", UriKind.Relative).ToRelativeUri();

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => relativeUri.GetComponents((RelativeUriComponents)0, UriFormat.Unescaped));

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => relativeUri
                .GetComponents((RelativeUriComponents)((int)(RelativeUriComponents.Query | RelativeUriComponents.Fragment | RelativeUriComponents.Path) + 1), UriFormat.Unescaped));
        }

        /// <summary>
        /// Gets the path component of a relative URI
        /// </summary>
        [TestMethod]
        public void GetPathComponent()
        {
            var relativeUri = new Uri("/test/uri?some=query#fragment", UriKind.Relative).ToRelativeUri();

            var path = relativeUri.GetComponents(RelativeUriComponents.Path, UriFormat.UriEscaped);

            Assert.AreEqual("test/uri", path);
        }

        /// <summary>
        /// Gets the path component of a relative URI
        /// </summary>
        [TestMethod]
        public void GetPathComponentWithDelimiter()
        {
            var relativeUri = new Uri("/test/uri?some=query#fragment", UriKind.Relative).ToRelativeUri();

            var path = relativeUri.GetComponents(RelativeUriComponents.Path | RelativeUriComponents.KeepDelimiter, UriFormat.UriEscaped);

            Assert.AreEqual("/test/uri", path);
        }

        /// <summary>
        /// Gets the query component of a relative URI
        /// </summary>
        [TestMethod]
        public void GetQueryComponent()
        {
            var relativeUri = new Uri("/test/uri?some=query#fragment", UriKind.Relative).ToRelativeUri();

            var path = relativeUri.GetComponents(RelativeUriComponents.Query, UriFormat.UriEscaped);

            Assert.AreEqual("some=query", path);
        }

        /// <summary>
        /// Gets the query component of a relative URI
        /// </summary>
        [TestMethod]
        public void GetQueryComponentWithDelimiter()
        {
            var relativeUri = new Uri("/test/uri?some=query#fragment", UriKind.Relative).ToRelativeUri();

            var path = relativeUri.GetComponents(RelativeUriComponents.Query | RelativeUriComponents.KeepDelimiter, UriFormat.UriEscaped);

            Assert.AreEqual("?some=query", path);
        }

        /// <summary>
        /// Gets the fragment component of a relative URI
        /// </summary>
        [TestMethod]
        public void GetFragmentComponent()
        {
            var relativeUri = new Uri("/test/uri?some=query#fragment", UriKind.Relative).ToRelativeUri();

            var path = relativeUri.GetComponents(RelativeUriComponents.Fragment, UriFormat.UriEscaped);

            Assert.AreEqual("fragment", path);
        }

        /// <summary>
        /// Gets the fragment component of a relative URI
        /// </summary>
        [TestMethod]
        public void GetFragmentComponentWithDelimiter()
        {
            var relativeUri = new Uri("/test/uri?some=query#fragment", UriKind.Relative).ToRelativeUri();

            var path = relativeUri.GetComponents(RelativeUriComponents.Fragment | RelativeUriComponents.KeepDelimiter, UriFormat.UriEscaped);

            Assert.AreEqual("#fragment", path);
        }

        /// <summary>
        /// Gets all components of a relative URI
        /// </summary>
        [TestMethod]
        public void GetAllComponents()
        {
            var relativeUri = new Uri("/test/uri?some=query#fragment", UriKind.Relative).ToRelativeUri();

            var path = relativeUri.GetComponents(RelativeUriComponents.Path | RelativeUriComponents.Query | RelativeUriComponents.Fragment, UriFormat.UriEscaped);

            Assert.AreEqual("/test/uri?some=query#fragment", path);
        }

        /// <summary>
        /// Gets all components of a relative URI
        /// </summary>
        [TestMethod]
        public void GetAllComponentsWithDelimiters()
        {
            var relativeUri = new Uri("/test/uri?some=query#fragment", UriKind.Relative).ToRelativeUri();

            var path = relativeUri.GetComponents(RelativeUriComponents.Path | RelativeUriComponents.Query | RelativeUriComponents.Fragment | RelativeUriComponents.KeepDelimiter, UriFormat.UriEscaped);

            Assert.AreEqual("/test/uri?some=query#fragment", path);
        }
    }
}
