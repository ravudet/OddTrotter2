namespace Fx.OdataPocRoot.Graph
{
    public sealed class DateTimeTimeZone
    {
        public DateTimeTimeZone(string dateTime, string timeZone)
        {
            DateTime = dateTime;
            TimeZone = timeZone;
        }

        public string DateTime { get; }

        public string TimeZone { get; }
    }
}
