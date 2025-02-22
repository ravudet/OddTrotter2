namespace System.Threading.Tasks
{
    using System.Runtime.CompilerServices;

    public interface ITask<out T>
    {
        ITaskAwaiter<T> GetAwaiter();
    }
}
