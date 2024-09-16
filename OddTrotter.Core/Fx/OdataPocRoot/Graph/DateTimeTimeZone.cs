namespace Fx.OdataPocRoot.Graph
{
    using Fx.OdataPocRoot.Odata;

    public sealed class DateTimeTimeZone
    {
        public DateTimeTimeZone(OdataInstanceProperty<string> dateTime, OdataInstanceProperty<string> timeZone)
        {
            DateTime = dateTime;
            TimeZone = timeZone;
        }

        [PropertyName("dateTime")]
        public OdataInstanceProperty<string> DateTime { get; }

        [PropertyName("timeZone")]
        public OdataInstanceProperty<string> TimeZone { get; }
    }
}
