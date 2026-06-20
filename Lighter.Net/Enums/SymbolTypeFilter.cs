using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace Lighter.Net.Enums;

/// <summary>
/// Symbol type filter
/// </summary>
[JsonConverter(typeof(EnumConverter<SymbolTypeFilter>))]
public enum SymbolTypeFilter
{
    /// <summary>
    /// ["<c>all</c>"] All
    /// </summary>
    [Map("all")]
    All,
    /// <summary>
    /// ["<c>spot</c>"] Spot
    /// </summary>
    [Map("spot")]
    Spot,
    /// <summary>
    /// ["<c>perp</c>"] Perp futures
    /// </summary>
    [Map("perp")]
    Perp,
}
