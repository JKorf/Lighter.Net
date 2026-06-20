using Lighter.Net.Objects.Internal;
using System;
using System.Text.Json.Serialization;

namespace Lighter.Net.Objects.Models
{
    /// <summary>
    /// Mark price kline update
    /// </summary>
    public record LighterMarkPriceKlineUpdate : LighterSocketUpdate
    {
        /// <summary>
        /// Timestamp
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Klines
        /// </summary>
        [JsonPropertyName("candles")]
        public LighterMarkPriceKlineUpdateData[] Klines { get; set; } = [];
    }

    /// <summary>
    /// Kline data
    /// </summary>
    public record LighterMarkPriceKlineUpdateData
    {
        /// <summary>
        /// ["<c>t</c>"] Open timestamp
        /// </summary>
        [JsonPropertyName("t")]
        public DateTime OpenTime { get; set; }
        /// <summary>
        /// ["<c>o</c>"] Open price
        /// </summary>
        [JsonPropertyName("o")]
        public decimal OpenPrice { get; set; }
        /// <summary>
        /// ["<c>h</c>"] High price
        /// </summary>
        [JsonPropertyName("h")]
        public decimal HighPrice { get; set; }
        /// <summary>
        /// ["<c>l</c>"] Low price
        /// </summary>
        [JsonPropertyName("l")]
        public decimal LowPrice { get; set; }
        /// <summary>
        /// ["<c>c</c>"] Close price
        /// </summary>
        [JsonPropertyName("c")]
        public decimal ClosePrice { get; set; }
        /// <summary>
        /// ["<c>sc</c>"] Sample count
        /// </summary>
        [JsonPropertyName("sc")]
        public long SampleCount { get; set; }
    }
}
