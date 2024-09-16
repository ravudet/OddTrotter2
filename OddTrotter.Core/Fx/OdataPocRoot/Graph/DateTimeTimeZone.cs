namespace Fx.OdataPocRoot.Graph
{
    using Fx.OdataPocRoot.Odata;

    public sealed class DateTimeTimeZone
    {
        public DateTimeTimeZone(OdataProperty<string> dateTime, OdataProperty<string> timeZone)
        {
            DateTime = dateTime;
            TimeZone = timeZone;
        }

        [PropertyName("dateTime")]
        public OdataProperty<string> DateTime { get; }

        [PropertyName("timeZone")]
        public OdataProperty<string> TimeZone { get; }
    }
}
