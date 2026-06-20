using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace Lighter.Net.Enums
{
    /// <summary>
    /// Time in force
    /// </summary>
    [JsonConverter(typeof(EnumConverter<TimeInForce>))]
    public enum TimeInForce
    {
        /// <summary>
        /// ["<c>0</c>"] Immediate or Cancel
        /// </summary>
        [Map("0", "immediate-or-cancel")]
        ImmediateOrCancel = 0,
        /// <summary>
        /// ["<c>1</c>"] Good Till Time
        /// </summary>
        [Map("1", "good-till-time")]
        GoodTillTime = 1,
        /// <summary>
        /// ["<c>2</c>"] Post Only
        /// </summary>
        [Map("2", "post-only")]
        PostOnly = 2,
    }
}
