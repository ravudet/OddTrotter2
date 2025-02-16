namespace Fx.Try
{
    using System.Diagnostics.CodeAnalysis;

    public static class TryCovariantExtensions
    {
        public static bool Try<TInput, TOutput>(this TryCovariant<TInput, TOutput> @try, TInput input, [MaybeNullWhen(false)] out TOutput output)
        {
            output = @try(input, out var tried);
            return tried;
        }

        public static Try<TInput, TOutput> ToTry<TInput, TOutput>(this TryCovariant<TInput, TOutput> @try)
        {
            return @try.Try;
        }
    }
}
