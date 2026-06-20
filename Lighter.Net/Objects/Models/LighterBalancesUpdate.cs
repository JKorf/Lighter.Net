using Lighter.Net.Objects.Internal;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Lighter.Net.Objects.Models
{
    /// <summary>
    /// Balances update
    /// </summary>
    public record LighterBalancesUpdate : LighterSocketUpdate
    {
        /// <summary>
        /// Timestamp
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Assets dictionary, asset id as key
        /// </summary>
        [JsonPropertyName("assets")]
        public Dictionary<string, LighterAccountBalance> Balances { get; set; } = new Dictionary<string, LighterAccountBalance>();
    }
}
