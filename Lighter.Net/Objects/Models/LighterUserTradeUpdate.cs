using Lighter.Net.Objects.Internal;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Lighter.Net.Objects.Models
{
    /// <summary>
    /// Account user trade update
    /// </summary>
    public record LighterUserTradeUpdate : LighterSocketUpdate
    {
        /// <summary>
        /// Total volume, only set on initial update
        /// </summary>
        [JsonPropertyName("total_volume")]
        public decimal? TotalVolume { get; set; }
        /// <summary>
        /// Monthly volume, only set on initial update
        /// </summary>
        [JsonPropertyName("monthly_volume")]
        public decimal? MonthlyVolume { get; set; }
        /// <summary>
        /// Weekly volume, only set on initial update
        /// </summary>
        [JsonPropertyName("weekly_volume")]
        public decimal? WeeklyVolume { get; set; }
        /// <summary>
        /// Daily volume, only set on initial update
        /// </summary>
        [JsonPropertyName("daily_volume")]
        public decimal? DailyVolume { get; set; }
        /// <summary>
        /// Trades
        /// </summary>
        [JsonPropertyName("trades")]
        public Dictionary<string, LighterUserTrade[]> Trades { get; set; } = new();
    }
}
