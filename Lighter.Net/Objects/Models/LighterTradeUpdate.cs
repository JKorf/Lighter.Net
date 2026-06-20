using Lighter.Net.Objects.Internal;
using System.Text.Json.Serialization;

namespace Lighter.Net.Objects.Models
{
    /// <summary>
    /// Trade update
    /// </summary>
    public record LighterTradeUpdate : LighterSocketUpdate
    {
        /// <summary>
        /// Nonce
        /// </summary>
        [JsonPropertyName("nonce")]
        public long Nonce { get; set; } = 0;
        /// <summary>
        /// Trades
        /// </summary>
        [JsonPropertyName("trades")]
        public LighterUserTrade[] Trades { get; set; } = [];
        /// <summary>
        /// Liquidation trades
        /// </summary>
        [JsonPropertyName("liquidation_trades")]
        public LighterLiquidationTrade[] LiquidationTrades { get; set; } = [];
    }
}
