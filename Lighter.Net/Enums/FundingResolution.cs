using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace Lighter.Net.Enums;

/// <summary>
/// Funding resolution
/// </summary>
[JsonConverter(typeof(EnumConverter<FundingResolution>))]
public enum FundingResolution
{
    /// <summary>
    /// ["<c>1h</c>"] One hour
    /// </summary>
    [Map("1h")]
    OneHour,
    /// <summary>
    /// ["<c>1d</c>"] One day
    /// </summary>
    [Map("1d")]
    OneDay,
}
