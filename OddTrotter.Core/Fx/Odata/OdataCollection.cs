namespace Fx.Odata
{
    using System.Collections.Generic;

    public sealed class OdataCollection<T>
    {
        public OdataCollection(IEnumerable<T> elements)
            : this(elements, null)
        {
        }

        public OdataCollection(IEnumerable<T> elements, string? lastRequestedPageUrl)
        {
            this.Elements = elements;
            this.LastRequestedPageUrl = lastRequestedPageUrl;
        }

        public IEnumerable<T> Elements { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// <see langword="null"/> if there are collection was read to completeness; has a value if reading a next link produced an error, where the value is the nextlink that
        /// produced the error
        /// </remarks>
        public string? LastRequestedPageUrl { get; }
    }
}
