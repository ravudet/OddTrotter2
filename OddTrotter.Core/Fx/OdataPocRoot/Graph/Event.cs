namespace Fx.OdataPocRoot.Graph
{
    public sealed class Event
    {
        public Event(string id, ItemBody body, DateTimeTimeZone end, bool isCancelled, ResponseStatus responseStatus, DateTimeTimeZone start, string subject, string type, string webLink)
        {
            Id = id;
            Body = body;
            End = end;
            IsCancelled = isCancelled;
            ResponseStatus = responseStatus;
            Start = start;
            Subject = subject;
            Type = type;
            WebLink = webLink;
        }

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
