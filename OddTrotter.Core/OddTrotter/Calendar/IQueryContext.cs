////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace OddTrotter.Calendar
{
    using OddTrotter.TodoList;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IQueryContext<TValue, TError>
    {
        Task<QueryResult<TValue, TError>> Evaluate();
    }

    public abstract class QueryResult<TValue, TError>
    {
        private QueryResult()
        {
        }

        protected abstract TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(QueryResult<TValue, TError> node, TContext context)
            {
                return node.Accept(this, context);
            }

            public abstract TResult Dispatch(Final node, TContext context);

            public abstract TResult Dispatch(Element node, TContext context);

            public abstract TResult Dispatch(Partial node, TContext context);
        }

        public sealed class Final : QueryResult<TValue, TError>
        {
            public Final()
            {
            }

            protected override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Dispatch(this, context);
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

            public abstract QueryResult<TValue, TError> Next();

            protected override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Dispatch(this, context);
            }
        }

        public sealed class Partial : QueryResult<TValue, TError>
        {
            public Partial(TError error)
            {
                this.Error = error;
            }

            public TError Error { get; }

            protected override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Dispatch(this, context);
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

    public static class QueryResultExtensions
    {
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

            public ErrorResult(QueryResult<TValue, TErrorStart>.Element queryResult, Func<TErrorStart, TErrorEnd> selector)
                : base(queryResult.Value)
            {
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
            private ErrorVisitor()
            {
            }

            public static ErrorVisitor<TValue, TErrorStart, TErrorEnd> Instance { get; } = new ErrorVisitor<TValue, TErrorStart, TErrorEnd>();

            public override QueryResult<TValue, TErrorEnd> Dispatch(QueryResult<TValue, TErrorStart>.Final node, Func<TErrorStart, TErrorEnd> context)
            {
                return new QueryResult<TValue, TErrorEnd>.Final();
            }

            public override QueryResult<TValue, TErrorEnd> Dispatch(QueryResult<TValue, TErrorStart>.Element node, Func<TErrorStart, TErrorEnd> context)
            {
                return new ErrorResult<TValue, TErrorStart, TErrorEnd>(node, context);
            }

            public override QueryResult<TValue, TErrorEnd> Dispatch(QueryResult<TValue, TErrorStart>.Partial node, Func<TErrorStart, TErrorEnd> context)
            {
                return new QueryResult<TValue, TErrorEnd>.Partial(context(node.Error));
            }
        }

        public static QueryResult<TValue, TErrorEnd> Error<TValue, TErrorStart, TErrorEnd>(this QueryResult<TValue, TErrorStart> queryResult, Func<TErrorStart, TErrorEnd> selector)
        {
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

            public SelectResult(QueryResult<TValueStart, TError>.Element queryResult, Func<TValueStart, TValueEnd> selector)
                : base(selector(queryResult.Value))
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
            private SelectVisitor()
            {
            }

            public static SelectVisitor<TValueStart, TValueEnd, TError> Instance { get; } = new SelectVisitor<TValueStart, TValueEnd, TError>();

            public override QueryResult<TValueEnd, TError> Dispatch(QueryResult<TValueStart, TError>.Final node, Func<TValueStart, TValueEnd> context)
            {
                return new QueryResult<TValueEnd, TError>.Final();
            }

            public override QueryResult<TValueEnd, TError> Dispatch(QueryResult<TValueStart, TError>.Element node, Func<TValueStart, TValueEnd> context)
            {
                return new SelectResult<TValueStart, TValueEnd, TError>(node, context);
            }

            public override QueryResult<TValueEnd, TError> Dispatch(QueryResult<TValueStart, TError>.Partial node, Func<TValueStart, TValueEnd> context)
            {
                return new QueryResult<TValueEnd, TError>.Partial(node.Error);
            }
        }

        public static QueryResult<TValueEnd, TError> Select<TValueStart, TValueEnd, TError>(this QueryResult<TValueStart, TError> queryResult, Func<TValueStart, TValueEnd> selector)
        {
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
