namespace Fx.Caching
{
    using System;

    using Microsoft.Extensions.Caching.Memory;

    public sealed class NullableObjectCache : INullableObjectCache
    {
        private readonly IMemoryCache memoryCache;

        public NullableObjectCache(IMemoryCache memoryCache)
        {
            if (memoryCache == null)
            {
                throw new ArgumentNullException(nameof(memoryCache));
            }

            this.memoryCache = memoryCache;
        }

        public void CreateEntry(object key, object? value)
        {
            using (var entry = this.memoryCache.CreateEntry(key))
            {
                entry.Value = value;
            }
        }

        public void Remove(object key)
        {
            this.memoryCache.Remove(key);
        }

        public bool TryGetValue(object key, out object? value)
        {
            return this.memoryCache.TryGetValue(key, out value);
        }
    }
}
