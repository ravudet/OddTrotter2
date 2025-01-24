namespace System
{
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;

    internal static class ArgumentNullInline
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="argument"></param>
        /// <param name="paramName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="argument"/> is <see langword="null"/></exception>
        public static T ThrowIfNull<T>([NotNull] T? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        {
            //// TODO TOPIC do you want to make this an extension method?
            //// TODO TOPIC do you want to restrict `t` to non-structs? how to do this? do you just need multiple overloads? will that be forward compatible with new nullable types that may get introduced?
            //// TODO TOPIC `argumentnullexception.throwifnull` casts it to an object instead of using a generic; it's not clear that is the best idea
            ArgumentNullException.ThrowIfNull(argument, paramName);

            return argument;
        }
    }
}
