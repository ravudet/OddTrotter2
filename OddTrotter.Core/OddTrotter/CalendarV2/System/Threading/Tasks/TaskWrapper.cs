namespace System.Threading.Tasks
{
    using System.Runtime.CompilerServices;

    public sealed class TaskWrapper<T, TTaskAwaiter> : ITask<T>
    {
        private readonly Task<T> task;

        public TaskWrapper(Task<T> task)
        {
            this.task = task;
        }
        
        public TTaskAwaiter GetAwaiter()
        {
            throw new NotImplementedException();
        }
    }
}
