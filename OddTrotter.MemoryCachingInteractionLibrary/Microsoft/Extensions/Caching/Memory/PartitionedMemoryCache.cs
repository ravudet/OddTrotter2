namespace Microsoft.Extensions.Caching.Memory
{
    using System;
    using System.Collections.Generic;

    public sealed class PartitionedMemoryCache<T> : IMemoryCache
    {
        private readonly IMemoryCache delegateMemoryCache;

        private readonly T partitionId;

        private readonly IEqualityComparer<T> comparer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="delegateMemoryCache"></param>
        /// <param name="partitionId"></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="delegateMemoryCache"/> or <paramref name="partitionId"/> is <see langword="null"/></exception>
        public PartitionedMemoryCache(IMemoryCache delegateMemoryCache, T partitionId)
            : this(delegateMemoryCache, partitionId, PartitionedMemoryCacheSettings<T>.Default)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="delegateMemoryCache"></param>
        /// <param name="partitionId"></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="delegateMemoryCache"/> or <paramref name="partitionId"/> or <paramref name="settings"/> is <see langword="null"/></exception>
        public PartitionedMemoryCache(IMemoryCache delegateMemoryCache, T partitionId, PartitionedMemoryCacheSettings<T> settings)
        {
            if (delegateMemoryCache == null)
            {
                throw new ArgumentNullException(nameof(delegateMemoryCache));
            }

            if (partitionId == null)
            {
                throw new ArgumentNullException(nameof(partitionId));
            }

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            this.delegateMemoryCache = delegateMemoryCache;
            this.partitionId = partitionId;
            this.comparer = settings.Comparer;
        }

        /// <inheritdoc/>
        public ICacheEntry CreateEntry(object key)
        {
            var keyContainer = new KeyContainer(this.partitionId, key, this.comparer);
            return this.delegateMemoryCache.CreateEntry(keyContainer);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
        }

        /// <inheritdoc/>
        public void Remove(object key)
        {
            var keyContainer = new KeyContainer(this.partitionId, key, this.comparer);
            this.delegateMemoryCache.Remove(keyContainer);
        }

        /// <inheritdoc/>
        public bool TryGetValue(object key, out object? value)
        {
            var keyContainer = new KeyContainer(this.partitionId, key, this.comparer);
            return this.delegateMemoryCache.TryGetValue(keyContainer, out value);
        }

        private sealed class KeyContainer : IEquatable<KeyContainer>
        {
            private readonly IEqualityComparer<T> comparer;

            public KeyContainer(T partitionId, object key, IEqualityComparer<T> comparer)
            {
                this.PartitionId = partitionId;
                this.Key = key;
                this.comparer = comparer;
            }

            public T PartitionId { get; }

            public object Key { get; }

            /// <inheritdoc/>
            public bool Equals(KeyContainer? other)
            {
                if (object.ReferenceEquals(this, other))
                {
                    return true;
                }

                if (other == null)
                {
                    return false;
                }

                return
                    this.comparer.Equals(this.PartitionId, other.PartitionId) &&
                    this.Key.Equals(other.Key);
            }

            /// <inheritdoc/>
            public override bool Equals(object? obj)
            {
                if (obj is KeyContainer keyContainer)
                {
                    return this.Equals(keyContainer);
                }

                return base.Equals(obj);
            }

            /// <inheritdoc/>
            public override int GetHashCode()
            {
                return
                    (this.PartitionId == null ? 0 : this.comparer.GetHashCode(this.PartitionId)) ^
                    this.Key.GetHashCode();
            }
        }
    }
}
