namespace System.Linq.V2
{
    public interface IApplyAggregationEnumerable<out TElement> : IV2Enumerable<TElement>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAggregate"></typeparam>
        /// <param name="seed"></param>
        /// <param name="aggregator"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="aggregator"/> is <see langword="null"/></exception>
        public IAggregatedEnumerable<TAggregate, TElement, IV2Enumerable<TElement>> ApplyAggregation<TAggregate>(
            TAggregate seed, 
            Func<TAggregate, TElement, TAggregate> aggregator)
        {
            if (aggregator == null)
            {
                throw new ArgumentNullException(nameof(aggregator));
            }

            return this.ApplyAggregationDefault(seed, aggregator);
        }
    }
}
