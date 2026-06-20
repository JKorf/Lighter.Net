using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace Lighter.Net.Enums;

/// <summary>
/// Kline interval
/// </summary>
[JsonConverter(typeof(EnumConverter<KlineInterval>))]
public enum KlineInterval
{
    /// <summary>
    /// ["<c>1m</c>"] One minute
    /// </summary>
    [Map("1m")]
    OneMinute = 60,
    /// <summary>
    /// ["<c>5m</c>"] Five minutes
    /// </summary>
    [Map("5m")]
    FiveMinutes = 60 * 5,
    /// <summary>
    /// ["<c>15m</c>"] Fifteen minutes
    /// </summary>
    [Map("15m")]
    FifteenMinutes = 60 * 15,
    /// <summary>
    /// ["<c>30m</c>"] Thirty minutes
    /// </summary>
    [Map("30m")]
    ThirtyMinutes = 60 * 30,
    /// <summary>
    /// ["<c>1h</c>"] One hours
    /// </summary>
    [Map("1h")]
    OneHour = 60 * 60,
    /// <summary>
    /// ["<c>4h</c>"] Four hours
    /// </summary>
    [Map("4h")]
    FourHours = 60 * 60 * 4,
    /// <summary>
    /// ["<c>12h</c>"] Twelve hours
    /// </summary>
    [Map("12h")]
    TwelveHours = 60 * 60 * 12,
    /// <summary>
    /// ["<c>1d</c>"] One day
    /// </summary>
    [Map("1d")]
    OneDay = 60 * 60 * 24,
}
