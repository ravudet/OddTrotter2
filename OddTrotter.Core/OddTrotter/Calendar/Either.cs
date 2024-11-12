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

    public static class Either
    {
        public static TResult Visit<TLeft, TRight, TResult, TContext>(
            this Either<TLeft, TRight> either,
            Func<Either<TLeft, TRight>.Left, TContext, TResult> leftDispatch,
            Func<Either<TLeft, TRight>.Right, TContext, TResult> rightDispatch,
            TContext context)
        {
            var visitor = new DelegateVisitor<TLeft, TRight, TResult, TContext>(leftDispatch, rightDispatch);
            return visitor.Visit(either, context);
        }

        public static Either<TLeft, TRight>.Visitor<TResult, TContext> Visitor<TLeft, TRight, TResult, TContext>(
            Func<Either<TLeft, TRight>.Left, TContext, TResult> leftDispatch,
            Func<Either<TLeft, TRight>.Right, TContext, TResult> rightDispatch)
        {
            return new DelegateVisitor<TLeft, TRight, TResult, TContext>(leftDispatch, rightDispatch);
        }

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
