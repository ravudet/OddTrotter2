namespace Fx.OdataPocRoot.Graph
{
    public sealed class DateTimeTimeZone
    {
        public DateTimeTimeZone(OdataProperty<string> dateTime, OdataProperty<string> timeZone)
        {
            DateTime = dateTime;
            TimeZone = timeZone;
        }

        public OdataProperty<string> DateTime { get; }

        public OdataProperty<string> TimeZone { get; }
    }
}
