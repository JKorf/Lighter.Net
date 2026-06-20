using System;
using System.Text.Json.Serialization;

namespace Lighter.Net.Objects.Models
{
    /// <summary>
    /// Lighter exchange status
    /// </summary>
    public record LighterStatus
    {
        /// <summary>
        /// Status
        /// </summary>
        [JsonPropertyName("status")]
        public int Status { get; set; } 
        /// <summary>
        /// Network id
        /// </summary>
        [JsonPropertyName("network_id")]
        public int NetworkId { get; set; }
        /// <summary>
        /// Timestamp
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
