namespace Fx.OdataPocRoot.Graph
{
    public sealed class DateTimeTimeZone
    {
        public DateTimeTimeZone(string dateTime, string timeZone)
        {
            DateTime = new OdataProperty<string>(dateTime);
            TimeZone = new OdataProperty<string>(timeZone);
        }

        public OdataProperty<string> DateTime { get; }

        public OdataProperty<string> TimeZone { get; }
    }
}
