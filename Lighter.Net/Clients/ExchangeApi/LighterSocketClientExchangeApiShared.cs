using CryptoExchange.Net;
using CryptoExchange.Net.Objects;
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
    internal partial class LighterSocketClientExchangeApi : ILighterSocketClientExchangeApiShared
    {
        private const string _exchangeName = "Lighter";
        private const string _topicSpotId = "LighterSpot";
        private const string _topicFuturesId = "LighterFutures";

        public TradingMode[] SupportedTradingModes => new[] { TradingMode.Spot, TradingMode.PerpetualLinear };

        public void SetDefaultExchangeParameter(string key, object value) => ExchangeParameters.SetStaticParameter(Exchange, key, value);
        public void ResetDefaultExchangeParameters() => ExchangeParameters.ResetStaticParameters();
        public SharedClientInfo Discover() => SharedUtils.GetClientInfo(LighterExchange.Metadata, this);

        #region Tickers client
        SubscribeTickersOptions ITickersSocketClient.SubscribeAllTickersOptions { get; } = new SubscribeTickersOptions(_exchangeName);
        async Task<WebSocketResult<UpdateSubscription>> ITickersSocketClient.SubscribeToAllTickersUpdatesAsync(SubscribeAllTickersRequest request, Action<DataEvent<SharedSpotTicker[]>> handler, CancellationToken ct)
        {
            var validationError = ((ITickersSocketClient)this).SubscribeAllTickersOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            if (request.TradingMode == null || request.TradingMode == TradingMode.Spot)
            {
                var result = await ExchangeData.SubscribeToSpotTickerUpdatesAsync(update => handler(update.ToType(
                    update.Data.Tickers.Values.Select(x =>
                    new SharedSpotTicker(
                        ExchangeSymbolCache.ParseSymbol(_topicSpotId, EnvironmentName, null, x.Symbol),
                        x.Symbol,
                        x.LastPrice,
                        x.HighPrice,
                        x.LowPrice,
                        x.Volume,
                        x.PriceChangePercentage)
                    {
                        QuoteVolume = x.QuoteVolume
                    }).ToArray())), ct).ConfigureAwait(false);
                return result;
            }
            else
            {
                var result = await ExchangeData.SubscribeToFuturesTickerUpdatesAsync(update => handler(update.ToType(
                    update.Data.Tickers.Values.Select(x =>
                    new SharedSpotTicker(
                        ExchangeSymbolCache.ParseSymbol(_topicFuturesId, EnvironmentName, null, x.Symbol),
                        x.Symbol,
                        x.LastPrice,
                        x.HighPrice,
                        x.LowPrice,
                        x.Volume,
                        x.PriceChangePercentage)
                    {
                        QuoteVolume = x.QuoteVolume
                    }).ToArray())), ct).ConfigureAwait(false);
                return result;
            }
        }

        #endregion

        #region Ticker client
        SubscribeTickerOptions ITickerSocketClient.SubscribeTickerOptions { get; } = new SubscribeTickerOptions(_exchangeName);
        async Task<WebSocketResult<UpdateSubscription>> ITickerSocketClient.SubscribeToTickerUpdatesAsync(SubscribeTickerRequest request, Action<DataEvent<SharedSpotTicker>> handler, CancellationToken ct)
        {
            var validationError = ((ITickerSocketClient)this).SubscribeTickerOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            if (request.Symbol!.TradingMode == TradingMode.Spot)
            {
                var result = await ExchangeData.SubscribeToSpotTickerUpdatesAsync(symbol, update => handler(update.ToType(
                    new SharedSpotTicker(
                        ExchangeSymbolCache.ParseSymbol(_topicSpotId, EnvironmentName, null, update.Data.Ticker.Symbol),
                        update.Data.Ticker.Symbol, 
                        update.Data.Ticker.LastPrice,
                        update.Data.Ticker.HighPrice,
                        update.Data.Ticker.LowPrice,
                        update.Data.Ticker.Volume,
                        update.Data.Ticker.PriceChangePercentage)
                {
                    QuoteVolume = update.Data.Ticker.QuoteVolume
                })), ct).ConfigureAwait(false);
                return result;
            }
            else
            {
                var result = await ExchangeData.SubscribeToFuturesTickerUpdatesAsync(symbol, update => handler(update.ToType(
                    new SharedSpotTicker(
                        ExchangeSymbolCache.ParseSymbol(_topicSpotId, EnvironmentName, null, update.Data.Ticker.Symbol),
                        update.Data.Ticker.Symbol,
                        update.Data.Ticker.LastPrice,
                        update.Data.Ticker.HighPrice,
                        update.Data.Ticker.LowPrice,
                        update.Data.Ticker.Volume,
                        update.Data.Ticker.PriceChangePercentage)
                    {
                        QuoteVolume = update.Data.Ticker.QuoteVolume
                    })), ct).ConfigureAwait(false);
                return result;
            }
        }
        #endregion

        #region Trade client

        SubscribeTradeOptions ITradeSocketClient.SubscribeTradeOptions { get; }
            = new SubscribeTradeOptions(_exchangeName, false);
        async Task<WebSocketResult<UpdateSubscription>> ITradeSocketClient.SubscribeToTradeUpdatesAsync(SubscribeTradeRequest request, Action<DataEvent<SharedTrade[]>> handler, CancellationToken ct)
        {
            var validationError = ((ITradeSocketClient)this).SubscribeTradeOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            var symbol = request.SymbolName(FormatSymbol);
            var result = await ExchangeData.SubscribeToTradeUpdatesAsync(symbol, update =>
            {
                if (update.UpdateType == SocketUpdateType.Snapshot)
                    return;

                handler(update.ToType(update.Data.Trades.Select(x =>
                    new SharedTrade(
                        request.Symbol,
                        symbol,
                        x.Quantity,
                        x.Price,
                        x.Timestamp)
                    {
                        Side = x.IsMakerAsk ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                    }).ToArray()));
            }, ct).ConfigureAwait(false);

            return result;
        }

        #endregion

        #region Book Ticker client

        SubscribeBookTickerOptions IBookTickerSocketClient.SubscribeBookTickerOptions { get; }
            = new SubscribeBookTickerOptions(_exchangeName, false);
        async Task<WebSocketResult<UpdateSubscription>> IBookTickerSocketClient.SubscribeToBookTickerUpdatesAsync(SubscribeBookTickerRequest request, Action<DataEvent<SharedBookTicker>> handler, CancellationToken ct)
        {
            var validationError = ((IBookTickerSocketClient)this).SubscribeBookTickerOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            var symbol = request.SymbolName(FormatSymbol);
            var result = await ExchangeData.SubscribeToBookTickerUpdatesAsync(symbol, update => handler(update.ToType(
                new SharedBookTicker(
                    request.Symbol,
                    update.Data.BookTicker.Symbol, 
                    update.Data.BookTicker.Ask.Price, 
                    update.Data.BookTicker.Ask.Quantity, 
                    update.Data.BookTicker.Bid.Price, 
                    update.Data.BookTicker.Bid.Quantity))), ct).ConfigureAwait(false);

            return result;
        }

        #endregion

        #region Kline client
        SubscribeKlineOptions IKlineSocketClient.SubscribeKlineOptions { get; } = new SubscribeKlineOptions(_exchangeName, false);
        async Task<WebSocketResult<UpdateSubscription>> IKlineSocketClient.SubscribeToKlineUpdatesAsync(SubscribeKlineRequest request, Action<DataEvent<SharedKline>> handler, CancellationToken ct)
        {
            var validationError = ((IKlineSocketClient)this).SubscribeKlineOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            var symbol = request.SymbolName(FormatSymbol);
            var result = await ExchangeData.SubscribeToKlineUpdatesAsync(symbol, (KlineInterval)request.Interval, update =>
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
                            kline.Volume)));
                }
            }, ct).ConfigureAwait(false);

            return result;
        }
        #endregion

        #region Balance client
        SubscribeBalanceOptions IBalanceSocketClient.SubscribeBalanceOptions { get; }
            = new SubscribeBalanceOptions(_exchangeName, true);
        async Task<WebSocketResult<UpdateSubscription>> IBalanceSocketClient.SubscribeToBalanceUpdatesAsync(SubscribeBalancesRequest request, Action<DataEvent<SharedBalance[]>> handler, CancellationToken ct)
        {
            var validationError = ((IBalanceSocketClient)this).SubscribeBalanceOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            var tradingMode = request.TradingMode ?? TradingMode.Spot;
            var result = await Account.SubscribeToAccountUpdatesAsync(null,
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

        #region Spot Order client

        SubscribeSpotOrderOptions ISpotOrderSocketClient.SubscribeSpotOrderOptions { get; }
            = new SubscribeSpotOrderOptions(_exchangeName, true);
        async Task<WebSocketResult<UpdateSubscription>> ISpotOrderSocketClient.SubscribeToSpotOrderUpdatesAsync(SubscribeSpotOrderRequest request, Action<DataEvent<SharedSpotOrder[]>> handler, CancellationToken ct)
        {
            var validationError = ((ISpotOrderSocketClient)this).SubscribeSpotOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            var result = await Trading.SubscribeToOrderUpdatesAsync(null,
                update =>
                {
                    var spotOrders = update.Data.Orders.SelectMany(x => x.Value).Where(x => x.MarketIndex >= 2048).ToList();
                    if (spotOrders.Count == 0)
                        return;

                    handler(update.ToType(spotOrders.Select(x =>
                        new SharedSpotOrder(
                            ExchangeSymbolCache.ParseSymbol(_topicSpotId, EnvironmentName, null, LighterUtils.GetSymbolName(EnvironmentName, x.MarketIndex)),
                            LighterUtils.GetSymbolName(EnvironmentName, x.MarketIndex) ?? string.Empty,
                            x.OrderIndex.ToString(),
                            ParseOrderType(x.OrderType),
                            x.IsAsk ? SharedOrderSide.Sell : SharedOrderSide.Buy,
                            ParseOrderStatus(x.Status),
                            x.CreateTime)
                            {
                                ClientOrderId = x.ClientOrderId.ToString(),
                                OrderPrice = x.Price,
                                OrderQuantity = new SharedOrderQuantity(x.InitialBaseQuantity),
                                QuantityFilled = new SharedOrderQuantity(x.QuantityFilled, x.QuoteQuantityFilled),
                                TimeInForce = ParseTimeInForce(x.TimeInForce),
                                UpdateTime = x.UpdateTime,
                                TriggerPrice = x.TriggerPrice > 0 ? x.TriggerPrice : null,
                                IsTriggerOrder = x.TriggerPrice > 0
                            }
                    ).ToArray()));
                },
                ct: ct).ConfigureAwait(false);

            return result;
        }

        private SharedOrderType ParseOrderType(OrderType type)
        {
            if (type == OrderType.Market) return SharedOrderType.Market;
            if (type == OrderType.Limit) return SharedOrderType.Limit;

            return SharedOrderType.Other;
        }

        private SharedTimeInForce? ParseTimeInForce(TimeInForce tif)
        {
            if (tif == TimeInForce.GoodTillTime) return SharedTimeInForce.GoodTillCanceled;
            if (tif == TimeInForce.ImmediateOrCancel) return SharedTimeInForce.ImmediateOrCancel;

            return null;
        }

        private SharedOrderStatus ParseOrderStatus(OrderStatus status)
        {
            if (status == OrderStatus.Canceled
                || status == OrderStatus.CanceledChild
                || status == OrderStatus.CanceledExpired
                || status == OrderStatus.CanceledInvalidBalance
                || status == OrderStatus.CanceledLiquidation
                || status == OrderStatus.CanceledMarginNotAllowed
                || status == OrderStatus.CanceledNotEnoughLiquidity
                || status == OrderStatus.CanceledOco
                || status == OrderStatus.CanceledPositionNotAllowed
                || status == OrderStatus.CanceledPostOnly
                || status == OrderStatus.CanceledReduceOnly
                || status == OrderStatus.CanceledSelfTrade
                || status == OrderStatus.CanceledTooMuchSlippage)
            {
                return SharedOrderStatus.Canceled;
            }

            if (status == OrderStatus.InProgress
                || status == OrderStatus.InProgress
                || status == OrderStatus.Open)
            {
                return SharedOrderStatus.Open;
            }

            if (status == OrderStatus.Filled)
                return SharedOrderStatus.Filled;

            return SharedOrderStatus.Unknown;
        }


        #endregion

        #region Futures Order client

        SubscribeFuturesOrderOptions IFuturesOrderSocketClient.SubscribeFuturesOrderOptions { get; }
            = new SubscribeFuturesOrderOptions(_exchangeName, true);
        async Task<WebSocketResult<UpdateSubscription>> IFuturesOrderSocketClient.SubscribeToFuturesOrderUpdatesAsync(SubscribeFuturesOrderRequest request, Action<DataEvent<SharedFuturesOrder[]>> handler, CancellationToken ct)
        {
            var validationError = ((IFuturesOrderSocketClient)this).SubscribeFuturesOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            var result = await Trading.SubscribeToOrderUpdatesAsync(null,
                update =>
                {
                    var futuresOrders = update.Data.Orders.SelectMany(x => x.Value).Where(x => x.MarketIndex < 2048).ToList();
                    if (futuresOrders.Count == 0)
                        return;

                    handler(update.ToType(futuresOrders.Select(x =>
                        new SharedFuturesOrder(
                            ExchangeSymbolCache.ParseSymbol(_topicSpotId, EnvironmentName, null, LighterUtils.GetSymbolName(EnvironmentName, x.MarketIndex)),
                            LighterUtils.GetSymbolName(EnvironmentName, x.MarketIndex) ?? string.Empty,
                            x.OrderIndex.ToString(),
                            ParseOrderType(x.OrderType),
                            x.IsAsk ? SharedOrderSide.Sell : SharedOrderSide.Buy,
                            ParseOrderStatus(x.Status),
                            x.CreateTime)
                        {
                            ClientOrderId = x.ClientOrderId.ToString(),
                            OrderPrice = x.Price,
                            OrderQuantity = new SharedOrderQuantity(x.InitialBaseQuantity),
                            QuantityFilled = new SharedOrderQuantity(x.QuantityFilled, x.QuoteQuantityFilled),
                            TimeInForce = ParseTimeInForce(x.TimeInForce),
                            UpdateTime = x.UpdateTime,
                            TriggerPrice = x.TriggerPrice > 0 ? x.TriggerPrice : null,
                            IsTriggerOrder = x.TriggerPrice > 0,
                            ReduceOnly = x.ReduceOnly
                        }
                    ).ToArray()));
                },
                ct: ct).ConfigureAwait(false);

            return result;
        }
        #endregion

        #region User Trade client
        SubscribeUserTradeOptions IUserTradeSocketClient.SubscribeUserTradeOptions { get; } = new SubscribeUserTradeOptions(_exchangeName, true);
        async Task<WebSocketResult<UpdateSubscription>> IUserTradeSocketClient.SubscribeToUserTradeUpdatesAsync(SubscribeUserTradeRequest request, Action<DataEvent<SharedUserTrade[]>> handler, CancellationToken ct)
        {
            var result = await Trading.SubscribeToUserTradeUpdatesAsync(null,
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
                                ExchangeSymbolCache.ParseSymbol(x.MarketId >= 2048 ? _topicSpotId : _topicFuturesId, EnvironmentName, null, LighterUtils.GetSymbolName(EnvironmentName, x.MarketId)),
                                LighterUtils.GetSymbolName(EnvironmentName, x.MarketId) ?? string.Empty,
                                (x.BidAccountId == ApiCredentials!.Credential!.AccountIndex ? x.BidId : x.AskId).ToString(),
                                x.TradeId.ToString(),
                                x.IsMakerAsk ? SharedOrderSide.Sell : SharedOrderSide.Buy,
                                x.Quantity,
                                x.Price,
                                x.Timestamp)
                        {
                            ClientOrderId = (x.BidAccountId == ApiCredentials.Credential.AccountIndex ? x.BidClientId : x.AskClientId).ToString(),
                            Fee = x.IsMakerAsk == (x.AskAccountId == ApiCredentials.Credential.AccountIndex) ? x.Quantity * x.MakerFee : x.Quantity * x.TakerFee,
                            Role = x.IsMakerAsk == (x.AskAccountId == ApiCredentials.Credential.AccountIndex) ? SharedRole.Maker : SharedRole.Taker
                    }).ToArray()));
                },
                ct: ct).ConfigureAwait(false);

            return result;
        }
        #endregion

        #region Position client
        SubscribePositionOptions IPositionSocketClient.SubscribePositionOptions { get; }
            = new SubscribePositionOptions(_exchangeName, true);
        async Task<WebSocketResult<UpdateSubscription>> IPositionSocketClient.SubscribeToPositionUpdatesAsync(SubscribePositionRequest request, Action<DataEvent<SharedPosition[]>> handler, CancellationToken ct)
        {
            var validationError = ((IPositionSocketClient)this).SubscribePositionOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            var result = await Trading.SubscribeToPositionUpdatesAsync(null,
                update => handler(update.ToType(update.Data.Positions.Values.Select(x => 
                    new SharedPosition(
                        ExchangeSymbolCache.ParseSymbol(_topicFuturesId, EnvironmentName, null, x.Symbol), 
                        x.Symbol, 
                        Math.Abs(x.Position), 
                        null)
                    {
                        AverageOpenPrice = x.AverageEntryPrice,
                        PositionMode = SharedPositionMode.OneWay,
                        PositionSide = x.PositionSide == Enums.PositionSide.Short ? SharedPositionSide.Short : SharedPositionSide.Long,
                        UnrealizedPnl = x.UnrealizedPnl
                    }).ToArray())),
                ct: ct).ConfigureAwait(false);

            return result;
        }

        #endregion
    }
}
