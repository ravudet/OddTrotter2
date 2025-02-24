
namespace System.Runtime.CompilerServices
{
    public sealed class ConfiguredTaskAwaiterWrapper<T> : ITaskAwaiter<T>
    {
        private readonly ConfiguredTaskAwaitable<T>.ConfiguredTaskAwaiter configuredTaskAwaiter;

        public ConfiguredTaskAwaiterWrapper(ConfiguredTaskAwaitable<T>.ConfiguredTaskAwaiter configuredTaskAwaiter)
        {
            this.configuredTaskAwaiter = configuredTaskAwaiter;
        }

        public bool IsCompleted
        {
            get
            {
                return this.configuredTaskAwaiter.IsCompleted;
            }
        }

        public T GetResult()
        {
            return this.configuredTaskAwaiter.GetResult();
        }

        public void OnCompleted(Action continuation)
        {
            this.configuredTaskAwaiter.OnCompleted(continuation);
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            this.configuredTaskAwaiter.UnsafeOnCompleted(continuation);
        }
    }
}
