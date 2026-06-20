using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace Lighter.Net.Enums;

/// <summary>
/// Withdraw type
/// </summary>
[JsonConverter(typeof(EnumConverter<WithdrawType>))]
public enum WithdrawType
{
    /// <summary>
    /// ["<c>secure</c>"] Secure
    /// </summary>
    [Map("secure")]
    Secure,
    /// <summary>
    /// ["<c>fast</c>"] Fast
    /// </summary>
    [Map("fast")]
    Fast,
}
