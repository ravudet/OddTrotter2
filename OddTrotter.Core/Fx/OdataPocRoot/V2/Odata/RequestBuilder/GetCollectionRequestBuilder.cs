////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.V2.Odata.RequestBuilder
{
    using Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter;
    using Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Select;
    using Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Top;
    using Fx.OdataPocRoot.V2.System.Uri;

    public sealed class GetCollectionRequestBuilder : IGetCollectionRequestBuilder
    {
        public GetCollectionRequestBuilder(Path uri)
            : this(uri, null, null, null)
        {
        }

        private GetCollectionRequestBuilder(Path uri, Filter? filter, Select? select, Top? top)
        {
            this.Request = new Request.GetCollection(uri, filter, select, top);
        }

        public Request.GetCollection Request { get; }

        public IGetCollectionRequestBuilder Filter(Filter filter)
        {
            var newFilter = filter;
            if (this.Request.Filter != null)
            {
                newFilter =
                    new Filter(
                        new BoolCommonExpression(
                            new CommonExpression.Part1Part4(
                                new CommonExpressionPart1.ParenExpr(
                                    new ParenExpr(this.Request.Filter.BoolCommonExpression.CommonExpression)),
                                new CommonExpressionPart4.AndExpr(filter.BoolCommonExpression))));
            }

            return new GetCollectionRequestBuilder(this.Request.Uri, newFilter, this.Request.Select, this.Request.Top);
        }

        public IGetCollectionRequestBuilder Select(Select select)
        {
            throw new global::System.NotImplementedException();
        }

        public IGetCollectionRequestBuilder Top(Top top)
        {
            throw new global::System.NotImplementedException();
        }
    }
}
