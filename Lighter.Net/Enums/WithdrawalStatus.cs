using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace Lighter.Net.Enums;

/// <summary>
/// Withdrawal status
/// </summary>
[JsonConverter(typeof(EnumConverter<WithdrawalStatus>))]
public enum WithdrawalStatus
{
    /// <summary>
    /// ["<c>failed</c>"] Failed
    /// </summary>
    [Map("failed")]
    Failed,
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
    /// ["<c>refunded</c>"] Refunded
    /// </summary>
    [Map("refunded")]
    Refunded,
    /// <summary>
    /// ["<c>completed</c>"] Completed
    /// </summary>
    [Map("completed")]
    Completed,
}
