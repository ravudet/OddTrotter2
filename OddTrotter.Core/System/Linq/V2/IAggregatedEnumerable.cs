namespace System.Linq.V2
{
    public interface IAggregatedEnumerable<out TAggregate1, out TElement, out TEnumerable> : IV2Enumerable<TElement> where TEnumerable : IV2Enumerable<TElement>
    {
        TAggregate1 Aggregation { get; }

        TEnumerable OnTopOf { get; }

        IAggregatedEnumerable<TAggregate2, TElement, IAggregatedEnumerable<TAggregate1, TElement, TEnumerable>> ApplyAggregation<TAggregate2>(
            TAggregate2 seed, 
            Func<TAggregate2, TElement, TAggregate2> aggregator);
    }
}
