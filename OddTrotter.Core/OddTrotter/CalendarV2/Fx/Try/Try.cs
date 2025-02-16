namespace Fx.Try
{
    using global::System.Diagnostics.CodeAnalysis;

    //// TODO my current conclusion:
    //// TODO try to write the `notnull` enumerable extension
    //// TODO use try everywhere that covariance is irrelevant; callers interested in consistently using trycovariant are welcome to call the adapter (this means, we should not create convenience overloads everywhere)

    public delegate bool Try<in TInput, TOutput>(TInput input, [MaybeNullWhen(false)] out TOutput output);
}
