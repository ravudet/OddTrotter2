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
        /// </exception> //// TODO TOPIC wrap this exception maybe?
        TResult Visit<TResult, TContext>(
            Func<TLeft, TContext, TResult> leftAccept, 
            Func<TRight, TContext, TResult> rightAccept,
            TContext context);
    }
}
