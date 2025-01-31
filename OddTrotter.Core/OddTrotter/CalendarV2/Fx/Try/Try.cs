namespace CalendarV2.Fx.Try
{
    using CalendarV2.System;
    using global::CalendarV2.Fx.Either;
    using global::System;
    using global::System.Diagnostics.CodeAnalysis;

    public delegate TOutput? Try1<in TInput, out TOutput>(TInput input, out bool success);
    public delegate bool Try2<in TInput, TOutput>(TInput input, [MaybeNullWhen(false)] out TOutput success);
    public delegate IEither<TOutput, CalendarV2.System.Void> Try3<in TInput, out TOutput>(TInput input);
    
    public static class Try1Extensions
    {
        public static bool Try<TInput, TOutput>(this Try1<TInput, TOutput> @try, TInput input, [MaybeNullWhen(false)] out TOutput output)
        {
            output = @try(input, out var success);
            return success;
        }

        public static Try2<TInput, TOutput> ToTry2<TInput, TOutput>(this Try1<TInput, TOutput> @try)
        {
            return @try.Try;
        }
    }

    public static class Try2Extensions
    {
        public static bool Try<TInput, TOutput>(this Try2<TInput, TOutput> @try, TInput input, [MaybeNullWhen(false)] out TOutput output)
        {
            return @try(input, out output);
        }

        /*public static Try<TInput, TOutput> ToTry<TInput, TOutput>(this OldTry<TInput, TOutput> oldTry)
        {
            return (TInput input, out bool success) =>
            {
                success = oldTry(input, out var output);
                return output!; //// TODO can you actually do this if `output` ends up being `null`?
            };
        }*/
    }

    public static class Try3Extensions
    {
        public static bool Try<TInput, TOutput>(this Try3<TInput, TOutput> @try, TInput input, [MaybeNullWhen(false)] out TOutput output)
        {
            return @try(input).TryLeft(out output);
        }

        public static Try2<TInput, TOutput> ToTry2<TInput, TOutput>(this Try3<TInput, TOutput> @try)
        {
            return @try.Try;
        }

        /*public static bool Try<TInput, TOutput>(this EitherTry<TInput, TOutput> eitherTry, TInput input, out TOutput output)
        {
            eitherTry(input).TryLeft()
        }*/

            /*public static Try<TInput, TOutput> ToTry<TInput, TOutput>(this Func<TInput, TOutput> func)
            {
                int? foo = 0;

                foo.ToEither();

                return (TInput input, out bool output) =>
                {
                    try
                    {
                        output = true;
                        return func(input);
                    }
                    catch
                    {
                        output = false;
                        return default!;
                    }
                };
            }*/

            /*public static IEither<T, CalendarV2.System.Void> ToEither<T>(this Nullable<T> nullable) where T : struct
            {
                //// TODO do you need this variant? `int? foo = 0; foo.ToEither();` works without it....
                if (nullable.HasValue)
                {
                    return Either.Left(nullable.Value).Right<CalendarV2.System.Void>();
                }
                else
                {
                    return Either.Left<T>().Right(new CalendarV2.System.Void());
                }
            }*/

        public static IEither<T, CalendarV2.System.Void> ToEither<T>(this T? nullable)
        {
            if (nullable == null)
            {
                return Either.Left<T>().Right(new CalendarV2.System.Void());
            }
            else
            {
                return Either.Left(nullable).Right<CalendarV2.System.Void>();
            }
        }
    }
}
