namespace System.Linq
{
    using System;
    using System.Collections.Generic;

    using Fx.Either;
    using Fx.Try;

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
            public TResult Apply<TResult, TContext>(
                global::System.Func<TFirst, TContext, TResult> leftAccept, 
                global::System.Func<TDefault, TContext, TResult> rightAccept,
                TContext context)
            {
                ArgumentNullException.ThrowIfNull(leftAccept);
                ArgumentNullException.ThrowIfNull(rightAccept);

                return either.Apply(leftAccept, rightAccept, context);
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

        public static TElement? Foo<TElement>(this IEnumerable<TElement> source)
        {
            //// TODO do you want a concenience overload for coalesce around nullables?
            return source.EitherFirstOrDefault().CoalesceLeft(left => left);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <typeparam name="TDefault"></typeparam>
        /// <param name="source"></param>
        /// <param name="default"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is <see langword="null"/></exception>
        /// <remarks>
        /// I wasn't sure if I liked having a second <typeparamref name="TDefault"/> type parameter, but ultimately it just gives callers more flexibility (i.e. cases where <typeparamref name="TElement"/> and <typeparamref name="TDefault"/> are the same get resolved by the caller to simply call this method anyway). Even if we only had one type parameter, we could still implement the LINQ overloads by delegating to this overload, and this method also provides the flexibility in cases where the type parameter is the same of diffentiating between and first and a default. 
        /// </remarks>
        public static FirstOrDefault<TElement, TDefault> EitherFirstOrDefault<TElement, TDefault>(
            this IEnumerable<TElement> source,
            TDefault @default)
        {
            ArgumentNullException.ThrowIfNull(source);

            using (var enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    return new FirstOrDefault<TElement, TDefault>(@default);
                }

                return new FirstOrDefault<TElement, TDefault>(enumerator.Current);
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
