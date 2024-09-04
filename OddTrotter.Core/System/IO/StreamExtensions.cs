namespace System.IO
{
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for <see cref="Stream"/>
    /// </summary>
    public static class StreamExtensions
    {
        public static async Task ReadBufferAsync(this Stream stream, byte[] buffer)
        {
            await stream.ReadBufferAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
        }

        public static async Task ReadBufferAsync(this Stream stream, byte[] buffer, int offset, int toRead)
        {
            for (int read; (read = await stream.ReadAsync(buffer, offset, toRead).ConfigureAwait(false)) != 0 && (toRead -= read) != 0; offset += read)
            {
            }
        }
    }
}
