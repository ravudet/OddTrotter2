////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.Odata.RequestBuilder
{
    using System;
    using System.Linq;

    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Expand;
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Filter;
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Select;

    /// <summary>
    /// TODO should this be a struct? for cases where it's used directly, this seems like a good idea, but does it matter for cases where it's boxed? do you want to just implement two of them? however, the generic version is going to use the interface, so maybe boxing will almost always happen anyway? what about cases like an aggregator service where generics are avoided entirely?
    /// </summary>
    public sealed class GetCollectionRequestBuilder : IGetCollectionRequestBuilder
    {
        private readonly RelativeUri uri;

        private readonly Expand? expand;
        private readonly Filter? filter;
        private readonly Select? select;

        public GetCollectionRequestBuilder(RelativeUri uri)
            : this(uri, null, null, null)
        {
        }

        private GetCollectionRequestBuilder(RelativeUri uri, Expand? expand, Filter? filter, Select? select)
        {
            this.uri = uri;
            this.expand = expand;
            this.filter = filter;
            this.select = select;
        }

        public OdataRequest.GetCollection Request
        {
            get
            {
                //// TODO use a lazy?
                return new OdataRequest.GetCollection(this.uri, this.expand, this.filter, this.select);
            }
        }

        public IGetInstanceRequestBuilder Expand(Expand query)
        {
            throw new System.NotImplementedException("TODO implement expand for request builder");
        }

        public IGetCollectionRequestBuilder Filter(Filter query)
        {
            var filter = query;
            if (this.filter != null)
            {
                filter = new Filter(
                    new BoolCommonExpression.Second(
                        this.filter.BoolCommonExpression, 
                        new AndExpression(filter.BoolCommonExpression)));
            }

            return new GetCollectionRequestBuilder(this.uri, this.expand, filter, this.select);
        }

        public IGetCollectionRequestBuilder Select(Select query)
        {
            var select = query;
            if (this.select != null)
            {
                select = new Select(this.select.SelectItems.Concat(select.SelectItems));
            }

            return new GetCollectionRequestBuilder(this.uri, this.expand, this.filter, select);
        }
    }
}
