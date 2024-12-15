////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace OddTrotter.Calendar
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    using OddTrotter.GraphClient;

    public interface IHttpClient
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="absoluteUri"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="absoluteUri"/> is <see langword="null"/></exception>
        /// <exception cref="HttpRequestException">
        /// Thrown if the request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout
        /// </exception>
        Task<HttpResponseMessage> GetAsync(AbsoluteUri absoluteUri, IEnumerable<HttpHeader> headers);
    }

    /// <summary>
    /// https://www.rfc-editor.org/rfc/rfc2616#section-4.2
    /// </summary>
    public sealed class HttpHeader
    {
        public HttpHeader(string name, string value)
        {
            //// TODO pull tests from graphclientunittests for anything that throws invalidaccesstokenexception
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"'{nameof(name)} cannot be empty", name);
            }

            if (value == null)
            {
                // empty values are allowed by the RFC
                throw new ArgumentNullException(nameof(value));
            }

            Name = name;
            Value = value;
        }

        public string Name { get; }
        public string Value { get; }
    }

    public sealed class HttpClientAdapter : IHttpClient
    {
        private readonly HttpClient httpClient;

        public HttpClientAdapter(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> GetAsync(AbsoluteUri absoluteUri, IEnumerable<HttpHeader> headers)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, absoluteUri);
            foreach (var header in headers)
            {
                request.Headers.Add(header.Name, header.Value);
            }

            return await this.httpClient.SendAsync(request).ConfigureAwait(false);
        }
    }
}
