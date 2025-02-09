/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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

        private static IEither<TLeftResult, TRight> SelectMany<TLeftSource, TLeftResult, TEither, TRight>(
            this IEither<TLeftSource, TRight> either,
            Func<TLeftSource, IEither<TEither, TRight>> selector,
            Func<TLeftSource, TEither, TLeftResult> resultSelector)
        {
            return default!;
        }

        private static IEither<TLeft, TRightResult> SelectMany<TRightSource, TRightResult, TEither, TLeft>(
            this IEither<TLeft, TRightSource> either,
            Func<TRightSource, IEither<TLeft, TEither>> selector,
            Func<TRightSource, TEither, TRightResult> resultSelector)
        {
            return default!;
        }

        public static IEither<TLeftResult, TRight> SelectMany<TLeftSource, TLeftResult, TRight>(
            this IEither<TLeftSource, TRight> either, 
            Func<TLeftSource, IEither<TLeftResult, TRight>> selector)
        {
            //// TODO you are here
            //// https://hackage.haskell.org/package/base-4.21.0.0/docs/Control-Monad.html
            //// https://hackage.haskell.org/package/base-4.21.0.0/docs/Data-Either.html
            //// https://almarefa.net/blog/how-to-combine-two-different-types-of-lists-in

            Either<Either<Either<string, int>, object>, System.Exception> either1 = default!;
            var result =
                from first in either1
                from second in first
                select new object();


            Either<int, Either<string, Either<object, System.Exception>>> either2 = default!;
            var result2 =
                from first2 in either2
                from second2 in first2
                select new object();

            return either
                .Apply(
                    left =>
                        selector(left)
                            .Apply(
                                nestedLeft => Either.Left(nestedLeft).Right<TRight>(),
                                nestedRight => Either.Left<TLeftResult>().Right(nestedRight)),
                    right => Either.Left<TLeftResult>().Right(right));
        }

        class Animal
        {
        }

        class Dog : Animal
        {
        }

        class Cat : Animal
        {
        }

        private static void PropagateRightBaseUseCase(IEither<IEither<string, Dog>, Cat> either)
        {
            var propagated = either.PropagateRight<string, Dog, Cat, Animal>();
            //// TODO TOPIC you can't really get the type inference to work even with a "factory"; i ran into this with asbase before //// TODO `cast` actually also has this problem, by the way

            either.CreateFactory().DoWork<Animal>();
            either.CreateFactory().DoWork<string>();
        }

        private sealed class FactoryThing<TLeft, TRightDerived1, TRightDerived2>
        {
            public IEither<TLeft, TRightBase> DoWork<TRightBase>()
            {
                return default!;
            }
        }

        private static FactoryThing<TLeft, TRightDerived1, TRightDerived2> CreateFactory<TLeft, TRightDerived1, TRightDerived2>(
            this IEither<IEither<TLeft, TRightDerived1>, TRightDerived2> either)
        {
            return new FactoryThing<TLeft, TRightDerived1, TRightDerived2>();
        }

        public static IEither<TLeft, TRightBase> PropagateRight<TLeft, TRightDerived1, TRightDerived2, TRightBase>(
            this IEither<IEither<TLeft, TRightDerived1>, TRightDerived2> either)
            where TRightDerived1 : TRightBase
            where TRightDerived2 : TRightBase
        {
            //// TODO TOPIC what name are you using instead of "propagate"? i've previously call this "shiftright"; maybe "consolidate"? //// TODO this is called `join` in haskell (look at the `flatten` example [here](https://learn-haskell.blog/06-errors_and_files/01-either.html))
            //// TODO TOPIC look at the parameterless coalesce as a degenerate case
            return either
                .SelectLeft(
                    left => left.SelectRight(right => (TRightBase)right))
                .SelectRight(
                    right => (TRightBase)right)
                .PropagateRight();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="either"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="either"/> is <see langword="null"/></exception>
        public static IEither<TLeft, TRight> PropagateRight<TLeft, TRight>(
            this IEither<IEither<TLeft, TRight>, TRight> either)
        {
            return either.SelectMany(left => left);

            //// TODO TOPIC what name are you using instead of "propagate"? i've previously call this "shiftright"; maybe "consolidate"? //// TODO this is called `join` in haskell (look at the `flatten` example [here](https://learn-haskell.blog/06-errors_and_files/01-either.html)); i believe that the linq `join` is some variant of list comprehension in haskell (see the example where `gcd i j == 1` [here](https://wiki.haskell.org/List_comprehension)); i think you might be wrong about the join thing; i think join is used with reverse bind to achieve this effect through some form of lifting; but what you've already gone past where the lifting is aspplicable, so what you are doing is a bind, with is isomorphic to the c# selectmany; if this is true, then this is a degenerate case of the attempt propagateby method below, and the propagateby below is actually the bind/selectmany (source: https://stackoverflow.com/questions/19321868/linq-selectmany-is-bind)
            //// TODO TOPIC look at the parameterless coalesce as a degenerate case
            ArgumentNullException.ThrowIfNull(either);

            return either.Apply(
                left => 
                    left.Apply(
                        subLeft => Either.Left(subLeft).Right<TRight>(),
                        subRight => Either.Left<TLeft>().Right(subRight)),
                right =>
                    Either.Left<TLeft>().Right(right));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="either"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="either"/> is <see langword="null"/></exception>
        public static IEither<TLeft, TRight> PropagateLeft<TLeft, TRight>(
            this IEither<TLeft, IEither<TLeft, TRight>> either)
        {
            //// TODO TOPIC what name are you using instead of "propagate"? i've previously call this "shiftright"; maybe "consolidate"? //// TODO this is called `join` in haskell (look at the `flatten` example [here](https://learn-haskell.blog/06-errors_and_files/01-either.html)); i believe that the linq `join` is some variant of list comprehension in haskell (see the example where `gcd i j == 1` [here](https://wiki.haskell.org/List_comprehension))
            //// TODO TOPIC look at the parameterless coalesce as a degenerate case
            ArgumentNullException.ThrowIfNull(either);

            return either.Apply(
                left =>
                    Either.Left(left).Right<TRight>(),
                right =>
                    right.Apply(
                        subLeft => Either.Left(subLeft).Right<TRight>(),
                        subRight => Either.Left<TLeft>().Right(subRight)));
        }

        /*public static void PropagateByRightUseCase()
        {
            var either = Either.Left<(string, IEither<int, Exception>)>().Right(new Exception());

            Either<(string, int), Exception> result = either.PropagateByRight<(string, IEither<int, Exception>), Exception, int, (string, int)> (
                left => left.Item2,
                (left, nested) => (left, nested));
        }

        //// TODO TOPIC what should this method signature actually look like?
        //// TODO get all the names right for this use case
        public static IEither<TLeftResult, TRight> PropagateByRight<TLeft, TRight, TLeftNested, TLeftResult>( //// TODO TOPIC does the "orientation" of this name make sense? //// TODO is this a "lift" actually?
            this IEither<TLeft, TRight> either,
            Func<TLeft, IEither<TLeftNested, TRight>> Propagator,
            Func<TLeft, TLeftNested, TLeftResult> aggregator)
        {
            either.Visit(
                left => left.)
        }*/

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
