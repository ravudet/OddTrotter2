namespace Microsoft.Extensions.Caching.Memory
{
    using System;

    public static class PartitionedMemoryCacheFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="partitionId"></param>
        /// <param name="memoryCache"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="memoryCache"/> is <see langword="null"/></exception>
        public static PartitionedMemoryCache<Guid> Create(Guid partitionId, IMemoryCache memoryCache)
        {
            if (memoryCache == null)
            {
                throw new ArgumentNullException(nameof(memoryCache));
            }

            return new PartitionedMemoryCache<Guid>(memoryCache, partitionId);
        }
    }
}
