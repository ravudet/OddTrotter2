namespace System.Runtime.CompilerServices
{
    public sealed class TaskAwaiterWrapper<T> : ITaskAwaiter<T>
    {
        private readonly TaskAwaiter<T> taskAwaiter;

        public TaskAwaiterWrapper(TaskAwaiter<T> taskAwaiter)
        {
            this.taskAwaiter = taskAwaiter;
        }

        public bool IsCompleted => throw new NotImplementedException();

        public T GetResult()
        {
            throw new NotImplementedException();
        }

        public void OnCompleted(Action continuation)
        {
            throw new NotImplementedException();
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            throw new NotImplementedException();
        }
    }
}
