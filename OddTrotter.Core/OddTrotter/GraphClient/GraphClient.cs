namespace OddTrotter.GraphClient
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    public sealed class GraphClient : IGraphClient
    {
        private readonly string rootUrl;

        private readonly string accessToken;

        private readonly TimeSpan httpClientTimeout;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootUrl"></param>
        /// <param name="accessToken"></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="accessToken"/> or <paramref name="settings"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="accessToken"/> is whitespace-only characters</exception>
        /// <exception cref="InvalidAccessTokenException">Thrown if <paramref name="accessToken"/> is not a valid HTTP authorization header value</exception>
        public GraphClient(string accessToken, GraphClientSettings settings)
        {
            if (accessToken == null)
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new ArgumentException($"'{nameof(accessToken)}' cannot be null or exclusively whitespace characters");
            }

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            this.rootUrl = settings.GraphRootUri.OriginalString.TrimEnd('/');
            this.accessToken = accessToken;
            this.httpClientTimeout = settings.HttpClientTimeout;

            try
            {
                CreateHttpClient().Dispose();
            }
            catch (FormatException e)
            {
                throw new InvalidAccessTokenException(accessToken, $"'{nameof(this.accessToken)}' had a value of '{accessToken}' which is not a valid 'Authorization' header value", e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FormatException">Thrown if <see cref="accessToken"/> is not a valid HTTP authorization header value; only thrown during initialization because <see cref="accessToken"/> is validated in the constructor</exception>
        private HttpClient CreateHttpClient()
        {
            HttpClient? httpClient = null;
            try
            {
                httpClient = new HttpClient();
                httpClient.Timeout = this.httpClientTimeout;

                // formatexception won't be thrown normally because we already validated accessToken in the constructor
                httpClient.DefaultRequestHeaders.Add("Authorization", this.accessToken);

                return httpClient;
            }
            catch
            {
                httpClient?.Dispose();
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> DeleteAsync(RelativeUri relativeUri)
        {
            if (relativeUri == null)
            {
                throw new ArgumentNullException(nameof(relativeUri));
            }

            using (var httpClient = CreateHttpClient())
            {
                HttpResponseMessage? httpResponse = null;
                try
                {
                    try
                    {
                        httpResponse = await httpClient.DeleteAsync(this.rootUrl + '/' + relativeUri.ToString()).ConfigureAwait(false);
                    }
                    catch (HttpRequestException e)
                    {
                        throw new HttpRequestException($"An error occurred during the DELETE request to {relativeUri.OriginalString}", e);
                    }

                    if (httpResponse.StatusCode == HttpStatusCode.Unauthorized || httpResponse.StatusCode == HttpStatusCode.Forbidden)
                    {
                        var httpResponseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new UnauthorizedAccessTokenException(relativeUri.OriginalString, this.accessToken, httpResponseContent);
                    }
                }
                catch
                {
                    httpResponse?.Dispose();
                    throw;
                }

                return httpResponse;
            }
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> GetAsync(RelativeUri relativeUri)
        {
            if (relativeUri == null)
            {
                throw new ArgumentNullException(nameof(relativeUri));
            }

            var requestUrl = new Uri(this.rootUrl + '/' + relativeUri.OriginalString.Trim('/'), UriKind.Absolute).ToAbsoluteUri();
            return await GetAsync(requestUrl).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> GetAsync(AbsoluteUri absoluteUri)
        {
            if (absoluteUri == null)
            {
                throw new ArgumentNullException(nameof(absoluteUri));
            }

            using (var httpClient = CreateHttpClient())
            {
                HttpResponseMessage? httpResponse = null;
                try
                {
                    try
                    {
                        httpResponse = await httpClient.GetAsync(absoluteUri).ConfigureAwait(false);
                    }
                    catch (HttpRequestException e)
                    {
                        throw new HttpRequestException($"An error occurred during the GET request to {absoluteUri.OriginalString}", e);
                    }

                    if (httpResponse.StatusCode == HttpStatusCode.Unauthorized || httpResponse.StatusCode == HttpStatusCode.Forbidden)
                    {
                        var httpResponseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new UnauthorizedAccessTokenException(absoluteUri.OriginalString, this.accessToken, httpResponseContent);
                    }
                }
                catch
                {
                    httpResponse?.Dispose();
                    throw;
                }

                return httpResponse;
            }
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> PatchAsync(RelativeUri relativeUri, HttpContent httpContent)
        {
            if (relativeUri == null)
            {
                throw new ArgumentNullException(nameof(relativeUri));
            }

            if (httpContent == null)
            {
                throw new ArgumentNullException(nameof(httpContent));
            }

            using (var httpClient = CreateHttpClient())
            {
                HttpResponseMessage? httpResponse = null;
                try
                {
                    try
                    {
                        httpResponse = await httpClient.PatchAsync(this.rootUrl + '/' + relativeUri.ToString(), httpContent).ConfigureAwait(false);
                    }
                    catch (HttpRequestException e)
                    {
                        throw new HttpRequestException($"An error occurred during the PATCH request to {relativeUri.OriginalString}", e);
                    }

                    if (httpResponse.StatusCode == HttpStatusCode.Unauthorized || httpResponse.StatusCode == HttpStatusCode.Forbidden)
                    {
                        var httpResponseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new UnauthorizedAccessTokenException(relativeUri.OriginalString, this.accessToken, httpResponseContent);
                    }
                }
                catch
                {
                    httpResponse?.Dispose();
                    throw;
                }

                return httpResponse;
            }
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> PostAsync(RelativeUri relativeUri, HttpContent httpContent)
        {
            if (relativeUri == null)
            {
                throw new ArgumentNullException(nameof(relativeUri));
            }

            if (httpContent == null)
            {
                throw new ArgumentNullException(nameof(httpContent));
            }

            using (var httpClient = CreateHttpClient())
            {
                HttpResponseMessage? httpResponse = null;
                try
                {
                    try
                    {
                        httpResponse = await httpClient.PostAsync(this.rootUrl + '/' + relativeUri.ToString(), httpContent).ConfigureAwait(false);
                    }
                    catch (HttpRequestException e)
                    {
                        throw new HttpRequestException($"An error occurred during the POST request to {relativeUri.OriginalString}", e);
                    }

                    if (httpResponse.StatusCode == HttpStatusCode.Unauthorized || httpResponse.StatusCode == HttpStatusCode.Forbidden)
                    {
                        var httpResponseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new UnauthorizedAccessTokenException(relativeUri.OriginalString, this.accessToken, httpResponseContent);
                    }
                }
                catch
                {
                    httpResponse?.Dispose();
                    throw;
                }

                return httpResponse;
            }
        }
    }
}