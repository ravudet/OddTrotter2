namespace CalendarV2.Fx.Either
{
    using global::System;

    /// <summary>
    /// TODO TOPIC all of the names of the extensions really
    /// </summary>
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
        public static TResult Visit<TLeft, TRight, TResult>( //// TODO TOPIC call this "aggregate" instead?
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
        public static IEither<TLeftResult, TRightResult> Select //// TODO TOPIC name this "map" instead?
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
        /// <typeparam name="TContext"></typeparam>
        /// <param name="either"></param>
        /// <param name="leftSelector"></param>
        /// <param name="rightSelector"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="either"/> or <paramref name="leftSelector"/> is <see langword="null"/>
        /// </exception>
        /// <exception cref="Exception">
        /// Throws any of the exceptions that <paramref name="leftSelector"/> can throw
        /// </exception> //// TODO TOPIC wrap this exception maybe?
        public static IEither<TLeftResult, TRightValue> SelectLeft //// TODO TOPIC name this "map" instead? //// TODO TOPIC what about the overall name?
            <
                TLeftValue,
                TRightValue,
                TLeftResult,
                TContext
            >
            (
                this IEither<TLeftValue, TRightValue> either,
                Func<TLeftValue, TContext, TLeftResult> leftSelector,
                TContext context)
        {
            ArgumentNullException.ThrowIfNull(either);
            ArgumentNullException.ThrowIfNull(leftSelector);

            return either.Visit(
                (left, context) => Either.Left(leftSelector(left, context)).Right<TRightValue>(),
                (right, context) => Either.Left<TLeftResult>().Right(right),
                context);
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
        /// Thrown if <paramref name="either"/> or <paramref name="rightSelector"/> is <see langword="null"/>
        /// </exception>
        /// <exception cref="Exception">
        /// Throws any of the exceptions that <paramref name="rightSelector"/> can throw
        /// </exception> //// TODO TOPIC wrap this exception maybe?
        public static IEither<TLeftValue, TRightResult> SelectRight //// TODO TOPIC name this "map" instead? //// TODO TOPIC what about the overall name?
            <
                TLeftValue,
                TRightValue,
                TRightResult,
                TContext
            >
            (
                this IEither<TLeftValue, TRightValue> either,
                Func<TRightValue, TContext, TRightResult> rightSelector,
                TContext context)
        {
            ArgumentNullException.ThrowIfNull(either);
            ArgumentNullException.ThrowIfNull(rightSelector);

            return either.Visit(
                (left, context) => Either.Left(left).Right<TRightResult>(),
                (right, context) => Either.Left<TLeftValue>().Right(rightSelector(right, context)),
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
        public static IEither<TLeftResult, TRightResult> Select //// TODO TOPIC name this "map" instead?
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
        /// Thrown if <paramref name="either"/> or <paramref name="leftSelector"/> is <see langword="null"/>
        /// </exception>
        /// <exception cref="Exception">
        /// Throws any of the exceptions that <paramref name="leftSelector"/> can throw
        /// </exception> //// TODO TOPIC wrap this exception maybe?
        public static IEither<TLeftResult, TRightValue> SelectLeft //// TODO TOPIC name this "map" instead? //// TODO TOPIC what about the overall name?
            <
                TLeftValue,
                TRightValue,
                TLeftResult
            >
            (
                this IEither<TLeftValue, TRightValue> either,
                Func<TLeftValue, TLeftResult> leftSelector)
        {
            ArgumentNullException.ThrowIfNull(either);
            ArgumentNullException.ThrowIfNull(leftSelector);

            return either.Visit(
                (left, context) => Either.Left(leftSelector(left)).Right<TRightValue>(),
                (right, context) => Either.Left<TLeftResult>().Right(right),
                new CalendarV2.System.Void());
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
        /// Thrown if <paramref name="either"/> or <paramref name="rightSelector"/> is <see langword="null"/>
        /// </exception>
        /// <exception cref="Exception">
        /// Throws any of the exceptions that <paramref name="rightSelector"/> can throw
        /// </exception> //// TODO TOPIC wrap this exception maybe?
        public static IEither<TLeftValue, TRightResult> SelectRight //// TODO TOPIC name this "map" instead? //// TODO TOPIC what about the overall name?
            <
                TLeftValue,
                TRightValue,
                TRightResult
            >
            (
                this IEither<TLeftValue, TRightValue> either,
                Func<TRightValue, TRightResult> rightSelector)
        {
            ArgumentNullException.ThrowIfNull(either);
            ArgumentNullException.ThrowIfNull(rightSelector);

            return either.Visit(
                (left, context) => Either.Left(left).Right<TRightResult>(),
                (right, context) => Either.Left<TLeftValue>().Right(rightSelector(right)),
                new CalendarV2.System.Void());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="either"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="either"/> is <see langword="null"/></exception>
        public static IEither<TLeft, TRight> PropogateRight<TLeft, TRight>(
            this IEither<IEither<TLeft, TRight>, TRight> either)
        {
            ArgumentNullException.ThrowIfNull(either);

            return either.Visit(
                left => 
                    left.Visit(
                        subLeft => Either.Left(subLeft).Right<TRight>(),
                        subRight => Either.Left<TLeft>().Right(subRight)),
                right =>
                    Either.Left<TLeft>().Right(right));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="either"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="either"/> is <see langword="null"/></exception>
        public static IEither<TLeft, TRight> PropogateLeft<TLeft, TRight>(
            this IEither<TLeft, IEither<TLeft, TRight>> either)
        {
            ArgumentNullException.ThrowIfNull(either);

            return either.Visit(
                left =>
                    Either.Left(left).Right<TRight>(),
                right =>
                    right.Visit(
                        subLeft => Either.Left(subLeft).Right<TRight>(),
                        subRight => Either.Left<TLeft>().Right(subRight)));
        }

        public static IEither<(TLeftFirst, TLeftSecond), TRight> Zip<TLeftFirst, TLeftSecond, TRight>(
            this IEither<TLeftFirst, TRight> first,
            IEither<TLeftSecond, TRight> second,
            Func<TRight, TRight, TRight> rightAggregator) //// TODO TOPIC naming of this //// TODO TOPIC other variants of this? like, does `tright` need to be the same for both eithers? and should you always return a tuple? don't forget your ultimate use-case of first.zip(second).throwright()
        {
        }

        //// TODO add "coalesce" variants
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
