using CryptoExchange.Net;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Errors;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.SharedApis;
using Lighter.Net.Enums;
using Lighter.Net.Interfaces.Clients.ExchangeApi;
using Lighter.Net.Objects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lighter.Net.Clients.ExchangeApi
{
    internal partial class LighterSocketClientExchangeSharedApi
    {
        #region Subscribe All Tickers

        async Task<WebSocketResult<UpdateSubscription>> ISubscribeAllTickersSocket.SubscribeToAllTickersUpdatesAsync(SubscribeAllTickersRequest request, Action<DataEvent<SharedTicker[]>> handler, CancellationToken ct)
            => await SubscribeToAllTickersUpdatesAsync(request, x => handler(x.ToType<SharedTicker[]>(x.Data)), ct).ConfigureAwait(false);

        public SubscribeTickersOptions SubscribeAllTickersOptions { get; } = new SubscribeTickersOptions(_exchangeName);
        public async Task<WebSocketResult<UpdateSubscription>> SubscribeToAllTickersUpdatesAsync(SubscribeAllTickersRequest request, Action<DataEvent<SharedSpotTicker[]>> handler, CancellationToken ct)
        {
            var validationError = SubscribeAllTickersOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            if (request.TradingMode == null || request.TradingMode == TradingMode.Spot)
            {
                var result = await _api.ExchangeData.SubscribeToSpotTickerUpdatesAsync(update => handler(update.ToType(
                    update.Data.Tickers.Values.Select(x =>
                    new SharedSpotTicker(
                        ExchangeSymbolCache.ParseSymbol(_topicSpotId, _api.EnvironmentName, null, x.Symbol),
                        x.Symbol,
                        x.LastPrice,
                        x.HighPrice,
                        x.LowPrice,
                        new SharedOrderQuantity(x.Volume, x.QuoteVolume),
                        x.PriceChangePercentage)
                    {
                    }).ToArray())), ct).ConfigureAwait(false);
                return result;
            }
            else
            {
                var result = await _api.ExchangeData.SubscribeToFuturesTickerUpdatesAsync(update => handler(update.ToType(
                    update.Data.Tickers.Values.Select(x =>
                    new SharedSpotTicker(
                        ExchangeSymbolCache.ParseSymbol(_topicFuturesId, _api.EnvironmentName, null, x.Symbol),
                        x.Symbol,
                        x.LastPrice,
                        x.HighPrice,
                        x.LowPrice,
                        new SharedOrderQuantity(x.Volume, x.QuoteVolume),
                        x.PriceChangePercentage)
                    {
                    }).ToArray())), ct).ConfigureAwait(false);
                return result;
            }
        }

        #endregion

        #region Subscribe Ticker

        async Task<WebSocketResult<UpdateSubscription>> ISubscribeTickerSocket.SubscribeToTickerUpdatesAsync(SubscribeTickerRequest request, Action<DataEvent<SharedTicker>> handler, CancellationToken ct)
            => await SubscribeToTickerUpdatesAsync(request, x => handler(x.ToType<SharedTicker>(x.Data)), ct).ConfigureAwait(false);

        public SubscribeTickerOptions SubscribeTickerOptions { get; } = new SubscribeTickerOptions(_exchangeName);
        public async Task<WebSocketResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(SubscribeTickerRequest request, Action<DataEvent<SharedSpotTicker>> handler, CancellationToken ct)
        {
            var validationError = SubscribeTickerOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            if (request.Symbol!.TradingMode == TradingMode.Spot)
            {
                var result = await _api.ExchangeData.SubscribeToSpotTickerUpdatesAsync(symbol, update => handler(update.ToType(
                    new SharedSpotTicker(
                        ExchangeSymbolCache.ParseSymbol(_topicSpotId, _api.EnvironmentName, null, update.Data.Ticker.Symbol),
                        update.Data.Ticker.Symbol, 
                        update.Data.Ticker.LastPrice,
                        update.Data.Ticker.HighPrice,
                        update.Data.Ticker.LowPrice,
                        new SharedOrderQuantity(update.Data.Ticker.Volume, update.Data.Ticker.QuoteVolume),
                        update.Data.Ticker.PriceChangePercentage)
                {
                })), ct).ConfigureAwait(false);
                return result;
            }
            else
            {
                var result = await _api.ExchangeData.SubscribeToFuturesTickerUpdatesAsync(symbol, update => handler(update.ToType(
                    new SharedSpotTicker(
                        ExchangeSymbolCache.ParseSymbol(_topicSpotId, _api.EnvironmentName, null, update.Data.Ticker.Symbol),
                        update.Data.Ticker.Symbol,
                        update.Data.Ticker.LastPrice,
                        update.Data.Ticker.HighPrice,
                        update.Data.Ticker.LowPrice,
                        new SharedOrderQuantity(update.Data.Ticker.Volume, update.Data.Ticker.QuoteVolume),
                        update.Data.Ticker.PriceChangePercentage)
                    {
                    })), ct).ConfigureAwait(false);
                return result;
            }
        }

        #endregion
    }
}
