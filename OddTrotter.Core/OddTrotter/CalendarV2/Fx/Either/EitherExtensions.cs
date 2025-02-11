/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.Either
{
    using CalendarV2.Fx.Try;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net.Http.Headers;

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
            //// TODO this is the "proper" implementation:
            //// return either.SelectManyRight(selector, (right, nestedRight) => nestedRight);

            return either.SelectRight(selector).SelectManyRight();
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
            //// TODO this is the "propoer" implementation:
            //// return either.SelectManyRight(right => right);

            return
                either
                    .Apply(
                        left => Either.Left(left).Right<TRight>(),
                        right =>
                            right
                                .Apply(
                                    nestedLeft => Either.Left(nestedLeft).Right<TRight>(),
                                    nestedRight => Either.Left<TLeft>().Right(nestedRight)));
        }

        /// <summary>
        /// placeholder
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="either"></param>
        /// <param name="left"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="either"/> is <see langword="null"/></exception>
        public static bool TryGetLeft<TLeft, TRight>(this IEither<TLeft, TRight> either, [MaybeNullWhen(false)] out TLeft left)
        {
            ArgumentNullException.ThrowIfNull(either);

            //// TODO FUTURE can you do a ref struct for these tuples?
            var result = either.Apply(
                left => (left, true),
                right => (default(TLeft), false));

            left = result.Item1;
            return result.Item2;
        }

        /// <summary>
        /// placeholder
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="either"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="either"/> is <see langword="null"/></exception>
        public static bool TryGetRight<TLeft, TRight>(this IEither<TLeft, TRight> either, [MaybeNullWhen(false)] out TRight right)
        {
            ArgumentNullException.ThrowIfNull(either);

            var result = either.Apply(
                left => (default(TRight), false),
                right => (right, true));

            right = result.Item1;
            return result.Item2;
        }

        /// <summary>
        /// placeholder
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <param name="either"></param>
        /// <param name="left"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="either"/> is <see langword="null"/></exception>
        public static bool TryGet<TLeft>(this IEither<TLeft, Nothing> either, [MaybeNullWhen(false)] out TLeft left)
        {
            ArgumentNullException.ThrowIfNull(either);

            return either.TryGetLeft(out left);
        }

        /// <summary>
        /// placeholder
        /// </summary>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="either"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="either"/> is <see langword=""/></exception>
        public static bool TryGet<TRight>(this IEither<Nothing, TRight> either, [MaybeNullWhen(false)] out TRight right)
        {
            ArgumentNullException.ThrowIfNull(either);

            return either.TryGetRight(out right);
        }

        //// TODO you are here

        public static void CoalesceUseCase()
        {
            IEither<string, string> either = default!;

            either.CoalesceLeft(left => left + "asfd");
            either.SelectLeft(left => left + "asdf").Coalesce();

            IEither<string, Nothing> either2 = default!;
            either2.Coalesce(string.Empty);
            either2.CoalesceRight(_ => "a big string");

            IEither<Nothing, Nothing> either3 = default!;
            either3.Coalesce();
            ////either3.Coalesce(new Nothing());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="either"></param>
        /// <param name="coalescer"></param>
        /// <returns></returns>
        /// <remarks>
        /// This is named "coalesce" to re-use the c# idiom of "null-coalescing operator". This is named "right" because, like the null-coalescing operator, if <typeparamref name="TRight"/> here were "null", then we would be "removing" the right type the same was we would be "removing" the null in a null-coalescing operator.
        /// </remarks>
        public static TLeft CoalesceRight<TLeft, TRight>(this IEither<TLeft, TRight> either, Func<TRight, TLeft> coalescer)
        {
            ArgumentNullException.ThrowIfNull(either);
            ArgumentNullException.ThrowIfNull(coalescer);

            return either.Apply(left => left, coalescer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="either"></param>
        /// <param name="coalescer"></param>
        /// <returns></returns>
        /// <remarks>
        /// This is named "coalesce" to re-use the c# idiom of "null-coalescing operator". This is named "right" because, like the null-coalescing operator, if <typeparamref name="TRight"/> here were "null", then we would be "removing" the right type the same was we would be "removing" the null in a null-coalescing operator.
        /// </remarks>
        public static TRight CoalesceLeft<TLeft, TRight>(this IEither<TLeft, TRight> either, Func<TLeft, TRight> coalescer)
        {
            return either.Apply(coalescer, right => right);
        }

        public static TValue Coalesce<TValue>(this IEither<TValue, TValue> either)
        {
            return either.CoalesceRight(right => right);
        }

        public static TLeft Coalesce<TLeft>(this IEither<TLeft, Nothing> either, TLeft @default)
        {
            return either.CoalesceRight(_ => @default);
        }

        public static TRight Coalesce<TRight>(this IEither<Nothing, TRight> either, TRight @default)
        {
            return either.CoalesceLeft(_ => @default);
        }

        public static TLeft ThrowRight<TLeft, TRight>(this IEither<TLeft, TRight> either) where TRight : Exception
        {
            ArgumentNullException.ThrowIfNull(either);

            //// TODO TOPIC naming
            //// TODO maybe the "try" conversation will illuminate a new name for this method, otherwise it's prtety solid
            return either.CoalesceRight(right => throw right);

            ////return either.Visit(left => left, right => throw right);
        }

        public static TRight ThrowLeft<TLeft, TRight>(this IEither<TLeft, TRight> either) where TLeft : Exception
        {
            return either.CoalesceLeft(left => throw left);
        }
    }
}
