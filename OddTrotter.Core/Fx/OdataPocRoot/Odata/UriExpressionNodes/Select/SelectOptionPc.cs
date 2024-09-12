
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
            public FilterNode(Fx.OdataPocRoot.Odata.UriExpressionNodes.Filter.FilterNode filter)
            {
                this.Filter = filter;
            }

            public Fx.OdataPocRoot.Odata.UriExpressionNodes.Filter.FilterNode Filter { get; }
        }

        public sealed class SearchNode : SelectOptionPc
        {
            public SearchNode(Fx.OdataPocRoot.Odata.UriExpressionNodes.Search.SearchNode search)
            {
                this.Search = search;
            }

            public Fx.OdataPocRoot.Odata.UriExpressionNodes.Search.SearchNode Search { get; }
        }

        public sealed class InlineCountNode : SelectOptionPc
        {
            public InlineCountNode(Fx.OdataPocRoot.Odata.UriExpressionNodes.InlineCount.InlineCountNode inclineCount)
            {
                this.InlineCount = inclineCount;
            }

            public Fx.OdataPocRoot.Odata.UriExpressionNodes.InlineCount.InlineCountNode InlineCount { get; }
        }

        public sealed class OrderByNode : SelectOptionPc
        {
            public OrderByNode(Fx.OdataPocRoot.Odata.UriExpressionNodes.OrderBy.OrderByNode orderBy)
            {
                this.OrderBy = orderBy;
            }

            public Fx.OdataPocRoot.Odata.UriExpressionNodes.OrderBy.OrderByNode OrderBy { get; }
        }

        public sealed class SkipNode : SelectOptionPc
        {
            public SkipNode(Fx.OdataPocRoot.Odata.UriExpressionNodes.Skip.SkipNode Skip)
            {
                this.Skip = Skip;
            }

            public Fx.OdataPocRoot.Odata.UriExpressionNodes.Skip.SkipNode Skip { get; }
        }

        public sealed class TopNode : SelectOptionPc
        {
            public TopNode(Fx.OdataPocRoot.Odata.UriExpressionNodes.Top.TopNode top)
            {
                this.Top = top;
            }

            public Fx.OdataPocRoot.Odata.UriExpressionNodes.Top.TopNode Top { get; }
        }
    }
}
