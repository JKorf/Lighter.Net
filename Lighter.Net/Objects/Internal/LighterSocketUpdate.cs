using System.Text.Json.Serialization;

namespace Lighter.Net.Objects.Internal
{
    /// <summary>
    /// WebSocket update
    /// </summary>
    public record LighterSocketUpdate
    {
        /// <summary>
        /// Channel
        /// </summary>
        [JsonPropertyName("channel")]
        public string Channel { get; set; } = string.Empty;
        /// <summary>
        /// Type
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;
    }
}
