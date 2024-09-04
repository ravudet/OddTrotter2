namespace OddTrotter.Encryptor
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;

    public sealed class Encryptor
    {
        private const int initializationVectorLengthInBytes = 128 / 8;

        private readonly byte[] key;

        private readonly Encoding encoding;

        private readonly long chunkSize;

        public Encryptor()
            : this(EncryptorSettings.Default)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="settings"/> is <see langword="null"/></exception>
        public Encryptor(EncryptorSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            this.encoding = settings.Encoding;
            this.chunkSize = settings.ChunkSize;

            var passwordBytes = this.encoding.GetBytes(settings.Password);
            using (var sha256 = SHA256.Create())
            {
                this.key = sha256.ComputeHash(passwordBytes);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="password2"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="data"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="data"/> is not long enough to have been encrypted with the configured password</exception>
        /// <exception cref="EncryptionException">Thrown if the configured password was not used to encrypt <paramref name="data"/></exception>
        /*public string Decrypt(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Length < initializationVectorLengthInBytes)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(data),
                    $"The data length must be at least {initializationVectorLengthInBytes} bytes to have been encrypted with the configured password.");
            }

            using (var memoryStream = new MemoryStream(data))
            {
                var initializationVector = new byte[initializationVectorLengthInBytes];
                memoryStream.Read(initializationVector, 0, initializationVector.Length); //// TODO because it's a memory stream, i'm assuming a full read; these below methods should be used instead

                using (var aes = Aes.Create())
                {
                    aes.Key = this.key;
                    aes.IV = initializationVector;
                    using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (var streamReader = new StreamReader(cryptoStream, this.encoding))
                            {
                                try
                                {
                                    // not using async here since everything is in-memory
                                    return streamReader.ReadToEnd();
                                }
                                catch (CryptographicException e)
                                {
                                    var base64 = Convert.ToBase64String(data);
                                    throw new EncryptionException(base64, e);
                                }
                            }
                        }
                    }
                }
            }
        }*/

        public string Decrypt(byte[] data)
        {
            using (var dataMemoryStream = new ChunkedMemoryStream(data, false))
            {
                var decrypted = this.Decrypt(dataMemoryStream).GetAwaiter().GetResult();
                using (var decryptedMemoryStream = new ChunkedMemoryStream())
                {
                    decrypted.CopyTo(decryptedMemoryStream);
                    var decryptedBytes = decryptedMemoryStream.ToArray();
                    return Encoding.Default.GetString(decryptedBytes);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="data"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="data"/> is not long enough to have been encrypted with the configured password</exception>
        /// <exception cref="NotSupportedException">Thrown if <paramref name="data"/> does not support reading</exception>
        /// <exception cref="ObjectDisposedException">Thrown if <paramref name="data"/> is disposed</exception>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="data"/> is currently in use by a previous read operation</exception>
        public async Task<Stream> Decrypt(Stream data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Length < initializationVectorLengthInBytes)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(data),
                    $"The data length must be at least {initializationVectorLengthInBytes} bytes to have been encrypted with the configured password.");
            }

            if (!data.CanRead)
            {
                throw new NotSupportedException($"'{nameof(data)}' does not support reading");
            }

            var initializationVector = new byte[initializationVectorLengthInBytes];
            //// TODO you have a bucnh of new code; make sure to unit test it all
            //// TODO do chunked memory stream...
            await data.ReadBufferAsync(initializationVector).ConfigureAwait(false);

            using (var aes = Aes.Create())
            {
                aes.Key = this.key;
                aes.IV = initializationVector;
                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    using (var cryptoStream = new CryptoStream(data, decryptor, CryptoStreamMode.Read))
                    {
                        ChunkedMemoryStream? memoryStream = null;
                        try
                        {
                            memoryStream = new ChunkedMemoryStream();

                            try
                            {
                                //// TODO you are here
                                await cryptoStream.CopyToAsync(memoryStream).ConfigureAwait(false);
                                memoryStream.Position = 0;
                                return memoryStream;
                            }
                            catch (CryptographicException e)
                            {
                                ////var base64 = Convert.ToBase64String(data);
                                throw new EncryptionException("TODO", e);
                            }
                        }
                        catch
                        {
                            memoryStream?.Dispose();
                            throw;
                        }
                    }
                }
            }
        }

        //// TODO move these to extensions
        private static void Read(Stream stream, byte[] buffer)
        {
            Read(stream, buffer, 0, buffer.Length);
        }

        private static void Read(Stream stream, byte[] buffer, int offset, int toRead)
        {
            int read;
            for (; (read = stream.Read(buffer, offset, toRead)) != 0 && (toRead -= read) != 0; offset += read)
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="data"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="data"/> is not a readable stream</exception>
        public async Task<Stream> Encrypt(Stream data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (!data.CanRead)
            {
                throw new ArgumentException($"'{nameof(data)}' is not a readable stream", nameof(data));
            }

            var initializationVector = RandomNumberGenerator.GetBytes(initializationVectorLengthInBytes);
            using (var aes = Aes.Create())
            {
                aes.Key = this.key;
                aes.IV = initializationVector;
                using (var decryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    ChunkedMemoryStream? memoryStream = null;
                    try
                    {
                        memoryStream = new ChunkedMemoryStream(this.chunkSize);
                        memoryStream.Write(initializationVector, 0, initializationVector.Length);
                        using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write, true))
                        {
                            await data.CopyToAsync(cryptoStream).ConfigureAwait(false);
                        }

                        memoryStream.Position = 0;
                        return memoryStream;
                    }
                    catch
                    {
                        memoryStream?.Dispose();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dataName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="data"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="data"/> is too long</exception>
        /// <exception cref="EncryptionException">Thrown if an error occurred while encrypting <paramref name="data"/> using the configured password</exception>
        public byte[] Encrypt(string data)
        {
            var bytes = Encoding.Default.GetBytes(data);
            using (var memoryStream = new ChunkedMemoryStream(bytes, false))
            {
                var encrypted = this.Encrypt(memoryStream).GetAwaiter().GetResult(); ;
                using (var returned = new ChunkedMemoryStream())
                {
                    encrypted.CopyTo(returned);
                    return returned.ToArray();
                }
            }
        }
    }
}
