namespace CalendarV2.Fx.Either
{
    using global::System;

    public static class EitherExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="either"></param>
        /// <param name="leftAccept"></param>
        /// <param name="rightAccept"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="either"/> or <paramref name="leftAccept"/> or <paramref name="rightAccept"/> is
        /// <see langword="null"/>
        /// </exception>
        /// <exception cref="Exception">
        /// Throws any of the exceptions that <paramref name="leftAccept"/> or <paramref name="rightAccept"/> can throw
        /// </exception> //// TODO TOPIC wrap this exception maybe?
        public static TResult Visit<TLeft, TRight, TResult>(
            this IEither<TLeft, TRight> either, 
            Func<TLeft, TResult> leftAccept,
            Func<TRight, TResult> rightAccept)
        {
            ArgumentNullException.ThrowIfNull(either);
            ArgumentNullException.ThrowIfNull(leftAccept);
            ArgumentNullException.ThrowIfNull(rightAccept);

            return either.Visit(
                (left, context) => leftAccept(left),
                (right, context) => rightAccept(right),
                new CalendarV2.System.Void());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TLeftValue"></typeparam>
        /// <typeparam name="TRightValue"></typeparam>
        /// <typeparam name="TLeftResult"></typeparam>
        /// <typeparam name="TRightResult"></typeparam>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="either"></param>
        /// <param name="leftSelector"></param>
        /// <param name="rightSelector"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="either"/> or <paramref name="leftSelector"/> or <paramref name="rightSelector"/> is
        /// <see langword="null"/>
        /// </exception>
        /// <exception cref="Exception">
        /// Throws any of the exceptions that <paramref name="leftSelector"/> or <paramref name="rightSelector"/> can throw
        /// </exception> //// TODO TOPIC wrap this exception maybe?
        public static IEither<TLeftResult, TRightResult> Select
            <
                TLeftValue, 
                TRightValue, 
                TLeftResult, 
                TRightResult, 
                TContext
            >
            (
                this IEither<TLeftValue, TRightValue> either,
                Func<TLeftValue, TContext, TLeftResult> leftSelector,
                Func<TRightValue, TContext, TRightResult> rightSelector,
                TContext context)
        {
            ArgumentNullException.ThrowIfNull(either);
            ArgumentNullException.ThrowIfNull(leftSelector);
            ArgumentNullException.ThrowIfNull(rightSelector);

            return either.Visit(
                (left, context) => Either.Left(leftSelector(left, context)).Right<TRightResult>(),
                (right, context) => Either.Left<TLeftResult>().Right(rightSelector(right, context)),
                context);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TLeftValue"></typeparam>
        /// <typeparam name="TRightValue"></typeparam>
        /// <typeparam name="TLeftResult"></typeparam>
        /// <typeparam name="TRightResult"></typeparam>
        /// <param name="either"></param>
        /// <param name="leftSelector"></param>
        /// <param name="rightSelector"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="either"/> or <paramref name="leftSelector"/> or <paramref name="rightSelector"/> is
        /// <see langword="null"/>
        /// </exception>
        /// <exception cref="Exception">
        /// Throws any of the exceptions that <paramref name="leftSelector"/> or <paramref name="rightSelector"/> can throw
        /// </exception> //// TODO TOPIC wrap this exception maybe?
        public static IEither<TLeftResult, TRightResult> Select
            <
                TLeftValue,
                TRightValue,
                TLeftResult,
                TRightResult
            >
            (
                this IEither<TLeftValue, TRightValue> either,
                Func<TLeftValue, TLeftResult> leftSelector,
                Func<TRightValue, TRightResult> rightSelector)
        {
            ArgumentNullException.ThrowIfNull(either);
            ArgumentNullException.ThrowIfNull(leftSelector);
            ArgumentNullException.ThrowIfNull(rightSelector);

            return either.Visit(
                (left, context) => Either.Left(leftSelector(left)).Right<TRightResult>(),
                (right, context) => Either.Left<TLeftResult>().Right(rightSelector(right)),
                new CalendarV2.System.Void());
        }
    }

    /// <summary>
    /// TODO can you use this anywhere?
    /// </summary>
    public static class ExceptionExtensions
    {
        public static CalendarV2.System.Void Throw<TException>(this TException exception) where TException : Exception
        {
            throw exception;
        }
    }
}
