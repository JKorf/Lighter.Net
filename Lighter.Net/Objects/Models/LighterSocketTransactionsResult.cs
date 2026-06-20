using Lighter.Net.Objects.Internal;
using System;
using System.Text.Json.Serialization;

namespace Lighter.Net.Objects.Models
{
    /// <summary>
    /// Transactions result
    /// </summary>
    public record LighterSocketTransactionsResult : LighterQueryResponse
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
