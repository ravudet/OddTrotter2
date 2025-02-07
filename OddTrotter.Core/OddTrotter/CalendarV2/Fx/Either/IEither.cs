/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.Either
{
    using System;

    public interface IEither<out TLeft, out TRight>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="leftMap"></param>
        /// <param name="rightMap"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="leftMap"/> or <paramref name="rightMap"/> is <see langword="null"/>
        /// </exception>
        /// <exception cref="LeftMapException">
        /// Thrown if <paramref name="leftMap"/> throws an exception. The <see cref="Exception.InnerException"/> will be set to
        /// whatever exception <paramref name="leftMap"/> threw.
        /// </exception>
        /// <exception cref="RightMapException">
        /// Thrown if <paramref name="rightMap"/> throws an exception. The <see cref="Exception.InnerException"/> will be set to
        /// whatever exception <paramref name="rightMap"/> threw.
        /// </exception>
        /// <remarks>
        /// This is named `apply`. Other names proposed:
        /// 1. `visit` - This leaks the design detail that the visitor pattern is used to implement the method; it's a fine name,
        /// but if we can do better, we should.
        /// 2. `aggregate`/`fold` - This is just definitely not a `fold`. The return type being a completely new, non-`ieither`
        /// value distracted me when considering this option, but ultimately there is only a single value in the `ieither`
        /// structure, so there is really no traversal happening that is essential to a `fold`, as noted 
        /// [here](https://en.wikipedia.org/wiki/Fold_(higher-order_function):
        /// > functions that analyze a recursive data structure and through use of a given combining operation, recombine the
        /// > results of recursively processing its constituent parts
        /// 3. `fmap` - Haskell has an `fmap` function
        /// ([reference](https://en.wikipedia.org/wiki/Functor#Computer_implementations)) that takes a functor. A functor maps
        /// [morphisms](https://en.wikipedia.org/wiki/Morphism) and morphisms are structure-preserving. In this method,
        /// <paramref name="leftMap"/> and <paramref name="rightMap"/> are the components of the piecewise function and (together
        /// or individually) they do *not* preserve structure (though they may be written in a way which *does* preserve
        /// structure). As a result, that piecewise function is *not* a functor, and therefore this is *not* `fmap`.
        /// 4. `morph` - This was an option because it seemed to be the "verb" form (and therefore more idiomatic to c#) of
        /// "morphism". However, as described above in `fmap`, <paramref name="leftMap"/> and <paramref name="rightMap"/> form a
        /// piecewise function that is *not* structure preserving and therefore is not a morphism.
        /// 5. `switch` - Similar to `visit`, this leaks the design detail that a discriminated union is being used to implement
        /// the method. Although this works well as an analog to the c#
        /// [switch expression](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/switch-expression),
        /// the name obfuscates the monadic nature of `ieither`.
        /// 
        /// [`apply`](https://en.wikipedia.org/wiki/Apply) was chosen because this method is applying the piecewise map composed
        /// of <paramref name="leftMap"/> and <paramref name="rightMap"/> to that map's `ieither` argument.
        /// </remarks>
        TResult Apply<TResult, TContext>(
            Func<TLeft, TContext, TResult> leftMap, 
            Func<TRight, TContext, TResult> rightMap,
            TContext context);

        //// TODO what are all of the visitor variants?
        //// async
        //// unsafe
        //// result allows ref struct
        //// context allows ref struct
        //// context by reference
        //// there are likely others
    }
}
