namespace OddTrotter
{
    using System;

    using global::OddTrotter.TodoList;

    public sealed class OddTrotter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="todoList"></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="todoList"/> is <see langword="null"/></exception>
        public OddTrotter(TodoListService todoList)
        {
            if (todoList == null)
            {
                throw new ArgumentNullException(nameof(todoList));
            }

            this.TodoList = todoList;
        }

        public TodoListService TodoList { get; }
    }
}
