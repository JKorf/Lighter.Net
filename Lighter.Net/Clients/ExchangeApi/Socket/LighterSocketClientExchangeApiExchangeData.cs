using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using Lighter.Net.Enums;
using Lighter.Net.Interfaces.Clients.ExchangeApi;
using Lighter.Net.Objects.Models;
using Lighter.Net.Objects.Sockets.Subscriptions;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lighter.Net.Clients.ExchangeApi
{
    /// <inheritdoc />
    internal class LighterSocketClientExchangeApiExchangeData : ILighterSocketClientExchangeApiExchangeData
    {
        private readonly LighterSocketClientExchangeApi _baseClient;
        private readonly ILogger _logger;

        internal LighterSocketClientExchangeApiExchangeData(ILogger logger, LighterSocketClientExchangeApi baseClient)
        {
            _baseClient = baseClient;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<WebSocketResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(
            string symbol, Action<DataEvent<LighterTradeUpdate>> onMessage, CancellationToken ct = default)
        {
            var symbolInfo = await _baseClient.GetSymbolInfoAsync(symbol).ConfigureAwait(false);
            if (!symbolInfo.Success)
                return WebSocketResult.Fail<UpdateSubscription>(_baseClient.Exchange, symbolInfo.Error!);

            var internalHandler = new Action<DateTime, string?, LighterTradeUpdate>((receiveTime, originalData, data) =>
            {
                var timestamp = data.Trades.Max(x => x.Timestamp);
                _baseClient.UpdateTimeOffset(timestamp);

                onMessage(
                    new DataEvent<LighterTradeUpdate>(LighterExchange.ExchangeName, data, receiveTime, originalData)
                        .WithStreamId(data.Channel)
                        .WithSymbol(symbol)
                        .WithSequenceNumber(data.Nonce)
                        .WithUpdateType(data.Type == "subscribed/trade" ? SocketUpdateType.Snapshot : SocketUpdateType.Update)
                        .WithDataTimestamp(timestamp, _baseClient.GetTimeOffset())
                    );
            });

            var subscription = new LighterSubscription<LighterTradeUpdate>(_baseClient, _logger, "trade", $"/{symbolInfo.Data.MarketId}", internalHandler, false);
            return await _baseClient.SubscribeAsync(subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebSocketResult<UpdateSubscription>> SubscribeToOrderBookUpdatesAsync(
            string symbol, Action<DataEvent<LighterOrderBookUpdate>> onMessage, CancellationToken ct = default)
        {
            var symbolInfo = await _baseClient.GetSymbolInfoAsync(symbol).ConfigureAwait(false);
            if (!symbolInfo.Success)
                return WebSocketResult.Fail<UpdateSubscription>(_baseClient.Exchange, symbolInfo.Error!);

            var internalHandler = new Action<DateTime, string?, LighterOrderBookUpdate>((receiveTime, originalData, data) =>
            {
                _baseClient.UpdateTimeOffset(data.Timestamp);

                onMessage(
                    new DataEvent<LighterOrderBookUpdate>(LighterExchange.ExchangeName, data, receiveTime, originalData)
                        .WithStreamId(data.Channel)
                        .WithSymbol(symbol)
                        .WithSequenceNumber(data.OrderBook.Nonce)
                        .WithUpdateType(data.Type == "subscribed/order_book" ? SocketUpdateType.Snapshot : SocketUpdateType.Update)
                        .WithDataTimestamp(data.Timestamp, _baseClient.GetTimeOffset())
                    );
            });

            var subscription = new LighterSubscription<LighterOrderBookUpdate>(_baseClient, _logger, "order_book", $"/{symbolInfo.Data.MarketId}", internalHandler, false);
            return await _baseClient.SubscribeAsync(subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebSocketResult<UpdateSubscription>> SubscribeToBookTickerUpdatesAsync(
            string symbol, Action<DataEvent<LighterBookTickerUpdate>> onMessage, CancellationToken ct = default)
        {
            var symbolInfo = await _baseClient.GetSymbolInfoAsync(symbol).ConfigureAwait(false);
            if (!symbolInfo.Success)
                return WebSocketResult.Fail<UpdateSubscription>(_baseClient.Exchange, symbolInfo.Error!);

            var internalHandler = new Action<DateTime, string?, LighterBookTickerUpdate>((receiveTime, originalData, data) =>
            {
                _baseClient.UpdateTimeOffset(data.Timestamp);

                onMessage(
                    new DataEvent<LighterBookTickerUpdate>(LighterExchange.ExchangeName, data, receiveTime, originalData)
                        .WithStreamId(data.Channel)
                        .WithSymbol(symbol)
                        .WithUpdateType(data.Type == "subscribed/ticker" ? SocketUpdateType.Snapshot : SocketUpdateType.Update)
                        .WithDataTimestamp(data.Timestamp, _baseClient.GetTimeOffset())
                    );
            });

            var subscription = new LighterSubscription<LighterBookTickerUpdate>(_baseClient, _logger, "ticker", $"/{symbolInfo.Data.MarketId}", internalHandler, false);
            return await _baseClient.SubscribeAsync(subscription, ct).ConfigureAwait(false);
        }

        public async Task<WebSocketResult<UpdateSubscription>> SubscribeToFuturesTickerUpdatesAsync(
            Action<DataEvent<LighterAllTickerUpdate>> onMessage, CancellationToken ct = default)
        {
            var internalHandler = new Action<DateTime, string?, LighterAllTickerUpdate>((receiveTime, originalData, data) =>
            {
                _baseClient.UpdateTimeOffset(data.Timestamp);

                onMessage(
                    new DataEvent<LighterAllTickerUpdate>(LighterExchange.ExchangeName, data, receiveTime, originalData)
                        .WithStreamId(data.Channel)
                        .WithUpdateType(data.Type == "subscribed/market_stats" ? SocketUpdateType.Snapshot : SocketUpdateType.Update)
                        .WithDataTimestamp(data.Timestamp, _baseClient.GetTimeOffset())
                    );
            });

            var subscription = new LighterSubscription<LighterAllTickerUpdate>(_baseClient, _logger, "market_stats", $"/all", internalHandler, false);
            return await _baseClient.SubscribeAsync(subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebSocketResult<UpdateSubscription>> SubscribeToFuturesTickerUpdatesAsync(
            string symbol, Action<DataEvent<LighterTickerUpdate>> onMessage, CancellationToken ct = default)
        {
            var symbolInfo = await _baseClient.GetSymbolInfoAsync(symbol).ConfigureAwait(false);
            if (!symbolInfo.Success)
                return WebSocketResult.Fail<UpdateSubscription>(_baseClient.Exchange, symbolInfo.Error!);

            var internalHandler = new Action<DateTime, string?, LighterTickerUpdate>((receiveTime, originalData, data) =>
            {
                _baseClient.UpdateTimeOffset(data.Timestamp);

                onMessage(
                    new DataEvent<LighterTickerUpdate>(LighterExchange.ExchangeName, data, receiveTime, originalData)
                        .WithStreamId(data.Channel)
                        .WithSymbol(symbol)
                        .WithUpdateType(data.Type == "subscribed/market_stats" ? SocketUpdateType.Snapshot : SocketUpdateType.Update)
                        .WithDataTimestamp(data.Timestamp, _baseClient.GetTimeOffset())
                    );
            });

            var subscription = new LighterSubscription<LighterTickerUpdate>(_baseClient, _logger, "market_stats", $"/{symbolInfo.Data.MarketId}", internalHandler, false);
            return await _baseClient.SubscribeAsync(subscription, ct).ConfigureAwait(false);
        }

        public async Task<WebSocketResult<UpdateSubscription>> SubscribeToSpotTickerUpdatesAsync(
            Action<DataEvent<LighterAllSpotTickerUpdate>> onMessage, CancellationToken ct = default)
        {
            var internalHandler = new Action<DateTime, string?, LighterAllSpotTickerUpdate>((receiveTime, originalData, data) =>
            {
                _baseClient.UpdateTimeOffset(data.Timestamp);

                onMessage(
                    new DataEvent<LighterAllSpotTickerUpdate>(LighterExchange.ExchangeName, data, receiveTime, originalData)
                        .WithStreamId(data.Channel)
                        .WithUpdateType(data.Type == "subscribed/spot_market_stats" ? SocketUpdateType.Snapshot : SocketUpdateType.Update)
                        .WithDataTimestamp(data.Timestamp, _baseClient.GetTimeOffset())
                    );
            });

            var subscription = new LighterSubscription<LighterAllSpotTickerUpdate>(_baseClient, _logger, "spot_market_stats", $"/all", internalHandler, false);
            return await _baseClient.SubscribeAsync(subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebSocketResult<UpdateSubscription>> SubscribeToSpotTickerUpdatesAsync(
            string symbol, Action<DataEvent<LighterSpotTickerUpdate>> onMessage, CancellationToken ct = default)
        {
            var symbolInfo = await _baseClient.GetSymbolInfoAsync(symbol).ConfigureAwait(false);
            if (!symbolInfo.Success)
                return WebSocketResult.Fail<UpdateSubscription>(_baseClient.Exchange, symbolInfo.Error!);

            var internalHandler = new Action<DateTime, string?, LighterSpotTickerUpdate>((receiveTime, originalData, data) =>
            {
                _baseClient.UpdateTimeOffset(data.Timestamp);

                onMessage(
                    new DataEvent<LighterSpotTickerUpdate>(LighterExchange.ExchangeName, data, receiveTime, originalData)
                        .WithStreamId(data.Channel)
                        .WithSymbol(symbol)
                        .WithUpdateType(data.Type == "subscribed/spot_market_stats" ? SocketUpdateType.Snapshot : SocketUpdateType.Update)
                        .WithDataTimestamp(data.Timestamp, _baseClient.GetTimeOffset())
                    );
            });

            var subscription = new LighterSubscription<LighterSpotTickerUpdate>(_baseClient, _logger, "spot_market_stats", $"/{symbolInfo.Data.MarketId}", internalHandler, false);
            return await _baseClient.SubscribeAsync(subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebSocketResult<UpdateSubscription>> SubscribeToKlineUpdatesAsync(
            string symbol, KlineInterval interval, Action<DataEvent<LighterKlineUpdate>> onMessage, CancellationToken ct = default)
        {
            var symbolInfo = await _baseClient.GetSymbolInfoAsync(symbol).ConfigureAwait(false);
            if (!symbolInfo.Success)
                return WebSocketResult.Fail<UpdateSubscription>(_baseClient.Exchange, symbolInfo.Error!);

            var internalHandler = new Action<DateTime, string?, LighterKlineUpdate>((receiveTime, originalData, data) =>
            {
                _baseClient.UpdateTimeOffset(data.Timestamp);

                onMessage(
                    new DataEvent<LighterKlineUpdate>(LighterExchange.ExchangeName, data, receiveTime, originalData)
                        .WithStreamId(data.Channel)
                        .WithSymbol(symbol)
                        .WithUpdateType(data.Type == "subscribed/candle" ? SocketUpdateType.Snapshot : SocketUpdateType.Update)
                        .WithDataTimestamp(data.Timestamp, _baseClient.GetTimeOffset())
                    );
            });

            var klineIntervalStr = EnumConverter.GetString(interval);
            var subscription = new LighterSubscription<LighterKlineUpdate>(_baseClient, _logger, "candle", $"/{symbolInfo.Data.MarketId}/{klineIntervalStr}", internalHandler, false);
            return await _baseClient.SubscribeAsync(subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebSocketResult<UpdateSubscription>> SubscribeToMarkPriceKlineUpdatesAsync(
            string symbol, KlineInterval interval, Action<DataEvent<LighterMarkPriceKlineUpdate>> onMessage, CancellationToken ct = default)
        {
            var symbolInfo = await _baseClient.GetSymbolInfoAsync(symbol).ConfigureAwait(false);
            if (!symbolInfo.Success)
                return WebSocketResult.Fail<UpdateSubscription>(_baseClient.Exchange, symbolInfo.Error!);

            var internalHandler = new Action<DateTime, string?, LighterMarkPriceKlineUpdate>((receiveTime, originalData, data) =>
            {
                _baseClient.UpdateTimeOffset(data.Timestamp);

                onMessage(
                    new DataEvent<LighterMarkPriceKlineUpdate>(LighterExchange.ExchangeName, data, receiveTime, originalData)
                        .WithStreamId(data.Channel)
                        .WithSymbol(symbol)
                        .WithUpdateType(data.Type == "subscribed/mark_price_candle" ? SocketUpdateType.Snapshot : SocketUpdateType.Update)
                        .WithDataTimestamp(data.Timestamp, _baseClient.GetTimeOffset())
                    );
            });

            var klineIntervalStr = EnumConverter.GetString(interval);
            var subscription = new LighterSubscription<LighterMarkPriceKlineUpdate>(_baseClient, _logger, "mark_price_candle", $"/{symbolInfo.Data.MarketId}/{klineIntervalStr}", internalHandler, false);
            return await _baseClient.SubscribeAsync(subscription, ct).ConfigureAwait(false);
        }
    }
}
