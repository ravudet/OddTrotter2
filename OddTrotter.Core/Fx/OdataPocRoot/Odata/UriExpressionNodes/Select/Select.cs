namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Select
{
    using System.Collections.Generic;

    public sealed class Select
    {
        public Select(IEnumerable<SelectItem> selectItems)
        {
            this.SelectItems = selectItems;
        }

        public IEnumerable<SelectItem> SelectItems { get; }
    }
}
