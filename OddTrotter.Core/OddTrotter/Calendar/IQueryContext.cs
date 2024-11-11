////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace OddTrotter.Calendar
{
    using OddTrotter.TodoList;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using static System.Net.Mime.MediaTypeNames;

    public static class Either
    {
        public static Either<TLeft, TRight>.Visitor<TResult, TContext> Visitor<TLeft, TRight, TResult, TContext>(
            Func<Either<TLeft, TRight>.Left, TContext, TResult> leftDispatch,
            Func<Either<TLeft, TRight>.Right, TContext, TResult> rightDispatch)
        {
            return new DelelgateVisitor<TLeft, TRight, TResult, TContext>(leftDispatch, rightDispatch);
        }

        private sealed class DelelgateVisitor<TLeft, TRight, TResult, TContext> : Either<TLeft, TRight>.Visitor<TResult, TContext>
        {
            private readonly Func<Either<TLeft, TRight>.Left, TContext, TResult> leftDispatch;
            private readonly Func<Either<TLeft, TRight>.Right, TContext, TResult> rightDispatch;

            public DelelgateVisitor(
                Func<Either<TLeft, TRight>.Left, TContext, TResult> leftDispatch,
                Func<Either<TLeft, TRight>.Right, TContext, TResult> rightDispatch)
            {
                this.leftDispatch = leftDispatch;
                this.rightDispatch = rightDispatch;
            }

            public override TResult Dispatch(Either<TLeft, TRight>.Left node, TContext context)
            {
                return this.leftDispatch(node, context);
            }

            public override TResult Dispatch(Either<TLeft, TRight>.Right node, TContext context)
            {
                return this.rightDispatch(node, context);
            }
        }
    }

    public abstract class Either<TLeft, TRight>
    {
        private Either()
        {
        }

        protected abstract TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(Either<TLeft, TRight> node, TContext context)
            {
                return node.Accept(this, context);
            }

            public abstract TResult Dispatch(Left node, TContext context);

            public abstract TResult Dispatch(Right node, TContext context);
        }

        public sealed class Left : Either<TLeft, TRight>
        {
            public Left(TLeft value)
            {
                Value = value;
            }

            public TLeft Value { get; }

            protected override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Dispatch(this, context);
            }
        }

        public sealed class Right : Either<TLeft, TRight>
        {
            public Right(TRight value)
            {
                Value = value;
            }

            public TRight Value { get; }

            protected override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Dispatch(this, context);
            }
        }
    }

    public interface IQueryContext<TValue, TError>
    {
        Task<QueryResult<TValue, TError>> Evaluate();
    }

    public abstract class QueryResult<TValue, TError>
    {
        private QueryResult()
        {
        }

        public sealed class Final : QueryResult<TValue, TError>
        {
            public Final()
            {
            }
        }

        public sealed class Element : QueryResult<TValue, TError>
        {
            public Element(TValue value, System.Threading.Tasks.Task<QueryResult<TValue, TError>> next)
            {
                this.Value = value;
                this.Next = next;
            }

            public TValue Value { get; }

            public System.Threading.Tasks.Task<QueryResult<TValue, TError>> Next { get; } //// TODO how inefficient is it to use Task here when the element is already known?
        }

        public sealed class Partial : QueryResult<TValue, TError>
        {
            public Partial(TError error)
            {
                this.Error = error;
            }

            public TError Error { get; }
        }
    }

    public abstract class QueryResultV2<TValue, TError>
    {
        private QueryResultV2()
        {
        }

        public sealed class Final : QueryResultV2<TValue, TError>
        {
            public Final()
            {
            }
        }

        public sealed class Element : QueryResultV2<TValue, TError>
        {
            public Element(TValue value)
            {
                this.Value = value;
            }

            public TValue Value { get; }
        }

        public sealed class Partial : QueryResultV2<TValue, TError>
        {
            public Partial(TError error)
            {
                this.Error = error;
            }

            public TError Error { get; }
        }
    }

    public static class QueryResultExtensions
    {
        public static async Task<QueryResult<TValue, TError>> Where<TValue, TError>(this QueryResult<TValue, TError> queryResult, Func<TValue, bool> predicate)
        {
            if (queryResult is QueryResult<TValue, TError>.Final)
            {
                return queryResult;
            }
            else if (queryResult is QueryResult<TValue, TError>.Partial partial)
            {
                return queryResult;
            }
            else if (queryResult is QueryResult<TValue, TError>.Element element)
            {
                if (predicate(element.Value))
                {
                    return new QueryResult<TValue, TError>.Element(element.Value, Where(await element.Next.ConfigureAwait(false), predicate));
                }
                else
                {
                    return await Where(await element.Next.ConfigureAwait(false), predicate).ConfigureAwait(false);
                }
            }
            else
            {
                throw new Exception("TODO use visitor");
            }
        }

        public static QueryResult<TValue, TError> ToQueryResult<TValue, TError>(this IEnumerable<TValue> enumerable)
        {
            using (var enumerator = enumerable.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    return new QueryResult<TValue, TError>.Final();
                }

                //// TODO is this actually lazy?
                return new QueryResult<TValue, TError>.Element(enumerator.Current, ToQueryResult<TValue, TError>(enumerator));
            }
        }

        private static async Task<QueryResult<TValue, TError>> ToQueryResult<TValue, TError>(IEnumerator<TValue> enumerator)
        {
            if (!enumerator.MoveNext())
            {
                return await Task.FromResult(new QueryResult<TValue, TError>.Final());
            }

            return new QueryResult<TValue, TError>.Element(enumerator.Current, ToQueryResult<TValue, TError>(enumerator));
        }

        public static QueryResult<TValue, TError> Concat<TValue, TError>(this QueryResult<TValue, TError> first, QueryResult<TValue, TError> second)
        {
            if (first is QueryResult<TValue, TError>.Final)
            {
                return second;
            }
            else if (first is QueryResult<TValue, TError>.Partial partial)
            {
                return first;
                //// TODO
            }
            else if (first is QueryResult<TValue, TError>.Element element)
            {
                return new QueryResult<TValue, TError>.Element(element.Value, element.Next.ContinueWith(_ => _.Result.Concat(second)));
            }
            else
            {
                throw new Exception("TODO use visitor");
            }
        }

        public static async Task<QueryResult<TValueEnd, TError>> Select<TValueStart, TValueEnd, TError>(this QueryResult<TValueStart, TError> queryResult, Func<TValueStart, TValueEnd> selector)
        {
            //// TODO is it a good idea for this to be async?
            if (queryResult is QueryResult<TValueStart, TError>.Final)
            {
                return new QueryResult<TValueEnd, TError>.Final();
            }
            else if (queryResult is QueryResult<TValueStart, TError>.Partial partial)
            {
                return new QueryResult<TValueEnd, TError>.Partial(partial.Error);
            }
            else if (queryResult is QueryResult<TValueStart, TError>.Element element)
            {
                return new QueryResult<TValueEnd, TError>.Element(
                    selector(element.Value),
                    (await element.Next.ConfigureAwait(false)).Select(selector));
            }
            else
            {
                throw new Exception("TODO use visitor");
            }
        }
    }

    public static class QueryResultDriver
    {
        public static async System.Threading.Tasks.Task DoWork(QueryResult<string, System.Exception>.Element result)
        {
            var next = await result.Next;
        }
    }
}
