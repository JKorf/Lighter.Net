using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace Lighter.Net.Enums;

/// <summary>
/// Liquidation type
/// </summary>
[JsonConverter(typeof(EnumConverter<LiquidationType>))]
public enum LiquidationType
{
    /// <summary>
    /// ["<c>partial</c>"] Partial
    /// </summary>
    [Map("partial")]
    Partial,
    /// <summary>
    /// ["<c>deleverage</c>"] Deleverage
    /// </summary>
    [Map("deleverage")]
    Deleverage,
}
