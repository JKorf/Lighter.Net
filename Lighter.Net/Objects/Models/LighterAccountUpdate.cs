using CryptoExchange.Net.Converters.SystemTextJson;
using Lighter.Net.Objects.Internal;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Lighter.Net.Objects.Models
{
    /// <summary>
    /// Account update
    /// </summary>
    public record LighterAccountUpdate : LighterSocketUpdate
    {
        /// <summary>
        /// ["<c>account</c>"] Account
        /// </summary>
        [JsonPropertyName("account")]
        public long Account { get; set; }
        /// <summary>
        /// ["<c>assets</c>"] Balances
        /// </summary>
        [JsonPropertyName("assets")]
        public Dictionary<string, LighterAccountBalance>? Balances { get; set; } = new();
        /// <summary>
        /// ["<c>daily_trades_count</c>"] Daily trades count
        /// </summary>
        [JsonPropertyName("daily_trades_count")]
        public int? DailyTradesCount { get; set; }
        /// <summary>
        /// ["<c>daily_volume</c>"] Daily volume
        /// </summary>
        [JsonPropertyName("daily_volume")]
        public decimal? DailyVolume { get; set; }
        /// <summary>
        /// ["<c>weekly_trades_count</c>"] Weekly trades count
        /// </summary>
        [JsonPropertyName("weekly_trades_count")]
        public int? WeeklyTradesCount { get; set; }
        /// <summary>
        /// ["<c>weekly_volume</c>"] Weekly volume
        /// </summary>
        [JsonPropertyName("weekly_volume")]
        public decimal? WeeklyVolume { get; set; }
        /// <summary>
        /// ["<c>monthly_trades_count</c>"] Monthly trades count
        /// </summary>
        [JsonPropertyName("monthly_trades_count")]
        public int? MonthlyTradesCount { get; set; }
        /// <summary>
        /// ["<c>monthly_volume</c>"] Monthly volume
        /// </summary>
        [JsonPropertyName("monthly_volume")]
        public decimal? MonthlyVolume { get; set; }
        /// <summary>
        /// ["<c>total_trades_count</c>"] Total trades count
        /// </summary>
        [JsonPropertyName("total_trades_count")]
        public int? TotalTradesCount { get; set; }
        /// <summary>
        /// ["<c>total_volume</c>"] Total volume
        /// </summary>
        [JsonPropertyName("total_volume")]
        public decimal? TotalVolume { get; set; }
        /// <summary>
        /// ["<c>funding_histories</c>"] Funding histories
        /// </summary>
        [JsonPropertyName("funding_histories")]
        [JsonConverter(typeof(ObjectOrArrayConverter))]
        public Dictionary<string, LighterFundingHistoryItem[]> FundingHistories { get; set; } = new();
        /// <summary>
        /// ["<c>positions</c>"] Positions
        /// </summary>
        [JsonPropertyName("positions")]
        public Dictionary<string, LighterPosition> Positions { get; set; } = null!;
        /// <summary>
        /// ["<c>trades</c>"] Trades
        /// </summary>
        [JsonPropertyName("trades")]
        public Dictionary<string, LighterUserTrade[]> Trades { get; set; } = null!;
    }
}
