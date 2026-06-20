using Lighter.Net.Objects.Internal;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Lighter.Net.Objects.Models
{
    /// <summary>
    /// Account position update
    /// </summary>
    public record LighterPositionUpdate : LighterSocketUpdate
    {
        /// <summary>
        /// Positions
        /// </summary>
        [JsonPropertyName("positions")]
        public Dictionary<string, LighterPosition> Positions { get; set; } = new();
        /// <summary>
        /// Pool shares
        /// </summary>
        [JsonPropertyName("shares")]
        public LighterPoolShares[] Shares { get; set; } = [];
        /// <summary>
        /// Last funding rounds, only set the moment funding has occurred
        /// </summary>
        [JsonPropertyName("last_funding_round")]
        public Dictionary<string, decimal> LastFundingRounds { get; set; } = new();
        /// <summary>
        /// Last funding round discounts, only set the moment funding has occurred
        /// </summary>
        [JsonPropertyName("last_funding_discount")]
        public Dictionary<string, decimal> LastFundingDiscounts { get; set; } = new();
    }
}
