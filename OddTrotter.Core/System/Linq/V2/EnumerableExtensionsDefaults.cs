namespace System.Linq.V2
{
    using System.Collections;
    using System.Collections.Generic;
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAggregate"></typeparam>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="self"></param>
        /// <param name="seed"></param>
        /// <param name="aggregator"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="self"/> or <paramref name="aggregator"/> is <see langword="null"/></exception>
        internal static IAggregatedEnumerable<TAggregate, TElement, IV2Enumerable<TElement>> ApplyAggregationDefault<TAggregate, TElement>(
            this IV2Enumerable<TElement> self,
            TAggregate seed,
            Func<TAggregate, TElement, TAggregate> aggregator)
        {
            if (self == null)
            {
                throw new ArgumentNullException(nameof(self));
            }

            if (aggregator == null)
            {
                throw new ArgumentNullException(nameof(aggregator));
            }

            if (self is IAggregatedOverloadEnumerable<TElement> aggregatedOverload)
            {
                return aggregatedOverload.Source.ApplyAggregation(seed, aggregator);
            }

            return new AggregatedEnumerable<TAggregate, TElement, IV2Enumerable<TElement>>(self, seed, aggregator);

        }

        private struct SpecialNullable<T>
        {
            public SpecialNullable(T value)
            {
                this.Value = value;
                this.HasValue = true;
            }

            public T Value { get; }

            public bool HasValue { get; }
        }

        private sealed class AggregatedEnumerable<TAggregate, TElement, TEnumerable> : IAggregatedEnumerable<TAggregate, TElement, TEnumerable> 
            where TEnumerable : IV2Enumerable<TElement>
        {
            private readonly TAggregate seed;
            private readonly Func<TAggregate, TElement, TAggregate> aggregator;

            private SpecialNullable<TAggregate> aggregation;

            public AggregatedEnumerable(TEnumerable source, TAggregate seed, Func<TAggregate, TElement, TAggregate> aggregator)
            {
                //// TODO do you really like this?

                this.OnTopOf = source;
                this.seed = seed;
                this.aggregator = aggregator;
            }

            public TAggregate Aggregation
            {
                get
                {
                    if (!this.aggregation.HasValue)
                    {
                        foreach (var element in this)
                        {
                        }
                    }

                    return this.aggregation.Value;
                }
            }

            public TEnumerable OnTopOf { get; }

            public IAggregatedEnumerable<TAggregate2, TElement, IAggregatedEnumerable<TAggregate, TElement, TEnumerable>> ApplyAggregation<TAggregate2>(
                TAggregate2 seed, 
                Func<TAggregate2, TElement, TAggregate2> aggregator)
            {
                return new AggregatedEnumerable<TAggregate2, TElement, AggregatedEnumerable<TAggregate, TElement, TEnumerable>>(this, seed, aggregator);
            }

            public IEnumerator<TElement> GetEnumerator()
            {
                var aggregation = this.seed;
                foreach (var element in this.OnTopOf)
                {
                    aggregation = this.aggregator(aggregation, element);
                    yield return element;
                }

                this.aggregation = new SpecialNullable<TAggregate>(aggregation);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }
    }
}
