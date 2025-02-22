namespace Fx.QueryContextOption2
{
    using System.Diagnostics;
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    public interface IQueryContext<out TValue, out TError>
    {
        ITask<IQueryResult<TValue, TError>> Evaluate();
    }

    public interface ITask<out T>
    {
        //// TODO it's tempting to call this `ifuture<T>`, but don't do it if any of the `await` requirements take an implied dependency on multi-tasking
        ITaskAwaiter<T> GetAwaiter();
    }

    public interface ITaskAwaiter<out T> : ICriticalNotifyCompletion
    {
        bool IsCompleted { get; }

        T GetResult();
    }

    public static class QueryContextPlayground
    {
        public static async Task DoWork(IQueryContext<string, Exception> context)
        {
            var result = await context.Evaluate();
        }
    }
}
