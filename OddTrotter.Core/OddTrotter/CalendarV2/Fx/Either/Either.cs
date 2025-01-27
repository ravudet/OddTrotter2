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
