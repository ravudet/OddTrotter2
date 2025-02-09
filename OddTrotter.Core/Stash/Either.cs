namespace Stash
{
    public static class Either
    {
        public static class Play
        {
            public static void DoPlay()
            {
                Fx.Either.Either<Fx.Either.Either<int, string>, System.Exception> either1 =
                    Either
                        .Left()
                            .Left(42)
                            .Right<string>()
                        .Right<System.Exception>();

                Fx.Either.Either<Fx.Either.Either<int, string>, System.Exception> either2 =
                    Either
                        .Left()
                            .Left<int>()
                            .Right(string.Empty)
                        .Right<System.Exception>();

                Fx.Either.Either<Fx.Either.Either<int, string>, System.Exception> either3 =
                    Either
                        .Left()
                            .Left<int>()
                            .Right<string>()
                        .Right(new System.Exception());

                Fx.Either.Either<int, Fx.Either.Either<string, System.Exception>> either4 =
                    Either
                        .Left(42)
                        .Right()
                            .Left<string>()
                            .Right<System.Exception>();

                Fx.Either.Either<int, Fx.Either.Either<string, System.Exception>> either5 =
                    Either
                        .Left<int>()
                        .Right()
                            .Left(string.Empty)
                            .Right<System.Exception>();

                Fx.Either.Either<int, Fx.Either.Either<string, System.Exception>> either6 =
                    Either
                        .Left<int>()
                        .Right()
                            .Left<string>()
                            .Right(new System.Exception());

                Fx.Either.Either<Fx.Either.Either<int, string>, Fx.Either.Either<System.Exception, object>> either7 =
                    Either
                        .Left()
                            .Left(42)
                            .Right<string>()
                        .Right()
                            .Left<System.Exception>()
                            .Right<object>();

                //// TODO these factories are very close, but they have this nesting issue; you *could* just implement some level of nesting and chose where the cutoff is, but if you can make it recursive, that would be best
                /*Fx.Either.Either<Fx.Either.Either<Fx.Either.Either<int, string>, System.Exception>, object> either8 =
                    Either
                        .Left()
                            .Left()
                                .Left(42)
                                .Right<string>()
                            .Right<System.Exception>()
                            .Right<object>();*/
            }
        }

        public static NoType Left()
        {
            return new NoType();
        }

        public readonly ref struct NoType
        {
            public Full<TLeft> Left<TLeft>(TLeft value)
            {
                return new Full<TLeft>(value);
            }

            public Empty<TLeft> Left<TLeft>()
            {
                return new Empty<TLeft>();
            }

            public NoType Left()
            {
                return new NoType();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="TLeft"></typeparam>
            /// <remarks>
            /// This class was named so that it does not conflict with intellisense's ability to find the <see cref="Left"/> method overloads
            /// </remarks>
            public readonly ref struct Empty<TLeft>
            {
                public Either.Full<Fx.Either.Either<TLeft, TRight>> Right<TRight>(TRight value)
                {
                    return new Either.Full<Fx.Either.Either<TLeft, TRight>>(new Fx.Either.Either<TLeft, TRight>.Right(value));
                }

                public Either.Empty<Fx.Either.Either<TLeft, TRight>> Right<TRight>()
                {
                    return new Either.Empty<Fx.Either.Either<TLeft, TRight>>();
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="TLeft"></typeparam>
            /// <remarks>
            /// This class was named so that it does not conflict with intellisense's ability to find the <see cref="Left"/> method overloads
            /// </remarks>
            public readonly ref struct Full<TLeft>
            {
                private readonly TLeft value;

                public Full(TLeft value)
                {
                    this.value = value;
                }

                public Either.Full<Fx.Either.Either<TLeft, TRight>> Right<TRight>()
                {
                    return new Either.Full<Fx.Either.Either<TLeft, TRight>>(new Fx.Either.Either<TLeft, TRight>.Left(this.value));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <remarks>
        /// This class was named so that it does not conflict with intellisense's ability to find the <see cref="Left"/> method overloads
        /// </remarks>
        public readonly ref struct Empty<TLeft>
        {
            /// <summary>
            /// placeholder
            /// </summary>
            /// <typeparam name="TRight"></typeparam>
            /// <param name="value"></param>
            /// <returns></returns>
            public Fx.Either.Either<TLeft, TRight> Right<TRight>(TRight value)
            {
                return new Fx.Either.Either<TLeft, TRight>.Right(value);
            }

            public NoType Right()
            {
                return new NoType();
            }

            public readonly ref struct NoType
            {
                public Full<TLeft2> Left<TLeft2>(TLeft2 value)
                {
                    return new Full<TLeft2>(value);
                }

                public readonly ref struct Full<TLeft2>
                {
                    private readonly TLeft2 value;

                    public Full(TLeft2 value)
                    {
                        this.value = value;
                    }

                    public Fx.Either.Either<TLeft, Fx.Either.Either<TLeft2, TRight>> Right<TRight>()
                    {
                        return new Fx.Either.Either<TLeft, Fx.Either.Either<TLeft2, TRight>>.Right(new Fx.Either.Either<TLeft2, TRight>.Left(this.value));
                    }
                }

                public Empty<TLeft2> Left<TLeft2>()
                {
                    return new Empty<TLeft2>();
                }

                public readonly ref struct Empty<TLeft2>
                {
                    public Fx.Either.Either<TLeft, Fx.Either.Either<TLeft2, TRight>> Right<TRight>(TRight value)
                    {
                        return new Fx.Either.Either<TLeft, Fx.Either.Either<TLeft2, TRight>>.Right(new Fx.Either.Either<TLeft2, TRight>.Right(value));
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <remarks>
        /// This class was named so that it does not conflict with intellisense's ability to find the <see cref="Left"/> method overloads
        /// </remarks>
        public readonly ref struct Full<TLeft>
        {
            private readonly TLeft value;

            /// <summary>
            /// placeholder
            /// </summary>
            /// <param name="value"></param>
            public Full(TLeft value)
            {
                this.value = value;
            }

            /// <summary>
            /// placeholder
            /// </summary>
            /// <typeparam name="TRight"></typeparam>
            /// <returns></returns>
            public Fx.Either.Either<TLeft, TRight> Right<TRight>()
            {
                return new Fx.Either.Either<TLeft, TRight>.Left(this.value);
            }

            public NoType Right()
            {
                return new NoType();
            }

            public readonly ref struct NoType
            {
                private readonly Either.Full<TLeft> full;

                public NoType(Either.Full<TLeft> full)
                {
                    this.full = full;
                }

                public Full<TLeft2> Left<TLeft2>()
                {
                    return new Full<TLeft2>(this.full.value);
                }

                public readonly ref struct Full<TLeft2>
                {
                    private readonly TLeft value;

                    public Full(TLeft value)
                    {
                        this.value = value;
                    }

                    public Fx.Either.Either<TLeft, Fx.Either.Either<TLeft2, TRight>> Right<TRight>()
                    {
                        return new Fx.Either.Either<TLeft, Fx.Either.Either<TLeft2, TRight>>.Left(this.value);
                    }
                }
            }
        }

        //// TODO maybe the factory methods should be able to go in either order?

        /// <summary>
        /// placeholder
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <returns></returns>
        public static Empty<TLeft> Left<TLeft>()
        {
            return new Empty<TLeft>();
        }

        /// <summary>
        /// placeholder
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
