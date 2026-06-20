using System.Text.Json.Serialization;

namespace Lighter.Net.Objects.Models
{
    /// <summary>
    /// Lighter response
    /// </summary>
    public record LighterResponse
    {
        /// <summary>
        /// Response code
        /// </summary>
        [JsonInclude, JsonPropertyName("code")]
        internal int Code { get; set; }
        /// <summary>
        /// Error message
        /// </summary>
        [JsonInclude, JsonPropertyName("message")]
        internal string? Message { get; set; }
    }
}
