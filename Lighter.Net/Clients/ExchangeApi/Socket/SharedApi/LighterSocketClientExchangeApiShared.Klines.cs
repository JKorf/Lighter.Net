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
        #region Subscribe Klines

        public SubscribeKlineOptions SubscribeKlineOptions { get; } = new SubscribeKlineOptions(_exchangeName, false);
        public async Task<WebSocketResult<UpdateSubscription>> SubscribeToKlineUpdatesAsync(SubscribeKlineRequest request, Action<DataEvent<SharedKline>> handler, CancellationToken ct)
        {
            var validationError = SubscribeKlineOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            var symbol = request.SymbolName(FormatSymbol);
            var result = await _api.ExchangeData.SubscribeToKlineUpdatesAsync(symbol, (KlineInterval)request.Interval, update =>
            {
                foreach (var kline in update.Data.Klines)
                {
                    handler(update.ToType(
                        new SharedKline(
                            request.Symbol,
                            symbol,
                            kline.OpenTime,
                            kline.ClosePrice,
                            kline.HighPrice,
                            kline.LowPrice,
                            kline.OpenPrice,
                            new SharedOrderQuantity(kline.Volume, kline.QuoteVolume))));
                }
            }, ct).ConfigureAwait(false);

            return result;
        }

        #endregion
    }
}
