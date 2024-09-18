using Fx.OdataPocRoot.Odata;
using System.Text.Json.Serialization;

namespace Fx.OdataPocRoot.Graph
{
    public sealed class Event
    {
        public Event(OdataInstanceProperty<string> id, OdataInstanceProperty<ItemBody> body, OdataInstanceProperty<DateTimeTimeZone> end, OdataInstanceProperty<bool> isCancelled, OdataInstanceProperty<ResponseStatus> responseStatus, OdataInstanceProperty<DateTimeTimeZone> start, OdataInstanceProperty<string> subject, OdataInstanceProperty<string> type, OdataInstanceProperty<string> webLink, OdataInstanceProperty<NestedBool> nested)
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
            Nested = nested;
        }

        [PropertyName("id")]
        public OdataInstanceProperty<string> Id { get; }

        [PropertyName("body")]
        public OdataInstanceProperty<ItemBody> Body { get; }

        [PropertyName("end")]
        public OdataInstanceProperty<DateTimeTimeZone> End { get; }

        [PropertyName("isCancelled")]
        public OdataInstanceProperty<bool> IsCancelled { get; }

        [PropertyName("responseStatus")]
        public OdataInstanceProperty<ResponseStatus> ResponseStatus { get; }

        [PropertyName("start")]
        public OdataInstanceProperty<DateTimeTimeZone> Start { get; }

        [PropertyName("subject")]
        public OdataInstanceProperty<string> Subject { get; }

        [PropertyName("type")]
        public OdataInstanceProperty<string> Type { get; }

        [PropertyName("webLink")]
        public OdataInstanceProperty<string> WebLink { get; }

        public OdataInstanceProperty<NestedBool> Nested { get; }
    }

    public sealed class NestedBool
    {
        public NestedBool(OdataInstanceProperty<bool> prop)
        {
            Prop = prop;
        }

        public OdataInstanceProperty<bool> Prop { get; }
    }
}
