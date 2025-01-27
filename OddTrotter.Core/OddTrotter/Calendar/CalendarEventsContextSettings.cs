////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace OddTrotter.Calendar
{
    using System;

    public sealed class CalendarEventsContextSettings
    {
        private CalendarEventsContextSettings(int pageSize, TimeSpan firstInstanceInSeriesLookahead)
        {
            this.PageSize = pageSize;
            FirstInstanceInSeriesLookahead = firstInstanceInSeriesLookahead;
        }

        public static CalendarEventsContextSettings Default { get; } = new CalendarEventsContextSettings(50, TimeSpan.FromDays(365));

        public int PageSize { get; }

        public TimeSpan FirstInstanceInSeriesLookahead { get; }

        public sealed class Builder
        {
            public int PageSize { get; set; } = CalendarEventsContextSettings.Default.PageSize;

            public TimeSpan FirstInstanceInSeriesLookahead { get; } = CalendarEventsContextSettings.Default.FirstInstanceInSeriesLookahead;

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            /// <exception cref="ArgumentOutOfRangeException">Thrown if <see cref="PageSize"/> or <see cref="FirstInstanceInSeriesLookahead"/> is not a positive value</exception>
            public CalendarEventsContextSettings Build()
            {
                if (this.PageSize <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(this.PageSize), $"'{nameof(this.PageSize)}' must be a positive value. The provided value was '{this.PageSize}'");
                }

                if (this.FirstInstanceInSeriesLookahead <= TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException(nameof(this.FirstInstanceInSeriesLookahead), $"'{nameof(this.FirstInstanceInSeriesLookahead)}' must be a prositive value. The provided value had '{nameof(TimeSpan.Ticks)}' of '{this.FirstInstanceInSeriesLookahead.Ticks}'");
                }

                return new CalendarEventsContextSettings(this.PageSize, this.FirstInstanceInSeriesLookahead);
            }
        }
    }
}
