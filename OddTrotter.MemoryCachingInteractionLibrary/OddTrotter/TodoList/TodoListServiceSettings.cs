namespace OddTrotter.TodoList
{
    using System;

    public sealed class TodoListServiceSettings
    {
        private TodoListServiceSettings(string todoListDataBlobName, int calendarEventPageSize)
        {
            this.TodoListDataBlobName = todoListDataBlobName;
            this.CalendarEventPageSize = calendarEventPageSize;
        }

        public static TodoListServiceSettings Default { get; } = new TodoListServiceSettings("todoListData", 50);

        public string TodoListDataBlobName { get; }

        public int CalendarEventPageSize { get; }

        public sealed class Builder
        {
            public string TodoListDataBlobName { get; set; } = TodoListServiceSettings.Default.TodoListDataBlobName;

            public int CalendarEventPageSize { get; set; } = TodoListServiceSettings.Default.CalendarEventPageSize;

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown if <see cref="TodoListDataBlobName"/> is <see langword="null"/></exception>
            /// <exception cref="ArgumentException">Thrown if <see cref="TodoListDataBlobName"/> is empty</exception>
            /// <exception cref="ArgumentOutOfRangeException">Thrown if <see cref="CalendarEventPageSize"/> is less than or equal to <see langword="0"/></exception>
            public TodoListServiceSettings Build()
            {
                if (this.TodoListDataBlobName == null)
                {
                    throw new ArgumentNullException(nameof(this.TodoListDataBlobName));
                }

                if (string.IsNullOrEmpty(this.TodoListDataBlobName))
                {
                    throw new ArgumentException($"{nameof(this.TodoListDataBlobName)} cannot be the empty string");
                }

                if (this.CalendarEventPageSize <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(this.CalendarEventPageSize), $"'{this.CalendarEventPageSize}' must be greater than 0");
                }

                return new TodoListServiceSettings(this.TodoListDataBlobName, this.CalendarEventPageSize);
            }
        }
    }
}
