using System.Text.Json.Serialization;

namespace Lighter.Net.Objects.Internal
{
    /// <summary>
    /// WebSocket pong response
    /// </summary>
    public record LighterSocketPong
    {
        /// <summary>
        /// Type
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;
    }
}
