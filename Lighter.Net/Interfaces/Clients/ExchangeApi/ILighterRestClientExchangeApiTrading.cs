using CryptoExchange.Net.Objects;
using Lighter.Net.Enums;
using Lighter.Net.Objects.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lighter.Net.Interfaces.Clients.ExchangeApi
{
    /// <summary>
    /// Lighter Exchange trading endpoints, placing and managing orders.
    /// </summary>
    public interface ILighterRestClientExchangeApiTrading
    {
        /// <summary>
        /// Place a new order
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/reference/sendtx" /><br />
        /// Endpoint:<br />
        /// GET /api/v1/sendTx<br />
        /// </para>
        /// </summary>
        /// <param name="symbol">Symbol name, for example `ETH/USDC` for spot or `ETH` for perps</param>
        /// <param name="side">Order side</param>
        /// <param name="orderType">Order type</param>
        /// <param name="quantity">Order quantity</param>
        /// <param name="price">Order limit price</param>
        /// <param name="timeInForce">Time in force</param>
        /// <param name="clientOrderIndex">Client order index</param>
        /// <param name="reduceOnly">Reduce only order</param>
        /// <param name="triggerPrice">Order trigger price</param>
        /// <param name="orderExpiry">Order expiry. If not provided will be set to 24h</param>
        /// <param name="nonce">The nonce, will be set automatically if not provided.</param>
        /// <param name="ct">Cancellation token</param>
        Task<HttpResult<LighterTransactionResult>> PlaceOrderAsync(
            string symbol,
            OrderSide side,
            OrderType orderType,
            decimal quantity,
            decimal price,
            TimeInForce timeInForce,
            long? clientOrderIndex = null,
            bool? reduceOnly = null,
            decimal? triggerPrice = null,
            DateTime? orderExpiry = null,
            long? nonce = null,
            CancellationToken ct = default);

        /// <summary>
        /// Place multiple orders
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/reference/sendtxbatch" /><br />
        /// Endpoint:<br />
        /// GET /api/v1/sendTxBatch<br />
        /// </para>
        /// </summary>
        /// <param name="orders">Orders</param>
        /// <param name="nonce">The nonce, will be set automatically if not provided.</param>
        /// <param name="ct">Cancellation token</param>
        Task<HttpResult<LighterTransactionsResult>> PlaceMultipleOrdersAsync(
            IEnumerable<LighterOrderRequest> orders,
            long? nonce = null,
            CancellationToken ct = default);

        /// <summary>
        /// Edit an order
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/reference/sendtx" /><br />
        /// Endpoint:<br />
        /// GET /api/v1/sendTx<br />
        /// </para>
        /// </summary>
        /// <param name="symbol">Symbol name, for example `ETH/USDC` for spot or `ETH` for perps</param>
        /// <param name="orderIndex">The order index to edit</param>
        /// <param name="quantity">New quantity</param>
        /// <param name="price">New price</param>
        /// <param name="triggerPrice">New trigger price</param>
        /// <param name="nonce">The nonce, will be set automatically if not provided.</param>
        /// <param name="ct">Cancellation token</param>
        Task<HttpResult<LighterTransactionResult>> EditOrderAsync(
            string symbol,
            long orderIndex,
            decimal quantity,
            decimal price,
            decimal? triggerPrice = null,
            long? nonce = null,
            CancellationToken ct = default);

        /// <summary>
        /// Cancel order
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/reference/sendtx" /><br />
        /// Endpoint:<br />
        /// GET /api/v1/sendTx<br />
        /// </para>
        /// </summary>
        /// <param name="symbol">Symbol name, for example `ETH/USDC` for spot or `ETH` for perps</param>
        /// <param name="orderIndex">The order index to cancel</param>
        /// <param name="nonce">The nonce, will be set automatically if not provided.</param>
        /// <param name="ct">Cancellation token</param>
        Task<HttpResult<LighterTransactionResult>> CancelOrderAsync(
            string symbol,
            long orderIndex,
            long? nonce = null,
            CancellationToken ct = default);

        /// <summary>
        /// Cancel all orders
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/reference/sendtx" /><br />
        /// Endpoint:<br />
        /// GET /api/v1/sendTx<br />
        /// </para>
        /// </summary>
        /// <param name="timeInForce">Time in force filter</param>
        /// <param name="nonce">The nonce, will be set automatically if not provided.</param>
        /// <param name="ct">Cancellation token</param>
        Task<HttpResult<LighterTransactionResult>> CancelAllOrdersAsync(
            TimeInForce timeInForce,
            long? nonce = null,
            CancellationToken ct = default);

        /// <summary>
        /// Get open orders
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/reference/accountactiveorders" /><br />
        /// Endpoint:<br />
        /// GET /api/v1/accountActiveOrders<br />
        /// </para>
        /// </summary>
        /// <param name="symbol">Filter by symbol name, for example `ETH/USDC` for spot or `ETH` for perps</param>
        /// <param name="marketType">["<c>market_type</c>"] Filter by market type</param>
        /// <param name="accountIndex">["<c>account_index</c>"] The account index. If not provided will be taken from credentials.</param>
        /// <param name="ct">Cancellation token</param>
        Task<HttpResult<LighterOrders>> GetOpenOrdersAsync(
            long? accountIndex = null,
            string? symbol = null,
            MarketType? marketType = null,
            CancellationToken ct = default);

        /// <summary>
        /// Get inactive orders
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/reference/accountinactiveorders" /><br />
        /// Endpoint:<br />
        /// GET api/v1/accountInactiveOrders<br />
        /// </para>
        /// </summary>
        /// <param name="accountIndex">["<c>account_index</c>"] Account index. If not provided will be taken from credentials</param>
        /// <param name="symbol">Symbol name, for example `ETH/USDC` for spot or `ETH` for perps</param>
        /// <param name="mode">["<c>market_type</c>"] Filter by mode</param>
        /// <param name="side">["<c>ask_filter</c>"] Filter by side</param>
        /// <param name="limit">["<c>limit</c>"] Max number of results, max 100</param>
        /// <param name="cursor">["<c>cursor</c>"] Pagination cursor</param>
        /// <param name="ct">Cancellation token</param>
        Task<HttpResult<LighterOrders>> GetClosedOrdersAsync(
            long? accountIndex = null,
            string? symbol = null,
            TradeMode? mode = null,
            OrderSide? side = null,
            //DateTime? startTime = null,
            //DateTime? endTime = null,
            int? limit = null,
            string? cursor = null,
            CancellationToken ct = default);

        /// <summary>
        /// Get user trade history
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/reference/trades" /><br />
        /// Endpoint:<br />
        /// GET api/v1/trades<br />
        /// </para>
        /// </summary>
        /// <param name="accountIndex">["<c>account_index</c>"] Account index. The provided value in credentials will be used if not provided</param>
        /// <param name="symbol">Symbol name, for example `ETH/USDC` for spot or `ETH` for perps</param>
        /// <param name="marketType">["<c>market_type</c>"] Filter by market type</param>
        /// <param name="orderIndex">["<c>order_index</c>"] Filter by order index</param>
        /// <param name="fromId">["<c>from</c>"] From id filter</param>
        /// <param name="aggregate">["<c>aggregate</c>"] Whether to aggregate trades</param>
        /// <param name="tradeType">["<c>type</c>"] Filter by trade type</param>
        /// <param name="side">["<c>ask_filter</c>"] Filter by side</param>
        /// <param name="role">["<c>role</c>"] Filter by role</param>
        /// <param name="cursor">["<c>cursor</c>"] Pagination cursor</param>
        /// <param name="sort">["<c>sort_by</c>"] Sorting order</param>
        /// <param name="limit">["<c>limit</c>"] Max number of results, max 100</param>
        /// <param name="ct">Cancellation token</param>
        Task<HttpResult<LighterUserTrades>> GetUserTradesAsync(
            long? accountIndex = null,
            string? symbol = null,
            MarketType? marketType = null,
            long? orderIndex = null,
            long? fromId = null,
            bool? aggregate = null,
            TradeType? tradeType = null,
            OrderSide? side = null,
            TradeRole? role = null,
            string? cursor = null,
            TradeSort? sort = null,
            int? limit = null,
            CancellationToken ct = default);

    }
}
