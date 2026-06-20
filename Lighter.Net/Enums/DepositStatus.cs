using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace Lighter.Net.Enums;

/// <summary>
/// Deposit status
/// </summary>
[JsonConverter(typeof(EnumConverter<DepositStatus>))]
public enum DepositStatus
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
    /// ["<c>completed</c>"] Completed
    /// </summary>
    [Map("completed")]
    Completed,
    /// <summary>
    /// ["<c>claimable</c>"] Claimable
    /// </summary>
    [Map("claimable")]
    Claimable,
}
