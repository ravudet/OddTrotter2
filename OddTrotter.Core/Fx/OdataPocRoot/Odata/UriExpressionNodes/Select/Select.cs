namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Select
{
    using Fx.OdataPocRoot.Odata.UriExpressionVisitors;
    using System.Collections.Generic;

    public sealed class Select : SelectBaseNode
    {
        public Select(IEnumerable<SelectItem> selectItems)
        {
            this.SelectItems = selectItems;
        }

        public IEnumerable<SelectItem> SelectItems { get; }

        public override void Accept(SelectVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
