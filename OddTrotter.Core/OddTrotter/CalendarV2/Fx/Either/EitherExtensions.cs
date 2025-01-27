namespace CalendarV2.Fx.Either
{
    public static class EitherExtensions
    {
        public static Either<TLeft, TRight> AsBase<TLeft, TRight>(this Either<TLeft, TRight> either)
        {
            return either;
        }
    }
}
