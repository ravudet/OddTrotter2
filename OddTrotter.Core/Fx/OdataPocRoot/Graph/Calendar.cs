namespace Fx.OdataPocRoot.Graph
{
    using System.Collections.Generic;

    public sealed class Calendar
    {
        public Calendar(string id, IEnumerable<Event> events)
        {
            this.Id = id;
            this.Events = events;
        }
        
        public string Id { get; }

        public IEnumerable<Event> Events { get; }
    }
}
