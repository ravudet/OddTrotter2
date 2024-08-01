namespace OddTrotter
{
    using System;

    using global::OddTrotter.TodoList;
    using global::OddTrotter.Calendar;

    public sealed class OddTrotter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="todoList"></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="todoList"/> or <paramref name="calendarService"/> is <see langword="null"/></exception>
        public OddTrotter(TodoListService todoList, CalendarService calendarService)
        {
            if (todoList == null)
            {
                throw new ArgumentNullException(nameof(todoList));
            }

            if (calendarService == null)
            {
                throw new ArgumentNullException(nameof(calendarService));
            }

            this.TodoList = todoList;
            this.CalendarService = calendarService;
        }

        public TodoListService TodoList { get; }

        public CalendarService CalendarService { get; }
    }
}
