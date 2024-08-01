namespace OddTrotter.Calendar
{
    public sealed class TentativeCalendarEvent
    {
        public TentativeCalendarEvent(string id, string subject, string webLink)
        {
            this.Id = id;
            this.Subject = subject;
            this.WebLink = webLink;
        }

        public string Id { get; }

        public string Subject { get; }

        public string WebLink { get; }
    }
}
