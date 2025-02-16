namespace Fx.Try
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Linq;

    using global::Fx.Either;
    using OddTrotter.TodoList;

    //// TODO my current conclusion:
    //// have try (the current pattern) and trycovariant (a covariant "overload")
    //// have an extension that uses the try pattern with trycovariant
    //// have an extension that adapts trycovariant to try
    //// use try everywhere that covariance is irrelevant; callers interested in consistently using trycovariant are welcome to call the adapter (this means, we should not create convenience overloads everywhere)
    //// TODO try to write the `notnull` enumerable extension

    public delegate bool Try<in TInput, TOutput>(TInput input, [MaybeNullWhen(false)] out TOutput output);
}
