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
    internal class LighterSocketClientExchangeSharedApi :
        SharedApiBase,
        ILighterSocketClientExchangeApiShared,
        ILighterSocketClientExchangeSharedApi
    {
        private readonly LighterSocketClientExchangeApi _api;

        private const string _exchangeName = "Lighter";
        private const string _topicSpotId = "LighterSpot";
        private const string _topicFuturesId = "LighterFutures";

        public override SharedClientInfo Discover() => SharedUtils.GetClientInfo(LighterExchange.Metadata, this);

        public LighterSocketClientExchangeSharedApi(LighterSocketClientExchangeApi api)
            : base(
                  api.Exchange,
                  [TradingMode.Spot, TradingMode.PerpetualLinear],
                  () => api.Authenticated,
                  api.FormatSymbol)
        {
            _api = api;

            SetCapabilities(
                SubscribeAllTickersOptions,
                SubscribeTickerOptions,
                SubscribeTradeOptions,
                SubscribeBookTickerOptions,
                SubscribeKlineOptions,
                SubscribeBalanceOptions,
                SubscribeFuturesOrderOptions,
                SubscribeSpotOrderOptions,
                SubscribeUserTradeOptions,
                SubscribePositionOptions,
                PlaceSpotOrderOptions,
                CancelSpotOrderOptions,
                PlaceFuturesOrderOptions,
                CancelFuturesOrderOptions
                );
        }

        #region Tickers client
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

        #region Ticker client

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

        #region Book Ticker client

        public SubscribeBookTickerOptions SubscribeBookTickerOptions { get; }
            = new SubscribeBookTickerOptions(_exchangeName, false);
        public async Task<WebSocketResult<UpdateSubscription>> SubscribeToBookTickerUpdatesAsync(SubscribeBookTickerRequest request, Action<DataEvent<SharedBookTicker>> handler, CancellationToken ct)
        {
            var validationError = SubscribeBookTickerOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            var symbol = request.SymbolName(FormatSymbol);
            var result = await _api.ExchangeData.SubscribeToBookTickerUpdatesAsync(symbol, update => handler(update.ToType(
                new SharedBookTicker(
                    request.Symbol,
                    update.Data.BookTicker.Symbol, 
                    update.Data.BookTicker.Ask.Price,
                    new SharedOrderQuantity(update.Data.BookTicker.Ask.Quantity), 
                    update.Data.BookTicker.Bid.Price,
                    new SharedOrderQuantity(update.Data.BookTicker.Bid.Quantity)))), ct).ConfigureAwait(false);

            return result;
        }

        #endregion

        #region Kline client
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

        #region Balance client
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

        #region Spot Order client

        async Task<WebSocketResult<UpdateSubscription>> ISpotOrderSocketClient.SubscribeToSpotOrderUpdatesAsync(SubscribeSpotOrderRequest request, Action<DataEvent<SharedSpotOrder[]>> handler, CancellationToken ct)
            => await SubscribeToSpotOrderUpdatesAsync(request, x => handler(x.ToType<SharedSpotOrder[]>(x.Data)), ct).ConfigureAwait(false);

        public SubscribeSpotOrderOptions SubscribeSpotOrderOptions { get; }
            = new SubscribeSpotOrderOptions(_exchangeName, true);
        public async Task<WebSocketResult<UpdateSubscription>> SubscribeToSpotOrderUpdatesAsync(SubscribeSpotOrderRequest request, Action<DataEvent<SharedSpotOrderUpdate[]>> handler, CancellationToken ct)
        {
            var validationError = SubscribeSpotOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            var result = await _api.Trading.SubscribeToOrderUpdatesAsync(null,
                update =>
                {
                    var spotOrders = update.Data.Orders.SelectMany(x => x.Value).Where(x => x.MarketIndex >= 2048).ToList();
                    if (spotOrders.Count == 0)
                        return;

                    handler(update.ToType(spotOrders.Select(x =>
                        new SharedSpotOrderUpdate(
                            ExchangeSymbolCache.ParseSymbol(_topicSpotId, _api.EnvironmentName, null, LighterUtils.GetSymbolName(_api.EnvironmentName, x.MarketIndex)),
                            LighterUtils.GetSymbolName(_api.EnvironmentName, x.MarketIndex) ?? string.Empty,
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

        async Task<WebSocketResult<UpdateSubscription>> IFuturesOrderSocketClient.SubscribeToFuturesOrderUpdatesAsync(SubscribeFuturesOrderRequest request, Action<DataEvent<SharedFuturesOrder[]>> handler, CancellationToken ct)
            => await SubscribeToFuturesOrderUpdatesAsync(request, x => handler(x.ToType<SharedFuturesOrder[]>(x.Data)), ct).ConfigureAwait(false);

        public SubscribeFuturesOrderOptions SubscribeFuturesOrderOptions { get; }
            = new SubscribeFuturesOrderOptions(_exchangeName, true);
        public async Task<WebSocketResult<UpdateSubscription>> SubscribeToFuturesOrderUpdatesAsync(SubscribeFuturesOrderRequest request, Action<DataEvent<SharedFuturesOrderUpdate[]>> handler, CancellationToken ct)
        {
            var validationError = SubscribeFuturesOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            var result = await _api.Trading.SubscribeToOrderUpdatesAsync(null,
                update =>
                {
                    var futuresOrders = update.Data.Orders.SelectMany(x => x.Value).Where(x => x.MarketIndex < 2048).ToList();
                    if (futuresOrders.Count == 0)
                        return;

                    handler(update.ToType(futuresOrders.Select(x =>
                        new SharedFuturesOrderUpdate(
                            ExchangeSymbolCache.ParseSymbol(_topicSpotId, _api.EnvironmentName, null, LighterUtils.GetSymbolName(_api.EnvironmentName, x.MarketIndex)),
                            LighterUtils.GetSymbolName(_api.EnvironmentName, x.MarketIndex) ?? string.Empty,
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

        #region Position client
        public SubscribePositionOptions SubscribePositionOptions { get; }
            = new SubscribePositionOptions(_exchangeName, true);
        public async Task<WebSocketResult<UpdateSubscription>> SubscribeToPositionUpdatesAsync(SubscribePositionRequest request, Action<DataEvent<SharedPosition[]>> handler, CancellationToken ct)
        {
            var validationError = SubscribePositionOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(Exchange, validationError);

            var result = await _api.Trading.SubscribeToPositionUpdatesAsync(null,
                update => handler(update.ToType(update.Data.Positions.Values.Select(x => 
                    new SharedPosition(
                        ExchangeSymbolCache.ParseSymbol(_topicFuturesId, _api.EnvironmentName, null, x.Symbol), 
                        x.Symbol,
                        new SharedOrderQuantity(Math.Abs(x.Position)), 
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

        #region Spot Order Client

        public SharedFeeDeductionType SpotFeeDeductionType => SharedFeeDeductionType.DeductFromOutput;
        public SharedFeeAssetType SpotFeeAssetType => SharedFeeAssetType.OutputAsset;
        public SharedOrderType[] SpotSupportedOrderTypes { get; } = new[] { SharedOrderType.Limit, SharedOrderType.Market, SharedOrderType.LimitMaker };
        public SharedTimeInForce[] SpotSupportedTimeInForce { get; } = new[] { SharedTimeInForce.GoodTillCanceled, SharedTimeInForce.ImmediateOrCancel };
        public SharedQuantitySupport SpotSupportedOrderQuantity { get; } = new SharedQuantitySupport(
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset);

        public string GenerateClientOrderId() => ExchangeHelpers.RandomLong(9).ToString();

        public PlaceSpotOrderSocketOptions PlaceSpotOrderOptions { get; } = new PlaceSpotOrderSocketOptions(_exchangeName)
        {
            RequiredRequestParameters = new List<ParameterDescription>
            {
                new ParameterDescription(nameof(PlaceSpotOrderRequest.Price), typeof(decimal), "Price for the order. For market orders this should be the current symbol price to calculate max slippage", 21.5m)
            },
        };
        public async Task<QueryResult<SharedId>> PlaceSpotOrderAsync(PlaceSpotOrderRequest request, CancellationToken ct)
        {
            var validationError = PlaceSpotOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return QueryResult.Fail<SharedId>(Exchange, validationError);

            long cid;
            if (request.ClientOrderId != null)
            {
                if (!long.TryParse(request.ClientOrderId, out var parsedCid))
                    return QueryResult.Fail<SharedId>(_exchangeName, new ServerError(ErrorType.InvalidParameter, "Client order id invalid; should be a number string"));

                cid = parsedCid;
            }
            else
            {
                cid = long.Parse(GenerateClientOrderId());
            }

            var result = await _api.Trading.PlaceOrderAsync(
                request.Symbol!.GetSymbol(FormatSymbol),
                request.Side == SharedOrderSide.Buy ? Enums.OrderSide.Buy : Enums.OrderSide.Sell,
                request.OrderType == SharedOrderType.Limit ? OrderType.Limit : OrderType.Market,
                quantity: request.Quantity?.QuantityInBaseAsset ?? 0,
                price: request.OrderType == SharedOrderType.Market ? GetSlippagePrice(request) : request.Price!.Value,
                timeInForce: GetTimeInForce(request.TimeInForce, request.OrderType),
                clientOrderIndex: cid,
                ct: ct).ConfigureAwait(false);

            if (!result.Success)
                return QueryResult.Fail<SharedId>(result);

            return QueryResult.Ok(result, new SharedId(null));

        }

        private Enums.TimeInForce GetTimeInForce(SharedTimeInForce? tif, SharedOrderType type)
        {
            if (tif == SharedTimeInForce.ImmediateOrCancel) return TimeInForce.ImmediateOrCancel;
            if (tif == SharedTimeInForce.GoodTillCanceled) return TimeInForce.GoodTillTime;
            if (type == SharedOrderType.LimitMaker) return TimeInForce.PostOnly;
            if (type == SharedOrderType.Market) return TimeInForce.ImmediateOrCancel;

            return TimeInForce.GoodTillTime;
        }

        private decimal GetSlippagePrice(PlaceSpotOrderRequest request)
        {
            // Calculate 5% max slippage
            if (request.Side == SharedOrderSide.Buy)
                return request.Price!.Value * 1.05m;

            return request.Price!.Value * 0.95m;
        }
        public CancelSpotOrderSocketOptions CancelSpotOrderOptions { get; }
            = new CancelSpotOrderSocketOptions(_exchangeName, true);
        public async Task<QueryResult<SharedId>> CancelSpotOrderAsync(CancelOrderRequest request, CancellationToken ct)
        {
            var validationError = CancelSpotOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return QueryResult.Fail<SharedId>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return QueryResult.Fail<SharedId>(Exchange, ArgumentError.Invalid(nameof(CancelOrderRequest.OrderId), "Invalid order id"));

            var order = await _api.Trading.CancelOrderAsync(request.Symbol!.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
            if (!order.Success)
                return QueryResult.Fail<SharedId>(order);

            return QueryResult.Ok(order, new SharedId(request.OrderId));
        }
        #endregion

        #region Futures Order Client

        public SharedFeeDeductionType FuturesFeeDeductionType => SharedFeeDeductionType.AddToCost;
        public SharedFeeAssetType FuturesFeeAssetType => SharedFeeAssetType.QuoteAsset;

        public SharedOrderType[] FuturesSupportedOrderTypes { get; } = new[] { SharedOrderType.Limit, SharedOrderType.Market, SharedOrderType.LimitMaker };
        public SharedTimeInForce[] FuturesSupportedTimeInForce { get; } = new[] { SharedTimeInForce.GoodTillCanceled, SharedTimeInForce.ImmediateOrCancel };
        public SharedQuantitySupport FuturesSupportedOrderQuantity { get; } = new SharedQuantitySupport(
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset);

        public PlaceFuturesOrderSocketOptions PlaceFuturesOrderOptions { get; } = new PlaceFuturesOrderSocketOptions(_exchangeName, false);
        public async Task<QueryResult<SharedId>> PlaceFuturesOrderAsync(PlaceFuturesOrderRequest request, CancellationToken ct)
        {
            var validationError = PlaceFuturesOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return QueryResult.Fail<SharedId>(Exchange, validationError);

            long cid;
            if (request.ClientOrderId != null)
            {
                if (!long.TryParse(request.ClientOrderId, out var parsedCid))
                    return QueryResult.Fail<SharedId>(_exchangeName, new ServerError(ErrorType.InvalidParameter, "Client order id invalid; should be a number string"));

                cid = parsedCid;
            }
            else
            {
                cid = long.Parse(GenerateClientOrderId());
            }

            var result = await _api.Trading.PlaceOrderAsync(
                request.Symbol!.GetSymbol(FormatSymbol),
                request.Side == SharedOrderSide.Buy ? Enums.OrderSide.Buy : Enums.OrderSide.Sell,
                request.OrderType == SharedOrderType.Limit ? OrderType.Limit : OrderType.Market,
                quantity: request.Quantity?.QuantityInBaseAsset ?? 0,
                price: request.OrderType == SharedOrderType.Market ? GetSlippagePrice(request) : request.Price!.Value,
                timeInForce: GetTimeInForce(request.TimeInForce, request.OrderType),
                clientOrderIndex: cid,
                ct: ct).ConfigureAwait(false);

            if (!result.Success)
                return QueryResult.Fail<SharedId>(result);

            return QueryResult.Ok(result, new SharedId(null));

        }

        private decimal GetSlippagePrice(PlaceFuturesOrderRequest request)
        {
            // Calculate 5% max slippage
            if (request.Side == SharedOrderSide.Buy)
                return request.Price!.Value * 1.05m;

            return request.Price!.Value * 0.95m;
        }

        public CancelFuturesOrderSocketOptions CancelFuturesOrderOptions { get; } = new CancelFuturesOrderSocketOptions(_exchangeName, true);
        public async Task<QueryResult<SharedId>> CancelFuturesOrderAsync(CancelOrderRequest request, CancellationToken ct)
        {
            var validationError = CancelFuturesOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return QueryResult.Fail<SharedId>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return QueryResult.Fail<SharedId>(Exchange, ArgumentError.Invalid(nameof(CancelOrderRequest.OrderId), "Invalid order id"));

            var order = await _api.Trading.CancelOrderAsync(request.Symbol!.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
            if (!order.Success)
                return QueryResult.Fail<SharedId>(order);

            return QueryResult.Ok(order, new SharedId(request.OrderId));

        }

        #endregion
    }
}
