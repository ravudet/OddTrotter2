namespace OddTrotter.Encryptor
{
    using System;
    using System.Text;

    public sealed class EncryptorSettings
    {
        private EncryptorSettings(string password, Encoding encoding)
        {
            this.Password = password;
            this.Encoding = encoding;
        }

        public static EncryptorSettings Default { get; } = new EncryptorSettings(string.Empty, Encoding.UTF8);

        public string Password { get; }

        public Encoding Encoding { get; }

        public sealed class Builder
        {
            public string Password { get; set; } = EncryptorSettings.Default.Password;

            public Encoding Encoding { get; set; } = EncryptorSettings.Default.Encoding;

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown if <see cref="Password"/> or <see cref="Encoding"/> is <see langword="null"/></exception>
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

                return new EncryptorSettings(this.Password, this.Encoding);
            }
        }
    }
}
