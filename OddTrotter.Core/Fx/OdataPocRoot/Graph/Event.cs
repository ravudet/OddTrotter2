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

        [PropertyName("body")]
        public OdataProperty<ItemBody> Body { get; }

        [PropertyName("end")]
        public OdataProperty<DateTimeTimeZone> End { get; }

        [PropertyName("isCancelled")]
        public OdataProperty<bool> IsCancelled { get; }

        [PropertyName("responseStatus")]
        public OdataProperty<ResponseStatus> ResponseStatus { get; }

        [PropertyName("start")]
        public OdataProperty<DateTimeTimeZone> Start { get; }

        [PropertyName("subject")]
        public OdataProperty<string> Subject { get; }

        [PropertyName("type")]
        public OdataProperty<string> Type { get; }

        [PropertyName("webLink")]
        public OdataProperty<string> WebLink { get; }
    }
}
