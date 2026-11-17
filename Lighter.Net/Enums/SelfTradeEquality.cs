using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace Lighter.Net.Enums;

/// <summary>
/// Self trade equality
/// </summary>
[JsonConverter(typeof(EnumConverter<SelfTradeEquality>))]
public enum SelfTradeEquality
{
    /// <summary>
    /// ["<c>0</c>"] Check equality on account index
    /// </summary>
    [Map("0")]
    AccountIndex,
    /// <summary>
    /// ["<c>1</c>"] Check equality on master account index
    /// </summary>
    [Map("1")]
    MasterAccountIndex,
}
