using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace Lighter.Net.Enums
{
    /// <summary>
    /// Order type
    /// </summary>
    [JsonConverter(typeof(EnumConverter<OrderType>))]
    public enum OrderType
    {
        /// <summary>
        /// ["<c>0</c>"] Limit
        /// </summary>
        [Map("0", "limit")]
        Limit = 0,
        /// <summary>
        /// ["<c>1</c>"] Market
        /// </summary>
        [Map("1", "market")]
        Market = 1,
        /// <summary>
        /// ["<c>2</c>"] Stop Loss
        /// </summary>
        [Map("2", "stop-loss")]
        StopLoss = 2,
        /// <summary>
        /// ["<c>3</c>"] Stop Loss Limit
        /// </summary>
        [Map("3", "stop-loss-limit")]
        StopLossLimit = 3,
        /// <summary>
        /// ["<c>4</c>"] Take Profit
        /// </summary>
        [Map("4", "take-profit")]
        TakeProfit = 4,
        /// <summary>
        /// ["<c>5</c>"] Take Profit Limit
        /// </summary>
        [Map("5", "take-profit-limit")]
        TakeProfitLimit = 5,
        /// <summary>
        /// ["<c>6</c>"] Time weighted average price (TWAP)
        /// </summary>
        [Map("6", "twap")]
        Twap = 6,
        /// <summary>
        /// ["<c>twap-sub</c>"] TWAP Sub Order
        /// </summary>
        [Map("twap-sub")]
        TwapSub,
        /// <summary>
        /// ["<c>liquidation</c>"] Liquidation
        /// </summary>
        [Map("liquidation")]
        Liquidation
    }
}
