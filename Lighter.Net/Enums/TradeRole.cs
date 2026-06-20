using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace Lighter.Net.Enums;

/// <summary>
/// Trade role
/// </summary>
[JsonConverter(typeof(EnumConverter<TradeRole>))]
public enum TradeRole
{
    /// <summary>
    /// ["<c>maker</c>"] Maker
    /// </summary>
    [Map("maker")]
    Maker,
    /// <summary>
    /// ["<c>taker</c>"] Taker
    /// </summary>
    [Map("taker")]
    Taker,
    /// <summary>
    /// ["<c>all</c>"] All (for filtering)
    /// </summary>
    [Map("all")]
    All,
}
