using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace Lighter.Net.Enums;

/// <summary>
/// Symbol type
/// </summary>
[JsonConverter(typeof(EnumConverter<SymbolType>))]
public enum SymbolType
{
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
