using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace Lighter.Net.Enums;

/// <summary>
/// Trading mode
/// </summary>
[JsonConverter(typeof(EnumConverter<TradeMode>))]
public enum TradeMode
{
    /// <summary>
    /// ["<c>1</c>"] Unified
    /// </summary>
    [Map("1")]
    Unified,
    /// <summary>
    /// ["<c>0</c>"] Classic
    /// </summary>
    [Map("0")]
    Classic,
}
