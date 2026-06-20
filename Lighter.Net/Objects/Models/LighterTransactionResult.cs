using System;
using System.Text.Json.Serialization;

namespace Lighter.Net.Objects.Models
{
    /// <summary>
    /// Transaction result
    /// </summary>
    public record LighterTransactionResult : LighterResponse
    {
        /// <summary>
        /// ["<c>tx_hash</c>"] Transaction hash
        /// </summary>
        [JsonPropertyName("tx_hash")]
        public string TransactionHash { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>predicted_execution_time_ms</c>"] Predicted execution time
        /// </summary>
        [JsonPropertyName("predicted_execution_time_ms")]
        public DateTime PredictedExecutionTime { get; set; }
    }
}
