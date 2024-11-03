////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace OddTrotter.Calendar
{
    public sealed class CalendarEventContextSettings
    {
        private CalendarEventContextSettings(int pageSize)
        {
            this.PageSize = pageSize;
        }

        public static CalendarEventContextSettings Default { get; } = new CalendarEventContextSettings(50);

        public int PageSize { get; }
    }
}
