using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace Lighter.Net.Enums;

/// <summary>
/// Account by
/// </summary>
[JsonConverter(typeof(EnumConverter<AccountBy>))]
public enum AccountBy
{
    /// <summary>
    /// ["<c>index</c>"] Account index
    /// </summary>
    [Map("index")]
    AccountIndex,
    /// <summary>
    /// ["<c>l1_address</c>"] Layer 1 address
    /// </summary>
    [Map("l1_address")]
    Layer1Address,
}
