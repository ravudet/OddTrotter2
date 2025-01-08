////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace OddTrotter.Calendar
{
    using OddTrotter.TodoList;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    public interface IQueryContext<TValue, TError>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //// TODO you are here
        Task<QueryResult<TValue, TError>> Evaluate();
    }

    /// <summary>
    /// TODO is it *really* that this is modeling queryresult*nodes* and that there are really 2 types of "queryresults": one that has just values, and one that has values and an error? this would let you mostly have ienumerables running around
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TError"></typeparam>
    public abstract class QueryResult<TValue, TError>
    {
        private QueryResult()
        {
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="visitor"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="visitor"/> is <see langword="null"/></exception>
        /// <exception cref="Exception">Throws any of the exceptions that the <see cref="Visitor{TResult, TContext}.Dispatch"/> overloads can throw</exception> //// TODO is this good?
        protected abstract Task<TResult> AcceptAsync<TResult, TContext>(AsyncVisitor<TResult, TContext> visitor, TContext context);

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
            public TResult Visit(QueryResult<TValue, TError> node, TContext context)
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
            public abstract TResult Dispatch(Final node, TContext context);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="node"></param>
            /// <param name="context"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="node"/> is <see langword="null"/></exception>
            /// <exception cref="Exception">Can throw any exception as documented by the derived type</exception>
            public abstract TResult Dispatch(Element node, TContext context);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="node"></param>
            /// <param name="context"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="node"/> is <see langword="null"/></exception>
            /// <exception cref="Exception">Can throw any exception as documented by the derived type</exception>
            public abstract TResult Dispatch(Partial node, TContext context);
        }

        public abstract class AsyncVisitor<TResult, TContext>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="node"></param>
            /// <param name="context"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="node"/> is <see langword="null"/></exception>
            /// <exception cref="Exception">Throws any of the exceptions that the <see cref="Dispatch"/> overloads can throw</exception> //// TODO is this good?
            public async Task<TResult> VisitAsync(QueryResult<TValue, TError> node, TContext context)
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
            public abstract Task<TResult> DispatchAsync(Final node, TContext context);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="node"></param>
            /// <param name="context"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="node"/> is <see langword="null"/></exception>
            /// <exception cref="Exception">Can throw any exception as documented by the derived type</exception>
            public abstract Task<TResult> DispatchAsync(Element node, TContext context);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="node"></param>
            /// <param name="context"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="node"/> is <see langword="null"/></exception>
            /// <exception cref="Exception">Can throw any exception as documented by the derived type</exception>
            public abstract Task<TResult> DispatchAsync(Partial node, TContext context);
        }

        public sealed class Final : QueryResult<TValue, TError>
        {
            /// <summary>
            /// 
            /// </summary>
            public Final()
            {
            }

            /// <inheritdoc/>
            protected override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                if (visitor == null)
                {
                    throw new ArgumentNullException(nameof(visitor));
                }

                return visitor.Dispatch(this, context);
            }

            /// <inheritdoc/>
            protected override async Task<TResult> AcceptAsync<TResult, TContext>(AsyncVisitor<TResult, TContext> visitor, TContext context)
            {
                if (visitor == null)
                {
                    throw new ArgumentNullException(nameof(visitor));
                }

                return await visitor.DispatchAsync(this, context).ConfigureAwait(false);
            }
        }

        public abstract class Element : QueryResult<TValue, TError>
        {
            public Element(TValue value)
            {
                //// TODO this value needs to be realized for the first element of the query result to be returned, and since the first element of the query result is the same object as the one returned, this means that we lose laziness; for example, if i have a method Foo that returns a query result that's pulled from a service and I do something like Foo().Concat(Foo()), both queries need to be executed before we can even return; the developer who is calling this concat *could* implement their own derived type of Element, but that is a significant burden over the concat call
                this.Value = value;
            }

            public TValue Value { get; }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            /// <remarks>
            /// this method should not throw; cases where throwing might make sense should instead be handled by returning <see cref="QueryResult{TValue, TError}.Partial"/>
            /// </remarks>
            public abstract QueryResult<TValue, TError> Next(); //// TODO you previously tried using `task`s here, but you realized that the tasks were all running in the background, taking away any laziness that might be useful; could you have something like a `lazytask` that doesn't start until awaited or something? make it a struct?

            /// <inheritdoc/>
            protected override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                if (visitor == null)
                {
                    throw new ArgumentNullException(nameof(visitor));
                }

                return visitor.Dispatch(this, context);
            }

            /// <inheritdoc/>
            protected override async Task<TResult> AcceptAsync<TResult, TContext>(AsyncVisitor<TResult, TContext> visitor, TContext context)
            {
                if (visitor == null)
                {
                    throw new ArgumentNullException(nameof(visitor));
                }

                return await visitor.DispatchAsync(this, context).ConfigureAwait(false);
            }
        }

        public sealed class Partial : QueryResult<TValue, TError>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="error"></param>
            public Partial(TError error)
            {
                this.Error = error;
            }

            public TError Error { get; }

            /// <inheritdoc/>
            protected override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                if (visitor == null)
                {
                    throw new ArgumentNullException(nameof(visitor));
                }

                return visitor.Dispatch(this, context);
            }

            /// <inheritdoc/>
            protected override async Task<TResult> AcceptAsync<TResult, TContext>(AsyncVisitor<TResult, TContext> visitor, TContext context)
            {
                if (visitor == null)
                {
                    throw new ArgumentNullException(nameof(visitor));
                }

                return await visitor.DispatchAsync(this, context).ConfigureAwait(false);
            }
        }
    }

    public static class ExceptionExtensions
    {
        public static Exception AsException<TException>(this TException exception) where TException : Exception
        {
            return exception;
        }
    }

    public static class QueryResultAsyncExtensions
    {
        public static async Task<QueryResult<TResult, TError>> Select<TSource, TError, TResult>(
            this Task<QueryResult<TSource, TError>> queryResult,
            Func<TSource, TResult> selector)
        {
            return (await queryResult.ConfigureAwait(false)).Select(selector);
        }

        public static async Task<QueryResult<TResult, TError>> SelectAsync<TSource, TError, TResult>(
            this Task<QueryResult<TSource, TError>> queryResult,
            Func<TSource, Task<TResult>> selector)
        {
            return await (await queryResult.ConfigureAwait(false)).SelectAsync(selector).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TError"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="queryResult"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="queryResult"/> or <paramref name="selector"/> is <see langword="null"/></exception>
        public static async Task<QueryResult<TResult, TError>> SelectAsync<TSource, TError, TResult>(
            this QueryResult<TSource, TError> queryResult,
            Func<TSource, Task<TResult>> selector)
        {
            if (queryResult == null)
            {
                throw new ArgumentNullException(nameof(queryResult));
            }

            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return await SelectAsyncVisitor<TSource, TError, TResult>.Instance.VisitAsync(queryResult, selector).ConfigureAwait(false);
        }

        private sealed class SelectAsyncResult<TSource, TError, TResult> : QueryResult<TResult, TError>.Element
        {
            private readonly QueryResult<TSource, TError>.Element queryResult;
            private readonly Func<TSource, Task<TResult>> selector;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="queryResult"></param>
            /// <param name="selector"></param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="selector"/> or <paramref name="queryResult"/> is <see langword="null"/></exception>
            public SelectAsyncResult(
                QueryResult<TSource, TError>.Element queryResult,
                Func<TSource, Task<TResult>> selector)
                :base((selector ?? throw new ArgumentNullException(nameof(selector)))((queryResult ?? throw new ArgumentNullException(nameof(queryResult))).Value).ConfigureAwait(false).GetAwaiter().GetResult()) //// TODO you should getresult here, you should have an async queryresult variant... //// TODO does this mean that `queryresult` should *not* have an async visitor, and `asyncqueryresult` should not have a synchronous visitor?
            {
                this.queryResult = queryResult;
                this.selector = selector;
            }

            /// <inheritdoc/>
            public override QueryResult<TResult, TError> Next()
            {
                return SelectAsyncVisitor<TSource, TError, TResult>.Instance.VisitAsync(this.queryResult.Next(), this.selector).ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }

        private class SelectAsyncVisitor<TSource, TError, TResult> : QueryResult<TSource, TError>.AsyncVisitor<QueryResult<TResult, TError>, Func<TSource, Task<TResult>>>
        {
            /// <summary>
            /// 
            /// </summary>
            protected SelectAsyncVisitor()
            {
            }

            /// <summary>
            /// 
            /// </summary>
            public static SelectAsyncVisitor<TSource, TError, TResult> Instance { get; } = new SelectAsyncVisitor<TSource, TError, TResult>();

            /// <inheritdoc/>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/>  is <see langword="null"/></exception> 
            public override async Task<QueryResult<TResult, TError>> DispatchAsync(QueryResult<TSource, TError>.Final node, Func<TSource, Task<TResult>> context)
            {
                if (node == null)
                {
                    throw new ArgumentNullException(nameof(node));
                }

                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                return await Task.FromResult(new QueryResult<TResult, TError>.Final()).ConfigureAwait(false);
            }

            /// <inheritdoc/>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/>  is <see langword="null"/></exception> 
            public override async Task<QueryResult<TResult, TError>> DispatchAsync(QueryResult<TSource, TError>.Element node, Func<TSource, Task<TResult>> context)
            {
                if (node == null)
                {
                    throw new ArgumentNullException(nameof(node));
                }

                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                return await Task.FromResult(new SelectAsyncResult<TSource, TError, TResult>(node, context)).ConfigureAwait(false);
            }

            /// <inheritdoc/>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/>  is <see langword="null"/></exception> 
            public override async Task<QueryResult<TResult, TError>> DispatchAsync(QueryResult<TSource, TError>.Partial node, Func<TSource, Task<TResult>> context)
            {
                if (node == null)
                {
                    throw new ArgumentNullException(nameof(node));
                }

                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                return await Task.FromResult(new QueryResult<TResult, TError>.Partial(node.Error)).ConfigureAwait(false);
            }
        }
    }

    public static class QueryResultExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TError"></typeparam>
        /// <param name="queryResult"></param>
        /// <returns></returns>
        public static QueryResult<TValue, TError> AsBase<TValue, TError>(this QueryResult<TValue, TError> queryResult)
        {
            return queryResult;
        }

        private sealed class SplitVisitor<TValue, TError, TRight> : QueryResult<Either<TValue, TRight>, TError>.Visitor<TError?, (Action<TValue>, Action<TRight>)>
        {
            private SplitVisitor()
            {
            }

            public static SplitVisitor<TValue, TError, TRight> Instance { get; } = new SplitVisitor<TValue, TError, TRight>();

            public override TError? Dispatch(QueryResult<Either<TValue, TRight>, TError>.Final node, (Action<TValue>, Action<TRight>) context)
            {
                return default;
            }

            public override TError? Dispatch(QueryResult<Either<TValue, TRight>, TError>.Element node, (Action<TValue>, Action<TRight>) context)
            {
                node.Value.Visit(
                    (left, @void) =>
                    {
                        context.Item1(left.Value);
                        return new Void();
                    },
                    (right, @void) =>
                    {
                        context.Item2(right.Value);
                        return new Void();
                    },
                    new Void()); //// TODO shouldn't you pass in `context` instead of creating closures?
                return default;
            }

            public override TError? Dispatch(QueryResult<Either<TValue, TRight>, TError>.Partial node, (Action<TValue>, Action<TRight>) context)
            {
                return node.Error;
            }
        }

        public static TError? Split<TValue, TError, TRight>(
            this QueryResult<Either<TValue, TRight>, TError> queryResult,
            Action<TValue> leftAction,
            Action<TRight> rightAction)
        {
            return SplitVisitor<TValue, TError, TRight>.Instance.Visit(queryResult, (leftAction, rightAction));
        }

        public static IEnumerable<TValue> ToEnumerable<TValue, TError>(this QueryResult<TValue, TError> queryResult)
        {
            while (queryResult is QueryResult<TValue, TError>.Element element)
            {
                yield return element.Value;
                queryResult = element.Next();
            }

            //// TODO throw if queryresult is now partial?
        }

        private sealed class ErrorResult<TValue, TErrorStart, TErrorEnd> : QueryResult<TValue, TErrorEnd>.Element
        {
            private readonly QueryResult<TValue, TErrorStart>.Element queryResult;
            private readonly Func<TErrorStart, TErrorEnd> selector;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="queryResult"></param>
            /// <param name="selector"></param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="queryResult"/> or <paramref name="selector"/> is <see langword="null"/></exception>
            public ErrorResult(QueryResult<TValue, TErrorStart>.Element queryResult, Func<TErrorStart, TErrorEnd> selector)
                : base((queryResult ?? throw new ArgumentNullException(nameof(queryResult))).Value)
            {
                if (selector == null)
                {
                    throw new ArgumentNullException(nameof(selector));
                }

                this.queryResult = queryResult;
                this.selector = selector;
            }

            public override QueryResult<TValue, TErrorEnd> Next()
            {
                return ErrorVisitor<TValue, TErrorStart, TErrorEnd>.Instance.Visit(this.queryResult.Next(), this.selector);
            }
        }

        private sealed class ErrorVisitor<TValue, TErrorStart, TErrorEnd> : QueryResult<TValue, TErrorStart>.Visitor<QueryResult<TValue, TErrorEnd>, Func<TErrorStart, TErrorEnd>>
        {
            /// <summary>
            /// 
            /// </summary>
            private ErrorVisitor()
            {
            }

            /// <summary>
            /// 
            /// </summary>
            public static ErrorVisitor<TValue, TErrorStart, TErrorEnd> Instance { get; } = new ErrorVisitor<TValue, TErrorStart, TErrorEnd>();

            /// <inheritdoc/>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/> is <see langword="null"/></exception>
            public override QueryResult<TValue, TErrorEnd> Dispatch(QueryResult<TValue, TErrorStart>.Final node, Func<TErrorStart, TErrorEnd> context)
            {
                if (node == null)
                {
                    throw new ArgumentNullException(nameof(node));
                }

                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                return new QueryResult<TValue, TErrorEnd>.Final();
            }

            /// <inheritdoc/>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/> is <see langword="null"/></exception>
            public override QueryResult<TValue, TErrorEnd> Dispatch(QueryResult<TValue, TErrorStart>.Element node, Func<TErrorStart, TErrorEnd> context)
            {
                if (node == null)
                {
                    throw new ArgumentNullException(nameof(node));
                }

                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                return new ErrorResult<TValue, TErrorStart, TErrorEnd>(node, context);
            }

            /// <inheritdoc/>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/> is <see langword="null"/></exception>
            public override QueryResult<TValue, TErrorEnd> Dispatch(QueryResult<TValue, TErrorStart>.Partial node, Func<TErrorStart, TErrorEnd> context)
            {
                if (node == null)
                {
                    throw new ArgumentNullException(nameof(node));
                }

                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                return new QueryResult<TValue, TErrorEnd>.Partial(context(node.Error));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TErrorStart"></typeparam>
        /// <typeparam name="TErrorEnd"></typeparam>
        /// <param name="queryResult"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="queryResult"/> or <paramref name="selector"/> is <see langword="null"/></exception>
        public static QueryResult<TValue, TErrorEnd> ErrorSelect<TValue, TErrorStart, TErrorEnd>(this QueryResult<TValue, TErrorStart> queryResult, Func<TErrorStart, TErrorEnd> selector)
        {
            //// TODO do you like this method name? do you want to normalize with names used in `either`?

            if (queryResult == null)
            {
                throw new ArgumentNullException(nameof(queryResult));
            }

            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return ErrorVisitor<TValue, TErrorStart, TErrorEnd>.Instance.Visit(queryResult, selector);
        }

        private sealed class WhereResult<TValue, TError> : QueryResult<TValue, TError>.Element
        {
            private readonly Element queryResult;
            private readonly Func<TValue, bool> predicate;

            public WhereResult(QueryResult<TValue, TError>.Element queryResult, Func<TValue, bool> predicate)
                : base(queryResult.Value)
            {
                this.queryResult = queryResult;
                this.predicate = predicate;
            }

            public sealed override QueryResult<TValue, TError> Next()
            {
                return WhereVisitor<TValue, TError>.Instance.Visit(this.queryResult.Next(), this.predicate);
            }
        }

        private sealed class WhereVisitor<TValue, TError> : QueryResult<TValue, TError>.Visitor<QueryResult<TValue, TError>, Func<TValue, bool>>
        {
            private WhereVisitor()
            {
            }

            public static WhereVisitor<TValue, TError> Instance { get; } = new WhereVisitor<TValue, TError>();

            public override QueryResult<TValue, TError> Dispatch(QueryResult<TValue, TError>.Final node, Func<TValue, bool> context)
            {
                return node;
            }

            public override QueryResult<TValue, TError> Dispatch(QueryResult<TValue, TError>.Element node, Func<TValue, bool> context)
            {
                if (context(node.Value))
                {
                    return new WhereResult<TValue, TError>(node, context);
                }
                else
                {
                    return this.Visit(node.Next(), context);
                }
            }

            public override QueryResult<TValue, TError> Dispatch(QueryResult<TValue, TError>.Partial node, Func<TValue, bool> context)
            {
                return node;
            }
        }

        public static QueryResult<TValue, TError> Where<TValue, TError>(this QueryResult<TValue, TError> queryResult, Func<TValue, bool> predicate)
        {
            return WhereVisitor<TValue, TError>.Instance.Visit(queryResult, predicate);
        }

        private sealed class ToQueryResultResult<TValue, TError> : QueryResult<TValue, TError>.Element
        {
            private readonly IEnumerator<TValue> enumerator;

            public ToQueryResultResult(IEnumerator<TValue> enumerator)
                : base(enumerator.Current)
            {
                //// TODO make queryresult.final disposable?
                this.enumerator = enumerator;
            }

            public override QueryResult<TValue, TError> Next()
            {
                if (!this.enumerator.MoveNext())
                {
                    return new QueryResult<TValue, TError>.Final();
                }

                //// TODO is this ok? it might not be properly immutable this way
                return new ToQueryResultResult<TValue, TError>(this.enumerator);
            }
        }

        public static QueryResult<TValue, TError> ToQueryResult<TValue, TError>(this IEnumerable<TValue> enumerable)
        {
            IEnumerator<TValue>? enumerator = null;
            try
            {
                enumerator = enumerable.GetEnumerator();
                if (!enumerator.MoveNext())
                {
                    return new QueryResult<TValue, TError>.Final();
                }

                return new ToQueryResultResult<TValue, TError>(enumerator);
            }
            catch
            {
                enumerator?.Dispose();
                throw;
            }
        }

        private sealed class ConcatResult<TValue, TError> : QueryResult<TValue, TError>.Element
        {
            private readonly QueryResult<TValue, TError>.Element element;
            private readonly QueryResult<TValue, TError> second;

            public ConcatResult(QueryResult<TValue, TError>.Element element, QueryResult<TValue, TError> second)
                : base(element.Value)
            {
                this.element = element;
                this.second = second;
            }

            public override QueryResult<TValue, TError> Next()
            {
                return ConcatVisitor<TValue, TError>.Instance.Visit(this.element.Next(), second);
            }
        }

        private sealed class ConcatVisitor<TValue, TError> : QueryResult<TValue, TError>.Visitor<QueryResult<TValue, TError>, QueryResult<TValue, TError>>
        {
            private ConcatVisitor()
            {
            }

            public static ConcatVisitor<TValue, TError> Instance { get; } = new ConcatVisitor<TValue, TError>();

            public override QueryResult<TValue, TError> Dispatch(QueryResult<TValue, TError>.Final node, QueryResult<TValue, TError> context)
            {
                return context;
            }

            public override QueryResult<TValue, TError> Dispatch(QueryResult<TValue, TError>.Element node, QueryResult<TValue, TError> context)
            {
                return new ConcatResult<TValue, TError>(node, context);
            }

            public override QueryResult<TValue, TError> Dispatch(QueryResult<TValue, TError>.Partial node, QueryResult<TValue, TError> context)
            {
                //// TODO what if there was in error in first *and* second?
                return node;
            }
        }

        public static QueryResult<TValue, TError> Concat<TValue, TError>(this QueryResult<TValue, TError> first, QueryResult<TValue, TError> second)
        {
            return ConcatVisitor<TValue, TError>.Instance.Visit(first, second);
        }

        private sealed class SelectResult<TValueStart, TValueEnd, TError> : QueryResult<TValueEnd, TError>.Element
        {
            private readonly QueryResult<TValueStart, TError>.Element queryResult;
            private readonly Func<TValueStart, TValueEnd> selector;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="queryResult"></param>
            /// <param name="selector"></param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="queryResult"/> or <paramref name="selector"/> is <see langword="null"/></exception>
            public SelectResult(QueryResult<TValueStart, TError>.Element queryResult, Func<TValueStart, TValueEnd> selector)
                : base((selector ?? throw new ArgumentNullException(nameof(selector)))((queryResult ?? throw new ArgumentNullException(nameof(queryResult))).Value))
            {
                this.queryResult = queryResult;
                this.selector = selector;
            }

            public override QueryResult<TValueEnd, TError> Next()
            {
                return SelectVisitor<TValueStart, TValueEnd, TError>.Instance.Visit(this.queryResult.Next(), this.selector);
            }
        }

        private sealed class SelectVisitor<TValueStart, TValueEnd, TError> : QueryResult<TValueStart, TError>.Visitor<QueryResult<TValueEnd, TError>, Func<TValueStart, TValueEnd>>
        {
            /// <summary>
            /// 
            /// </summary>
            private SelectVisitor()
            {
            }

            /// <summary>
            /// 
            /// </summary>
            public static SelectVisitor<TValueStart, TValueEnd, TError> Instance { get; } = new SelectVisitor<TValueStart, TValueEnd, TError>();

            /// <inheritdoc/>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/> is <see langword="null"/></exception>
            public override QueryResult<TValueEnd, TError> Dispatch(QueryResult<TValueStart, TError>.Final node, Func<TValueStart, TValueEnd> context)
            {
                if (node == null)
                {
                    throw new ArgumentNullException(nameof(node));
                }

                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                return new QueryResult<TValueEnd, TError>.Final();
            }

            /// <inheritdoc/>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/> is <see langword="null"/></exception>
            public override QueryResult<TValueEnd, TError> Dispatch(QueryResult<TValueStart, TError>.Element node, Func<TValueStart, TValueEnd> context)
            {
                if (node == null)
                {
                    throw new ArgumentNullException(nameof(node));
                }

                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                return new SelectResult<TValueStart, TValueEnd, TError>(node, context);
            }

            /// <inheritdoc/>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/> is <see langword="null"/></exception>
            public override QueryResult<TValueEnd, TError> Dispatch(QueryResult<TValueStart, TError>.Partial node, Func<TValueStart, TValueEnd> context)
            {
                if (node == null)
                {
                    throw new ArgumentNullException(nameof(node));
                }

                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                return new QueryResult<TValueEnd, TError>.Partial(node.Error);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TValueStart"></typeparam>
        /// <typeparam name="TValueEnd"></typeparam>
        /// <typeparam name="TError"></typeparam>
        /// <param name="queryResult"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="queryResult"/> or <paramref name="selector"/> is <see langword="null"/></exception>
        public static QueryResult<TValueEnd, TError> Select<TValueStart, TValueEnd, TError>(this QueryResult<TValueStart, TError> queryResult, Func<TValueStart, TValueEnd> selector)
        {
            if (queryResult == null)
            {
                throw new ArgumentNullException(nameof(queryResult));
            }

            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return SelectVisitor<TValueStart, TValueEnd, TError>.Instance.Visit(queryResult, selector);
        }

        private sealed class DistinctByResult<TValue, TError, TKey> : QueryResult<TValue, TError>.Element
        {
            private readonly Element queryResult;
            private readonly DistinctByContext<TValue, TKey> context;

            public DistinctByResult(QueryResult<TValue, TError>.Element queryResult, DistinctByContext<TValue, TKey> context)
                : base(queryResult.Value)
            {
                this.queryResult = queryResult;
                this.context = context;
            }

            public override QueryResult<TValue, TError> Next()
            {
                return DistinctByVisitor<TValue, TError, TKey>.Instance.Visit(this.queryResult.Next(), this.context);
            }
        }

        private sealed class DistinctByContext<TValue, TKey>
        {
            public DistinctByContext(Func<TValue, TKey> keySelector, IEqualityComparer<TKey> comparer)
            {
                this.KeySelector = keySelector;
                this.Comparer = comparer;
            }

            public Func<TValue, TKey> KeySelector { get; }

            public IEqualityComparer<TKey> Comparer { get; }
        }

        private sealed class DistinctByVisitor<TValue, TError, TKey> : QueryResult<TValue, TError>.Visitor<QueryResult<TValue, TError>, DistinctByContext<TValue, TKey>>
        {
            private DistinctByVisitor()
            {
            }

            public static DistinctByVisitor<TValue, TError, TKey> Instance { get; } = new DistinctByVisitor<TValue, TError, TKey>();

            public override QueryResult<TValue, TError> Dispatch(QueryResult<TValue, TError>.Final node, DistinctByContext<TValue, TKey> context)
            {
                return node;
            }

            public override QueryResult<TValue, TError> Dispatch(QueryResult<TValue, TError>.Element node, DistinctByContext<TValue, TKey> context)
            {
                throw new NotImplementedException();
            }

            public override QueryResult<TValue, TError> Dispatch(QueryResult<TValue, TError>.Partial node, DistinctByContext<TValue, TKey> context)
            {
                return node;
            }
        }

        public static QueryResult<TValue, TError> DistinctBy<TValue, TError, TKey>(this QueryResult<TValue, TError> queryResult, Func<TValue, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            var context = new DistinctByContext<TValue, TKey>(keySelector, comparer);
            return DistinctByVisitor<TValue, TError, TKey>.Instance.Visit(queryResult, context);
        }
    }
}
