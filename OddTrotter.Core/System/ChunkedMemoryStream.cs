namespace System.IO
{
    using System.Collections.Generic;
    using System.Globalization;

    using Microsoft.VisualBasic;

    /// <summary>
    /// A <see cref="Stream"/> implementation which stores all of its data in memory, but stores them as a collection of arrays in order to get around the object size limit as well as the array size limit that are imposed by the .NET framework.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    public sealed class ChunkedMemoryStream : Stream
    {
        /// <summary>
        /// The size, in bytes, of each chunk that is stored within this <see cref="Stream"/>
        /// </summary>
        private readonly long chunkSize;

        /// <summary>
        /// The collection of byte arrays which, when concatenated, contain all of the data represented by this <see cref="Stream"/>
        /// </summary>
        private readonly List<byte[]> data;

        /// <summary>
        /// Whether or not this <see cref="Stream"/> can be written to
        /// </summary>
        private readonly bool writable;

        /// <summary>
        /// The position of the next byte within this <see cref="Stream"/>, relative to the beginning of the <see cref="Stream"/>
        /// </summary>
        private long position;

        /// <summary>
        /// The index of the next available free byte of the last array in <see cref="data"/>
        /// </summary>
        private uint partialIndex;

        /// <summary>
        /// Whether or not this object is disposed
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChunkedMemoryStream"/> class, using a default chunk size of 4MB
        /// </summary>
        public ChunkedMemoryStream()
            : this(4 * 1024 * 1024)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChunkedMemoryStream"/> class
        /// </summary>
        /// <param name="chunkSize">The size, in bytes, of each chunk that is stored within this <see cref="Stream"/></param>
        public ChunkedMemoryStream(long chunkSize)
        {
            ////Ensure.IsPositive(chunkSize, nameof(chunkSize));

            this.chunkSize = chunkSize;

            this.data = new List<byte[]>(new[] { new byte[this.chunkSize] });
            this.writable = true;

            this.position = 0;
            this.partialIndex = 0;
            this.disposed = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChunkedMemoryStream"/> class that contains the data in <paramref name="data"/>
        /// </summary>
        /// <param name="data">The bytes that should be contained within the resulting <see cref="Stream"/></param>
        /// <param name="writable">Whether or not the resulting <see cref="Stream"/> should be allowed to be written to, or if it should be read-only</param>
        public ChunkedMemoryStream(byte[] data, bool writable)
            : this()
        {
            ////Ensure.NotNull(data, nameof(data));

            this.Write(data, 0, data.Length);
            this.position = 0;

            this.writable = writable;
        }

        /// <summary>
        /// Gets that the <see cref="ChunkedMemoryStream"/> always supports reading
        /// </summary>
        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets that the <see cref="ChunkedMemoryStream"/> always supports seeking
        /// </summary>
        public override bool CanSeek
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets whether or not this <see cref="Stream"/> supporting being written to
        /// </summary>
        public override bool CanWrite
        {
            get
            {
                return this.writable;
            }
        }

        /// <summary>
        /// Gets the length, in bytes, of this <see cref="Stream"/>
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown if this <see cref="Stream"/> is disposed</exception>
        public override long Length
        {
            get
            {
                this.ThrowIfDisposed();

                return this.partialIndex + ((this.data.Count - 1) * this.chunkSize);
            }
        }

        /// <summary>
        /// Gets or sets the current position within this <see cref="Stream"/>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if setting to a negative value</exception>
        /// <exception cref="ObjectDisposedException">Thrown if this <see cref="Stream"/> is disposed</exception>
        public override long Position
        {
            get
            {
                this.ThrowIfDisposed();

                return this.position;
            }

            set
            {
                ////Ensure.NotNegative(value, nameof(value));

                this.ThrowIfDisposed();

                this.position = value;
            }
        }

        /// <summary>
        /// Since all data is accessed directly in memory, this method is merely a placeholder and does nothing
        /// </summary>
        public override void Flush()
        {
        }

        /// <summary>
        /// Reads a sequence of bytes from this <see cref="Stream"/> and advances the position within the <see cref="Stream"/> by the number of bytes read
        /// </summary>
        /// <param name="buffer">When the method returns, this array will contain its original contents with the values between <paramref name="offset"/> and <paramref name="offset"/> + <paramref name="count"/> - 1 (inclusive) replaced by the bytes read from this <see cref="Stream"/></param>
        /// <param name="offset">The zero-based offset in <paramref name="buffer"/> to which to begin storing the data read from this <see cref="Stream"/></param>
        /// <param name="count">The maximum number of bytes to be read from this <see cref="Stream"/></param>
        /// <returns>The total number of bytes read into <paramref name="buffer"/>. This can be less than the number of bytes requested if that many bytes are not currently available, or zero if the end of the stream has been reached</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="buffer"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="offset"/> or <paramref name="count"/> is negative</exception>
        /// <exception cref="ArgumentException">Thrown if the sum of <paramref name="offset"/> and <paramref name="count"/> is larger than <paramref name="buffer"/>'s length</exception>
        /// <exception cref="ObjectDisposedException">Thrown if this <see cref="Stream"/> is disposed</exception>
        public override int Read(byte[] buffer, int offset, int count)
        {
            ////.NotNull(buffer, nameof(buffer));
            ////Ensure.NotNegative(offset, nameof(offset));
            ////Ensure.NotNegative(count, nameof(count));
            if (buffer.Length - offset < count)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "The buffer size must be large enough to hold 'count' bytes at 'offset'; count: '{0}'; offset: '{1}'; buffer length: '{2}'", count, offset, buffer.Length));
            }

            this.ThrowIfDisposed();

            var chunkIndex = this.position / this.chunkSize;
            var byteIndex = this.position % this.chunkSize;

            var read = 0L;
            while (read < count && this.Position < this.Length)
            {
                var toRead = this.chunkSize - byteIndex;
                if (chunkIndex == this.data.Count - 1)
                {
                    toRead = this.partialIndex - byteIndex;
                }

                toRead = Min(toRead, count - read);
                Array.Copy(this.data[Convert.ToInt32(chunkIndex)], byteIndex, buffer, offset + read, toRead);
                this.position += toRead;
                read += toRead;
                byteIndex = 0;
                ++chunkIndex;
            }

            return Convert.ToInt32(read);
        }

        /// <summary>
        /// Sets the position within this <see cref="Stream"/> to the specified value
        /// </summary>
        /// <param name="offset">The new position within this <see cref="Stream"/>, relative to <paramref name="origin"/></param>
        /// <param name="origin">The reference point within this <see cref="Stream"/> to begin seeking at</param>
        /// <returns>The new position within this <see cref="Stream"/></returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if seeking will cause this <see cref="Stream"/>'s position to be at a negative value or <paramref name="origin"/> is not a valid <see cref="SeekOrigin"/></exception>
        /// <exception cref="ObjectDisposedException">Thrown if this <see cref="Stream"/> is disposed</exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            ////Ensure.IsDefinedEnum(origin, nameof(origin));

            this.ThrowIfDisposed();

            var futurePosition = this.Position;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    futurePosition = offset;
                    break;
                case SeekOrigin.Current:
                    futurePosition += offset;
                    break;
                case SeekOrigin.End:
                    futurePosition = this.Length + offset;
                    break;
            }

            ////Ensure.NotNegative(futurePosition, nameof(futurePosition));
            this.Position = futurePosition;
            return this.Position;
        }

        /// <summary>
        /// Sets the length of this <see cref="Stream"/> to <paramref name="value"/>
        /// </summary>
        /// <param name="value">The value to set the length to</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is negative</exception>
        /// <exception cref="NotSupportedException">Thrown if this <see cref="Stream"/> is not currently writable</exception>
        /// <exception cref="ObjectDisposedException">Thrown if this <see cref="Stream"/> is disposed</exception>
        public override void SetLength(long value)
        {
            ////Ensure.NotNegative(value, nameof(value));

            this.ThrowIfDisposed();

            if (!this.writable)
            {
                throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "This stream does not support writing"));
            }

            this.Expand(value);
        }

        /// <summary>
        /// Writes a sequence of bytes to this <see cref="Stream"/> and advances the current position within this <see cref="Stream"/> by the number of bytes written
        /// </summary>
        /// <param name="buffer">This method copies <paramref name="count"/> bytes from <paramref name="buffer"/> to this <see cref="Stream"/></param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin copying bytes to this <see cref="Stream"/></param>
        /// <param name="count">The number of bytes to be written to this <see cref="Stream"/></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="buffer"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="offset"/> or <paramref name="count"/> is negative</exception>
        /// <exception cref="ArgumentException">Thrown if the sum of <paramref name="offset"/> and <paramref name="count"/> is larger than <paramref name="buffer"/>'s length</exception>
        /// <exception cref="NotSupportedException">Thrown if this <see cref="Stream"/> does not currently support writing</exception>
        /// <exception cref="ObjectDisposedException">Thrown if this <see cref="Stream"/> is disposed</exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
            ////Ensure.NotNull(buffer, nameof(buffer));
            ////Ensure.NotNegative(offset, nameof(offset));
            ////Ensure.NotNegative(count, nameof(count));
            if (buffer.Length - offset < count)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "The buffer size must be large enough to hold 'count' bytes at 'offset'; count: '{0}'; offset: '{1}'; buffer length: '{2}'", count, offset, buffer.Length));
            }

            this.ThrowIfDisposed();

            if (!this.writable)
            {
                throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "This stream does not support writing"));
            }

            this.Expand(Max(this.Length, this.position + count));

            var chunkIndex = this.position / this.chunkSize;
            var byteIndex = this.position % this.chunkSize;

            var written = 0L;
            while (count > written)
            {
                var toWrite = this.chunkSize - byteIndex;
                toWrite = Min(toWrite, count - written);
                Array.Copy(buffer, offset + written, this.data[Convert.ToInt32(chunkIndex)], byteIndex, toWrite);

                written += toWrite;
                this.partialIndex = Convert.ToUInt32((byteIndex + toWrite) % this.chunkSize);
                byteIndex = 0;
                this.position += toWrite;
                ++chunkIndex;
            }
        }

        /// <summary>
        /// Converts the data represented by this <see cref="Stream"/> into an array containing that data
        /// </summary>
        /// <returns>The data represented by this <see cref="Stream"/></returns>
        /// <exception cref="ObjectDisposedException">Thrown if this <see cref="Stream"/> is disposed</exception>
        /// <remarks>.NET imposes size limits to both objects and arrays. Because of this, calling this method when there are 2GB or more data stored within this <see cref="Stream"/> will result in either a <see cref="OutOfMemoryException"/> being thrown, or this method hanging indefinitely. You should check the <see cref="Length"/> of this <see cref="Stream"/> before calling this method.</remarks>
        public byte[] ToArray()
        {
            this.ThrowIfDisposed();

            var result = new byte[this.Length];
            var copied = 0L;
            for (int i = 0; i < this.data.Count - 1; ++i)
            {
                Array.Copy(this.data[i], 0, result, copied, this.chunkSize);
                copied += this.chunkSize;
            }

            Array.Copy(this.data[this.data.Count - 1], 0, result, copied, this.partialIndex);
            return result;
        }

        /// <summary>
        /// Releases the unmanaged resources used by this <see cref="Stream"/>, and optionally the managed resources
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources, false to release only unmanaged resources</param>
        protected override void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.data.Clear();
            }

            this.disposed = true;
            base.Dispose(disposing);
        }

        /// <summary>
        /// Determines the larger of two values
        /// </summary>
        /// <param name="a">The first value</param>
        /// <param name="b">The second value</param>
        /// <returns>The larger of <paramref name="a"/> and <paramref name="b"/></returns>
        private static long Max(long a, long b)
        {
            return a > b ? a : b;
        }

        /// <summary>
        /// Determines the smaller of two values
        /// </summary>
        /// <param name="a">The first value</param>
        /// <param name="b">The second value</param>
        /// <returns>The smaller of <paramref name="a"/> and <paramref name="b"/></returns>
        private static long Min(long a, long b)
        {
            return a < b ? a : b;
        }

        /// <summary>
        /// Expands the number of arrays store in <see cref="data"/> to be large enough to at least contain a length of <paramref name="length"/> bytes
        /// </summary>
        /// <param name="length">The number of bytes that should be able to be contained within <see cref="data"/> once this method returns</param>
        private void Expand(long length)
        {
            var chunkCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(length) / Convert.ToDouble(this.chunkSize)));
            var byteIndex = length % this.chunkSize;
            if (chunkCount < this.data.Count)
            {
                this.data.RemoveRange(chunkCount, this.data.Count - chunkCount);
            }

            while (chunkCount != this.data.Count)
            {
                this.data.Add(new byte[this.chunkSize]);
            }

            if (byteIndex == 0)
            {
                this.data.Add(new byte[this.chunkSize]);
            }

            this.partialIndex = Convert.ToUInt32(byteIndex);
        }

        /// <summary>
        /// Determines if this object is disposed, and throw a <see cref="ObjectDisposedException"/> if it is
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown if this object is disposed</exception>
        private void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(typeof(ChunkedMemoryStream).Name);
            }
        }
    }
}