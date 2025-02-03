namespace CalendarV2.System.Linq
{
    using global::System.Collections.Generic;

    using CalendarV2.Fx.Either;
    using CalendarV2.Fx.Try;

    public static class EnumerableExtensions
    {
        public sealed class EitherFirstOrDefaultResult<TElement, TDefault> : IEither<TElement, TDefault>
        {
            private readonly IEither<TElement, TDefault> either;

            public EitherFirstOrDefaultResult(IEither<TElement, TDefault> either)
            {
                //// TODO any type that looks like this can actually end up implementing their own either and visitor and whatnot if the performance of the eitherdelegatevisitor ends up being bad...
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

        public static EitherFirstOrDefaultResult<TElement, TDefault> EitherFirstOrDefault<TElement, TDefault>(
            this IEnumerable<TElement> source,
            TDefault @default)
        {
            using (var enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    return new EitherFirstOrDefaultResult<TElement, TDefault>(new Either<TElement, TDefault>.Right(
                        @default));
                }

                return new EitherFirstOrDefaultResult<TElement, TDefault>(new Either<TElement, TDefault>.Left(
                    enumerator.Current));
            }
        }

        public static IEnumerable<TResult> TrySelect<TElement, TResult>(
            this IEnumerable<TElement> source,
            Try<TElement, TResult> @try)
        {
            foreach (var element in source)
            {
                if (@try(element, out var result))
                {
                    yield return result;
                }
            }
        }
    }
}
