/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.Try
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// placeholder
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    /// <param name="input"></param>
    /// <param name="output"></param>
    /// <returns></returns>
    public delegate bool Try<in TInput, TOutput>(TInput input, [MaybeNullWhen(false)] out TOutput output);
}
