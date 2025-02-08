namespace Fx.Either
{
    public static class Either
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <remarks>
        /// This class was named so that it does not conflict with intellisense's ability to find the <see cref="Left"/> method overloads
        /// </remarks>
        public ref struct Empty<TLeft>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="TRight"></typeparam>
            /// <param name="value"></param>
            /// <returns></returns>
            public Either<TLeft, TRight> Right<TRight>(TRight value)
            {
                return new Either<TLeft, TRight>.Right(value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <remarks>
        /// This class was named so that it does not conflict with intellisense's ability to find the <see cref="Left"/> method overloads
        /// </remarks>
        public ref struct Full<TLeft>
        {
            private readonly TLeft value;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="value"></param>
            public Full(TLeft value)
            {
                this.value = value;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="TRight"></typeparam>
            /// <returns></returns>
            public Either<TLeft, TRight> Right<TRight>()
            {
                return new Either<TLeft, TRight>.Left(this.value);
            }
        }

        //// TODO maybe the factory methods should be able to go in either order?

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <returns></returns>
        public static Empty<TLeft> Left<TLeft>()
        {
            return new Empty<TLeft>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Full<TLeft> Left<TLeft>(TLeft value)
        {
            return new Full<TLeft>(value);
        }
    }
}
