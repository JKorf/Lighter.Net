using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace Lighter.Net.Enums
{
    /// <summary>
    /// Margin mode
    /// </summary>
    [JsonConverter(typeof(EnumConverter<MarginMode>))]
    public enum MarginMode
    {
        /// <summary>
        /// ["<c>0</c>"] Cross Margin
        /// </summary>
        [Map("0")]
        CrossMargin = 0,
        /// <summary>
        /// ["<c>1</c>"] Isolated Margin
        /// </summary>
        [Map("1")]
        IsolatedMargin = 1
    }
}
