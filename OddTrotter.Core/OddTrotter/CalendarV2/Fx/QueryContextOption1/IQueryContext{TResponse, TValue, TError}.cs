namespace Fx.QueryContextOption1
{
    using Stash;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    using Fx.QueryContext;

    public interface IQueryContext<out TResponse, out TValue, out TError> //// TODO you can't get covariance without an interface for queryresultnode
    {
        ITask<IQueryResult<TResponse, TError>> Evaluate();
    }

    public static class Play
    {
        public static async Task Do(IQueryContext<string, string, System.Exception> context)
        {
            await context.Evaluate();
        }
    }
}
