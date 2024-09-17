////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.Odata.RequestBuilder
{
    using System;
    using System.Linq;

    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Expand;
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Select;

    public sealed class GetInstanceRequestBuilder : IGetInstanceRequestBuilder
    {
        private readonly RelativeUri uri;

        private readonly Expand? expand;
        private readonly Select? select;

        public GetInstanceRequestBuilder(RelativeUri uri)
            : this(uri, null, null)
        {
        }

        private GetInstanceRequestBuilder(RelativeUri uri, Expand? expand, Select? select)
        {
            this.uri = uri;
            this.expand = expand;
            this.select = select;
        }

        public OdataRequest.GetInstance Request
        {
            get
            {
                return new OdataRequest.GetInstance(this.uri, this.expand, this.select);
            }
        }

        public IGetInstanceRequestBuilder Expand(Expand query)
        {
            throw new System.NotImplementedException("TODO implement expand for request builder");
        }

        public IGetInstanceRequestBuilder Select(Select query)
        {
            var select = query;
            if (this.select != null)
            {
                select = new Select(this.select.SelectItems.Concat(select.SelectItems));
            }

            return new GetInstanceRequestBuilder(this.uri, this.expand, select);
        }
    }
}
