﻿namespace System.Threading.Tasks
{
    using System.Runtime.CompilerServices;

    public sealed class TaskWrapper<T> : ITask<T>
    {
        private readonly Task<T> task;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="task"/> is <see langword="null"/></exception>
        public TaskWrapper(Task<T> task)
        {
            ArgumentNullException.ThrowIfNull(task);

            this.task = task;
        }

        public IConfiguredAwaitable<T> ConfigureAwait(bool continueOnCapturedContext)
        {
            return new ConfiguredAwaitableWrapper<T>(this.task.ConfigureAwait(false));
        }

        /// <inheritdoc/>
        public ITaskAwaiter<T> GetAwaiter()
        {
            return new TaskAwaiterWrapper<T>(this.task.GetAwaiter());
        }
    }
}
