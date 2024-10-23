////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.V2.Odata.RequestBuilder
{
    using Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter;
    using Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Select;
    using Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Top;
    using Fx.OdataPocRoot.V2.System.Uri;
    using System.Net.NetworkInformation;

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
                var filterVisitor = new FilterVisitor(); //// TODO "context" is supposed to be used for a builder, but doing it this way allows to have a singleton visitor; is that worth it?
                newFilter = 
                    new Filter(
                        new BoolCommonExpression(    
                            filterVisitor.Traverse(
                                this.Request.Filter.BoolCommonExpression.CommonExpression, 
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

        private sealed class FilterVisitor : CommonExpression.Visitor<CommonExpression, CommonExpressionPart4.AndExpr>
        {
            public sealed override CommonExpression Visit(CommonExpression.Part1Only node, CommonExpressionPart4.AndExpr context)
            {
                return new CommonExpression.Part1Part4(
                    node.Part1,
                    context);
            }

            public sealed override CommonExpression Visit(CommonExpression.Part1Part2 node, CommonExpressionPart4.AndExpr context)
            {
                return new CommonExpression.Part1Part2Part4(
                    node.Part1,
                    node.Part2,
                    context);
            }

            public sealed override CommonExpression Visit(CommonExpression.Part1Part2Part3 node, CommonExpressionPart4.AndExpr context)
            {
                return new CommonExpression.Part1Part2Part3Part4(
                    node.Part1,
                    node.Part2,
                    node.Part3,
                    context);
            }

            public sealed override CommonExpression Visit(CommonExpression.Part1Part2Part3Part4 node, CommonExpressionPart4.AndExpr context)
            {
                return new CommonExpression.Part1Part2Part3Part4(
                    node.Part1,
                    node.Part2,
                    node.Part3,
                    )
                throw new global::System.NotImplementedException();
            }

            public sealed override CommonExpression Visit(CommonExpression.Part1Part3 node, CommonExpressionPart4.AndExpr context)
            {
                return new CommonExpression.Part1Part3Part4(
                    node.Part1,
                    node.Part3,
                    context);
            }

            public sealed override CommonExpression Visit(CommonExpression.Part1Part3Part4 node, CommonExpressionPart4.AndExpr context)
            {
                throw new global::System.NotImplementedException();
            }

            public sealed override CommonExpression Visit(CommonExpression.Part1Part4 node, CommonExpressionPart4.AndExpr context)
            {
                throw new global::System.NotImplementedException();
            }

            public override CommonExpression Visit(CommonExpression.Part1Part2Part4 node, CommonExpressionPart4.AndExpr context)
            {
                throw new global::System.NotImplementedException();
            }
        }
    }
}
