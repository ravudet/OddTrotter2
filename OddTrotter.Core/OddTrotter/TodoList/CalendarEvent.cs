namespace OddTrotter.TodoList
{
    using System.Text.Json.Serialization;

    public sealed class CalendarEvent
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("subject")]
        public string? Subject { get; set; }

        [JsonPropertyName("start")]
        public TimeStructure? Start { get; set; }

        [JsonPropertyName("body")]
        public BodyStructure? Body { get; set; }
    }

    public sealed class BodyStructure
    {
        [JsonPropertyName("content")]
        public string? Content { get; set; }
    }

    public sealed class TimeStructure
    {
        [JsonPropertyName("dateTime")]
        public string? DateTime { get; set; }

        [JsonPropertyName("timeZone")]
        public string? TimeZone { get; set; }
    }
}