using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OddTrotter.Calendar
{
    public interface IGraphOdataStructuredContext
    {
        OdataServiceRoot ServiceRoot { get; }

        Task<OdataResponse<OdataCollectionResponse>> GetCollection(OdataGetCollectionRequest request);
    }

    public sealed class GraphOdataStructuredContext : IGraphOdataStructuredContext
    {
        private readonly IOdataStructuredContext odataStructuredContext;
        private readonly string accessToken;

        public GraphOdataStructuredContext(IOdataStructuredContext odataStructuredContext, string accessToken)
        {
            this.odataStructuredContext = odataStructuredContext;
            this.accessToken = accessToken; //// TODO pull tests from graphclientunittests for anything that throws invalidaccesstokenexception
            this.ServiceRoot = odataStructuredContext.ServiceRoot;
        }

        public OdataServiceRoot ServiceRoot { get; }

        public async Task<OdataResponse<OdataCollectionResponse>> GetCollection(OdataGetCollectionRequest request)
        {
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
