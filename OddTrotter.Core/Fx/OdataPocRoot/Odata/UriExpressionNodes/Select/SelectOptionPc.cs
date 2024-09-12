
namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Select
{
    public abstract class SelectOptionPc
    {
        private SelectOptionPc()
        {
            //// TODO this is where the spaghetti occurs; you now have to pull in all of the required nodes for the rest of the query options
        }

        public sealed class FilterNode : SelectOptionPc
        {
            public FilterNode(Fx.OdataPocRoot.Odata.UriExpressionNodes.Filter.Filter filter)
            {
                this.Filter = filter;
            }

            public Fx.OdataPocRoot.Odata.UriExpressionNodes.Filter.Filter Filter { get; }
        }

        public sealed class SearchNode : SelectOptionPc
        {
            public SearchNode(Fx.OdataPocRoot.Odata.UriExpressionNodes.Search.Search search)
            {
                this.Search = search;
            }

            public Fx.OdataPocRoot.Odata.UriExpressionNodes.Search.Search Search { get; }
        }

        public sealed class InlineCountNode : SelectOptionPc
        {
            public InlineCountNode(Fx.OdataPocRoot.Odata.UriExpressionNodes.InlineCount.InlineCount inclineCount)
            {
                this.InlineCount = inclineCount;
            }

            public Fx.OdataPocRoot.Odata.UriExpressionNodes.InlineCount.InlineCount InlineCount { get; }
        }

        public sealed class OrderByNode : SelectOptionPc
        {
            public OrderByNode(Fx.OdataPocRoot.Odata.UriExpressionNodes.OrderBy.OrderBy orderBy)
            {
                this.OrderBy = orderBy;
            }

            public Fx.OdataPocRoot.Odata.UriExpressionNodes.OrderBy.OrderBy OrderBy { get; }
        }

        public sealed class SkipNode : SelectOptionPc
        {
            public SkipNode(Fx.OdataPocRoot.Odata.UriExpressionNodes.Skip.Skip Skip)
            {
                this.Skip = Skip;
            }

            public Fx.OdataPocRoot.Odata.UriExpressionNodes.Skip.Skip Skip { get; }
        }

        public sealed class TopNode : SelectOptionPc
        {
            public TopNode(Fx.OdataPocRoot.Odata.UriExpressionNodes.Top.Top top)
            {
                this.Top = top;
            }

            public Fx.OdataPocRoot.Odata.UriExpressionNodes.Top.Top Top { get; }
        }
    }
}
