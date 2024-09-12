namespace Fx.OdataPocRoot.Graph
{
    public sealed class Event
    {
        public string Id { get; }

        public ItemBody Body { get; }

        public DateTimeTimeZone End { get; }

        public bool IsCancelled { get; }

        public ResponseStatus ResponseStatus { get; }

        public DateTimeTimeZone Start { get; }

        public string Subject { get; }

        public string Type { get; }

        public string WebLink { get; }
    }
}
