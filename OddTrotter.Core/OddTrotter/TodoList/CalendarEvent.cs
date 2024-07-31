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

        [JsonPropertyName("responseStatus")]
        public ResponseStatusStructure? ResponseStatus { get; set; }

        [JsonPropertyName("webLink")]
        public string? WebLink { get; set; }
    }

    public sealed class ResponseStatusStructure
    {
        [JsonPropertyName("response")]
        public string? Response { get; set; }

        [JsonPropertyName("time")]
        public string? Time { get; set; }
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