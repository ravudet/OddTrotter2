namespace OddTrotter.Calendar
{
    using System;

    /// <summary>
    /// TODO i really don't like this class as written, there was basically no through put into it; revisit this
    /// </summary>
    public sealed class UriPath
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="path"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="path"/> is the empty string or contains more URI components than just a URI path or has a trailing '/'</exception>
        public UriPath(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("TODO", nameof(path));
            }

            if (new Uri(path).IsAbsoluteUri)
            {
                throw new ArgumentException("TODO", nameof(path));
            }

            if (path.Contains("?", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("TODO", nameof(path));
            }

            if (path.EndsWith("/", StringComparison.OrdinalIgnoreCase)) //// TODO is this check a good idea?
            {
                throw new ArgumentException("TODO", nameof(path));
            }

            this.Path = path;
        }

        public string Path { get; }
    }
}
