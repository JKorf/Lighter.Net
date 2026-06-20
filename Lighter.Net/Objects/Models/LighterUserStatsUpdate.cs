using Lighter.Net.Enums;
using Lighter.Net.Objects.Internal;
using System;
using System.Text.Json.Serialization;

namespace Lighter.Net.Objects.Models
{
    /// <summary>
    /// User stats update
    /// </summary>
    public record LighterUserStatsUpdate : LighterSocketUpdate
    {
        /// <summary>
        /// Timestamp
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// Stats
        /// </summary>
        [JsonPropertyName("stats")]
        public LighterUserStatsUpdateData Stats { get; set; } = default!;
    }

    /// <summary>
    /// Stats update
    /// </summary>
    public record LighterUserStatsUpdateData
    {
        /// <summary>
        /// ["<c>collateral</c>"] Collateral
        /// </summary>
        [JsonPropertyName("collateral")]
        public decimal Collateral { get; set; }
        /// <summary>
        /// ["<c>portfolio_value</c>"] Portfolio value
        /// </summary>
        [JsonPropertyName("portfolio_value")]
        public decimal PortfolioValue { get; set; }
        /// <summary>
        /// ["<c>leverage</c>"] Leverage
        /// </summary>
        [JsonPropertyName("leverage")]
        public decimal Leverage { get; set; }
        /// <summary>
        /// ["<c>available_balance</c>"] Available balance
        /// </summary>
        [JsonPropertyName("available_balance")]
        public decimal AvailableBalance { get; set; }
        /// <summary>
        /// ["<c>margin_usage</c>"] Margin usage
        /// </summary>
        [JsonPropertyName("margin_usage")]
        public decimal MarginUsage { get; set; }
        /// <summary>
        /// ["<c>buying_power</c>"] Buying power
        /// </summary>
        [JsonPropertyName("buying_power")]
        public decimal BuyingPower { get; set; }
        /// <summary>
        /// ["<c>account_trading_mode</c>"] Account trading mode
        /// </summary>
        [JsonPropertyName("account_trading_mode")]
        public TradeMode TradingMode { get; set; }
        /// <summary>
        /// ["<c>cross_stats</c>"] Cross stats
        /// </summary>
        [JsonPropertyName("cross_stats")]
        public LighterUserStatsUpdateDataStats CrossStats { get; set; } = null!;
        /// <summary>
        /// ["<c>total_stats</c>"] Total stats
        /// </summary>
        [JsonPropertyName("total_stats")]
        public LighterUserStatsUpdateDataStats TotalStats { get; set; } = null!;
    }

    /// <summary>
    /// Stats
    /// </summary>
    public record LighterUserStatsUpdateDataStats
    {
        /// <summary>
        /// ["<c>collateral</c>"] Collateral
        /// </summary>
        [JsonPropertyName("collateral")]
        public decimal Collateral { get; set; }
        /// <summary>
        /// ["<c>portfolio_value</c>"] Portfolio value
        /// </summary>
        [JsonPropertyName("portfolio_value")]
        public decimal PortfolioValue { get; set; }
        /// <summary>
        /// ["<c>leverage</c>"] Leverage
        /// </summary>
        [JsonPropertyName("leverage")]
        public decimal Leverage { get; set; }
        /// <summary>
        /// ["<c>available_balance</c>"] Available balance
        /// </summary>
        [JsonPropertyName("available_balance")]
        public decimal AvailableBalance { get; set; }
        /// <summary>
        /// ["<c>margin_usage</c>"] Margin usage
        /// </summary>
        [JsonPropertyName("margin_usage")]
        public decimal MarginUsage { get; set; }
        /// <summary>
        /// ["<c>buying_power</c>"] Buying power
        /// </summary>
        [JsonPropertyName("buying_power")]
        public decimal BuyingPower { get; set; }
    }
}
