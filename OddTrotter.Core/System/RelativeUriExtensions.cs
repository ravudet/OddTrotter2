namespace System
{
    /// <summary>
    /// Extension methods for <see cref="RelativeUri"/>
    /// </summary>
    public static class RelativeUriExtensions
    {
        /// <summary>
        /// Gets the specified components of the <paramref name="relativeUri"/> using the specified escaping for special characters.
        /// </summary>
        /// <param name="relativeUri">The URI to parse the components of</param>
        /// <param name="relativeUriComponents">A bitwise combination of the <see cref="RelativeUriComponents"/> values that specifies which parts of the current instance to return to the caller.</param>
        /// <param name="uriFormat">One of the enumeration values that controls how special characters are escaped.</param>
        /// <returns>The components of the <paramref name="relativeUri"/>, or <see cref="string.Empty"/> if those components are not present in <paramref name="relativeUri"/> TODO write tests for this.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="relativeUri"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="relativeUriComponents"/> is not a valid combination of <see cref="RelativeUriComponents"/></exception>
        public static string GetComponents(this RelativeUri relativeUri, RelativeUriComponents relativeUriComponents, UriFormat uriFormat)
        {
            if (relativeUri == null)
            {
                throw new ArgumentNullException(nameof(relativeUri));
            }

            var uriComponents = (UriComponents)0;
            if ((relativeUriComponents & RelativeUriComponents.Path) == RelativeUriComponents.Path)
            {
                uriComponents |= UriComponents.Path;
                relativeUriComponents ^= RelativeUriComponents.Path;
            }

            if ((relativeUriComponents & RelativeUriComponents.Query) == RelativeUriComponents.Query)
            {
                uriComponents |= UriComponents.Query;
                relativeUriComponents &= ~RelativeUriComponents.Query;
            }

            if ((relativeUriComponents & RelativeUriComponents.Fragment) == RelativeUriComponents.Fragment)
            {
                uriComponents |= UriComponents.Fragment;
                relativeUriComponents &= ~RelativeUriComponents.Fragment;
            }

            if ((relativeUriComponents & RelativeUriComponents.KeepDelimiter) == RelativeUriComponents.KeepDelimiter)
            {
                uriComponents |= UriComponents.KeepDelimiter;
                relativeUriComponents &= ~RelativeUriComponents.KeepDelimiter;
            }

            if (uriComponents == (UriComponents)0 || relativeUriComponents != (RelativeUriComponents)0)
            {
                throw new ArgumentOutOfRangeException(nameof(relativeUriComponents));
            }

            var absoluteUri = new Uri(new Uri("https://localhost/"), relativeUri);
            return absoluteUri.GetComponents(uriComponents, uriFormat);
        }
    }
}
