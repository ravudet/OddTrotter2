////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.V2.Odata.RequestEvaluator
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using System.Net.Http;
    using global::System.Net.Http;
    using global::System.Text;
    using global::System.Threading.Tasks;

    using Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter.Visitors;
    using Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Select.Visitors;
    using Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Top.Visitors;

    public sealed class HttpClientRequestEvaluator : IRequestEvaluator
    {
        private readonly IHttpClient httpClient;

        private readonly CommonExpressionToStringVisitor commonExpressionToStringVisitor;
        private readonly SelectItemToStringVisitor selectItemToStringVisitor;
        private readonly DigitToStringVisitor digitToStringVisitor;

        public HttpClientRequestEvaluator(
            IHttpClient httpClient,
            CommonExpressionToStringVisitor commonExpressionToStringVisitor,
            SelectItemToStringVisitor selectItemToStringVisitor,
            DigitToStringVisitor digitToStringVisitor)
        {
            this.httpClient = httpClient;
            this.commonExpressionToStringVisitor = commonExpressionToStringVisitor;
            this.selectItemToStringVisitor = selectItemToStringVisitor;
            this.digitToStringVisitor = digitToStringVisitor;
        }

        public async Task<Response> Evaluate(Request.GetCollection request)
        {
            var queryOptions = new List<string>();
            var subsequentOption = false;
            var stringBuilder = new StringBuilder(request.Path.Value);

            if (request.Filter != null)
            {
                if (subsequentOption)
                {
                    stringBuilder.Append("&");
                }
                else
                {
                    stringBuilder.Append("?");
                }

                stringBuilder.Append("$filter=");
                //// TODO call something "dispatch" in the visitors?
                this.commonExpressionToStringVisitor.Traverse(request.Filter.BoolCommonExpression.CommonExpression, stringBuilder);
            }

            if (request.Select != null)
            {
                if (subsequentOption)
                {
                    stringBuilder.Append("&");
                }
                else
                {
                    stringBuilder.Append("?");
                }

                stringBuilder.Append("$select=");
                this.selectItemToStringVisitor.Traverse(
                    request.Select.SelectItem,
                    stringBuilder);
                foreach (var selectItem in request.Select.SelectItems)
                {
                    stringBuilder.Append(",");
                    this.selectItemToStringVisitor.Traverse(
                        selectItem,
                        stringBuilder);
                }
            }

            if (request.Top != null)
            {
                if (subsequentOption)
                {
                    stringBuilder.Append("&");
                }
                else
                {
                    stringBuilder.Append("?");
                }

                stringBuilder.Append("$top=");
                this.digitToStringVisitor.Traverse(
                    request.Top.Digit,
                    stringBuilder);
                foreach (var digit in request.Top.Digits)
                {
                    this.digitToStringVisitor.Traverse(
                        digit,
                        stringBuilder);
                }
            }

            var uri = new Uri(stringBuilder.ToString(), UriKind.Relative).ToRelativeUri();
            HttpResponseMessage? httpResponseMessage = null;
            try
            {
                httpResponseMessage = await this.httpClient.GetAsync(uri).ConfigureAwait(false);
                return new Response.Collection(
                    (int)httpResponseMessage.StatusCode, 
                    httpResponseMessage.Headers.ToDictionary(element => element.Key, element => element.Value), 
                    await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false));
            }
            catch
            {
                httpResponseMessage?.Dispose();
                throw;
            }
        }
    }
}
