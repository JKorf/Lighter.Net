using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace Lighter.Net.Enums;

/// <summary>
/// Trigger status
/// </summary>
[JsonConverter(typeof(EnumConverter<TriggerStatus>))]
public enum TriggerStatus
{
    /// <summary>
    /// ["<c>na</c>"] None
    /// </summary>
    [Map("na")]
    None,
    /// <summary>
    /// ["<c>ready</c>"] Ready
    /// </summary>
    [Map("ready")]
    Ready,
    /// <summary>
    /// ["<c>mark-price</c>"] Mark Price
    /// </summary>
    [Map("mark-price")]
    MarkPrice,
    /// <summary>
    /// ["<c>twap</c>"] TWAP
    /// </summary>
    [Map("twap")]
    Twap,
    /// <summary>
    /// ["<c>parent-order</c>"] Parent Order
    /// </summary>
    [Map("parent-order")]
    ParentOrder,

}
