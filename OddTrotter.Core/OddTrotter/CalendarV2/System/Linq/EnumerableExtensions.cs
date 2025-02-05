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

        public static EitherFirstOrDefaultResult<TElement, TElement> EitherFirstOrDefault<TElement>(
            this IEnumerable<TElement> source)
        {
            return source.EitherFirstOrDefault(default(TElement)!);
        }

        public static EitherFirstOrDefaultResult<TElement, TDefault> EitherFirstOrDefault<TElement, TDefault>(
            this IEnumerable<TElement> source,
            TDefault @default)
        {
            //// TODO you questioned this before, but here's a conclusion: i actually like that the default is it's own type parameter because it allows for more verbose cases; having the same type would just be another convenience method, but it's still beneficial over the linq implementation because you can differentiate between first or default; further, you can implement the linq overload as a convenience method on top of this one

            //// TODO actually write the convenience method mentioned in the comment above

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
