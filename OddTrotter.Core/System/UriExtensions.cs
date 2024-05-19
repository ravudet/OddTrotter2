namespace System
{
    public static class UriExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="uri"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="uri"/> is not an absolute URI</exception>
        public static AbsoluteUri ToAbsoluteUri(this Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            return new AbsoluteUri(uri);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="uri"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="uri"/> is not a relative URI</exception>
        public static RelativeUri ToRelativeUri(this Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            return new RelativeUri(uri);
        }
    }
}
