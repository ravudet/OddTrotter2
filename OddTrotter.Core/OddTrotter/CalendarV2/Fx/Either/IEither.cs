namespace OddTrotter.CalendarV2.Fx.Either
{
    using System;

    public interface IEither<out TLeft, out TRight>
    {
        TResult Visit<TResult, TContext>(Func<TLeft, TContext, TResult> leftAccept, Func<TRight, TContext, TResult> rightAccept, TContext context);
    }
}
