using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace Lighter.Net.Enums;

/// <summary>
/// Market type
/// </summary>
[JsonConverter(typeof(EnumConverter<MarketType>))]
public enum MarketType
{
    /// <summary>
    /// ["<c>spot</c>"] Spot
    /// </summary>
    [Map("spot")]
    Spot,
    /// <summary>
    /// ["<c>perp</c>"] Perps
    /// </summary>
    [Map("perp")]
    Perps,
    /// <summary>
    /// ["<c>all</c>"] All
    /// </summary>
    [Map("all")]
    All,
}
