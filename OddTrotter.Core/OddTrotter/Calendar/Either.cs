////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;

namespace OddTrotter.Calendar
{
    public abstract class Either<TLeft, TRight>
    {
        /// <summary>
        /// 
        /// </summary>
        private Either()
        {
        }

        /// <summary>
        /// TODO normalize all of your visitors with the pattern (naming, internal protected in the visitor, etc) that you are using in other repos
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="visitor"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="visitor"/> is <see langword="null"/></exception>
        /// <exception cref="Exception">Throws any of the exceptions that the <see cref="Visitor{TResult, TContext}.Dispatch"/> overloads can throw</exception> //// TODO is this good?
        protected abstract TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="node"></param>
            /// <param name="context"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="node"/> is <see langword="null"/></exception>
            /// <exception cref="Exception">Throws any of the exceptions that the <see cref="Dispatch"/> overloads can throw</exception> //// TODO is this good?
            public TResult Visit(Either<TLeft, TRight> node, TContext context)
            {
                if (node == null)
                {
                    throw new ArgumentNullException(nameof(node));
                }

                return node.Accept(this, context);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="node"></param>
            /// <param name="context"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="node"/> is <see langword="null"/></exception>
            /// <exception cref="Exception">Can throw any exception as documented by the derived type</exception>
            public abstract TResult Dispatch(Left node, TContext context);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="node"></param>
            /// <param name="context"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="node"/> is <see langword="null"/></exception>
            /// <exception cref="Exception">Can throw any exception as documented by the derived type</exception>
            public abstract TResult Dispatch(Right node, TContext context);
        }

        public sealed class Left : Either<TLeft, TRight>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="value"></param>
            public Left(TLeft value)
            {
                Value = value;
            }

            public TLeft Value { get; }

            /// <inheritdoc/>
            protected sealed override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                if (visitor == null)
                {
                    throw new ArgumentNullException(nameof(visitor));
                }

                return visitor.Dispatch(this, context);
            }
        }

        public sealed class Right : Either<TLeft, TRight>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="value"></param>
            public Right(TRight value)
            {
                Value = value;
            }

            public TRight Value { get; }

            /// <inheritdoc/>
            protected sealed override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                if (visitor == null)
                {
                    throw new ArgumentNullException(nameof(visitor));
                }

                return visitor.Dispatch(this, context);
            }
        }
    }

    public static class Extensions
    {
        public static T? ToNullable<T>(this T value)
        {
            return value;
        }
    }

    public static class EitherExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="leftFactory"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Either<TLeft, TRight> Left<TLeft, TRight>(this Either.FactoryLeft<TRight> leftFactory, TLeft value)
        {
            // `leftfactory` is only be used to "carry" the `tright` type, so you don't need to do any assertions on it; `value` is allowed to be whatever the caller wants, so we don't do any assertions on it

            return new Either<TLeft, TRight>.Left(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="rightFactory"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Either<TLeft, TRight> Right<TLeft, TRight>(this Either.FactoryRight<TLeft> rightFactory, TRight value)
        {
            // `rightfactory` is only be used to "carry" the `tleft` type, so you don't need to do any assertions on it; `value` is allowed to be whatever the caller wants, so we don't do any assertions on it

            return new Either<TLeft, TRight>.Right(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TLeftOld"></typeparam>
        /// <typeparam name="TRightOld"></typeparam>
        /// <typeparam name="TLeftNew"></typeparam>
        /// <typeparam name="TRightNew"></typeparam>
        /// <param name="either"></param>
        /// <param name="leftSelector"></param>
        /// <param name="rightSelector"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="either"/> or <paramref name="leftSelector"/> or <paramref name="rightSelector"/> is <see langword="null"/></exception>
        /// <exception cref="Exception">Throws any of the exceptions that <paramref name="leftSelector"/> or <paramref name="rightSelector"/> can throw</exception> //// TODO is this good?
        public static Either<TLeftNew, TRightNew> VisitSelect<TLeftOld, TRightOld, TLeftNew, TRightNew>(
            this Either<TLeftOld, TRightOld> either,
            Func<TLeftOld, TLeftNew> leftSelector,
            Func<TRightOld, TRightNew> rightSelector)
        {
            if (either == null)
            {
                throw new ArgumentNullException(nameof(either));
            }

            if (leftSelector == null)
            {
                throw new ArgumentNullException(nameof(leftSelector));
            }

            if (rightSelector == null)
            {
                throw new ArgumentNullException(nameof(rightSelector));
            }

            return either.Visit(
                left => Either.Right<TRightNew>().Left(leftSelector(left)), //// TODO can you avoid these closures somehow?
                right => Either.Left<TLeftNew>().Right(rightSelector(right)));
        }

        public static Either<(T1, T2), TRight> ShiftRight<T1, T2, TRight>(this Either<(T1, Either<T2, TRight>), TRight> either)
        {
            return either.Visit(
                (left, context) => left.Value.Item2.Visit(
                    (subLeft, subContext) => Either.Right<TRight>().Left((left.Value.Item1, subLeft.Value)),
                    (subRight, subContext) => Either.Left<(T1, T2)>().Right(subRight.Value), 
                    new Void()),
                (right, context) => Either.Left<(T1, T2)>().Right(right.Value),
                new Void());
        }

        public static Either<TLeft, TRight> ShiftRight<TLeft, TRight>(this Either<Either<TLeft, TRight>, TRight> either)
        {
            return either.Visit(
                (left, context) => left.Value.Visit(
                    (subLeft, subContext) => Either.Right<TRight>().Left(subLeft.Value),
                    (subRight, subContext) => Either.Left<TLeft>().Right(subRight.Value),
                    new Void()),
                (right, context) => Either.Left<TLeft>().Right(right.Value),
                new Void());
        }

        public static Either<(TLeftFirst, TLeftSecond), TRight> Zip<TLeftFirst, TLeftSecond, TRight>(this Either<TLeftFirst, TRight> first, Either<TLeftSecond, TRight> second)
        {
            //// TODO is this actually a selectmany?

            return first.Visit(
                (leftFirst, contextFirst) => second.Visit(
                    (leftSecond, contextSecond) => Either.Right<TRight>().Left((leftFirst.Value, leftSecond.Value)),
                    (rightSecond, contextSecond) => Either.Left<(TLeftFirst, TLeftSecond)>().Right(rightSecond.Value),
                    new Void()),
                (rightFirst, contextFirst) => Either.Left<(TLeftFirst, TLeftSecond)>().Right(rightFirst.Value), //// TODO if second is *also* right, you lose track of that one; do you want a TRight aggregator?
                new Void());
        }
    }

    /// <summary>
    /// TODO reconcile this class with the extensions class
    /// TODO are there any other variations of these methods that you should implement "out of the box"?
    /// </summary>
    public static class Either
    {
        /// <summary>
        /// you named it this way so that the `right` method and `rightfactory` don't conflict in the intellisense prompts
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        public sealed class FactoryRight<TLeft>
        {
            /// <summary>
            /// 
            /// </summary>
            private FactoryRight()
            {
            }

            /// <summary>
            /// 
            /// </summary>
            public static FactoryRight<TLeft> Instance { get; } = new FactoryRight<TLeft>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <returns></returns>
        public static FactoryRight<TLeft> Left<TLeft>()
        {
            return FactoryRight<TLeft>.Instance;
        }

        /// <summary>
        /// you named it this way so that the `left` method and `leftfactory` don't conflict in the intellisense prompts
        /// </summary>
        /// <typeparam name="TRight"></typeparam>
        public sealed class FactoryLeft<TRight>
        {
            /// <summary>
            /// 
            /// </summary>
            private FactoryLeft()
            {
            }

            /// <summary>
            /// 
            /// </summary>
            public static FactoryLeft<TRight> Instance { get; } = new FactoryLeft<TRight>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TRight"></typeparam>
        /// <returns></returns>
        public static FactoryLeft<TRight> Right<TRight>()
        {
            return FactoryLeft<TRight>.Instance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="either"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="either"/> is <see langword="null"/></exception>
        /// <exception cref="TRight">Thrown if <paramref name="either"/> is <see cref="Either{TLeft, TRight}.Right"/></exception>
        public static TLeft ThrowRight<TLeft, TRight>(this Either<TLeft, TRight> either) where TRight : Exception
        {
            if (either == null)
            {
                throw new ArgumentNullException(nameof(either));
            }

            return either.Visit(left => left, right => throw right);
        }

        public static Either<TLeft, TRightNew> SelectRight<TLeft, TRightOld, TRightNew>(this Either<TLeft, TRightOld> either, Func<TRightOld, TRightNew> selector)
        {
            return either.Visit<TLeft, TRightOld, Either<TLeft, TRightNew>, bool>(
                (left, context) => new Either<TLeft, TRightNew>.Left(left.Value),
                (right, context) => new Either<TLeft, TRightNew>.Right(selector(right.Value)),
                false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="either"></param>
        /// <param name="leftDispatch"></param>
        /// <param name="rightDispatch"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="either"/> or <paramref name="leftDispatch"/> or <paramref name="rightDispatch"/> is <see langword="null"/></exception>
        /// <exception cref="Exception">Throws any of the exceptions that <paramref name="leftDispatch"/> or <paramref name="rightDispatch"/> can throw</exception> //// TODO is this good?
        public static TResult Visit<TLeft, TRight, TResult>(
            this Either<TLeft, TRight> either,
            Func<TLeft, TResult> leftDispatch,
            Func<TRight, TResult> rightDispatch)
        {
            if (either == null)
            {
                throw new ArgumentNullException(nameof(either));
            }

            if (leftDispatch == null)
            {
                throw new ArgumentNullException(nameof(leftDispatch));
            }

            if (rightDispatch == null)
            {
                throw new ArgumentNullException(nameof(rightDispatch));
            }

            return either.Visit<TLeft, TRight, TResult, Void>(
                (left, context) => leftDispatch(left.Value), //// TODO is there a way to wrap these without creating a closure?
                (right, context) => rightDispatch(right.Value),
                default);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="either"></param>
        /// <param name="leftDispatch"></param>
        /// <param name="rightDispatch"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="either"/> or <paramref name="leftDispatch"/> or <paramref name="rightDispatch"/> or <paramref name="context"/> is <see langword="null"/></exception>
        /// <exception cref="Exception">Throws any of the exceptions that <paramref name="leftDispatch"/> or <paramref name="rightDispatch"/> can throw</exception> //// TODO is this good?
        public static TResult Visit<TLeft, TRight, TResult, TContext>(
            this Either<TLeft, TRight> either,
            Func<Either<TLeft, TRight>.Left, TContext, TResult> leftDispatch,
            Func<Either<TLeft, TRight>.Right, TContext, TResult> rightDispatch,
            TContext context)
        {
            if (either == null)
            {
                throw new ArgumentNullException(nameof(either));
            }

            if (leftDispatch == null)
            {
                throw new ArgumentNullException(nameof(leftDispatch));
            }

            if (rightDispatch == null)
            {
                throw new ArgumentNullException(nameof(rightDispatch));
            }

            if (context == null)
            {
                //// TODO the purpose of `context` is to pass context into the dispatch method; it being `null` defeats the purpose of that concept, though maybe this is just being too strict; you do have `Void` if any caller really doesn't want to provide something; i'm not sure what's right; whichever you decide (keep this check or remove it), you need to be consistent across all of the visitors
                throw new ArgumentNullException(nameof(context));
            }

            var visitor = new DelegateVisitor<TLeft, TRight, TResult, TContext>(leftDispatch, rightDispatch);
            return visitor.Visit(either, context);


        }

        public static Either<TLeft, TRight>.Visitor<TResult, TContext> Visitor<TLeft, TRight, TResult, TContext>(
            Func<Either<TLeft, TRight>.Left, TContext, TResult> leftDispatch,
            Func<Either<TLeft, TRight>.Right, TContext, TResult> rightDispatch)
        {
            return new DelegateVisitor<TLeft, TRight, TResult, TContext>(leftDispatch, rightDispatch);
        }

        /// <summary>
        /// TODO should this be a struct?
        /// TODO it can't be a struct because the `Either.Visitor` is not an interface, and it "can't" be an interface because it has an implementation for `Visit` (i put "can't" in quotes because maybe there's a way around that)
        /// TODO what you might be able to do is have a concrete `Either.Visitor` class (or maybe that should be a struct?) and have a new interface `IDispatcher` that the `visitor` takes in the constructor; then, you could have `dispatcher`s that are structs
        /// TODO does this fix anything if you keep `either.visitor` as a class? and won't boxing occur when you pass the `dispatcher` into the constructor?
        /// TODO you can probably avoid the boxing if you type parameterize the `dispatcher`
        /// 
        /// TODO i think you can actually address all of this with something like (but you need .net 9):
        /// public void M() {
        ///    var dispatcher = new Dispatcher<string>();
        ///    var visitor = new Visitor<string, Dispatcher<string>>(dispatcher);
        ///}
        ///
        ///public ref struct Dispatcher<TResult> : IDispatcher<TResult>
        ///{
        ///}
        ///
        ///public interface IDispatcher<TResult>
        ///{
        ///}
        ///
        ///public ref struct Visitor<TResult, TDispatcher> where TDispatcher : IDispatcher<TResult>, allows ref struct
        ///{
        ///    public Visitor(TDispatcher dispatcher)
        ///    {
        ///    }
        ///}
        ///
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TContext"></typeparam>
        private sealed class DelegateVisitor<TLeft, TRight, TResult, TContext> : Either<TLeft, TRight>.Visitor<TResult, TContext>
        {
            private readonly Func<Either<TLeft, TRight>.Left, TContext, TResult> leftDispatch;
            private readonly Func<Either<TLeft, TRight>.Right, TContext, TResult> rightDispatch;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="leftDispatch"></param>
            /// <param name="rightDispatch"></param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="leftDispatch"/> or <paramref name="rightDispatch"/> is <see langword="null"/></exception>
            public DelegateVisitor(
                Func<Either<TLeft, TRight>.Left, TContext, TResult> leftDispatch,
                Func<Either<TLeft, TRight>.Right, TContext, TResult> rightDispatch)
            {
                if (leftDispatch == null)
                {
                    throw new ArgumentNullException(nameof(leftDispatch));
                }

                if (rightDispatch == null)
                {
                    throw new ArgumentNullException(nameof(rightDispatch));
                }

                this.leftDispatch = leftDispatch;
                this.rightDispatch = rightDispatch;
            }

            /// <inheritdoc/>
            /// <exception cref="Exception">Throws any of the exceptions that the <see cref="DelegateVisitor{TLeft, TRight, TResult, TContext}.leftDispatch"/> delegate can throw</exception> //// TODO is this good?
            public override TResult Dispatch(Either<TLeft, TRight>.Left node, TContext context)
            {
                return this.leftDispatch(node, context);
            }

            /// <inheritdoc/>
            /// <exception cref="Exception">Throws any of the exceptions that the <see cref="DelegateVisitor{TLeft, TRight, TResult, TContext}.rightDispatch"/> delegate can throw</exception> //// TODO is this good?
            public override TResult Dispatch(Either<TLeft, TRight>.Right node, TContext context)
            {
                return this.rightDispatch(node, context);
            }
        }
    }
}
