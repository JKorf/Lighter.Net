using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace Lighter.Net.Enums;

/// <summary>
/// Symbol status
/// </summary>
[JsonConverter(typeof(EnumConverter<SymbolStatus>))]
public enum SymbolStatus
{
    /// <summary>
    /// ["<c>active</c>"] Active
    /// </summary>
    [Map("active")]
    Active,
    /// <summary>
    /// ["<c>inactive</c>"] Inactive
    /// </summary>
    [Map("inactive")]
    Inactive,
}
