﻿/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.Either
{
    using CalendarV2.Fx.Try;
    using System;
    using System.Diagnostics.CodeAnalysis;

    public static class EitherExtensions
    {
        //// TODO TOPIC should all of this be lazy?

        //// TODO see how the implicit conversions can be leveraged in this extensions class
        //// TODO make sure all of these names are aligned with the `accept`, `leftmap`, `rightmap`, and `context` names for `ieither`
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="either"></param>
        /// <param name="leftMap"></param>
        /// <param name="rightMap"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="either"/> or <paramref name="leftMap"/> or <paramref name="rightMap"/> is
        /// <see langword="null"/>
        /// </exception>
        /// <exception cref="LeftMapException">
        /// Thrown if <paramref name="leftMap"/> throws an exception. The <see cref="Exception.InnerException"/> will be set to
        /// whatever exception <paramref name="leftMap"/> threw.
        /// </exception>
        /// <exception cref="RightMapException">
        /// Thrown if <paramref name="rightMap"/> throws an exception. The <see cref="Exception.InnerException"/> will be set to
        /// whatever exception <paramref name="rightMap"/> threw.
        /// </exception>
        public static TResult Apply<TLeft, TRight, TResult>(
            this IEither<TLeft, TRight> either,
            Func<TLeft, TResult> leftMap,
            Func<TRight, TResult> rightMap)
        {
            ArgumentNullException.ThrowIfNull(either);
            ArgumentNullException.ThrowIfNull(leftMap);
            ArgumentNullException.ThrowIfNull(rightMap);

            return either.Apply(
                (left, _) => leftMap(left),
                (right, _) => rightMap(right),
                new Nothing());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TLeftValue"></typeparam>
        /// <typeparam name="TRightValue"></typeparam>
        /// <typeparam name="TLeftResult"></typeparam>
        /// <typeparam name="TRightResult"></typeparam>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="either"></param>
        /// <param name="leftSelector"></param>
        /// <param name="rightSelector"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="either"/> or <paramref name="leftSelector"/> or <paramref name="rightSelector"/> is
        /// <see langword="null"/>
        /// </exception>
        /// <exception cref="LeftMapException">
        /// Thrown if <paramref name="leftSelector"/> throws an exception. The <see cref="Exception.InnerException"/> will be set
        /// to whatever exception <paramref name="leftSelector"/> threw.
        /// </exception>
        /// <exception cref="RightMapException">
        /// Thrown if <paramref name="rightSelector"/> throws an exception. The <see cref="Exception.InnerException"/> will be
        /// set to whatever exception <paramref name="rightSelector"/> threw.
        /// </exception>
        public static IEither<TLeftResult, TRightResult> Select
            <
                TLeftValue,
                TRightValue,
                TLeftResult,
                TRightResult,
                TContext
            >
            (
                this IEither<TLeftValue, TRightValue> either,
                Func<TLeftValue, TContext, TLeftResult> leftSelector,
                Func<TRightValue, TContext, TRightResult> rightSelector,
                TContext context
            )
        {
            ArgumentNullException.ThrowIfNull(either);
            ArgumentNullException.ThrowIfNull(leftSelector);
            ArgumentNullException.ThrowIfNull(rightSelector);

            return either.Apply(
                (left, context) => Either.Left(leftSelector(left, context)).Right<TRightResult>(),
                (right, context) => Either.Left<TLeftResult>().Right(rightSelector(right, context)),
                context);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TLeftValue"></typeparam>
        /// <typeparam name="TRightValue"></typeparam>
        /// <typeparam name="TLeftResult"></typeparam>
        /// <typeparam name="TRightResult"></typeparam>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="either"></param>
        /// <param name="leftSelector"></param>
        /// <param name="rightSelector"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="either"/> or <paramref name="leftSelector"/> is <see langword="null"/>
        /// </exception>
        /// <exception cref="LeftMapException">
        /// Thrown if <paramref name="leftSelector"/> throws an exception. The <see cref="Exception.InnerException"/> will be set
        /// to whatever exception <paramref name="leftSelector"/> threw.
        /// </exception>
        public static IEither<TLeftResult, TRightValue> SelectLeft
            <
                TLeftValue,
                TRightValue,
                TLeftResult,
                TContext
            >
            (
                this IEither<TLeftValue, TRightValue> either,
                Func<TLeftValue, TContext, TLeftResult> leftSelector,
                TContext context
            )
        {
            ArgumentNullException.ThrowIfNull(either);
            ArgumentNullException.ThrowIfNull(leftSelector);

            return either.Select(
                leftSelector, 
                (right, _) => right, 
                context);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TLeftValue"></typeparam>
        /// <typeparam name="TRightValue"></typeparam>
        /// <typeparam name="TLeftResult"></typeparam>
        /// <typeparam name="TRightResult"></typeparam>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="either"></param>
        /// <param name="leftSelector"></param>
        /// <param name="rightSelector"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="either"/> or <paramref name="rightSelector"/> is <see langword="null"/>
        /// </exception>
        /// <exception cref="RightMapException">
        /// Thrown if <paramref name="rightSelector"/> throws an exception. The <see cref="Exception.InnerException"/> will be
        /// set to whatever exception <paramref name="rightSelector"/> threw.
        /// </exception>
        public static IEither<TLeftValue, TRightResult> SelectRight
            <
                TLeftValue,
                TRightValue,
                TRightResult,
                TContext
            >
            (
                this IEither<TLeftValue, TRightValue> either,
                Func<TRightValue, TContext, TRightResult> rightSelector,
                TContext context
            )
        {
            ArgumentNullException.ThrowIfNull(either);
            ArgumentNullException.ThrowIfNull(rightSelector);

            return either.Select(
                (left, _) => left,
                rightSelector,
                context);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TLeftValue"></typeparam>
        /// <typeparam name="TRightValue"></typeparam>
        /// <typeparam name="TLeftResult"></typeparam>
        /// <typeparam name="TRightResult"></typeparam>
        /// <param name="either"></param>
        /// <param name="leftSelector"></param>
        /// <param name="rightSelector"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="either"/> or <paramref name="leftSelector"/> or <paramref name="rightSelector"/> is
        /// <see langword="null"/>
        /// </exception>
        /// <exception cref="LeftMapException">
        /// Thrown if <paramref name="leftSelector"/> throws an exception. The <see cref="Exception.InnerException"/> will be set
        /// to whatever exception <paramref name="leftSelector"/> threw.
        /// </exception>
        /// <exception cref="RightMapException">
        /// Thrown if <paramref name="rightSelector"/> throws an exception. The <see cref="Exception.InnerException"/> will be
        /// set to whatever exception <paramref name="rightSelector"/> threw.
        /// </exception>
        public static IEither<TLeftResult, TRightResult> Select
            <
                TLeftValue,
                TRightValue,
                TLeftResult,
                TRightResult
            >
            (
                this IEither<TLeftValue, TRightValue> either,
                Func<TLeftValue, TLeftResult> leftSelector,
                Func<TRightValue, TRightResult> rightSelector
            )
        {
            ArgumentNullException.ThrowIfNull(either);
            ArgumentNullException.ThrowIfNull(leftSelector);
            ArgumentNullException.ThrowIfNull(rightSelector);

            return either.Select(
                (left, _) => leftSelector(left),
                (right, _) => rightSelector(right), 
                new Nothing());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TLeftValue"></typeparam>
        /// <typeparam name="TRightValue"></typeparam>
        /// <typeparam name="TLeftResult"></typeparam>
        /// <typeparam name="TRightResult"></typeparam>
        /// <param name="either"></param>
        /// <param name="leftSelector"></param>
        /// <param name="rightSelector"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="either"/> or <paramref name="leftSelector"/> is <see langword="null"/>
        /// </exception>
        /// <exception cref="LeftMapException">
        /// Thrown if <paramref name="leftSelector"/> throws an exception. The <see cref="Exception.InnerException"/> will be set
        /// to whatever exception <paramref name="leftSelector"/> threw.
        /// </exception>
        public static IEither<TLeftResult, TRightValue> SelectLeft
            <
                TLeftValue,
                TRightValue,
                TLeftResult
            >
            (
                this IEither<TLeftValue, TRightValue> either,
                Func<TLeftValue, TLeftResult> leftSelector
            )
        {
            ArgumentNullException.ThrowIfNull(either);
            ArgumentNullException.ThrowIfNull(leftSelector);

            return either.Select(
                leftSelector,
                _ => _);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TLeftValue"></typeparam>
        /// <typeparam name="TRightValue"></typeparam>
        /// <typeparam name="TLeftResult"></typeparam>
        /// <typeparam name="TRightResult"></typeparam>
        /// <param name="either"></param>
        /// <param name="leftSelector"></param>
        /// <param name="rightSelector"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="either"/> or <paramref name="rightSelector"/> is <see langword="null"/>
        /// </exception>
        /// <exception cref="RightMapException">
        /// Thrown if <paramref name="rightSelector"/> throws an exception. The <see cref="Exception.InnerException"/> will be
        /// set to whatever exception <paramref name="rightSelector"/> threw.
        /// </exception>
        public static IEither<TLeftValue, TRightResult> SelectRight
            <
                TLeftValue,
                TRightValue,
                TRightResult
            >
            (
                this IEither<TLeftValue, TRightValue> either,
                Func<TRightValue, TRightResult> rightSelector
            )
        {
            ArgumentNullException.ThrowIfNull(either);
            ArgumentNullException.ThrowIfNull(rightSelector);

            return either.Select(
                _ => _,
                rightSelector);
        }

        /// <summary>
        /// placeholder
        /// </summary>
        /// <typeparam name="TLeftSource"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <typeparam name="TEither"></typeparam>
        /// <typeparam name="TLeftResult"></typeparam>
        /// <param name="either"></param>
        /// <param name="selector"></param>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="either"/> or <paramref name="selector"/> or <paramref name="resultSelector"/> is
        /// <see langword="null"/>
        /// </exception>
        /// <exception cref="LeftMapException">
        /// Thrown if <paramref name="selector"/> or <paramref name="resultSelector"/> throws
        /// </exception>
        /// <remarks>
        /// This method and its variants are analogous to the haskell bind. This is corroborated on [stackoverflow](https://stackoverflow.com/questions/19321868/linq-selectmany-is-bind):, and we can confirm this directly in the [haskell documentation](https://wiki.haskell.org/Monad):
        /// > class Monad m where
        /// > (>>=)  :: m a -> (  a -> m b) -> m b
        /// > (>>)   :: m a ->  m b         -> m b
        /// > return ::   a                 -> m a
        /// > ...
        /// > ...(i.e. the two varieties of bind: (>>=) and (>>))...
        /// </remarks>
        public static IEither<TLeftResult, TRight> SelectMany<TLeftSource, TRight, TEither, TLeftResult>(
            this IEither<TLeftSource, TRight> either,
            Func<TLeftSource, IEither<TEither, TRight>> selector,
            Func<TLeftSource, TEither, TLeftResult> resultSelector)
        {
            ArgumentNullException.ThrowIfNull(either);
            ArgumentNullException.ThrowIfNull(selector);
            ArgumentNullException.ThrowIfNull(resultSelector);

            return
                either
                    .Apply(
                        left =>
                        {
                            try
                            {
                                return 
                                    selector(left)
                                        .Apply(
                                            nestedLeft => Either.Left(resultSelector(left, nestedLeft)).Right<TRight>(),
                                            right => Either.Left<TLeftResult>().Right(right));
                            }
                            catch (LeftMapException leftMapException)
                            {
                                //// TODO do you want leftmapexception to only be instantiated with an exception?
                                throw leftMapException.InnerException!;
                            }
                        },
                        right => 
                            Either.Left<TLeftResult>().Right(right));
        }

        /// <summary>
        /// placeholder
        /// </summary>
        /// <typeparam name="TLeftSource"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <typeparam name="TEither"></typeparam>
        /// <typeparam name="TLeftResult"></typeparam>
        /// <param name="either"></param>
        /// <param name="selector"></param>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="either"/> or <paramref name="selector"/> or <paramref name="resultSelector"/> is
        /// <see langword="null"/>
        /// </exception>
        /// <exception cref="LeftMapException">
        /// Thrown if <paramref name="selector"/> or <paramref name="resultSelector"/> throws
        /// </exception>
        public static IEither<TLeftResult, TRight> SelectManyLeft<TLeftSource, TRight, TEither, TLeftResult>(
            this IEither<TLeftSource, TRight> either,
            Func<TLeftSource, IEither<TEither, TRight>> selector,
            Func<TLeftSource, TEither, TLeftResult> resultSelector)
        {
            ArgumentNullException.ThrowIfNull(either);
            ArgumentNullException.ThrowIfNull(selector);
            ArgumentNullException.ThrowIfNull(resultSelector);

            return either.SelectMany(selector, resultSelector);
        }

        /// <summary>
        /// placeholder
        /// </summary>
        /// <typeparam name="TLeftSource"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <typeparam name="TLeftResult"></typeparam>
        /// <param name="either"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="either"/> or <paramref name="selector"/> is <see langword="null"/>
        /// </exception>
        /// <exception cref="LeftMapException">
        /// Thrown if <paramref name="selector"/> throws
        /// </exception>
        public static IEither<TLeftResult, TRight> SelectManyLeft<TLeftSource, TRight, TLeftResult>(
            this IEither<TLeftSource, TRight> either,
            Func<TLeftSource, IEither<TLeftResult, TRight>> selector)
        {
            ArgumentNullException.ThrowIfNull(either);
            ArgumentNullException.ThrowIfNull(selector);

            return either.SelectManyLeft(selector, (left, nestedLeft) => nestedLeft);
        }

        /// <summary>
        /// placeholder
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="either"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="either"/> is <see langword="null"/></exception>
        public static IEither<TLeft, TRight> SelectManyLeft<TLeft, TRight>(
            this IEither<IEither<TLeft, TRight>, TRight> either)
        {
            ArgumentNullException.ThrowIfNull(either);

            return either.SelectManyLeft(left => left);
        }

        /*public static IEither<TLeft, TRightResult> SelectMany<TRightSource, TRightResult, TEither, TLeft, TLeftInner>(
            this IEither<TLeft, TRightSource> either,
            Func<TRightSource, IEither<TLeftInner, TEither>> selector,
            Func<TRightSource, TLeftInner, TLeft> leftResultSelector,
            Func<TRightSource, TEither, TRightResult> resultSelector)
        {
            Either<Either<string, int>, int> otherVal = default!;
            IEither<string, int> otherSelected  = otherVal.SelectManyLeft(
                left => left,
                (left, nestedLeft) => nestedLeft);

            Either<string, Either<string, int>> val = default!;
            IEither<string, int> selected = val.SelectMany(
                right => right,
                (right, nestedLeft) => nestedLeft,
                (right, nestedRight) => nestedRight);

            Either<string, Either<short, int>> val2 = default!;
            IEither<string, int> selected2 = val2.SelectMany(
                right => right,
                (right, nestedLeft) => nestedLeft.ToString(),
                (right, nestedRight) => nestedRight);

            return
                either
                    .Apply(
                        left =>
                            Either.Left(left).Right<TRightResult>(),
                        right =>
                            selector(right)
                                .Apply(
                                    nestedLeft => leftResultSelector(right, nestedLeft),
                                    nestedRight => Either.Left<TLeft>().Right(resultSelector(right, nestedRight))));
        }*/

        /// <summary>
        /// TODO i don't know how to implement this; see above block comment 
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRightSource"></typeparam>
        /// <typeparam name="TLeftInner"></typeparam>
        /// <typeparam name="TEither"></typeparam>
        /// <typeparam name="TRightResult"></typeparam>
        /// <param name="either"></param>
        /// <param name="selector"></param>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        public static IEither<TLeft, TRightResult> SelectMany<TLeft, TRightSource, TLeftInner, TEither, TRightResult>(
            this IEither<TLeft, TRightSource> either,
            Func<TRightSource, IEither<TLeftInner, TEither>> selector,
            Func<TRightSource, TEither, TRightResult> resultSelector)
        {
            return default!;
        }

        /// <summary>
        /// TODO finish this when you have the callee implemented 
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRightSource"></typeparam>
        /// <typeparam name="TLeftInner"></typeparam>
        /// <typeparam name="TEither"></typeparam>
        /// <typeparam name="TRightResult"></typeparam>
        /// <param name="either"></param>
        /// <param name="selector"></param>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        public static IEither<TLeft, TRightResult> SelectManyRight<TLeft, TRightSource, TLeftInner, TEither, TRightResult>(
            this IEither<TLeft, TRightSource> either,
            Func<TRightSource, IEither<TLeftInner, TEither>> selector,
            Func<TRightSource, TEither, TRightResult> resultSelector)
        {
            return either.SelectMany(selector, resultSelector);
        }

        /// <summary>
        /// TODO finish this when you have the callee implemented 
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRightSource"></typeparam>
        /// <typeparam name="TRightResult"></typeparam>
        /// <param name="either"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IEither<TLeft, TRightResult> SelectManyRight<TLeft, TRightSource, TRightResult>(
            this IEither<TLeft, TRightSource> either,
            Func<TRightSource, IEither<TLeft, TRightResult>> selector)
        {
            return either.SelectManyRight(selector, (right, nestedRight) => nestedRight);
        }

        /// <summary>
        /// TODO finish this when you have the callee implemented 
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="either"></param>
        /// <returns></returns>
        public static IEither<TLeft, TRight> SelectManyRight<TLeft, TRight>(
            this IEither<TLeft, IEither<TLeft, TRight>> either)
        {
            return either.SelectManyRight(right => right);
        }

        //// TODO you are here

        //// https://hackage.haskell.org/package/base-4.21.0.0/docs/Control-Monad.html
        //// https://hackage.haskell.org/package/base-4.21.0.0/docs/Data-Either.html
        //// https://almarefa.net/blog/how-to-combine-two-different-types-of-lists-in
        //// (look at the `flatten` example [here](https://learn-haskell.blog/06-errors_and_files/01-either.html)
        //// i believe that the linq `join` is some variant of list comprehension in haskell (see the example where `gcd i j == 1` [here](https://wiki.haskell.org/List_comprehension))

        public static IEither<(TLeftFirst, TLeftSecond), TRight> Zip<TLeftFirst, TLeftSecond, TRight>(
            this IEither<TLeftFirst, TRight> first,
            IEither<TLeftSecond, TRight> second,
            Func<TRight, TRight, TRight> rightAggregator) //// TODO TOPIC naming of this //// TODO TOPIC other variants of this? like, does `tright` need to be the same for both eithers? and should you always return a tuple? don't forget your ultimate use-case of first.zip(second).throwright()
        {
            return
                first
                    .Apply(
                        firstLeft =>
                            second
                                .Apply(
                                    secondLeft =>
                                        Either.Left((firstLeft, secondLeft)).Right<TRight>(),
                                    secondRight =>
                                        Either.Left<(TLeftFirst, TLeftSecond)>().Right(secondRight)), //// TODO is it ok that you are losing `firstleft`? is this method actually a convenience overload of a more general method that asks the caller for a delegate for each left case?
                        firstRight =>
                            second
                                .Apply(
                                    secondLeft =>
                                        Either.Left<(TLeftFirst, TLeftSecond)>().Right(firstRight),
                                    secondRight =>
                                        Either.Left<(TLeftFirst, TLeftSecond)>().Right(rightAggregator(firstRight, secondRight))));

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="either"></param>
        /// <param name="left"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="either"/> is <see langword="null"/></exception>
        public static bool TryLeft<TLeft, TRight>(this IEither<TLeft, TRight> either, [MaybeNullWhen(false)] out TLeft left)
        {
            //// TODO TOPIC naming
            ArgumentNullException.ThrowIfNull(either);

            var result = either.Apply(
                left => (left, true),
                right => (default(TLeft), false));

            left = result.Item1;
            return result.Item2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="either"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="either"/> is <see langword="null"/></exception>
        public static bool TryRight<TLeft, TRight>(this IEither<TLeft, TRight> either, [MaybeNullWhen(false)] out TRight right)
        {
            //// TODO TOPIC naming
            ArgumentNullException.ThrowIfNull(either);

            var result = either.Apply(
                left => (default(TRight), false),
                right => (right, true));

            right = result.Item1;
            return result.Item2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <param name="either"></param>
        /// <param name="left"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="either"/> is <see langword="null"/></exception>
        public static bool Try<TLeft>(this IEither<TLeft, Nothing> either, [MaybeNullWhen(false)] out TLeft left)
        {
            ArgumentNullException.ThrowIfNull(either);

            //// TODO TOPIC naming? i doubt this is actually a `try` because it doesn't take an input
            var result = either.Apply(
                left => (left, true),
                right => (default(TLeft), false));

            left = result.Item1;
            return result.Item2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="either"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="either"/> is <see langword="null"/></exception>
        public static TLeft ThrowRight<TLeft, TRight>(this IEither<TLeft, TRight> either) where TRight : Exception
        {
            ArgumentNullException.ThrowIfNull(either);

            //// TODO TOPIC naming
            //// TODO maybe the "try" conversation will illuminate a new name for this method, otherwise it's prtety solid
            return either.Coalesce(right => throw right);

            ////return either.Visit(left => left, right => throw right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="either"></param>
        /// <param name="coalescer"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="either"/> or <paramref name="coalescer"/> is <see langword="null"/></exception>
        public static TLeft Coalesce<TLeft, TRight>(this IEither<TLeft, TRight> either, Func<TRight, TLeft> coalescer)
        {
            ArgumentNullException.ThrowIfNull(either);
            ArgumentNullException.ThrowIfNull(coalescer);

            return either.Apply(left => left, coalescer);
        }

        public static TValue Coalesce<TValue>(this IEither<TValue, TValue> either)
        {
            //// TODO TOPIC this is like a degenerate case of the "shift right" thing...
            return either.Apply(left => left, right => right);
        }

        //// TODO write somewhere that nullable<TLeft> is equivalent to IEither<TLeft, CalendarV2.System.Void>

        public static void CoalesceUseCase()
        {
            object? foo = null;

            var bar = foo ?? new object();


        }

        public static TLeft Coalesce<TLeft>(this IEither<TLeft, Nothing> either, TLeft @default)
        {
            //// TODO should there be an overload of Func<TLeft> defaultCoalescer? //// TODO TOPIC no, because that's just coalesce(either<left, right>)
            //// this is equivalent to the null coalescing operator; how to generalize?
            //// TODO wait, is *visit* actually the general-form "coalesce"? and that's why you can't seem to find a more general method signature?
            /*return either.Visit(
                left => left,
                right => @default);
            */
            return either.Coalesce(_ => @default);
        }
    }

    /// <summary>
    /// TODO can you use this anywhere?
    /// </summary>
    public static class ExceptionExtensions
    {
        public static Nothing Throw<TException>(this TException exception) where TException : Exception
        {
            throw exception;
        }
    }

    public struct Throw<T>
    {
        public static implicit operator Nothing(Throw<T> @throw)
        {
            return new Nothing();
        }

        public static implicit operator T(Throw<T> @throw)
        {
            //// TODO probably not safe, but we are wanting it here for type inference and lambdas
            return default(T)!;
        }
    }
}
