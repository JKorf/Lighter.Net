using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace Lighter.Net.Enums;

/// <summary>
/// Account tier
/// </summary>
[JsonConverter(typeof(EnumConverter<AccountTier>))]
public enum AccountTier
{
    /// <summary>
    /// ["<c>standard</c>"] Standard
    /// </summary>
    [Map("standard")]
    Standard,
    /// <summary>
    /// ["<c>plus</c>"] Plus
    /// </summary>
    [Map("plus")]
    Plus,
    /// <summary>
    /// ["<c>premium</c>"] Premium
    /// </summary>
    [Map("premium")]
    Premium,
}
