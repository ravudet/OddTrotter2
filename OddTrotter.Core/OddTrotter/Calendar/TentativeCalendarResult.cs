namespace OddTrotter.Calendar
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class TentativeCalendarResult
    {
        public TentativeCalendarResult(IEnumerable<TentativeCalendarEvent> tentativeCalendarEvents)
        {
            if (tentativeCalendarEvents == null)
            {
                throw new ArgumentNullException(nameof(tentativeCalendarEvents));
            }

            TentativeCalendarEvents = tentativeCalendarEvents.ToList();
        }

        public IEnumerable<TentativeCalendarEvent> TentativeCalendarEvents { get; }
    }
}