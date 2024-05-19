namespace Microsoft.Extensions.Caching.Memory
{
    using System;

    public sealed class PartitionedMemoryCache<T> : IMemoryCache
    {
        private readonly IMemoryCache delegateMemoryCache;

        private readonly T partitionId;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="delegateMemoryCache"></param>
        /// <param name="partitionId"></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="delegateMemoryCache"/> or <paramref name="partitionId"/> is <see langword="null"/></exception>
        public PartitionedMemoryCache(IMemoryCache delegateMemoryCache, T partitionId)
        {
            if (delegateMemoryCache == null)
            {
                throw new ArgumentNullException(nameof(delegateMemoryCache));
            }

            if (partitionId == null)
            {
                throw new ArgumentNullException(nameof(partitionId));
            }

            this.delegateMemoryCache = delegateMemoryCache;
            this.partitionId = partitionId;
        }

        public ICacheEntry CreateEntry(object key)
        {
            var keyContainer = new KeyContainer(this.partitionId, key);
            return this.delegateMemoryCache.CreateEntry(keyContainer);
        }

        public void Dispose()
        {
        }

        public void Remove(object key)
        {
            var keyContainer = new KeyContainer(this.partitionId, key);
            this.delegateMemoryCache.Remove(keyContainer);
        }

        public bool TryGetValue(object key, out object? value)
        {
            var keyContainer = new KeyContainer(this.partitionId, key);
            return this.delegateMemoryCache.TryGetValue(keyContainer, out value);
        }

        private sealed class KeyContainer
        {
            public KeyContainer(T partitionId, object key)
            {
                PartitionId = partitionId;
                Key = key;
            }

            public T PartitionId { get; }
            public object Key { get; }
        }
    }
}
