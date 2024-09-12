namespace Fx.OdataPocRoot.Graph
{
    using System.Collections.Generic;

    public sealed class Calendar
    {
        public Calendar(string id, IEnumerable<Event> events)
        {
            this.Id = id;
            this.Events = events;

            this.Foo = new DateTimeTimeZone(string.Empty, string.Empty);
        }
        
        public string Id { get; }

        public DateTimeTimeZone Foo { get; }

        public IEnumerable<Event> Events { get; }
    }
}
