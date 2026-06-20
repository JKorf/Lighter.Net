using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace Lighter.Net.Enums;

/// <summary>
/// Deposit filter
/// </summary>
[JsonConverter(typeof(EnumConverter<WithdrawDepositFilter>))]
public enum WithdrawDepositFilter
{
    /// <summary>
    /// ["<c>pending</c>"] Pending
    /// </summary>
    [Map("pending")]
    Pending,
    /// <summary>
    /// ["<c>claimable</c>"] Claimable
    /// </summary>
    [Map("claimable")]
    Claimable,
    /// <summary>
    /// ["<c>all</c>"] All
    /// </summary>
    [Map("all")]
    All,
}
