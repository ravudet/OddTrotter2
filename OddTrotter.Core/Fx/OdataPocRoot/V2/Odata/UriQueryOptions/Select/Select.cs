////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Select
{
    using global::System.Collections.Generic;

    public sealed class Select
    {
        public Select(SelectItem selectItem, IEnumerable<SelectItem> selectItems)
        {
            this.SelectItem = selectItem;
            this.SelectItems = selectItems;
        }

        public SelectItem SelectItem { get; }

        public IEnumerable<SelectItem> SelectItems { get; }
    }
}
