using System;
using System.Text.Json.Serialization;

namespace Lighter.Net.Objects.Models
{
    /// <summary>
    /// Transaction result
    /// </summary>
    public record LighterTransactionsResult : LighterResponse
    {
        /// <summary>
        /// ["<c>tx_hash</c>"] Transaction hashes
        /// </summary>
        [JsonPropertyName("tx_hash")]
        public string[] TransactionHashes { get; set; } = [];
        /// <summary>
        /// ["<c>predicted_execution_time_ms</c>"] Predicted execution time
        /// </summary>
        [JsonPropertyName("predicted_execution_time_ms")]
        public DateTime PredictedExecutionTime { get; set; }
    }
}
