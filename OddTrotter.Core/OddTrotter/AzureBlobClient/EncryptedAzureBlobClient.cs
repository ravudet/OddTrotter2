namespace OddTrotter.AzureBlobClient
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;

    using OddTrotter.Encryptor;

    public sealed class EncryptedAzureBlobClient : IAzureBlobClient
    {
        private readonly IAzureBlobClient delegateClient;

        private readonly Encryptor encryptor;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="delegateClient"></param>
        /// <param name="encryptor"></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="delegateClient"/> or <paramref name="encryptor"/> is <see langword="null"/></exception>
        public EncryptedAzureBlobClient(IAzureBlobClient delegateClient, Encryptor encryptor)
        {
            if (delegateClient == null)
            {
                throw new ArgumentNullException(nameof(delegateClient));
            }

            if (encryptor == null)
            {
                throw new ArgumentNullException(nameof(encryptor));
            }

            this.delegateClient = delegateClient;
            this.encryptor = encryptor;
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> GetAsync(string blobName)
        {
            if (blobName == null)
            {
                throw new ArgumentNullException(nameof(blobName));
            }

            if (string.IsNullOrEmpty(blobName))
            {
                throw new ArgumentException($"'{nameof(blobName)}' cannot be empty");
            }

            HttpResponseMessage? httpResponse = null;
            try
            {
                httpResponse = await this.delegateClient.GetAsync(blobName).ConfigureAwait(false);
                if (!httpResponse.IsSuccessStatusCode)
                {
                    // if the request wasn't successful, then it means that the response payload isn't the blob contents, so there's nothing to actually decrypt
                    return httpResponse;
                }
            }
            catch
            {
                httpResponse?.Dispose();
                throw;
            }

            using (httpResponse)
            {
                var responseContent = await httpResponse.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                string decryptedContent;
                try
                {
                    decryptedContent = this.encryptor.Decrypt(responseContent);
                }
                catch (Exception e) when (e is ArgumentOutOfRangeException || e is EncryptionException)
                {
                    throw new InvalidBlobNameException($"An error occurred decrypting the content of blob '{blobName}'", e);
                }

                StringContent? decryptedHttpResponseContent = null;
                try
                {
                    decryptedHttpResponseContent = new StringContent(decryptedContent);
                    HttpResponseMessage? decryptedHttpResponse = null;
                    try
                    {
                        decryptedHttpResponse = Duplicate(httpResponse);
                        decryptedHttpResponse.Content = decryptedHttpResponseContent;

                        return decryptedHttpResponse;
                    }
                    catch
                    {
                        decryptedHttpResponse?.Dispose();
                        throw;
                    }
                }
                catch
                {
                    decryptedHttpResponseContent?.Dispose();
                    throw;
                }
            }
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> PutAsync(string blobName, HttpContent httpContent)
        {
            if (blobName == null)
            {
                throw new ArgumentNullException(nameof(blobName));
            }

            if (string.IsNullOrEmpty(blobName))
            {
                throw new ArgumentException($"'{nameof(blobName)}' cannot be empty");
            }

            if (httpContent == null)
            {
                throw new ArgumentNullException(nameof(httpContent));
            }

            Stream encryptedContent;
            try
            {
                encryptedContent = this.encryptor.Encrypt(await httpContent.ReadAsStreamAsync().ConfigureAwait(false));
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new InvalidBlobDataException($"The data in '{nameof(httpContent)}' was too long to be encrypted", e);
            }
            catch (EncryptionException e)
            {
                throw new InvalidBlobDataException($"An error occurred while encrypting '{nameof(httpContent)}'", e);
            }

            using (var byteArrayContent = new StreamContent(encryptedContent))
            {
                return await this.delegateClient.PutAsync(blobName, byteArrayContent).ConfigureAwait(false);
            }
        }

        private static HttpResponseMessage Duplicate(HttpResponseMessage httpResponse)
        {
            HttpResponseMessage? duplicate = null;
            try
            {
                duplicate = new HttpResponseMessage(httpResponse.StatusCode);

                duplicate.Content = httpResponse.Content;
                duplicate.Headers.Clear();
                foreach (var header in httpResponse.Headers)
                {
                    duplicate.Headers.Add(header.Key, header.Value);
                }

                duplicate.ReasonPhrase = httpResponse.ReasonPhrase;
                duplicate.RequestMessage = httpResponse.RequestMessage;
                duplicate.Version = httpResponse.Version;

                if (duplicate.Version != Version.Parse("2.0"))
                {
                    // HTTP/2 can contain psuedo headers in the trailing headers frame; we will throw an exception if we try to copy them when that happens, so we just skip
                    // copying HTTP/2 trailing headers
                    duplicate.TrailingHeaders.Clear();
                    foreach (var header in httpResponse.TrailingHeaders)
                    {
                        duplicate.TrailingHeaders.Add(header.Key, header.Value);
                    }
                }

                return duplicate;
            }
            catch
            {
                duplicate?.Dispose();
                throw;
            }
        }
    }
}
