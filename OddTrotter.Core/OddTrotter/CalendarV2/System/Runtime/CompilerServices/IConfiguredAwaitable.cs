namespace System.Runtime.CompilerServices
{
    public interface IConfiguredAwaitable<out T>
    {
        ITaskAwaiter<T> GetAwaiter();
    }
}
