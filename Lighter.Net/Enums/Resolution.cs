using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace Lighter.Net.Enums;

/// <summary>
/// Resolution
/// </summary>
[JsonConverter(typeof(EnumConverter<Resolution>))]
public enum Resolution
{
    /// <summary>
    /// ["<c>1m</c>"] One minute
    /// </summary>
    [Map("1m")]
    OneMinute,
    /// <summary>
    /// ["<c>5m</c>"] Five minutes
    /// </summary>
    [Map("5m")]
    FiveMinutes,
    /// <summary>
    /// ["<c>15m</c>"] Fifteen minutes
    /// </summary>
    [Map("15m")]
    FifteenMinutes,
    /// <summary>
    /// ["<c>1h</c>"] One hour
    /// </summary>
    [Map("1h")]
    OneHour,
    /// <summary>
    /// ["<c>4h</c>"] Four hours
    /// </summary>
    [Map("4h")]
    FourHours,
    /// <summary>
    /// ["<c>1d</c>"] One day
    /// </summary>
    [Map("1d")]
    OneDay,
}
