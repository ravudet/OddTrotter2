using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stash
{
    public static class Either2
    {
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
        private sealed class DelegateVisitor<TLeft, TRight, TResult, TContext> : Fx.Either.Either<TLeft, TRight>.Visitor<TResult, TContext>
        {
            private readonly Func<Fx.Either.Either<TLeft, TRight>.Left, TContext, TResult> leftDispatch;
            private readonly Func<Fx.Either.Either<TLeft, TRight>.Right, TContext, TResult> rightDispatch;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="leftDispatch"></param>
            /// <param name="rightDispatch"></param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="leftDispatch"/> or <paramref name="rightDispatch"/> is <see langword="null"/></exception>
            public DelegateVisitor(
                Func<Fx.Either.Either<TLeft, TRight>.Left, TContext, TResult> leftDispatch,
                Func<Fx.Either.Either<TLeft, TRight>.Right, TContext, TResult> rightDispatch)
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

            protected internal override TResult Accept(Fx.Either.Either<TLeft, TRight>.Left node, TContext context)
            {
                return this.leftDispatch(node, context);
            }

            protected internal override TResult Accept(Fx.Either.Either<TLeft, TRight>.Right node, TContext context)
            {
                return this.rightDispatch(node, context);
            }
        }
    }
}
