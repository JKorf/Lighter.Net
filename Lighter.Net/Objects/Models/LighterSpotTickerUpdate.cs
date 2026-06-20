using Lighter.Net.Objects.Internal;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Lighter.Net.Objects.Models
{
    /// <summary>
    /// All markets ticker update
    /// </summary>
    public record LighterAllSpotTickerUpdate : LighterSocketUpdate
    {
        /// <summary>
        /// Timestamp
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Market stats dictionary, market id as key
        /// </summary>
        [JsonPropertyName("spot_market_stats")]
        public Dictionary<string, LighterSpotTickerUpdateData> Tickers { get; set; } = new Dictionary<string, LighterSpotTickerUpdateData>();
    }

    /// <summary>
    /// Book ticker update
    /// </summary>
    public record LighterSpotTickerUpdate : LighterSocketUpdate
    {
        /// <summary>
        /// Timestamp
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Ticker
        /// </summary>
        [JsonPropertyName("spot_market_stats")]
        public LighterSpotTickerUpdateData Ticker { get; set; } = default!;
    }

    /// <summary>
    /// Ticker data
    /// </summary>
    public record LighterSpotTickerUpdateData
    {
        /// <summary>
        /// ["<c>symbol</c>"] Symbol name
        /// </summary>
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>market_id</c>"] Market id
        /// </summary>
        [JsonPropertyName("market_id")]
        public long MarketId { get; set; }
        /// <summary>
        /// ["<c>index_price</c>"] Index price
        /// </summary>
        [JsonPropertyName("index_price")]
        public decimal IndexPrice { get; set; }
        /// <summary>
        /// ["<c>mid_price</c>"] Mid price
        /// </summary>
        [JsonPropertyName("mid_price")]
        public decimal? MidPrice { get; set; }
        /// <summary>
        /// ["<c>last_trade_price</c>"] Last trade price
        /// </summary>
        [JsonPropertyName("last_trade_price")]
        public decimal LastPrice { get; set; }
        /// <summary>
        /// ["<c>daily_base_token_volume</c>"] Volume last 24h in base asset
        /// </summary>
        [JsonPropertyName("daily_base_token_volume")]
        public decimal Volume { get; set; }
        /// <summary>
        /// ["<c>daily_quote_token_volume</c>"] Volume last 24h in quote asset
        /// </summary>
        [JsonPropertyName("daily_quote_token_volume")]
        public decimal QuoteVolume { get; set; }
        /// <summary>
        /// ["<c>daily_price_low</c>"] Low price 24h
        /// </summary>
        [JsonPropertyName("daily_price_low")]
        public decimal LowPrice { get; set; }
        /// <summary>
        /// ["<c>daily_price_high</c>"] High price 24h
        /// </summary>
        [JsonPropertyName("daily_price_high")]
        public decimal HighPrice { get; set; }
        /// <summary>
        /// ["<c>daily_price_change</c>"] Price change 24h
        /// </summary>
        [JsonPropertyName("daily_price_change")]
        public decimal PriceChangePercentage { get; set; }
    }
}
