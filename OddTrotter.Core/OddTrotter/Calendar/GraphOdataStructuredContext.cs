using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OddTrotter.Calendar
{
    public interface IGraphOdataStructuredContext
    {
        Task<OdataResponse<OdataCollectionResponse>> GetCollection(OdataGetCollectionRequest request);
    }

    public sealed class GraphOdataStructuredContext : IGraphOdataStructuredContext
    {
        private readonly IOdataStructuredContext odataStructuredContext;
        private readonly string accessToken;

        public GraphOdataStructuredContext(IOdataStructuredContext odataStructuredContext, string accessToken)
        {
            this.odataStructuredContext = odataStructuredContext;
            this.accessToken = accessToken;
        }

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
                        (left, context) => JsonSerializer.Serialize(left.Value),
                        (right, context) => JsonSerializer.Serialize(right.Value),
                        new Void()));
            }

            return response;
        }
    }
}
