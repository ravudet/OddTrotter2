namespace CalendarV2.Fx.Try
{
    public delegate bool Try<in TInput, TOutput>(TInput input, out TOutput output); //// TODO return toutput and have an extension method to switch the return values so that you can have covariance

    public delegate TOutput Try2<in TInput, out TOutput>(TInput input, out bool success);

    public static class TryExtensions
    {
        public static bool Try<TInput, TOutput>(this Try2<TInput, TOutput> @try, TInput input, out TOutput output)
        {
            output = @try(input, out var success);
            return success;
        }

        public static Fx.Either.IEither<TOutput, CalendarV2.System.Void> ToEither<TInput, TOutput>(this Try<TInput, TOutput> @try, TInput input)
        {
            //// TODO this method really highlights that `either`s don't hvae lazy evaluation
            if (@try(input, out var output))
            {
                return Fx.Either.Either.Left(output).Right<CalendarV2.System.Void>();
            }

            return Fx.Either.Either.Left<TOutput>().Right(new CalendarV2.System.Void());
        }
    }
}
