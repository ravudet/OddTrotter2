namespace OddTrotter.CalendarV2.Fx.Either
{
    using System;

    public abstract class Either<TLeft, TRight> : IEither<TLeft, TRight>
    {
        public TResult Visit<TResult, TContext>(
            Func<TLeft, TContext> leftAccept, 
            Func<TRight, TContext> rightAccept)
        {
            throw new NotImplementedException();
        }

        public TResult Visit<TResult, TContext>(Func<TLeft, TContext, TResult> leftAccept, Func<TRight, TContext, TResult> rightAccept, TContext context)
        {
            return new DelegateVisitor<TResult, TContext>(leftAccept, rightAccept).Visit(this, context);
        }

        private sealed class DelegateVisitor<TResult, TContext> : Visitor<TResult, TContext>
        {
            private readonly Func<TLeft, TContext, TResult> leftAccept;
            private readonly Func<TRight, TContext, TResult> rightAccept;

            public DelegateVisitor(Func<TLeft, TContext, TResult> leftAccept, Func<TRight, TContext, TResult> rightAccept)
            {
                this.leftAccept = leftAccept;
                this.rightAccept = rightAccept;
            }

            protected internal override TResult Accept(Left node, TContext context)
            {
                return this.leftAccept(node.Value, context);
            }

            protected internal override TResult Accept(Right node, TContext context)
            {
                return this.rightAccept(node.Value, context);
            }
        }

        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(Either<TLeft, TRight> node, TContext context)
            {
                return node.Dispatch(this, context);
            }

            protected internal abstract TResult Accept(Either<TLeft, TRight>.Left node, TContext context);
            protected internal abstract TResult Accept(Either<TLeft, TRight>.Right node, TContext context);
        }

        public sealed class Left : Either<TLeft, TRight>
        {
            public Left(TLeft value)
            {
                Value = value;
            }

            public TLeft Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class Right : Either<TLeft, TRight>
        {
            public Right(TRight value)
            {
                Value = value;
            }

            public TRight Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
}
