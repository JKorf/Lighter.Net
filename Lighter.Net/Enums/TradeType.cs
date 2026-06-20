using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace Lighter.Net.Enums;

/// <summary>
/// Trade type
/// </summary>
[JsonConverter(typeof(EnumConverter<TradeType>))]
public enum TradeType
{
    /// <summary>
    /// ["<c>trade</c>"] Trade
    /// </summary>
    [Map("trade")]
    Trade,
    /// <summary>
    /// ["<c>liquidation</c>"] Liquidation
    /// </summary>
    [Map("liquidation")]
    Liquidation,
    /// <summary>
    /// ["<c>deleverage</c>"] Deleverage
    /// </summary>
    [Map("deleverage")]
    Deleverage,
    /// <summary>
    /// ["<c>market-settlement</c>"] Market settlement
    /// </summary>
    [Map("market-settlement")]
    Settlement,
}
