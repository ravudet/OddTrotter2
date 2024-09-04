namespace OddTrotter.UserExtension
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;

    using OddTrotter.AzureBlobClient;
    using OddTrotter.Encryptor;
    using OddTrotter.GraphClient;

    public sealed class UserExtensionService
    {
        private readonly IGraphClient graphClient;

        private readonly Encryptor encryptor;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="encryptor"></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="graphClient"/> or <paramref name="encryptor"/> is <see langword="null"/></exception>
        public UserExtensionService(IGraphClient graphClient, Encryptor encryptor)
        {
            if (graphClient == null)
            {
                throw new ArgumentNullException(nameof(graphClient));
            }

            if (encryptor == null)
            {
                throw new ArgumentNullException(nameof(encryptor));
            }

            this.graphClient = graphClient;
            this.encryptor = encryptor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oddTrotterBlobSettings"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="oddTrotterBlobSettings"/> is <see langword="null"/></exception>
        /// <exception cref="UserExtensionEncryptionException">Thrown if the <paramref name="oddTrotterBlobSettings"/> are too long to encrypt</exception>
        /// <exception cref="EncryptionException">Thrown if an error occurred encrypting <paramref name="oddTrotterBlobSettings"/></exception>
        /// <exception cref="HttpRequestException">
        /// Thrown if the request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        /// <exception cref="UnauthorizedAccessTokenException">
        /// Thrown if the access token configured on the provided <see cref="IGraphClient"/> is invalid or provides insufficient privileges for the requests</exception>
        /// <exception cref="GraphException">Thrown if graph produced an error when writing the oddtrotter user extension</exception>
        public async Task ConfigureUserExtension(OddTrotterBlobSettings oddTrotterBlobSettings)
        {
            if (oddTrotterBlobSettings == null)
            {
                throw new ArgumentNullException(nameof(oddTrotterBlobSettings));
            }

            /////var serializedSettings = JsonSerializer.Serialize(oddTrotterBlobSettings);

            using (var serializedSettings = new MemoryStream())
            {
                JsonSerializer.Serialize(serializedSettings, oddTrotterBlobSettings);

                Stream encrypted;
                try
                {
                    encrypted = await encryptor.Encrypt(serializedSettings);
                }
                catch (ArgumentOutOfRangeException)
                {
                    throw;
                    ////throw new UserExtensionEncryptionException($"An error occurred while encrypting the OddTrotter user extension because the extension data was too long", serializedSettings, e);
                }

                var extension = new OddTrotterUserExtension()
                {
                    Data = ToBase64String(encrypted),
                };
                await UpdateUserExtension(this.graphClient, extension).ConfigureAwait(false);
            }
        }


        private static string ToBase64String(Stream stream)
        {
            //// TODO
            if (stream is MemoryStream memoryStream)
            {
                return Convert.ToBase64String(memoryStream.ToArray());
            }

            var bytes = new Byte[(int)stream.Length];

            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(bytes, 0, (int)stream.Length);

            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="extensionData"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="graphClient"/> or <paramref name="extensionData"/> is <see langword="null"/></exception>
        /// <exception cref="HttpRequestException">
        /// Thrown if the request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        /// <exception cref="UnauthorizedAccessTokenException">Thrown if the access token configured in <paramref name="graphClient"/> is invalid or provides insufficient privileges for the request</exception>
        /// <exception cref="GraphException">Thrown if graph produced an error when writing the oddtrotter user extension</exception>
        private static async Task UpdateUserExtension(IGraphClient graphClient, OddTrotterUserExtension extensionData)
        {
            if (graphClient == null)
            {
                throw new ArgumentNullException(nameof(graphClient));
            }

            if (extensionData == null)
            {
                throw new ArgumentNullException(nameof(extensionData));
            }

            var serializedExtension = JsonSerializer.Serialize(extensionData);
            using (var stringContent = new StringContent(serializedExtension))
            {
                var patchUrl = new Uri($"/me/extensions/{extensionData.Id}", UriKind.Relative).ToRelativeUri();
                var postUrl = new Uri("/me/extensions", UriKind.Relative).ToRelativeUri();

                using (var httpResponse = await PatchUserExtension(graphClient, patchUrl, postUrl, stringContent).ConfigureAwait(false))
                {
                    try
                    {
                        httpResponse.EnsureSuccessStatusCode();
                    }
                    catch (HttpRequestException e)
                    {
                        var responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new GraphException(responseContent, e);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphClient">assumed to not be <see langword="null"/></param>
        /// <param name="patchUrl">assumed to not be <see langword="null"/></param>
        /// <param name="postUrl"></param>
        /// <param name="httpContent"></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException">
        /// Thrown if the request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        /// <exception cref="UnauthorizedAccessTokenException">Thrown if the access token configured in <paramref name="graphClient"/> is invalid or provides insufficient privileges for the request</exception>
        private static async Task<HttpResponseMessage> PatchUserExtension(IGraphClient graphClient, RelativeUri patchUrl, RelativeUri postUrl, HttpContent httpContent)
        {
            HttpResponseMessage? httpResponse = null;
            try
            {
                httpResponse = await graphClient.PatchAsync(patchUrl, httpContent).ConfigureAwait(false);
                if (httpResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    httpResponse.Dispose();
                    return await PostUserExtension(graphClient, patchUrl, postUrl, httpContent).ConfigureAwait(false);
                }

                return httpResponse;

            }
            catch
            {
                httpResponse?.Dispose();
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphClient">assumed to not be <see langword="null"/></param>
        /// <param name="patchUrl"></param>
        /// <param name="postUrl">assumed to not be <see langword="null"/></param>
        /// <param name="httpContent">assumed to not be <see langword="null"/></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException">
        /// Thrown if the request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        /// <exception cref="UnauthorizedAccessTokenException">Thrown if the access token configured in <paramref name="graphClient"/> is invalid or provides insufficient privileges for the request</exception>
        private static async Task<HttpResponseMessage> PostUserExtension(IGraphClient graphClient, RelativeUri patchUrl, RelativeUri postUrl, HttpContent httpContent)
        {
            HttpResponseMessage? httpResponse = null;
            try
            {
                httpResponse = await graphClient.PostAsync(postUrl, httpContent).ConfigureAwait(false);
                if (httpResponse.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    httpResponse.Dispose();
                    return await PatchUserExtension(graphClient, patchUrl, postUrl, httpContent).ConfigureAwait(false);
                }

                return httpResponse;

            }
            catch
            {
                httpResponse?.Dispose();
                throw;
            }
        }
    }
}
