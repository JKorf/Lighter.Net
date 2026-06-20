using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace Lighter.Net.Enums;

/// <summary>
/// Account status
/// </summary>
[JsonConverter(typeof(EnumConverter<AccountStatus>))]
public enum AccountStatus
{
    /// <summary>
    /// ["<c>1</c>"] Active
    /// </summary>
    [Map("1")]
    Active,
    /// <summary>
    /// ["<c>0</c>"] Inactive
    /// </summary>
    [Map("0")]
    Inactive,
}
