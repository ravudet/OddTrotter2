namespace System.Runtime.CompilerServices
{
    public interface ITaskAwaiter<out T> : ICriticalNotifyCompletion
    {
        bool IsCompleted { get; }

        T GetResult();
    }
}
