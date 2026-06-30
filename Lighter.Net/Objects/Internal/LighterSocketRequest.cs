using System.Text.Json.Serialization;

namespace Lighter.Net.Objects.Internal
{
    internal record LighterSocketRequest
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;
        [JsonPropertyName("channel")]
        public string Channel { get; set; } = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), JsonPropertyName("auth")]
        public string? Auth { get; set; }
    }
}
