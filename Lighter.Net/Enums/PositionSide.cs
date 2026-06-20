using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace Lighter.Net.Enums;

/// <summary>
/// Position side
/// </summary>
[JsonConverter(typeof(EnumConverter<PositionSide>))]
public enum PositionSide
{
    /// <summary>
    /// ["<c>long</c>"] Long position
    /// </summary>
    [Map("long", "1")]
    Long,
    /// <summary>
    /// ["<c>short</c>"] Short position
    /// </summary>
    [Map("short", "-1")]
    Short,
    /// <summary>
    /// ["<c>all</c>"] All positions (for filter parameters)
    /// </summary>
    [Map("all")]
    All,
}
