////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace OddTrotter.Calendar
{
    using System;

    public sealed class CalendarEventsContextSettings
    {
        private CalendarEventsContextSettings(int pageSize)
        {
            this.PageSize = pageSize;
        }

        public static CalendarEventsContextSettings Default { get; } = new CalendarEventsContextSettings(50);

        public int PageSize { get; }

        public sealed class Builder
        {
            public int PageSize { get; set; } = CalendarEventsContextSettings.Default.PageSize;

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            /// <exception cref="ArgumentOutOfRangeException">Thrown if <see cref="PageSize"/> is not a positive value</exception>
            public CalendarEventsContextSettings Build()
            {
                if (this.PageSize <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(this.PageSize), $"{nameof(this.PageSize)} must be a positive value. The provided value was '{this.PageSize}'");
                }

                return new CalendarEventsContextSettings(this.PageSize);
            }
        }
    }
}
