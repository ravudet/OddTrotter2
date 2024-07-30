namespace Microsoft.Extensions.Caching.Memory
{
    using System;
    using System.Collections.Generic;

    public sealed class PartitionedMemoryCacheSettings<T>
    {
        private PartitionedMemoryCacheSettings(IEqualityComparer<T> comparer)
        {
            this.Comparer = comparer;
        }

        public static PartitionedMemoryCacheSettings<T> Default { get; } = new PartitionedMemoryCacheSettings<T>(EqualityComparer<T>.Default);

        public IEqualityComparer<T> Comparer { get; }

        public sealed class Builder
        {
            public IEqualityComparer<T> Comparer { get; set; } = PartitionedMemoryCacheSettings<T>.Default.Comparer;

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown if <see cref="Comparer"/> is <see langword="null"/></exception>
            public PartitionedMemoryCacheSettings<T> Build()
            {
                if (this.Comparer == null)
                {
                    throw new ArgumentNullException(nameof(this.Comparer));
                }

                return new PartitionedMemoryCacheSettings<T>(this.Comparer);
            }
        }
    }
}
