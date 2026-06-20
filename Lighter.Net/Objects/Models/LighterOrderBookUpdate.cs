using CryptoExchange.Net.Interfaces;
using Lighter.Net.Objects.Internal;
using System;
using System.Text.Json.Serialization;

namespace Lighter.Net.Objects.Models
{
    /// <summary>
    /// Order book update
    /// </summary>
    public record LighterOrderBookUpdate : LighterSocketUpdate
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
        /// Offset
        /// </summary>
        [JsonPropertyName("offset")]
        public long Offset { get; set; }

        /// <summary>
        /// Book
        /// </summary>
        [JsonPropertyName("order_book")]
        public LighterOrderBookUpdateData OrderBook { get; set; } = default!;
    }

    /// <summary>
    /// Order book data
    /// </summary>
    public record LighterOrderBookUpdateData
    {
        /// <summary>
        /// Offset
        /// </summary>
        [JsonPropertyName("offset")]
        public long Offset { get; set; }
        /// <summary>
        /// Nonce
        /// </summary>
        [JsonPropertyName("nonce")]
        public long Nonce { get; set; }
        /// <summary>
        /// Begin nonce
        /// </summary>
        [JsonPropertyName("begin_nonce")]
        public long BeginNonce { get; set; }
        /// <summary>
        /// Last update time
        /// </summary>
        [JsonPropertyName("last_updated_at")]
        public DateTime LastUpdate { get; set; }

        /// <summary>
        /// Asks
        /// </summary>
        [JsonPropertyName("asks")]
        public LighterOrderBookUpdateItem[] Asks { get; set; } = [];
        /// <summary>
        /// Bids
        /// </summary>
        [JsonPropertyName("bids")]
        public LighterOrderBookUpdateItem[] Bids { get; set; } = [];
    }

    /// <summary>
    /// Order book entry
    /// </summary>
    public record LighterOrderBookUpdateItem : ISymbolOrderBookEntry
    {
        /// <summary>
        /// Price
        /// </summary>
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
        /// <summary>
        /// Quantity
        /// </summary>
        [JsonPropertyName("size")]
        public decimal Quantity { get; set; }
    }
}
