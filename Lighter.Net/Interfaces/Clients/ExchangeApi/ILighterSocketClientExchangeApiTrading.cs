using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using Lighter.Net.Enums;
using Lighter.Net.Objects.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lighter.Net.Interfaces.Clients.ExchangeApi
{
    /// <summary>
    /// Lighter trading streams and requests. Trading endpoints include order placement, order cancellation, and trade execution
    /// </summary>
    public interface ILighterSocketClientExchangeApiTrading
    {
        /// <summary>
        /// Subscribe to user order updates
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/docs/websocket-reference#account-all-orders" /><br />
        /// Endpoint:<br />
        /// WS /stream (channel: account_all_orders)
        /// </para>
        /// </summary>
        /// <param name="accountIndex">The account index. Will be taken from credentials if not provided</param>
        /// <param name="onMessage">The data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected and to unsubscribe</returns>
        Task<WebSocketResult<UpdateSubscription>> SubscribeToOrderUpdatesAsync(
            long? accountIndex, Action<DataEvent<LighterOrderUpdate>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to user trade updates
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/docs/websocket-reference#account-all-trades" /><br />
        /// Endpoint:<br />
        /// WS /stream (channel: account_all_trades)
        /// </para>
        /// </summary>
        /// <param name="accountIndex">The account index. Will be taken from credentials if not provided</param>
        /// <param name="onMessage">The data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected and to unsubscribe</returns>
        Task<WebSocketResult<UpdateSubscription>> SubscribeToUserTradeUpdatesAsync(
            long? accountIndex, Action<DataEvent<LighterUserTradeUpdate>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to position updates
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/docs/websocket-reference#account-all-positions" /><br />
        /// Endpoint:<br />
        /// WS /stream (channel: account_all_positions)
        /// </para>
        /// </summary>
        /// <param name="accountIndex">The account index. Will be taken from credentials if not provided</param>
        /// <param name="onMessage">The data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected and to unsubscribe</returns>
        Task<WebSocketResult<UpdateSubscription>> SubscribeToPositionUpdatesAsync(
            long? accountIndex, Action<DataEvent<LighterPositionUpdate>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Place a new order
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/docs/websocket-reference#send-tx" /><br />
        /// Endpoint:<br />
        /// WS /stream<br />
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
        Task<QueryResult<LighterSocketTransactionResult>> PlaceOrderAsync(
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
        /// <a href="https://apidocs.lighter.xyz/docs/websocket-reference#send-batch-tx" /><br />
        /// Endpoint:<br />
        /// WS /streamBatch<br />
        /// </para>
        /// </summary>
        /// <param name="orders">Orders</param>
        /// <param name="nonce">The nonce, will be set automatically if not provided.</param>
        /// <param name="ct">Cancellation token</param>
        Task<QueryResult<LighterSocketTransactionsResult>> PlaceMultipleOrdersAsync(
            IEnumerable<LighterOrderRequest> orders,
            long? nonce = null,
            CancellationToken ct = default);

        /// <summary>
        /// Edit an order
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/docs/websocket-reference#send-tx" /><br />
        /// Endpoint:<br />
        /// WS /stream<br />
        /// </para>
        /// </summary>
        /// <param name="symbol">Symbol name, for example `ETH/USDC` for spot or `ETH` for perps</param>
        /// <param name="orderIndex">The order index to edit</param>
        /// <param name="quantity">New quantity</param>
        /// <param name="price">New price</param>
        /// <param name="triggerPrice">New trigger price</param>
        /// <param name="nonce">The nonce, will be set automatically if not provided.</param>
        /// <param name="ct">Cancellation token</param>
        Task<QueryResult<LighterSocketTransactionResult>> EditOrderAsync(
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
        /// <a href="https://apidocs.lighter.xyz/docs/websocket-reference#send-tx" /><br />
        /// Endpoint:<br />
        /// WS /stream<br />
        /// </para>
        /// </summary>
        /// <param name="symbol">Symbol name, for example `ETH/USDC` for spot or `ETH` for perps</param>
        /// <param name="orderIndex">The order index to cancel</param>
        /// <param name="nonce">The nonce, will be set automatically if not provided.</param>
        /// <param name="ct">Cancellation token</param>
        Task<QueryResult<LighterSocketTransactionResult>> CancelOrderAsync(
            string symbol,
            long orderIndex,
            long? nonce = null,
            CancellationToken ct = default);

        /// <summary>
        /// Cancel all orders
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/docs/websocket-reference#send-tx" /><br />
        /// Endpoint:<br />
        /// WS /stream<br />
        /// </para>
        /// </summary>
        /// <param name="timeInForce">Time in force filter</param>
        /// <param name="nonce">The nonce, will be set automatically if not provided.</param>
        /// <param name="ct">Cancellation token</param>
        Task<QueryResult<LighterSocketTransactionResult>> CancelAllOrdersAsync(
            TimeInForce timeInForce,
            long? nonce = null,
            CancellationToken ct = default);
    }
}
