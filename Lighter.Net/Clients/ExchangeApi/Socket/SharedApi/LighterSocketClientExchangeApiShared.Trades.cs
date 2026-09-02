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
        #region Trade client

        public SubscribeTradeOptions SubscribeTradeOptions { get; }
            = new SubscribeTradeOptions(_exchangeName, false);
        public async Task<WebSocketResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(SubscribeTradeRequest request, Action<DataEvent<SharedTrade[]>> handler, CancellationToken ct)
        {
            var validationError = SubscribeTradeOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            var symbol = request.SymbolName(FormatSymbol);
            var result = await _api.ExchangeData.SubscribeToTradeUpdatesAsync(symbol, update =>
            {
                if (update.UpdateType == SocketUpdateType.Snapshot)
                    return;

                handler(update.ToType(update.Data.Trades.Select(x =>
                    new SharedTrade(
                        request.Symbol,
                        symbol,
                        new SharedOrderQuantity(x.Quantity),
                        x.Price,
                        x.Timestamp)
                    {
                        Side = x.IsMakerAsk ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                    }).ToArray()));
            }, ct).ConfigureAwait(false);

            return result;
        }

        #endregion
    }
}
