using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace Lighter.Net.Enums;

/// <summary>
/// Trade mode
/// </summary>
[JsonConverter(typeof(EnumConverter<AccountTradeMode>))]
public enum AccountTradeMode
{
    /// <summary>
    /// ["<c>0</c>"] Classic
    /// </summary>
    [Map("0")]
    Classic,
    /// <summary>
    /// ["<c>1</c>"] Unified
    /// </summary>
    [Map("1")]
    Unified,
}
