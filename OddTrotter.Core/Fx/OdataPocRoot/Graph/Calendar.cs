namespace Fx.OdataPocRoot.Graph
{
    using System.Collections.Generic;

    public sealed class Calendar
    {
        public Calendar(IEnumerable<Event> events)
        {
            this.Events = events;
        }

        public IEnumerable<Event> Events { get; }
    }
}
