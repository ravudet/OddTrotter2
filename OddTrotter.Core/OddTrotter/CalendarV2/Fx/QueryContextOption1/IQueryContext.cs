namespace Fx.QueryContextOption1
{
    using Fx.QueryContextOption1.Mixins;
    using Stash;
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
        /*public static TQueryContext Where<TQueryContext, TResponse, TValue, TError>(this TQueryContext queryContext, System.Linq.Expressions.Expression<Func<TValue, bool>> predicate)
            where TQueryContext : Mixins.IWhereQueryContextMixin<TResponse, TValue, TError, TQueryContext>
        {
            return queryContext.Where(predicate);
        }*/

        public sealed class Foo : IWhereQueryContextMixin<string, string, Exception, Foo>
        {
            public Task<IQueryResult<string, Exception>> Evaluate()
            {
                throw new NotImplementedException();
            }

            public Foo Where(Expression<Func<string, bool>> predicate)
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
            return monad.Unit<TQueryContext, TResponse, TValue, TError>()(monad.Source.Where(predicate));

            throw new NotImplementedException();
        }

        public sealed class Bar<TQueryContext, TResponse, TValue, TError> : IQueryContextMonad<TQueryContext, TResponse, TValue, TError>, IWhereQueryContextMixin<TResponse, TValue, TError, Bar<TQueryContext, TResponse, TValue, TError>>

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

            public Bar<TQueryContext, TResponse, TValue, TError> Where(Expression<Func<TValue, bool>> predicate)
            {
                throw new NotImplementedException();
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

            foo = foo.Where(val => true).Where(val => true);
        }

        public sealed class Buzz : IWhereQueryContextMixin<string, string, Exception, Buzz>, IHasTypeParameters<string, string, Exception>
        {
            public ITypeParameters<string, string, Exception> TypeParameters => throw new NotImplementedException();

            public Task<IQueryResult<string, Exception>> Evaluate()
            {
                throw new NotImplementedException();
            }

            public Buzz Where(Expression<Func<string, bool>> predicate)
            {
                throw new NotImplementedException();
            }
        }

        public static void TypeParametersDriver(Foo foo, Buzz buzz)
        {
            var bar = foo.ToBar(Of.Type<string>(), Of.Type<string>(), Of.Type<Exception>());

            var bar2 = buzz.ToBar(buzz.TypeParameters);

        }

        public static Bar<TQueryContext, TResponse, TValue, TError> ToBar<TQueryContext, TResponse, TValue, TError>(this TQueryContext queryContext, ITypeParameters<TResponse, TValue, TError> typeParameters)
            where TQueryContext : IQueryContext<TResponse, TValue, TError>
        {
            return ToBar(queryContext, typeParameters.Type1, typeParameters.Type2, typeParameters.Type3);
        }

        public static Bar<TQueryContext, TResponse, TValue, TError> ToBar<TQueryContext, TResponse, TValue, TError>(this TQueryContext queryContext, Type<TResponse> response, Type<TValue> value, Type<TError> error)
            where TQueryContext : IQueryContext<TResponse, TValue, TError>
        {
            return ToBar<TQueryContext, TResponse, TValue, TError>(queryContext);
        }

        public interface IHasTypeParameters<T1, T2, T3>
        {
            ITypeParameters<T1, T2, T3> TypeParameters { get; }
        }

        public interface ITypeParameters<T1, T2, T3>
        {
            Type<T1> Type1 { get; }
            Type<T2> Type2 { get; }
            Type<T3> Type3 { get; }
        }

        public static class Of
        {
            public static Type<T> Type<T>()
            {
                return QueryContextExtensions.Type<T>.Instance;
            }
        }

        public sealed class Type<T>
        {
            private Type()
            {
            }

            public static Type<T> Instance { get; } = new Type<T>();
        }
    }
}
