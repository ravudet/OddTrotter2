namespace Fx.QueryContextOption1
{
    using Fx.QueryContextOption1.Mixins;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    /// <summary>
    /// TODO covariance and contravariance
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TError"></typeparam>
    public interface IQueryContext<TResponse, TValue, TError>
    {
        Task<IQueryResult<TResponse, TError>> Evaluate();
    }

    public interface IQueryContext<TValue, TError> : IQueryContext<TValue, TValue, TError> //// TODO is this overload really helpful? maybe, but do you want to set a precedent that you need to do that for the mixins too? or maybe mixins are "advanced" enough that you should expect the dev to be able to handle it
    {
    }



    public static class QueryContextExtensions
    {
        public static TQueryContext Where<TQueryContext, TResponse, TValue, TError>(this TQueryContext queryContext, System.Linq.Expressions.Expression<Func<TValue, bool>> predicate)
            where TQueryContext : Mixins.IWhereQueryContextMixin<TResponse, TValue, TError, TQueryContext>
        {
            return queryContext.WhereImpl(predicate);
        }

        public sealed class Foo : IWhereQueryContextMixin<string, string, Exception, Foo>
        {
            public Task<IQueryResult<string, Exception>> Evaluate()
            {
                throw new NotImplementedException();
            }

            public Foo WhereImpl(Expression<Func<string, bool>> predicate)
            {
                throw new NotImplementedException();
            }
        }

        public interface IQueryContextMonad<TQueryContext, TResponse, TValue, TError> : IQueryContext<TResponse, TValue, TError> where TQueryContext : IQueryContext<TResponse, TValue, TError>
        {
            TQueryContext Source { get; }

            Unit<TQueryContext2, TResponse2, TValue2, TError2> Unit<TQueryContext2, TResponse2, TValue2, TError2>() where TQueryContext2 : IQueryContext<TResponse2, TValue2, TError2>;
        }

        public delegate IQueryContextMonad<TQueryContext, TResponse, TValue, TError> Unit<TQueryContext, TResponse, TValue, TError>(TQueryContext queryContext) where TQueryContext : IQueryContext<TResponse, TValue, TError>;

        public static IQueryContextMonad<TQueryContext, TResponse, TValue, TError> Where<TQueryContext, TResponse, TValue, TError>(this IQueryContextMonad<TQueryContext, TResponse, TValue, TError> monad, Expression<Func<TValue, bool>> predicate)
            //where TQueryContext : IQueryContext<TResponse, TValue, TError>
            where TQueryContext : IWhereQueryContextMixin<TResponse, TValue, TError, TQueryContext>
        {
            return monad.Unit<TQueryContext, TResponse, TValue, TError>()(monad.Source.WhereImpl(predicate));

            throw new NotImplementedException();
        }

        public sealed class Bar<TQueryContext, TResponse, TValue, TError> : IQueryContextMonad<TQueryContext, TResponse, TValue, TError>

            where TQueryContext : IQueryContext<TResponse, TValue, TError>
        {
            public Bar(TQueryContext queryContext)
            {
                this.Source = queryContext;
            }

            public TQueryContext Source { get; }

            public Task<IQueryResult<TResponse, TError>> Evaluate()
            {
                throw new NotImplementedException();
            }

            public Unit<TQueryContext2, TResponse2, TValue2, TError2> Unit<TQueryContext2, TResponse2, TValue2, TError2>() where TQueryContext2 : IQueryContext<TResponse2, TValue2, TError2>
            {
                ////return context => new Bar<TQueryContext2, TResponse2, TValue2, TError2>(context);
                return ToBar<TQueryContext2, TResponse2, TValue2, TError2>;
            }
        }

        public static Bar<TQueryContext, TResponse, TValue, TError> ToBar<TQueryContext, TResponse, TValue, TError>(this TQueryContext queryContext)
            where TQueryContext : IQueryContext<TResponse, TValue, TError>
        {
            return new Bar<TQueryContext, TResponse, TValue, TError>(queryContext);
        }

        public sealed class Fizz : IQueryContext<string, string, Exception>
        {
            public Task<IQueryResult<string, Exception>> Evaluate()
            {
                throw new NotImplementedException();
            }
        }

        public static void DoWork(Foo foo, Fizz fizz)
        {
            ////foo.Where(val => true);

            var bar = foo.ToBar<Foo, string, string, Exception>();
            var result = bar.Where(val => true).Where(val => true);

            var bar2 = fizz.ToBar<Fizz, string, string, Exception>();
            ////var result2 = bar2.Where(val => true);

            foo = foo.WhereImpl(val => true).WhereImpl(val => true);
        }
    }
}
