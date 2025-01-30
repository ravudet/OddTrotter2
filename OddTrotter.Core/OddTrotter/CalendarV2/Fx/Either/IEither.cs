namespace CalendarV2.Fx.Either
{
    using global::System;

    public interface IEither<out TLeft, out TRight>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="leftAccept"></param>
        /// <param name="rightAccept"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="leftAccept"/> or <paramref name="rightAccept"/> is <see langword="null"/>
        /// </exception>
        /// <exception cref="Exception">
        /// Throws any of the exceptions that <paramref name="leftAccept"/> or <paramref name="rightAccept"/> can throw
        /// </exception> //// TODO TOPIC wrap this exception maybe? //// TODO look into performance implications and come back //// TODO also feedback that the documenation is not easier to consume with acustom exception type //// TODO do a mockup of a caller catching the exception; *maybe* there's value in differentiating between left or right throwing
        TResult Visit<TResult, TContext>(
            Func<TLeft, TContext, TResult> leftAccept, 
            Func<TRight, TContext, TResult> rightAccept,
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
