////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace OddTrotter.Calendar
{
    using System;

    public sealed class CalendarEventsContextTranslationException : Exception
    {
        public CalendarEventsContextTranslationException(string message)
            : base(message)
        {
        }

        public CalendarEventsContextTranslationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
