/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Stash
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Fx.Either;

    public static class EitherExtensions
    {
        public static void NullPropagateUseCase()
        {
            var foo1 = Foo1()?.Bar1()?.Frob1();

            var foo2 = Foo2().NullPropagate(Bar1).NullPropagate(Frob1);
        }

        public static object? Foo1()
        {
            return new object();
        }

        public static string Bar1(this object value)
        {
            return value.ToString() ?? string.Empty;
        }

        public static int Frob1(this string value)
        {
            return value.Length;
        }

        public static Either<string, Nothing> Foo2() //// TOPIC TODO you need a "null" type instead of overriding void probably
        {
            return Either.Left("asdf").Right<Nothing>();
        }

        public static IEither<TLeftResult, Nothing> NullPropagate<TLeftValue, TLeftResult>(
            this IEither<TLeftValue, Nothing> either,
            Func<TLeftValue, TLeftResult> selector)
        {
            //// TODO TOPIC is this extension even worth having?
            return either.SelectLeft(selector);
        }
        public static IEither<TLeft, Nothing> NullPropagate<TLeft>(this IEither<IEither<TLeft, Nothing>, Nothing> either)
        {
            //// TODO TOPIC this extension is *not* null propagate; find a better name; previously, we liked the name "propagate", but it's definitely not (see above);
            return either.SelectManyLeft();
        }

    }
}
