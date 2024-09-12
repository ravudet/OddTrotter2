namespace OddTrotter.Calendar
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Linq.V2;
    using System.Net.Http;
    using System.Reflection.Metadata.Ecma335;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;
    using OddTrotter.AzureBlobClient;
    using OddTrotter.GraphClient;
    using OddTrotter.TodoList;

    public sealed class ResponseStatusStructure
    {
        [JsonPropertyName("response")]
        public string? Response { get; set; }

        [JsonPropertyName("time")]
        public string? Time { get; set; }
    }

    public sealed class BodyStructure
    {
        [JsonPropertyName("content")]
        public string? Content { get; set; }
    }

    public sealed class TimeStructure
    {
        [JsonPropertyName("dateTime")]
        public DateTime? DateTime { get; set; } //// TODO can you trust this will be parsed correctly?

        [JsonPropertyName("timeZone")]
        public string? TimeZone { get; set; }
    }

    public interface IV2Queryable<out T> : IV2Enumerable<T>
    {
    }

    public delegate IQueryableMonad<TSource> Unit<TSource>(IV2Queryable<TSource> source);

    public interface IQueryableMonad<TElement> : IV2Queryable<TElement>
    {
        IV2Queryable<TElement> Source { get; }

        Unit<TSource> Unit<TSource>();
    }

    public interface IWhereQueryable<TSource> : IV2Queryable<TSource>
    {
        public IV2Queryable<TSource> Where(Expression<Func<TSource, bool>> predicate)
        {
            return this.WhereDefault(predicate);
        }
    }

    public interface IOrderByQueryable<TSource> : IV2Queryable<TSource>
    {
        public IV2Queryable<TSource> OrderBy<TKey>(Expression<Func<TSource, TKey>> keySelector)
        {
            return this.OrderByDefault(keySelector);
        }
    }

    public static class QueryableExtensions
    {
        public static IQueryableMonad<TUnit> Create<TElement, TUnit>(this IQueryableMonad<TElement> monad, IV2Queryable<TUnit> queryable)
        {
            if (monad.Source is IQueryableMonad<TElement> nestedMonad)
            {
                queryable = nestedMonad.Create(queryable);
            }

            return monad.Unit<TUnit>()(queryable);
        }

        public static IV2Queryable<TSource> Where<TSource>(this IV2Queryable<TSource> queryable, Expression<Func<TSource, bool>> predicate)
        {
            if (queryable is IWhereQueryable<TSource> where)
            {
                var whered = where.Where(predicate);
                if (queryable is IQueryableMonad<TSource> monad)
                {
                    return monad.Create(whered);
                }

                return whered;
            }

            return queryable.WhereDefault(predicate);
        }

        internal static IV2Queryable<TSource> WhereDefault<TSource>(this IV2Queryable<TSource> queryable, Expression<Func<TSource, bool>> predicate)
        {
            return new QueryableAdapter<TSource>(queryable.AsV2Enumerable().Where(predicate.Compile()));
        }

        public static IV2Queryable<TSource> OrderBy<TSource, TKey>(this IV2Queryable<TSource> queryable, Expression<Func<TSource, TKey>> keySelector)
        {
            if (queryable is IOrderByQueryable<TSource> orderBy)
            {
                var orderByed = orderBy.OrderBy(keySelector);
                if (queryable is IQueryableMonad<TSource> monad)
                {
                    return monad.Create(orderByed);
                }

                return orderByed;
            }

            return queryable.OrderByDefault(keySelector);
        }

        internal static IV2Queryable<TSource> OrderByDefault<TSource, TKey>(this IV2Queryable<TSource> queryable, Expression<Func<TSource, TKey>> keySelector)
        {
            return new QueryableAdapter<TSource>(queryable.AsV2Enumerable().OrderBy(keySelector.Compile()));
        }

        private sealed class QueryableAdapter<T> : IV2Queryable<T>
        {
            private readonly IV2Enumerable<T> enumerable;

            public QueryableAdapter(IV2Enumerable<T> enumerable)
            {
                this.enumerable = enumerable;
            }

            public IEnumerator<T> GetEnumerator()
            {
                return this.enumerable.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }
    }
}
