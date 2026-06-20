using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace Lighter.Net.Enums;

/// <summary>
/// Enabled status
/// </summary>
[JsonConverter(typeof(EnumConverter<EnabledStatus>))]
public enum EnabledStatus
{
    /// <summary>
    /// ["<c>enabled</c>"] Enabled
    /// </summary>
    [Map("enabled")]
    Enabled,
    /// <summary>
    /// ["<c>disabled</c>"] Disabled
    /// </summary>
    [Map("disabled")]
    Disabled,
}
