using Lighter.Net.Objects.Internal;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Lighter.Net.Objects.Models
{
    /// <summary>
    /// All markets ticker update
    /// </summary>
    public record LighterAllTickerUpdate : LighterSocketUpdate
    {
        /// <summary>
        /// Timestamp
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Market stats dictionary, market id as key
        /// </summary>
        [JsonPropertyName("market_stats")]
        public Dictionary<string, LighterTickerUpdateData> Tickers { get; set; } = new Dictionary<string, LighterTickerUpdateData>();
    }

    /// <summary>
    /// Book ticker update
    /// </summary>
    public record LighterTickerUpdate : LighterSocketUpdate
    {
        /// <summary>
        /// Timestamp
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Ticker
        /// </summary>
        [JsonPropertyName("market_stats")]
        public LighterTickerUpdateData Ticker { get; set; } = default!;
    }

    /// <summary>
    /// Ticker data
    /// </summary>
    public record LighterTickerUpdateData
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
        /// ["<c>mark_price</c>"] Mark price
        /// </summary>
        [JsonPropertyName("mark_price")]
        public decimal MarkPrice { get; set; }
        /// <summary>
        /// ["<c>mid_price</c>"] Mid price
        /// </summary>
        [JsonPropertyName("mid_price")]
        public decimal? MidPrice { get; set; }
        /// <summary>
        /// ["<c>best_ask_price</c>"] Best ask price
        /// </summary>
        [JsonPropertyName("best_ask_price")]
        public decimal? BestAskPrice { get; set; }
        /// <summary>
        /// ["<c>best_bid_price</c>"] Best bid price
        /// </summary>
        [JsonPropertyName("best_bid_price")]
        public decimal? BestBidPrice { get; set; }
        /// <summary>
        /// ["<c>open_interest</c>"] Open interest
        /// </summary>
        [JsonPropertyName("open_interest")]
        public decimal OpenInterest { get; set; }
        /// <summary>
        /// ["<c>open_interest_limit</c>"] Open interest limit
        /// </summary>
        [JsonPropertyName("open_interest_limit")]
        public decimal OpenInterestLimit { get; set; }
        /// <summary>
        /// ["<c>funding_clamp_small</c>"] Funding clamp small
        /// </summary>
        [JsonPropertyName("funding_clamp_small")]
        public decimal FundingClampSmall { get; set; }
        /// <summary>
        /// ["<c>funding_clamp_big</c>"] Funding clamp big
        /// </summary>
        [JsonPropertyName("funding_clamp_big")]
        public decimal FundingClampBig { get; set; }
        /// <summary>
        /// ["<c>last_trade_price</c>"] Last trade price
        /// </summary>
        [JsonPropertyName("last_trade_price")]
        public decimal LastPrice { get; set; }
        /// <summary>
        /// ["<c>current_funding_rate</c>"] Current funding rate
        /// </summary>
        [JsonPropertyName("current_funding_rate")]
        public decimal CurrentFundingRate { get; set; }
        /// <summary>
        /// ["<c>funding_rate</c>"] Funding rate
        /// </summary>
        [JsonPropertyName("funding_rate")]
        public decimal FundingRate { get; set; }
        /// <summary>
        /// ["<c>funding_timestamp</c>"] Funding timestamp
        /// </summary>
        [JsonPropertyName("funding_timestamp")]
        public DateTime FundingTimestamp { get; set; }
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
        /// <summary>
        /// ["<c>base_interest_rate</c>"] Base interest rate
        /// </summary>
        [JsonPropertyName("base_interest_rate")]
        public decimal BaseInterestRate { get; set; }
    }
}
