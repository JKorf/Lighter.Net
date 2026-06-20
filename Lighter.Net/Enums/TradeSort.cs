using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace Lighter.Net.Enums;

/// <summary>
/// Trade sort options
/// </summary>
[JsonConverter(typeof(EnumConverter<TradeSort>))]
public enum TradeSort
{
    /// <summary>
    /// ["<c>block_height</c>"] Sort by block number
    /// </summary>
    [Map("block_height")]
    BlockHeight,
    /// <summary>
    /// ["<c>timestamp</c>"] Sort by timestamp
    /// </summary>
    [Map("timestamp")]
    Timestamp,
    /// <summary>
    /// ["<c>trade_id</c>"] Sort by trade id
    /// </summary>
    [Map("trade_id")]
    TradeId,
}
