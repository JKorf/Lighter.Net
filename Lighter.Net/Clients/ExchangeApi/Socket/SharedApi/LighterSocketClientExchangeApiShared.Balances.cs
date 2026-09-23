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
        #region Subscribe Balances

        public SubscribeBalanceOptions SubscribeBalanceOptions { get; }
            = new SubscribeBalanceOptions(_exchangeName, true);
        public async Task<WebSocketResult<UpdateSubscription>> SubscribeToBalanceUpdatesAsync(SubscribeBalancesRequest request, Action<DataEvent<SharedBalance[]>> handler, CancellationToken ct)
        {
            var validationError = SubscribeBalanceOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            var tradingMode = request.TradingMode ?? TradingMode.Spot;
            var result = await _api.Account.SubscribeToAccountUpdatesAsync(null,
                update =>
                {
                    if (update.Data.Balances == null)
                        return;

                    if (request.TradingMode == null || request.TradingMode == TradingMode.Spot)
                    {
                        handler(update.ToType(update.Data.Balances.Select(x =>
                            new SharedBalance(
                                tradingMode,
                                x.Value.Symbol,
                                x.Value.Balance - x.Value.LockedBalance,
                                x.Value.Balance)).ToArray()));
                    }

                    if (request.TradingMode == null || request.TradingMode == TradingMode.PerpetualLinear)
                    {
                        handler(update.ToType(update.Data.Balances.Select(x =>
                            new SharedBalance(
                                tradingMode,
                                x.Value.Symbol,
                                x.Value.MarginBalance,
                                x.Value.MarginBalance)).ToArray()));
                    }
                },
                ct: ct).ConfigureAwait(false);

            return result;
        }

        #endregion

    }
}
