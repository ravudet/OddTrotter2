namespace Fx.Try
{
    using global::System.Diagnostics.CodeAnalysis;

    public delegate bool Try<in TInput, TOutput>(TInput input, [MaybeNullWhen(false)] out TOutput output);

    public static class Play
    {
        public static System.Collections.Generic.IEnumerable<T> NotNull<T>(this System.Collections.Generic.IEnumerable<T?> source)
        {
            return source.TrySelect<T?, T>(TryGetValue);
        }

        public static bool TryGetValue<T>(this T? nullable, [MaybeNullWhen(false)] out T value)
        {
            if (nullable == null)
            {
                value = default;
                return false;
            }

            value = nullable;
            return true;
        }

        public static System.Collections.Generic.IEnumerable<TResult> TrySelect<TSource, TResult>(this System.Collections.Generic.IEnumerable<TSource> source, Try<TSource, TResult> @try)
        {
            foreach (var element in source)
            {
                if (@try(element, out var result))
                {
                    yield return result;
                }
            }
        }
    }
}
