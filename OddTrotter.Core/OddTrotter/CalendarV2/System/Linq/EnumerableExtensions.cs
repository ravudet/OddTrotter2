namespace CalendarV2.System.Linq
{
    using global::System;
    using global::System.Collections.Generic;

    using CalendarV2.Fx.Either;
    using CalendarV2.Fx.Try;

    public static class EnumerableExtensions
    {
        public sealed class FirstOrDefault<TFirst, TDefault> : IEither<TFirst, TDefault>
        {
            private readonly IEither<TFirst, TDefault> either;

            public FirstOrDefault(Either<TFirst, TDefault> either)
            {
                this.either = either;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="either"></param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="either"/> is <see langword="null"/></exception>
            public FirstOrDefault(IEither<TFirst, TDefault> either)
            {
                ArgumentNullException.ThrowIfNull(either);

                //// TODO any type that looks like this can actually end up implementing their own either and visitor and whatnot if the performance of the eitherdelegatevisitor ends up being bad...
                this.either = either;
            }

            /// <inheritdoc/>
            public TResult Visit<TResult, TContext>(
                global::System.Func<TFirst, TContext, TResult> leftAccept, 
                global::System.Func<TDefault, TContext, TResult> rightAccept,
                TContext context)
            {
                ArgumentNullException.ThrowIfNull(leftAccept);
                ArgumentNullException.ThrowIfNull(rightAccept);

                return either.Visit(leftAccept, rightAccept, context);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is <see langword="null"/></exception>
        public static FirstOrDefault<TElement, TElement?> EitherFirstOrDefault<TElement>(
            this IEnumerable<TElement> source)
        {
            ArgumentNullException.ThrowIfNull(source);

            return source.EitherFirstOrDefault(default(TElement));
        }

        /*public static TElement? Foo<TElement>(this IEnumerable<TElement> source)
        {
            return source.EitherFirstOrDefault().Coalesce();
        }*/

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <typeparam name="TDefault"></typeparam>
        /// <param name="source"></param>
        /// <param name="default"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is <see langword="null"/></exception>
        public static FirstOrDefault<TElement, TDefault> EitherFirstOrDefault<TElement, TDefault>(
            this IEnumerable<TElement> source,
            TDefault @default)
        {
            ArgumentNullException.ThrowIfNull(source);

            //// TODO you questioned this before, but here's a conclusion: i actually like that the default is it's own type parameter because it allows for more verbose cases; having the same type would just be another convenience method, but it's still beneficial over the linq implementation because you can differentiate between first or default; further, you can implement the linq overload as a convenience method on top of this one

            //// TODO actually write the convenience method mentioned in the comment above

            using (var enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    //// TODO use implicit cast when it's availalbe?
                    return new FirstOrDefault<TElement, TDefault>(new Either<TElement, TDefault>.Right(
                        @default));
                }

                //// TODO use implicit case when it's available?
                return new FirstOrDefault<TElement, TDefault>(new Either<TElement, TDefault>.Left(
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
