using System;
using System.Text.Json.Serialization;

namespace Lighter.Net.Objects.Models
{
    /// <summary>
    /// Pool shares
    /// </summary>
    public record LighterPoolShares
    {
        /// <summary>
        /// ["<c>public_pool_index</c>"] Public pool index
        /// </summary>
        [JsonPropertyName("public_pool_index")]
        public long PublicPoolIndex { get; set; }
        /// <summary>
        /// ["<c>shares_amount</c>"] Shares quantity
        /// </summary>
        [JsonPropertyName("shares_amount")]
        public decimal SharesQuantity { get; set; }
        /// <summary>
        /// ["<c>entry_usdc</c>"] Entry usdc
        /// </summary>
        [JsonPropertyName("entry_usdc")]
        public decimal EntryUsdc { get; set; }
        /// <summary>
        /// ["<c>principal_amount</c>"] Principal quantity
        /// </summary>
        [JsonPropertyName("principal_amount")]
        public decimal PrincipalQuantity { get; set; }
        /// <summary>
        /// ["<c>entry_timestamp</c>"] Entry timestamp
        /// </summary>
        [JsonPropertyName("entry_timestamp")]
        public DateTime EntryTimestamp { get; set; }
    }

}
