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
    /// Lighter exchange data streams and requests. Exchange data endpoints include market info, order book info, and trade info
    /// </summary>
    public interface ILighterSocketClientExchangeApiExchangeData
    {
        /// <summary>
        /// Subscribe to public trade updates
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/docs/websocket-reference#trade" /><br />
        /// Endpoint:<br />
        /// WS /stream (channel: trade)
        /// </para>
        /// </summary>
        /// <param name="symbol">Symbol name, for example `ETH/USDC` for spot or `ETH` for perps</param>
        /// <param name="onMessage">The data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected and to unsubscribe</returns>
        Task<WebSocketResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(
            string symbol, Action<DataEvent<LighterTradeUpdate>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to order book updates
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/docs/websocket-reference#order-book" /><br />
        /// Endpoint:<br />
        /// WS /stream (channel: order_book)
        /// </para>
        /// </summary>
        /// <param name="symbol">Symbol name, for example `ETH/USDC` for spot or `ETH` for perps</param>
        /// <param name="onMessage">The data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected and to unsubscribe</returns>
        Task<WebSocketResult<UpdateSubscription>> SubscribeToOrderBookUpdatesAsync(
            string symbol, Action<DataEvent<LighterOrderBookUpdate>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to book ticker updates
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/docs/websocket-reference#best-bid-and-offer-bbo" /><br />
        /// Endpoint:<br />
        /// WS /stream (channel: ticker)
        /// </para>
        /// </summary>
        /// <param name="symbol">Symbol name, for example `ETH/USDC` for spot or `ETH` for perps</param>
        /// <param name="onMessage">The data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected and to unsubscribe</returns>
        Task<WebSocketResult<UpdateSubscription>> SubscribeToBookTickerUpdatesAsync(
            string symbol, Action<DataEvent<LighterBookTickerUpdate>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to ticker updates for all futures symbols
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/docs/websocket-reference#market-stats" /><br />
        /// Endpoint:<br />
        /// WS /stream (channel: market_stats)
        /// </para>
        /// </summary>
        /// <param name="onMessage">The data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected and to unsubscribe</returns>
        Task<WebSocketResult<UpdateSubscription>> SubscribeToFuturesTickerUpdatesAsync(
            Action<DataEvent<LighterAllTickerUpdate>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to ticker updates
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/docs/websocket-reference#market-stats" /><br />
        /// Endpoint:<br />
        /// WS /stream (channel: market_stats)
        /// </para>
        /// </summary>
        /// <param name="symbol">Symbol name, for example `ETH`</param>
        /// <param name="onMessage">The data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected and to unsubscribe</returns>
        Task<WebSocketResult<UpdateSubscription>> SubscribeToFuturesTickerUpdatesAsync(
            string symbol, Action<DataEvent<LighterTickerUpdate>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to ticker updates for all spot symbols
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/docs/websocket-reference#spot-market-stats" /><br />
        /// Endpoint:<br />
        /// WS /stream (channel: spot_market_stats)
        /// </para>
        /// </summary>
        /// <param name="onMessage">The data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected and to unsubscribe</returns>
        Task<WebSocketResult<UpdateSubscription>> SubscribeToSpotTickerUpdatesAsync(
            Action<DataEvent<LighterAllSpotTickerUpdate>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to spot ticker updates
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/docs/websocket-reference#spot-market-stats" /><br />
        /// Endpoint:<br />
        /// WS /stream (channel: spot_market_stats)
        /// </para>
        /// </summary>
        /// <param name="symbol">Symbol name, for example `ETH`</param>
        /// <param name="onMessage">The data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected and to unsubscribe</returns>
        Task<WebSocketResult<UpdateSubscription>> SubscribeToSpotTickerUpdatesAsync(
            string symbol, Action<DataEvent<LighterSpotTickerUpdate>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to kline/candle updates
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/docs/websocket-reference#candlesticks" /><br />
        /// Endpoint:<br />
        /// WS /stream (channel: candle)
        /// </para>
        /// </summary>
        /// <param name="symbol">Symbol name, for example `ETH/USDC` for spot or `ETH` for perps</param>
        /// <param name="interval">Kline interval</param>
        /// <param name="onMessage">The data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected and to unsubscribe</returns>
        Task<WebSocketResult<UpdateSubscription>> SubscribeToKlineUpdatesAsync(
            string symbol, KlineInterval interval, Action<DataEvent<LighterKlineUpdate>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to mark price kline/candle updates
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/docs/websocket-reference#mark-price-candlesticks" /><br />
        /// Endpoint:<br />
        /// WS /stream (channel: mark_price_candle)
        /// </para>
        /// </summary>
        /// <param name="symbol">Symbol name, for example `ETH`</param>
        /// <param name="interval">Kline interval</param>
        /// <param name="onMessage">The data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected and to unsubscribe</returns>
        Task<WebSocketResult<UpdateSubscription>> SubscribeToMarkPriceKlineUpdatesAsync(
            string symbol, KlineInterval interval, Action<DataEvent<LighterMarkPriceKlineUpdate>> onMessage, CancellationToken ct = default);
    }
}
