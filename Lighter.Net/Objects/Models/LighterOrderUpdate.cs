using Lighter.Net.Objects.Internal;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Lighter.Net.Objects.Models
{
    /// <summary>
    /// Account orders update
    /// </summary>
    public record LighterOrderUpdate : LighterSocketUpdate
    {
        /// <summary>
        /// Orders
        /// </summary>
        [JsonPropertyName("orders")]
        public Dictionary<string, LighterOrder[]> Orders { get; set; } = new();
    }
}
