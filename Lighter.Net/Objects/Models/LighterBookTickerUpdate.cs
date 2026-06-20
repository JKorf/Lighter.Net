using Lighter.Net.Objects.Internal;
using System;
using System.Text.Json.Serialization;

namespace Lighter.Net.Objects.Models
{
    /// <summary>
    /// Book ticker update
    /// </summary>
    public record LighterBookTickerUpdate : LighterSocketUpdate
    {
        /// <summary>
        /// Last update time
        /// </summary>
        [JsonPropertyName("last_updated_at")]
        public DateTime LastUpdate { get; set; }
        /// <summary>
        /// Timestamp
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Ticker
        /// </summary>
        [JsonPropertyName("ticker")]
        public LighterBookTickerUpdateData BookTicker { get; set; } = default!;
    }

    /// <summary>
    /// Ticker data
    /// </summary>
    public record LighterBookTickerUpdateData
    {
        /// <summary>
        /// Last update time
        /// </summary>
        [JsonPropertyName("last_updated_at")]
        public DateTime LastUpdate { get; set; }
        /// <summary>
        /// Symbol name
        /// </summary>
        [JsonPropertyName("s")]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// Best ask
        /// </summary>
        [JsonPropertyName("a")]
        public LighterOrderBookUpdateItem Ask { get; set; } = default!;
        /// <summary>
        /// Best bid
        /// </summary>
        [JsonPropertyName("b")]
        public LighterOrderBookUpdateItem Bid { get; set; } = default!;
    }
}
