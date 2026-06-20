using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using Lighter.Net.Enums;
using Lighter.Net.Objects.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lighter.Net.Interfaces.Clients.ExchangeApi
{
    /// <summary>
    /// Lighter account streams and requests. Account endpoints include balance info, withdraw/deposit info and requesting and account settings
    /// </summary>
    public interface ILighterSocketClientExchangeApiAccount
    {
        /// <summary>
        /// Subscribe to account updates
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/docs/websocket-reference#account-all" /><br />
        /// Endpoint:<br />
        /// WS /stream (channel: account_all)
        /// </para>
        /// </summary>
        /// <param name="accountIndex">The account index. Will be taken from credentials if not provided</param>
        /// <param name="onMessage">The data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected and to unsubscribe</returns>
        Task<WebSocketResult<UpdateSubscription>> SubscribeToAccountUpdatesAsync(
            long? accountIndex, Action<DataEvent<LighterAccountUpdate>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to user stats
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/docs/websocket-reference#account-stats" /><br />
        /// Endpoint:<br />
        /// WS /stream (channel: user_stats)
        /// </para>
        /// </summary>
        /// <param name="accountIndex">The account index. Will be taken from credentials if not provided</param>
        /// <param name="onMessage">The data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected and to unsubscribe</returns>
        Task<WebSocketResult<UpdateSubscription>> SubscribeToUserStatsUpdatesAsync(
            long? accountIndex, Action<DataEvent<LighterUserStatsUpdate>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to user balance updates
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/docs/websocket-reference#account-all-assets" /><br />
        /// Endpoint:<br />
        /// WS /stream (channel: account_all_assets)
        /// </para>
        /// </summary>
        /// <param name="accountIndex">The account index. Will be taken from credentials if not provided</param>
        /// <param name="onMessage">The data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected and to unsubscribe</returns>
        Task<WebSocketResult<UpdateSubscription>> SubscribeToBalanceUpdatesAsync(
            long? accountIndex, Action<DataEvent<LighterBalancesUpdate>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Set leverage for a specific symbol
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/docs/websocket-reference#send-tx" /><br />
        /// Endpoint:<br />
        /// WS /stream<br />
        /// </para>
        /// </summary>
        /// <param name="symbol">Symbol name, for example `ETH`</param>
        /// <param name="leverage">New leverage</param>
        /// <param name="marginMode">Margin mode</param>
        /// <param name="nonce">The nonce, will be set automatically if not provided.</param>
        /// <param name="ct">Cancellation token</param>
        Task<QueryResult<LighterSocketTransactionResult>> SetLeverageAsync(
            string symbol,
            int leverage,
            MarginMode marginMode,
            long? nonce = null,
            CancellationToken ct = default);

        /// <summary>
        /// Update margin for a symbol
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/docs/websocket-reference#send-tx" /><br />
        /// Endpoint:<br />
        /// WS /stream<br />
        /// </para>
        /// </summary>
        /// <param name="symbol">Symbol name, for example `ETH`</param>
        /// <param name="usdcAmount">Amount in USDC to add or remove</param>
        /// <param name="addOrRemove">True to add, false to remove</param>
        /// <param name="nonce">The nonce, will be set automatically if not provided.</param>
        /// <param name="ct">Cancellation token</param>
        Task<QueryResult<LighterSocketTransactionResult>> UpdateMarginAsync(
            string symbol,
            decimal usdcAmount,
            bool addOrRemove,
            long? nonce,
            CancellationToken ct = default);
    }
}
