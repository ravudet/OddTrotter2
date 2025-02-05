namespace CalendarV2.Fx.Either
{
    using global::System;
    using global::System.Diagnostics.CodeAnalysis;
    using OddTrotter.TodoList;

    public sealed class Null
    {
    }

    public sealed class Null<T>
    {
        public static implicit operator Null<T>(Null @null)
        {
            return new Null<T>();
        }

        public static implicit operator Null(Null<T> @null)
        {
            return new Null();
        }
    }

    public static class NullPlaygroundExtension
    {
        public static EitherHelper2<TLeft> Left<TLeft>(this TLeft left)
        {
            return new EitherHelper2<TLeft>();
        }



        public sealed class EitherHelper2<TValue>
        {
            public NullPlayground.NewEither<TValue, TRight> Right<TRight>()
            {
                return null!;
            }
        }
    }

    public static class NullPlayground
    {
        public static void DoWork()
        {
            global::System.Collections.Generic.List<Null> stuff = new global::System.Collections.Generic.List<Null>();
            stuff.Add(new Null<string>());

            global::System.Collections.Generic.List<Null<string>> stuff2 = new global::System.Collections.Generic.List<Null<string>>();
            stuff2.Add(new Null());

            var stuff3 = new global::System.Collections.Generic.List<Nullable2<string>>();
            stuff3.Add("asdf");
            stuff3.Add(new Null<string>());
            ///stuff3.Add(new Null());

            var stuff4 = new global::System.Collections.Generic.List<NewEither<string, Exception>>();
            stuff4.Add(new Exception());
            stuff4.Add("sadf");

            var stuff5 = new global::System.Collections.Generic.List<IEither<string, Exception>>();
            ////stuff5.Add(new Exception());

            //// TODO i think an implication of all of this is that your extensions should really return concrete `either` implementations, but the stuff5 example shows that this will probably permeate itself through caller code too; is that ok?

            var stuff6 = new global::System.Collections.Generic.List<NewEither<string, NewEither<InvalidOperationException, ArgumentException>>>();
            ////stuff6.Add(new ArgumentException());
            stuff6.Add("dsaf");
            ////stuff6.Add(new InvalidOperationException());
            ////stuff6.Add(EitherHelper.Create(new ArgumentException()));
            NewEither<InvalidOperationException, ArgumentException> right = new ArgumentException();
            stuff6.Add(right);
            stuff6.Add(new InvalidOperationException().Left().Right<ArgumentException>());
        }

        public static class EitherHelper
        {
            public static EitherHelper<TValue> Create<TValue>(TValue value)
            {
                return new EitherHelper<TValue>();
            }
        }

        public sealed class EitherHelper<TValue>
        {

        }

        public sealed class NewEither<TLeft, TRight> : IEither<TLeft, TRight>
        {
            private readonly Nested nested;

            private NewEither(Nested nested)
            {
                this.nested = nested;
            }

            public TResult Visit<TResult, TContext>(Func<TLeft, TContext, TResult> leftAccept, Func<TRight, TContext, TResult> rightAccept, TContext context)
            {
                return this.nested.Visit(leftAccept, rightAccept, context);
            }

            public static implicit operator NewEither<TLeft, TRight>(EitherHelper<TLeft> left)
            {
                return new NewEither<TLeft, TRight>(new Nested.Left(default!));
            }

            public static implicit operator NewEither<TLeft, TRight>(EitherHelper<TRight> right)
            {
                return new NewEither<TLeft, TRight>(new Nested.Right(default!));
            }

            public static implicit operator NewEither<TLeft, TRight>(TLeft value)
            {
                //// TODO TOPIC there's something to this; if you can't figure it out after discussion, go ahead and just add it, making a note that maybe it's useless, but if you don't have it implemented, you will never accidentally discover the utility
                return null!;
                ////return new NewEither<TLeft, TRight>(new Nested.Left(value));
            }

            public static implicit operator NewEither<TLeft, TRight>(TRight value)
            {
                return null!;
                ////return new NewEither<TLeft, TRight>(new Nested.Right(value));
            }

            private abstract class Nested : IEither<TLeft, TRight>
            {
                private Nested()
                {
                }

                public TResult Visit<TResult, TContext>(Func<TLeft, TContext, TResult> leftAccept, Func<TRight, TContext, TResult> rightAccept, TContext context)
                {
                    throw new NotImplementedException();
                }

                public sealed class Left : Nested
                {
                    public Left(TLeft value)
                    {
                        Value = value;
                    }

                    public TLeft Value { get; }
                }

                public sealed class Right : Nested
                {
                    public Right(TRight value)
                    {
                        Value = value;
                    }

                    public TRight Value { get; }
                }
            }
        }

        public sealed class Nullable2<T> : IEither<T, Null<T>>
        {
            public Nullable2()
            {
            }
            
            public TResult Visit<TResult, TContext>(Func<T, TContext, TResult> leftAccept, Func<Null<T>, TContext, TResult> rightAccept, TContext context)
            {
                throw new NotImplementedException();
            }

            public static implicit operator Nullable2<T>(T value)
            {
                return new Nullable2<T>();
            }

            public static implicit operator Nullable2<T>(Null<T> @null)
            {
                return new Nullable2<T>();
            }
        }
    }

    /// <summary>
    /// TODO pick up at line 637
    /// TODO wrap exceptions for accepts
    /// TODO FUTURE mixins for all of these
    /// 
    /// TODO TOPIC is spacing ok? not sure there's a better way...
    /// TODO TOPIC should all of this be lazy?
    /// TODO TOPIC any notable operations that are missing?
    /// </summary>
    public static class EitherExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="either"></param>
        /// <param name="leftAccept"></param>
        /// <param name="rightAccept"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="either"/> or <paramref name="leftAccept"/> or <paramref name="rightAccept"/> is
        /// <see langword="null"/>
        /// </exception>
        /// <exception cref="Exception">
        /// Throws any of the exceptions that <paramref name="leftAccept"/> or <paramref name="rightAccept"/> can throw
        /// </exception>
        public static TResult Visit<TLeft, TRight, TResult>( //// TODO TOPIC call this "aggregate" instead? //// TODO aggregate sounds wrong, maybe we think a bit more; what linq calls aggregate is called "foldleft"; "fold" my be useful as a name below regarding your "propagateby" extension; look at "catamorphism" of either; TODO i believe `Visit` itself is actually a "functor", but method names in c# should mostly be verbs; is it really a functor, and, if so, what should we call it so it's a verb? https://en.wikipedia.org/wiki/Catamorphism https://en.wikipedia.org/wiki/Functor#endofunctor
            this IEither<TLeft, TRight> either,
            Func<TLeft, TResult> leftAccept,
            Func<TRight, TResult> rightAccept)
        {
            ArgumentNullException.ThrowIfNull(either);
            ArgumentNullException.ThrowIfNull(leftAccept);
            ArgumentNullException.ThrowIfNull(rightAccept);

            return either.Visit(
                (left, context) => leftAccept(left),
                (right, context) => rightAccept(right),
                new CalendarV2.System.Void());
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
        /// <exception cref="Exception">
        /// Throws any of the exceptions that <paramref name="leftSelector"/> or <paramref name="rightSelector"/> can throw
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
                TContext context) //// TODO TOPIC should this go on the next line?
        {
            ArgumentNullException.ThrowIfNull(either);
            ArgumentNullException.ThrowIfNull(leftSelector);
            ArgumentNullException.ThrowIfNull(rightSelector);

            return either.Visit(
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
        /// <exception cref="Exception">
        /// Throws any of the exceptions that <paramref name="leftSelector"/> can throw
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
                TContext context)
        {
            ArgumentNullException.ThrowIfNull(either);
            ArgumentNullException.ThrowIfNull(leftSelector);

            return either.Visit(
                (left, context) => Either.Left(leftSelector(left, context)).Right<TRightValue>(),
                (right, context) => Either.Left<TLeftResult>().Right(right),
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
        /// <exception cref="Exception">
        /// Throws any of the exceptions that <paramref name="rightSelector"/> can throw
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
                TContext context)
        {
            ArgumentNullException.ThrowIfNull(either);
            ArgumentNullException.ThrowIfNull(rightSelector);

            return either.Visit(
                (left, context) => Either.Left(left).Right<TRightResult>(),
                (right, context) => Either.Left<TLeftValue>().Right(rightSelector(right, context)),
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
        /// <exception cref="Exception">
        /// Throws any of the exceptions that <paramref name="leftSelector"/> or <paramref name="rightSelector"/> can throw
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
                Func<TRightValue, TRightResult> rightSelector)
        {
            ArgumentNullException.ThrowIfNull(either);
            ArgumentNullException.ThrowIfNull(leftSelector);
            ArgumentNullException.ThrowIfNull(rightSelector);

            return either.Visit(
                (left, context) => Either.Left(leftSelector(left)).Right<TRightResult>(),
                (right, context) => Either.Left<TLeftResult>().Right(rightSelector(right)),
                new CalendarV2.System.Void());
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
        /// <exception cref="Exception">
        /// Throws any of the exceptions that <paramref name="leftSelector"/> can throw
        /// </exception>
        public static IEither<TLeftResult, TRightValue> SelectLeft
            <
                TLeftValue,
                TRightValue,
                TLeftResult
            >
            (
                this IEither<TLeftValue, TRightValue> either,
                Func<TLeftValue, TLeftResult> leftSelector)
        {
            ArgumentNullException.ThrowIfNull(either);
            ArgumentNullException.ThrowIfNull(leftSelector);

            return either.Visit(
                left => Either.Left(leftSelector(left)).Right<TRightValue>(),
                right => Either.Left<TLeftResult>().Right(right));
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
        /// <exception cref="Exception">
        /// Throws any of the exceptions that <paramref name="rightSelector"/> can throw
        /// </exception>
        public static IEither<TLeftValue, TRightResult> SelectRight
            <
                TLeftValue,
                TRightValue,
                TRightResult
            >
            (
                this IEither<TLeftValue, TRightValue> either,
                Func<TRightValue, TRightResult> rightSelector)
        {
            ArgumentNullException.ThrowIfNull(either);
            ArgumentNullException.ThrowIfNull(rightSelector);

            return either.Visit(
                (left, context) => Either.Left(left).Right<TRightResult>(),
                (right, context) => Either.Left<TLeftValue>().Right(rightSelector(right)),
                new CalendarV2.System.Void());
        }

        public static void NullPropagateUseCase()
        {
            var foo1 = Foo1()?.Bar1()?.Frob1();

            var foo2 = Foo2().NullPropagate(Bar1).NullPropagate(Frob1);
        }

        public static object? Foo1()
        {
            return new object();
        }

        public static string Bar1(this object value)
        {
            return value.ToString() ?? string.Empty;
        }

        public static int Frob1(this string value)
        {
            return value.Length;
        }

        public static Either<string, CalendarV2.System.Void> Foo2() //// TOPIC TODO you need a "null" type instead of overriding void probably
        {
            return Either.Left("asdf").Right<CalendarV2.System.Void>();
        }

        public static IEither<TLeftResult, CalendarV2.System.Void> NullPropagate<TLeftValue, TLeftResult>(
            this IEither<TLeftValue, CalendarV2.System.Void> either,
            Func<TLeftValue, TLeftResult> selector)
        {
            //// TODO TOPIC is this extension even worth having?
            return either.SelectLeft(selector);
        }

        public static IEither<TLeft, CalendarV2.System.Void> NullPropagate<TLeft>(this IEither<IEither<TLeft, CalendarV2.System.Void>, CalendarV2.System.Void> either)
        {
            //// TODO TOPIC this extension is *not* null propagate; find a better name; previously, we liked the name "propagate", but it's definitely not (see above);
            return either.PropagateRight();
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
            //// TODO TOPIC what name are you using instead of "propagate"? i've previously call this "shiftright"; maybe "consolidate"?
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
            //// TODO TOPIC what name are you using instead of "propagate"? i've previously call this "shiftright"; maybe "consolidate"?
            ArgumentNullException.ThrowIfNull(either);

            return either.Visit(
                left => 
                    left.Visit(
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
            //// TODO TOPIC what name are you using instead of "propagate"? i've previously call this "shiftright"; maybe "consolidate"?
            ArgumentNullException.ThrowIfNull(either);

            return either.Visit(
                left =>
                    Either.Left(left).Right<TRight>(),
                right =>
                    right.Visit(
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
                    .Visit(
                        firstLeft =>
                            second
                                .Visit(
                                    secondLeft =>
                                        Either.Left((firstLeft, secondLeft)).Right<TRight>(),
                                    secondRight =>
                                        Either.Left<(TLeftFirst, TLeftSecond)>().Right(secondRight)), //// TODO is it ok that you are losing `firstleft`? is this method actually a convenience overload of a more general method that asks the caller for a delegate for each left case?
                        firstRight =>
                            second
                                .Visit(
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

            var result = either.Visit(
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

            var result = either.Visit(
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
        public static bool Try<TLeft>(this IEither<TLeft, CalendarV2.System.Void> either, [MaybeNullWhen(false)] out TLeft left)
        {
            ArgumentNullException.ThrowIfNull(either);

            //// TODO TOPIC naming? i doubt this is actually a `try` because it doesn't take an input
            var result = either.Visit(
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

            return either.Visit(left => left, coalescer);
        }

        //// TODO write somewhere that nullable<TLeft> is equivalent to IEither<TLeft, CalendarV2.System.Void>

        public static void CoalesceUseCase()
        {
            object? foo = null;

            var bar = foo ?? new object();


        }

        public static TLeft Coalesce<TLeft>(this IEither<TLeft, CalendarV2.System.Void> either, TLeft @default)
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
        public static CalendarV2.System.Void Throw<TException>(this TException exception) where TException : Exception
        {
            throw exception;
        }
    }

    public struct Throw<T>
    {
        public static implicit operator CalendarV2.System.Void(Throw<T> @throw)
        {
            return new CalendarV2.System.Void();
        }

        public static implicit operator T(Throw<T> @throw)
        {
            //// TODO probably not safe, but we are wanting it here for type inference and lambdas
            return default(T)!;
        }
    }
}
