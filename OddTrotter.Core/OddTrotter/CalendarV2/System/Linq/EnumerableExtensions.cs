namespace CalendarV2.System.Linq
{
    using global::System.Collections.Generic;

    public static class EnumerableExtensions
    {
        public abstract class FirstOrDefaultResult<TElement, TDefault>
        {
            private FirstOrDefaultResult()
            {
            }

            public sealed class First : FirstOrDefaultResult<TElement, TDefault>
            {
                public First(TElement element)
                {
                }
            }

            public sealed class Default : FirstOrDefaultResult<TElement, TDefault>
            {
                public Default(TDefault @default)
                {
                }
            }
        }

        public static FirstOrDefaultResult<TElement, TDefault> FirstOrDefault<TElement, TDefault>(
            this IEnumerable<TElement> source,
            TDefault @default)
        {

        }
    }
}
