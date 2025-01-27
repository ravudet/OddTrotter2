////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace OddTrotter.Calendar
{
    using System;

    public sealed class CalendarEventsContextPagingException : Exception
    {
        public CalendarEventsContextPagingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
