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
        #region Subscribe User Trades

        public SubscribeUserTradeOptions SubscribeUserTradeOptions { get; } = new SubscribeUserTradeOptions(_exchangeName, true);
        public async Task<WebSocketResult<UpdateSubscription>> SubscribeToUserTradeUpdatesAsync(SubscribeUserTradeRequest request, Action<DataEvent<SharedUserTrade[]>> handler, CancellationToken ct)
        {
            var result = await _api.Trading.SubscribeToUserTradeUpdatesAsync(null,
                update =>
                {
                    List<LighterUserTrade> trades;
                    if (request.TradingMode == null)
                        trades = update.Data.Trades.SelectMany(x => x.Value).ToList();
                    else if(request.TradingMode == TradingMode.Spot)
                        trades = update.Data.Trades.SelectMany(x => x.Value).Where(x => x.MarketId >= 2048).ToList();
                    else
                        trades = update.Data.Trades.SelectMany(x => x.Value).Where(x => x.MarketId < 2048).ToList();

                    if (trades.Count == 0)
                        return;

                    handler(update.ToType<SharedUserTrade[]>(trades.Select(x => new SharedUserTrade(
                                ExchangeSymbolCache.ParseSymbol(x.MarketId >= 2048 ? _topicSpotId : _topicFuturesId, _api.EnvironmentName, null, LighterUtils.GetSymbolName(_api.EnvironmentName, x.MarketId)),
                                LighterUtils.GetSymbolName(_api.EnvironmentName, x.MarketId) ?? string.Empty,
                                (x.BidAccountId == _api.ApiCredentials!.Credential!.AccountIndex ? x.BidId : x.AskId).ToString(),
                                x.TradeId.ToString(),
                                x.AskAccountId == _api.ApiCredentials.Credential.AccountIndex ? SharedOrderSide.Sell : SharedOrderSide.Buy,
                                new SharedOrderQuantity(x.Quantity),
                                x.Price,
                                x.Timestamp)
                        {
                            ClientOrderId = (x.BidAccountId == _api.ApiCredentials.Credential.AccountIndex ? x.BidClientId : x.AskClientId).ToString(),
                            Fee = x.IsMakerAsk == (x.AskAccountId == _api.ApiCredentials.Credential.AccountIndex) ? x.Quantity * x.MakerFee : x.Quantity * x.TakerFee,
                            Role = x.IsMakerAsk == (x.AskAccountId == _api.ApiCredentials.Credential.AccountIndex) ? SharedRole.Maker : SharedRole.Taker
                    }).ToArray()));
                },
                ct: ct).ConfigureAwait(false);

            return result;
        }

        #endregion
    }
}
