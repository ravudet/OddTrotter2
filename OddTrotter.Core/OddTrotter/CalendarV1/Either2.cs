////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using Fx.Either;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OddTrotter.Calendar
{
    public abstract class Either<TLeft, TRight>
    {
        /// <summary>
        /// 
        /// </summary>
        private Either()
        {
        }

        /// <summary>
        /// TODO normalize all of your visitors with the pattern (naming, internal protected in the visitor, etc) that you are using in other repos
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="visitor"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="visitor"/> is <see langword="null"/></exception>
        /// <exception cref="Exception">Throws any of the exceptions that the <see cref="Visitor{TResult, TContext}.Dispatch"/> overloads can throw</exception> //// TODO is this good?
        protected abstract TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        /// <summary>
        /// TODO normalize all of your visitors with the pattern (naming, internal protected in the visitor, etc) that you are using in other repos
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="visitor"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="visitor"/> is <see langword="null"/></exception>
        /// <exception cref="Exception">Throws any of the exceptions that the <see cref="AsyncVisitor{TResult, TContext}.DispatchAsync"/> overloads can throw</exception> //// TODO is this good?
        protected abstract Task<TResult> AcceptAsync<TResult, TContext>(AsyncVisitor<TResult, TContext> visitor, TContext context);

        public abstract class AsyncVisitor<TResult, TContext>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="node"></param>
            /// <param name="context"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="node"/> is <see langword="null"/></exception>
            /// <exception cref="Exception">Throws any of the exceptions that the <see cref="DispatchAsync"/> overloads can throw</exception> //// TODO is this good?
            public async Task<TResult> VisitAsync(Either<TLeft, TRight> node, TContext context)
            {
                if (node == null)
                {
                    throw new ArgumentNullException(nameof(node));
                }

                return await node.AcceptAsync(this, context).ConfigureAwait(false);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="node"></param>
            /// <param name="context"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="node"/> is <see langword="null"/></exception>
            /// <exception cref="Exception">Can throw any exception as documented by the derived type</exception>
            public abstract Task<TResult> DispatchAsync(Left node, TContext context);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="node"></param>
            /// <param name="context"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="node"/> is <see langword="null"/></exception>
            /// <exception cref="Exception">Can throw any exception as documented by the derived type</exception>
            public abstract Task<TResult> DispatchAsync(Right node, TContext context);
        }

        public abstract class Visitor<TResult, TContext>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="node"></param>
            /// <param name="context"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="node"/> is <see langword="null"/></exception>
            /// <exception cref="Exception">Throws any of the exceptions that the <see cref="Dispatch"/> overloads can throw</exception> //// TODO is this good?
            public TResult Visit(Either<TLeft, TRight> node, TContext context)
            {
                if (node == null)
                {
                    throw new ArgumentNullException(nameof(node));
                }

                return node.Accept(this, context);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="node"></param>
            /// <param name="context"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="node"/> is <see langword="null"/></exception>
            /// <exception cref="Exception">Can throw any exception as documented by the derived type</exception>
            public abstract TResult Dispatch(Left node, TContext context);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="node"></param>
            /// <param name="context"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="node"/> is <see langword="null"/></exception>
            /// <exception cref="Exception">Can throw any exception as documented by the derived type</exception>
            public abstract TResult Dispatch(Right node, TContext context);
        }

        public sealed class Left : Either<TLeft, TRight>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="value"></param>
            public Left(TLeft value)
            {
                Value = value;
            }

            public TLeft Value { get; }

            /// <inheritdoc/>
            protected sealed override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                if (visitor == null)
                {
                    throw new ArgumentNullException(nameof(visitor));
                }

                return visitor.Dispatch(this, context);
            }

            /// <inheritdoc/>
            protected sealed override async Task<TResult> AcceptAsync<TResult, TContext>(AsyncVisitor<TResult, TContext> visitor, TContext context)
            {
                if (visitor == null)
                {
                    throw new ArgumentNullException(nameof(visitor));
                }

                return await visitor.DispatchAsync(this, context).ConfigureAwait(false);
            }
        }

        public sealed class Right : Either<TLeft, TRight>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="value"></param>
            public Right(TRight value)
            {
                Value = value;
            }

            public TRight Value { get; }

            /// <inheritdoc/>
            protected sealed override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                if (visitor == null)
                {
                    throw new ArgumentNullException(nameof(visitor));
                }

                return visitor.Dispatch(this, context);
            }

            /// <inheritdoc/>
            protected sealed override async Task<TResult> AcceptAsync<TResult, TContext>(AsyncVisitor<TResult, TContext> visitor, TContext context)
            {
                if (visitor == null)
                {
                    throw new ArgumentNullException(nameof(visitor));
                }

                return await visitor.DispatchAsync(this, context).ConfigureAwait(false);
            }
        }
    }

    public static class Extensions
    {
        public static T? ToNullable<T>(this T value)
        {
            return value;
        }
    }

    public static class EitherAsyncExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TLeftOld"></typeparam>
        /// <typeparam name="TRightOld"></typeparam>
        /// <typeparam name="TLeftNew"></typeparam>
        /// <typeparam name="TRightNew"></typeparam>
        /// <param name="either"></param>
        /// <param name="leftSelector"></param>
        /// <param name="rightSelector"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="either"/> or <paramref name="leftSelector"/> or <paramref name="rightSelector"/> is <see langword="null"/></exception>
        /// <exception cref="Exception">Throws any of the exceptions that <paramref name="leftSelector"/> or <paramref name="rightSelector"/> can throw</exception> //// TODO is this good?
        public static async Task<IEither<TLeftNew, TRightNew>> SelectAsync<TLeftOld, TRightOld, TLeftNew, TRightNew>(
            this Fx.Either.IEither<TLeftOld, TRightOld> either,
            Func<TLeftOld, Task<TLeftNew>> leftSelector,
            Func<TRightOld, Task<TRightNew>> rightSelector)
        {
            if (either == null)
            {
                throw new ArgumentNullException(nameof(either));
            }

            if (leftSelector == null)
            {
                throw new ArgumentNullException(nameof(leftSelector));
            }

            if (rightSelector == null)
            {
                throw new ArgumentNullException(nameof(rightSelector));
            }

            return await Task.FromResult(
                either
                    .Select(
                    left => leftSelector(left).ConfigureAwait(false).GetAwaiter().GetResult(),
                    right => rightSelector(right).ConfigureAwait(false).GetAwaiter().GetResult()));

            /*return await either
                .VisitAsync(
                    async (left, context) => Either.Right<TRightNew>().Left(await leftSelector(left.Value).ConfigureAwait(false)),
                    async (right, context) => Either.Left<TLeftNew>().Right(await rightSelector(right.Value).ConfigureAwait(false)),
                    new Nothing())
                .ConfigureAwait(false);*/
        }
    }

    /// <summary>
    /// TODO reconcile this class with the extensions class
    /// TODO are there any other variations of these methods that you should implement "out of the box"?
    /// </summary>
    public static class Either2
    {
        /// <summary>
        /// you named it this way so that the `right` method and `rightfactory` don't conflict in the intellisense prompts
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        public sealed class FactoryRight<TLeft>
        {
            /// <summary>
            /// 
            /// </summary>
            private FactoryRight()
            {
            }

            /// <summary>
            /// 
            /// </summary>
            public static FactoryRight<TLeft> Instance { get; } = new FactoryRight<TLeft>();
        }

        /// <summary>
        /// you named it this way so that the `left` method and `leftfactory` don't conflict in the intellisense prompts
        /// </summary>
        /// <typeparam name="TRight"></typeparam>
        public sealed class FactoryLeft<TRight>
        {
            /// <summary>
            /// 
            /// </summary>
            private FactoryLeft()
            {
            }
        }

        /// <summary>
        /// TODO should this be a struct?
        /// TODO it can't be a struct because the `Either.Visitor` is not an interface, and it "can't" be an interface because it has an implementation for `Visit` (i put "can't" in quotes because maybe there's a way around that)
        /// TODO what you might be able to do is have a concrete `Either.Visitor` class (or maybe that should be a struct?) and have a new interface `IDispatcher` that the `visitor` takes in the constructor; then, you could have `dispatcher`s that are structs
        /// TODO does this fix anything if you keep `either.visitor` as a class? and won't boxing occur when you pass the `dispatcher` into the constructor?
        /// TODO you can probably avoid the boxing if you type parameterize the `dispatcher`
        /// 
        /// TODO i think you can actually address all of this with something like (but you need .net 9):
        /// public void M() {
        ///    var dispatcher = new Dispatcher<string>();
        ///    var visitor = new Visitor<string, Dispatcher<string>>(dispatcher);
        ///}
        ///
        ///public ref struct Dispatcher<TResult> : IDispatcher<TResult>
        ///{
        ///}
        ///
        ///public interface IDispatcher<TResult>
        ///{
        ///}
        ///
        ///public ref struct Visitor<TResult, TDispatcher> where TDispatcher : IDispatcher<TResult>, allows ref struct
        ///{
        ///    public Visitor(TDispatcher dispatcher)
        ///    {
        ///    }
        ///}
        ///
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TContext"></typeparam>
        private sealed class DelegateVisitor<TLeft, TRight, TResult, TContext> : Either<TLeft, TRight>.Visitor<TResult, TContext>
        {
            private readonly Func<Either<TLeft, TRight>.Left, TContext, TResult> leftDispatch;
            private readonly Func<Either<TLeft, TRight>.Right, TContext, TResult> rightDispatch;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="leftDispatch"></param>
            /// <param name="rightDispatch"></param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="leftDispatch"/> or <paramref name="rightDispatch"/> is <see langword="null"/></exception>
            public DelegateVisitor(
                Func<Either<TLeft, TRight>.Left, TContext, TResult> leftDispatch,
                Func<Either<TLeft, TRight>.Right, TContext, TResult> rightDispatch)
            {
                if (leftDispatch == null)
                {
                    throw new ArgumentNullException(nameof(leftDispatch));
                }

                if (rightDispatch == null)
                {
                    throw new ArgumentNullException(nameof(rightDispatch));
                }

                this.leftDispatch = leftDispatch;
                this.rightDispatch = rightDispatch;
            }

            /// <inheritdoc/>
            /// <exception cref="Exception">Throws any of the exceptions that the <see cref="DelegateVisitor{TLeft, TRight, TResult, TContext}.leftDispatch"/> delegate can throw</exception> //// TODO is this good?
            public override TResult Dispatch(Either<TLeft, TRight>.Left node, TContext context)
            {
                return this.leftDispatch(node, context);
            }

            /// <inheritdoc/>
            /// <exception cref="Exception">Throws any of the exceptions that the <see cref="DelegateVisitor{TLeft, TRight, TResult, TContext}.rightDispatch"/> delegate can throw</exception> //// TODO is this good?
            public override TResult Dispatch(Either<TLeft, TRight>.Right node, TContext context)
            {
                return this.rightDispatch(node, context);
            }
        }
    }
}
