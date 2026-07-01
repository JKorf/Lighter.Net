using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace Lighter.Net.Enums
{
    /// <summary>
    /// Order status
    /// </summary>
    [JsonConverter(typeof(EnumConverter<OrderStatus>))]
    public enum OrderStatus
    {
        /// <summary>
        /// ["<c>in-progress</c>"] In progress
        /// </summary>
        [Map("in-progress")]
        InProgress,
        /// <summary>
        /// ["<c>pending</c>"] Pending
        /// </summary>
        [Map("pending")]
        Pending,
        /// <summary>
        /// ["<c>open</c>"] Open
        /// </summary>
        [Map("open")]
        Open,
        /// <summary>
        /// ["<c>filled</c>"] Filled
        /// </summary>
        [Map("filled")]
        Filled,
        /// <summary>
        /// ["<c>canceled</c>"] Canceled
        /// </summary>
        [Map("canceled")]
        Canceled,
        /// <summary>
        /// ["<c>canceled-post-only</c>"] Canceled post only
        /// </summary>
        [Map("canceled-post-only")]
        CanceledPostOnly,
        /// <summary>
        /// ["<c>canceled-reduce-only</c>"] Canceled reduce only
        /// </summary>
        [Map("canceled-reduce-only")]
        CanceledReduceOnly,
        /// <summary>
        /// ["<c>canceled-position-not-allowed</c>"] Canceled position not allowed
        /// </summary>
        [Map("canceled-position-not-allowed")]
        CanceledPositionNotAllowed,
        /// <summary>
        /// ["<c>canceled-margin-not-allowed</c>"] Canceled margin not allowed
        /// </summary>
        [Map("canceled-margin-not-allowed")]
        CanceledMarginNotAllowed,
        /// <summary>
        /// ["<c>canceled-too-much-slippage</c>"] Canceled too much slippage
        /// </summary>
        [Map("canceled-too-much-slippage")]
        CanceledTooMuchSlippage,
        /// <summary>
        /// ["<c>canceled-not-enough-liquidity</c>"] Canceled not enough liquidity
        /// </summary>
        [Map("canceled-not-enough-liquidity")]
        CanceledNotEnoughLiquidity,
        /// <summary>
        /// ["<c>canceled-self-trade</c>"] Canceled self trade
        /// </summary>
        [Map("canceled-self-trade")]
        CanceledSelfTrade,
        /// <summary>
        /// ["<c>canceled-expired</c>"] Canceled expired
        /// </summary>
        [Map("canceled-expired")]
        CanceledExpired,
        /// <summary>
        /// ["<c>canceled-oco</c>"] Canceled oco
        /// </summary>
        [Map("canceled-oco")]
        CanceledOco,
        /// <summary>
        /// ["<c>canceled-child</c>"] Canceled child
        /// </summary>
        [Map("canceled-child")]
        CanceledChild,
        /// <summary>
        /// ["<c>canceled-liquidation</c>"] Canceled liquidation
        /// </summary>
        [Map("canceled-liquidation")]
        CanceledLiquidation,
        /// <summary>
        /// ["<c>canceled-invalid-balance</c>"] Canceled invalid balance
        /// </summary>
        [Map("canceled-invalid-balance")]
        CanceledInvalidBalance,
    }
}