namespace OddTrotter.UserExtension
{
    using System.Text.Json.Serialization;

    public sealed class OddTrotterUserExtension
    {
        [JsonPropertyName("id")]
        public string Id { get; } = "microsoft.oddTrotter";

        [JsonRequired]
        [JsonPropertyName("data")]
        public string? Data { get; set; }
    }
}
