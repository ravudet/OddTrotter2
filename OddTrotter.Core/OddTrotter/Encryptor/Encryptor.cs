namespace OddTrotter.Encryptor
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    public sealed class Encryptor
    {
        private const int initializationVectorLengthInBytes = 128 / 8;

        private readonly byte[] key;

        private readonly Encoding encoding;

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
                var decrypted = this.Decrypt(dataMemoryStream);
                using (var decryptedMemoryStream = new ChunkedMemoryStream())
                {
                    decrypted.CopyTo(decryptedMemoryStream);
                    var decryptedBytes = decryptedMemoryStream.ToArray();
                    return Encoding.Default.GetString(decryptedBytes);
                }
            }
        }

        public Stream Decrypt(Stream data)
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

            var initializationVector = new byte[initializationVectorLengthInBytes];
            data.Read(initializationVector, 0, initializationVector.Length); //// TODO because it's a memory stream, i'm assuming a full read; these below methods should be used instead

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
                                // not using async here since everything is in-memory
                                cryptoStream.CopyTo(memoryStream);
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

        /*
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
        */

        public Stream Encrypt(Stream data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            /*var maxLength = StringUtilities.MaxLength - initializationVectorLengthInBytes;
            if (data.Length >= maxLength)
            {
                // we use memory streams later; reading the source code for memory stream here: https://source.dot.net/#System.Private.CoreLib/src/libraries/System.Private.CoreLib/src/System/IO/MemoryStream.cs,a27df287b28d9a2a,references
                // we can see that IOException is thrown if the payload overflows the postition, which is stored as an int; we know that the data fits in a string, which uses an
                // int length, so we must check if we can fit the data *and* the initialization vector at the same time
                // 
                // using a chunkedmemorystream was explored in order to avoid this suggestion, but ultimately a byte[] is returned and it has a length with an integer value
                throw new ArgumentOutOfRangeException(
                    $"The length of '{nameof(data)}' must not be larger than '{maxLength}'");
            }*/

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
                        memoryStream = new ChunkedMemoryStream();

                        // reading the source code for memory stream here: https://source.dot.net/#System.Private.CoreLib/src/libraries/System.Private.CoreLib/src/System/IO/MemoryStream.cs,a27df287b28d9a2a,references
                        // we can see that IOException is thrown if the payload overflows the postition, which is stored as an int; we know the size of the initialization vector,
                        // so we know that it fits
                        memoryStream.Write(initializationVector, 0, initializationVector.Length);

                        using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write, true))
                        {
                            try
                            {
                                // not using async here since everything is in-memory
                                //
                                // we've previously assert that the data length cannot be too large, so IOException won't be thrown here
                                data.CopyTo(cryptoStream);
                            }
                            catch (CryptographicException e)
                            {
                                throw new EncryptionException("TODO", e);
                            }
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
            using (var memoryStream = new MemoryStream(bytes, false))
            {
                var encrypted = this.Encrypt(memoryStream);
                using (var returned = new MemoryStream())
                {
                    encrypted.CopyTo(returned);
                    return returned.ToArray();
                }
            }
        }
    }
}
