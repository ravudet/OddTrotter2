////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;

namespace OddTrotter.Calendar
{
    public abstract class Either<TLeft, TRight>
    {
        private Either()
        {
        }

        protected abstract TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(Either<TLeft, TRight> node, TContext context)
            {
                return node.Accept(this, context);
            }

            public abstract TResult Dispatch(Left node, TContext context);

            public abstract TResult Dispatch(Right node, TContext context);
        }

        public sealed class Left : Either<TLeft, TRight>
        {
            public Left(TLeft value)
            {
                Value = value;
            }

            public TLeft Value { get; }

            protected sealed override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Dispatch(this, context);
            }
        }

        public sealed class Right : Either<TLeft, TRight>
        {
            public Right(TRight value)
            {
                Value = value;
            }

            public TRight Value { get; }

            protected sealed override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
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
        public static Either<TLeft, TRight> Left<TLeft, TRight>(this Either.LeftFactory<TRight> leftFactory, TLeft value)
        {
            return new Either<TLeft, TRight>.Left(value);
        }

        public static Either<TLeft, TRight> Right<TLeft, TRight>(this Either.RightFactory<TLeft> rightFactory, TRight value)
        {
            return new Either<TLeft, TRight>.Right(value);
        }

        public static Either<TLeftNew, TRightNew> VisitSelect<TLeftOld, TRightOld, TLeftNew, TRightNew>(
            this Either<TLeftOld, TRightOld> either,
            Func<TLeftOld, TLeftNew> leftSelector,
            Func<TRightOld, TRightNew> rightSelector)
        {
            return either.Visit(
                (left, context) => Either.Right<TRightNew>().Left(leftSelector(left.Value)),
                (right, context) => Either.Left<TLeftNew>().Right(rightSelector(right.Value)),
                new Void());
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
        public sealed class RightFactory<TLeft>
        {
            private RightFactory()
            {
            }

            public static RightFactory<TLeft> Instance { get; } = new RightFactory<TLeft>();
        }

        public static RightFactory<TLeft> Left<TLeft>()
        {
            return RightFactory<TLeft>.Instance;
        }

        public sealed class LeftFactory<TRight>
        {
            private LeftFactory()
            {
            }

            public static LeftFactory<TRight> Instance { get; } = new LeftFactory<TRight>();
        }

        public static LeftFactory<TRight> Right<TRight>()
        {
            return LeftFactory<TRight>.Instance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="either"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="either"/> is <see langword="null"/></exception>
        public static TLeft ThrowRight<TLeft, TRight>(this Either<TLeft, TRight> either) where TRight : Exception
        {
            if (either == null)
            {
                throw new ArgumentNullException(nameof(either));
            }

            //// TODO you are here
            return either.Visit(left => left, right => throw right);
        }

        public static Either<TLeft, TRightNew> SelectRight<TLeft, TRightOld, TRightNew>(this Either<TLeft, TRightOld> either, Func<TRightOld, TRightNew> selector)
        {
            return either.Visit<TLeft, TRightOld, Either<TLeft, TRightNew>, bool>(
                (left, context) => new Either<TLeft, TRightNew>.Left(left.Value),
                (right, context) => new Either<TLeft, TRightNew>.Right(selector(right.Value)),
                false);
        }

        public static TResult Visit<TLeft, TRight, TResult>(
            this Either<TLeft, TRight> either,
            Func<TLeft, TResult> leftDispatch,
            Func<TRight, TResult> rightDispatch)
        {
            //// TODO you are here
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
                //// TODO the purpose of `context` is to pass context into the dispatch method; it being `null` defeats the purpose of that concept, though maybe this is just being too strict; you do have `Void` if any caller really doesn't want to provide something; i'm not sure what's right
                throw new ArgumentNullException(nameof(context));
            }

            //// TODO you are here
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
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TContext"></typeparam>
        private sealed class DelegateVisitor<TLeft, TRight, TResult, TContext> : Either<TLeft, TRight>.Visitor<TResult, TContext>
        {
            private readonly Func<Either<TLeft, TRight>.Left, TContext, TResult> leftDispatch;
            private readonly Func<Either<TLeft, TRight>.Right, TContext, TResult> rightDispatch;

            public DelegateVisitor(
                Func<Either<TLeft, TRight>.Left, TContext, TResult> leftDispatch,
                Func<Either<TLeft, TRight>.Right, TContext, TResult> rightDispatch)
            {
                this.leftDispatch = leftDispatch;
                this.rightDispatch = rightDispatch;
            }

            public override TResult Dispatch(Either<TLeft, TRight>.Left node, TContext context)
            {
                return this.leftDispatch(node, context);
            }

            public override TResult Dispatch(Either<TLeft, TRight>.Right node, TContext context)
            {
                return this.rightDispatch(node, context);
            }
        }
    }
}
