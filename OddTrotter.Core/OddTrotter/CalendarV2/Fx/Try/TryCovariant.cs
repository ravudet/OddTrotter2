namespace Fx.Try
{
    using System.Diagnostics.CodeAnalysis;

    [return: MaybeNull]
    public delegate TOutput TryCovariant<in TInput, out TOutput>(TInput input, out bool tried);
}
