namespace System
{
    public sealed class RelativeUri : Uri
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="uri"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="uri"/> is not a relative URI</exception>
        public RelativeUri(Uri uri)
            : base(
                uri == null ?
                    throw new ArgumentNullException(nameof(uri)) :
                    uri.IsAbsoluteUri ?
                        throw new ArgumentException($"{nameof(uri)} can only be a relative URI") :
                        uri.OriginalString,
                UriKind.Relative)
        {
        }
    }
}
