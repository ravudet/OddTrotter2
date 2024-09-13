namespace Fx.OdataPocRoot.Graph
{
    public sealed class Event
    {
        public Event(string id, ItemBody body, DateTimeTimeZone end, bool isCancelled, ResponseStatus responseStatus, DateTimeTimeZone start, string subject, string type, string webLink)
        {
            Id = new OdataProperty<string>(id);
            Body = new OdataProperty<ItemBody>(body);
            End = new OdataProperty<DateTimeTimeZone>(end);
            IsCancelled = new OdataProperty<bool>(isCancelled);
            ResponseStatus = new OdataProperty<ResponseStatus>(responseStatus);
            Start = new OdataProperty<DateTimeTimeZone>(start);
            Subject = new OdataProperty<string>(subject);
            Type = new OdataProperty<string>(type);
            WebLink = new OdataProperty<string>(webLink);
        }

        public OdataProperty<string> Id { get; }

        public OdataProperty<ItemBody> Body { get; }

        public OdataProperty<DateTimeTimeZone> End { get; }

        public OdataProperty<bool> IsCancelled { get; }

        public OdataProperty<ResponseStatus> ResponseStatus { get; }

        public OdataProperty<DateTimeTimeZone> Start { get; }

        public OdataProperty<string> Subject { get; }

        public OdataProperty<string> Type { get; }

        public OdataProperty<string> WebLink { get; }
    }
}
