namespace CalendarV2.System.Linq
{
    using global::System.Collections.Generic;

    using OddTrotter.CalendarV2.Fx.Either;

    public static class EnumerableExtensions
    {
        public sealed class FirstOrDefaultResult<TElement, TDefault> : IEither<TElement, TDefault>
        {
            private readonly IEither<TElement, TDefault> either;

            public FirstOrDefaultResult(IEither<TElement, TDefault> either)
            {
                this.either = either;
            }

            public TResult Visit<TResult, TContext>(
                global::System.Func<TElement, TContext, TResult> leftAccept, 
                global::System.Func<TDefault, TContext, TResult> rightAccept,
                TContext context)
            {
                return either.Visit(leftAccept, rightAccept, context);
            }
        }

        public static FirstOrDefaultResult<TElement, TDefault> FirstOrDefault<TElement, TDefault>(
            this IEnumerable<TElement> source,
            TDefault @default)
        {
            using (var enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    return new FirstOrDefaultResult<TElement, TDefault>(new Either<TElement, TDefault>.Right(@default));
                }

                return new FirstOrDefaultResult<TElement, TDefault>(new Either<TElement, TDefault>.Left(enumerator.Current));
            }
        }
    }
}
