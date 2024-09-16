using Fx.OdataPocRoot.Odata;
using System.Text.Json.Serialization;

namespace Fx.OdataPocRoot.Graph
{
    public sealed class Event
    {
        public Event(OdataProperty<string> id, OdataProperty<ItemBody> body, OdataProperty<DateTimeTimeZone> end, OdataProperty<bool> isCancelled, OdataProperty<ResponseStatus> responseStatus, OdataProperty<DateTimeTimeZone> start, OdataProperty<string> subject, OdataProperty<string> type, OdataProperty<string> webLink)
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

        [PropertyName("id")]
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
