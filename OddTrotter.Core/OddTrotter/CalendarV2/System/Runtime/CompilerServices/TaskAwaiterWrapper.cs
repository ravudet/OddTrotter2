namespace System.Runtime.CompilerServices
{
    public sealed class TaskAwaiterWrapper<T> : ITaskAwaiter<T>
    {
        private readonly TaskAwaiter<T> taskAwaiter;

        public TaskAwaiterWrapper(TaskAwaiter<T> taskAwaiter)
        {
            this.taskAwaiter = taskAwaiter;
        }

        public bool IsCompleted
        {
            get
            {
                return this.taskAwaiter.IsCompleted;
            }
        }

        public T GetResult()
        {
            return this.taskAwaiter.GetResult();
        }

        public void OnCompleted(Action continuation)
        {
            this.taskAwaiter.OnCompleted(continuation);
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            this.taskAwaiter.UnsafeOnCompleted(continuation);
        }
    }
}
