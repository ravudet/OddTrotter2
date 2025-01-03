namespace OddTrotter.Calendar
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;

    public interface IGraphOdataStructuredContext
    {
        OdataServiceRoot ServiceRoot { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// TODO you are here
        Task<OdataResponse<OdataCollectionResponse>> GetCollection(OdataGetCollectionRequest request);
    }

    public sealed class InvalidAccessTokenException : Exception
    {
        public InvalidAccessTokenException(string accessToken, string message, Exception innerException)
            : base(message, innerException)
        {
            AccessToken = accessToken;
        }

        public string AccessToken { get; }
    }


    public sealed class GraphOdataStructuredContext : IGraphOdataStructuredContext
    {
        private readonly IOdataStructuredContext odataStructuredContext;
        private readonly string accessToken;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="odataStructuredContext"></param>
        /// <param name="accessToken"></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="odataStructuredContext"/> or <paramref name="accessToken"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="accessToken"/> is whitespace-only characters</exception>
        /// <exception cref="InvalidAccessTokenException">Thrown if <paramref name="accessToken"/> is not a valid HTTP authorization header value</exception>
        public GraphOdataStructuredContext(IOdataStructuredContext odataStructuredContext, string accessToken)
        {
            if (odataStructuredContext == null)
            {
                throw new ArgumentNullException(nameof(odataStructuredContext));
            }

            if (accessToken == null)
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new ArgumentException($"'{nameof(accessToken)}' cannot be exclusively whitespace characters. The provided value was '{accessToken}'", nameof(accessToken));
            }

            // we need a whole `HttpClient` because we can't instantiate an `HttpRequestHeaders` due to the `internal` constructor
            using (var httpClient = new HttpClient())
            {
                try
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", accessToken);
                }
                catch (FormatException formatException)
                {
                    throw new InvalidAccessTokenException(accessToken, $"'{nameof(this.accessToken)}' had a value of '{accessToken}' which is not a valid 'Authorization' header value", formatException);
                }
            }

            this.odataStructuredContext = odataStructuredContext;
            this.accessToken = accessToken; //// TODO pull tests from graphclientunittests for anything that throws invalidaccesstokenexception
            this.ServiceRoot = odataStructuredContext.ServiceRoot;
        }

        public OdataServiceRoot ServiceRoot { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="request"/> is <see langword="null"/></exception>
        /// <exception cref="GraphClient.UnauthorizedAccessTokenException"></exception>
        public async Task<OdataResponse<OdataCollectionResponse>> GetCollection(OdataGetCollectionRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            //// TODO you are here
            var authorizedRequest = new OdataGetCollectionRequest(
                request.RelativeUri,
                request
                    .Headers
                    .Append(
                        new HttpHeader("Authorization", this.accessToken)));

            var response = await this.odataStructuredContext.GetCollection(authorizedRequest);

            if (response.HttpStatusCode == System.Net.HttpStatusCode.Unauthorized || response.HttpStatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new GraphClient.UnauthorizedAccessTokenException(
                    request.RelativeUri.OriginalString,
                    this.accessToken,
                    response.ResponseContent.Visit(
                        left => JsonSerializer.Serialize(left),
                        right => JsonSerializer.Serialize(right)));
            }

            return response;
        }
    }
}
