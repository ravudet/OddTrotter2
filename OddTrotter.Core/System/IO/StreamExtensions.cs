namespace System.IO
{
    using System.Globalization;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for <see cref="Stream"/>
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="stream"/> or <paramref name="buffer"/> is <see langword="null"/></exception>
        /// <exception cref="NotSupportedException">Thrown if <paramref name="stream"/> does not support reading</exception>
        /// <exception cref="ObjectDisposedException">Thrown if <paramref name="stream"/> is disposed</exception>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="stream"/> is currently in use by a previous read operation</exception>
        public static async Task ReadBufferAsync(this Stream stream, byte[] buffer)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (!stream.CanRead)
            {
                throw new NotSupportedException($"'{nameof(stream)}' does not support reading");
            }

            await stream.ReadBufferAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="toRead"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="stream"/> or <paramref name="buffer"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="offset"/> or <paramref name="toRead"/> is negative</exception>
        /// <exception cref="ArgumentException">Thrown if the sum of <paramref name="offset"/> and <paramref name="toRead"/> is larger than <paramref name="buffer"/>'s length</exception>
        /// <exception cref="NotSupportedException">Thrown if <paramref name="stream"/> does not support reading</exception>
        /// <exception cref="ObjectDisposedException">Thrown if <paramref name="stream"/> is disposed</exception>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="stream"/> is currently in use by a previous read operation</exception>
        public static async Task ReadBufferAsync(this Stream stream, byte[] buffer, int offset, int toRead)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), $"'{nameof(offset)}' must not be negative; the provided value was '{offset}'");
            }

            if (toRead < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(toRead), $"'{nameof(toRead)}' must not be negative; the provided value was '{toRead}'");
            }

            if (buffer.Length - offset < toRead)
            {
                throw new ArgumentException($"The buffer size must be large enough to hold '{nameof(toRead)}' bytes at '{nameof(offset)}'; '{toRead}': '{toRead}'; '{nameof(offset)}': '{offset}'; buffer length: '{buffer.Length}'");
            }

            for (int read; (read = await stream.ReadAsync(buffer, offset, toRead).ConfigureAwait(false)) != 0 && (toRead -= read) != 0; offset += read)
            {
            }
        }
    }
}
