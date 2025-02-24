namespace System.Threading.Tasks
{
    using System.Runtime.CompilerServices;

    public interface ITask<out T>
    {
        /// <inheritdoc cref="Task{TResult}.GetAwaiter"/>
        ITaskAwaiter<T> GetAwaiter(); //// TODO you don't have configureawait(false) available here...
    }
}
