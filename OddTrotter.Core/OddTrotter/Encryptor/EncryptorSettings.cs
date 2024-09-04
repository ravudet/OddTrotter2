namespace OddTrotter.Encryptor
{
    using System;
    using System.Text;

    public sealed class EncryptorSettings
    {
        private EncryptorSettings(string password, Encoding encoding, long chunkSize)
        {
            this.Password = password;
            this.Encoding = encoding;
            this.ChunkSize = chunkSize;
        }

        public static EncryptorSettings Default { get; } = new EncryptorSettings(string.Empty, Encoding.UTF8, 4 * 1024 * 1024);

        public string Password { get; }

        public Encoding Encoding { get; }

        public long ChunkSize { get; }

        public sealed class Builder
        {
            public string Password { get; set; } = EncryptorSettings.Default.Password;

            public Encoding Encoding { get; set; } = EncryptorSettings.Default.Encoding;

            public long ChunkSize { get; set; } = EncryptorSettings.Default.ChunkSize;

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown if <see cref="Password"/> or <see cref="Encoding"/> is <see langword="null"/></exception>
            /// <exception cref="ArgumentOutOfRangeException">Thrown if <see cref="ChunkSize"/> is not a positive value</exception>
            public EncryptorSettings Build()
            {
                if (this.Password == null)
                {
                    throw new ArgumentNullException(nameof(this.Password));
                }

                if (this.Encoding == null)
                {
                    throw new ArgumentNullException(nameof(this.Encoding));
                }

                if (this.ChunkSize <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(this.ChunkSize), $"'{nameof(this.ChunkSize)}' must be a postive value; the provided value was '{this.ChunkSize}'.");
                }

                return new EncryptorSettings(this.Password, this.Encoding, this.ChunkSize);
            }
        }
    }
}
