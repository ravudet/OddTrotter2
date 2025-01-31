namespace CalendarV2.Fx.Try
{
    public delegate TOutput Try<in TInput, out TOutput>(TInput input, out bool success);

    public static class TryExtensions
    {
        public static bool Try<TInput, TOutput>(this Try<TInput, TOutput> @try, TInput input, out TOutput output)
        {
            output = @try(input, out var success);
            return success;
        }

        /*public static Fx.Either.IEither<TOutput, CalendarV2.System.Void> ToEither<TInput, TOutput>(this Try<TInput, TOutput> @try, TInput input)
        {
            //// TODO this method really highlights that `either`s don't hvae lazy evaluation
            if (@try(input, out var output))
            {
                return Fx.Either.Either.Left(output).Right<CalendarV2.System.Void>();
            }

            return Fx.Either.Either.Left<TOutput>().Right(new CalendarV2.System.Void());
        }*/

        public static OldTry<TInput, TOutput> ToOldTry<TInput, TOutput>(this Try<TInput, TOutput> @try)
        {
            return @try.Try;
        }
    }

    public delegate bool OldTry<in TInput, TOutput>(TInput input, out TOutput output);

    public static class OldTryExtensions
    {
        public static Try<TInput, TOutput> ToTry<TInput, TOutput>(this OldTry<TInput, TOutput> oldTry)
        {
            return (TInput input, out bool success) =>
            {
                success = oldTry(input, out var output);
                return output;
            };
        }
    }
}
