using CryptoExchange.Net;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Errors;
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
    internal partial class LighterRestClientExchangeSharedApi
    {
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

        public PlaceSpotOrderOptions PlaceSpotOrderOptions { get; } = new PlaceSpotOrderOptions(_exchangeName)
        {
            RequiredRequestParameters = new List<ParameterDescription>
            {
                new ParameterDescription(nameof(PlaceSpotOrderRequest.Price), typeof(decimal), "Price for the order. For market orders this should be the current symbol price to calculate max slippage", 21.5m)
            },
        };
        public async Task<HttpResult<SharedId>> PlaceSpotOrderAsync(PlaceSpotOrderRequest request, CancellationToken ct)
        {
            var validationError = PlaceSpotOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            long cid;
            if (request.ClientOrderId != null)
            {
                if (!long.TryParse(request.ClientOrderId, out var parsedCid))
                    return HttpResult.Fail<SharedId>(_exchangeName, new ServerError(ErrorType.InvalidParameter, "Client order id invalid; should be a number string"));

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
                return HttpResult.Fail<SharedId>(result);

            return HttpResult.Ok(result, new SharedId(null));

        }

        private decimal GetSlippagePrice(PlaceSpotOrderRequest request)
        {
            // Calculate 5% max slippage
            if (request.Side == SharedOrderSide.Buy)
                return request.Price!.Value * 1.05m;

            return request.Price!.Value * 0.95m;
        }

        public GetSpotOrderOptions GetSpotOrderOptions { get; } = new GetSpotOrderOptions(_exchangeName, true);
        public async Task<HttpResult<SharedSpotOrder>> GetSpotOrderAsync(GetOrderRequest request, CancellationToken ct)
        {
            var validationError = GetSpotOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotOrder>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedSpotOrder>(Exchange, ArgumentError.Invalid(nameof(GetOrderRequest.OrderId), "Invalid order id"));

            LighterOrder? orderInfo = null;
            var openOrders = await _api.Trading.GetOpenOrdersAsync(symbol: request.Symbol!.GetSymbol(FormatSymbol), ct: ct).ConfigureAwait(false);
            if (!openOrders.Success)
                return HttpResult.Fail<SharedSpotOrder>(openOrders);

            orderInfo = openOrders.Data.Orders.SingleOrDefault(x => x.OrderId == orderId || x.ClientOrderId == orderId);
            if (orderInfo == null)
            {
                var closedOrders = await _api.Trading.GetClosedOrdersAsync(symbol: request.Symbol!.GetSymbol(FormatSymbol), limit: 100, ct: ct).ConfigureAwait(false);
                if (!closedOrders.Success)
                    return HttpResult.Fail<SharedSpotOrder>(closedOrders);
                orderInfo = closedOrders.Data.Orders.SingleOrDefault(x => x.OrderId == orderId || x.ClientOrderId == orderId);
            }

            if (orderInfo == null)
                return HttpResult.Fail<SharedSpotOrder>(openOrders, new ServerError(ErrorType.UnknownOrder, "Order not found"));

            return HttpResult.Ok(openOrders, new SharedSpotOrder(
                ExchangeSymbolCache.ParseSymbol(_topicSpotId, _api.EnvironmentName, null, LighterUtils.GetSymbolName(_api.EnvironmentName, orderInfo.MarketIndex)),
                LighterUtils.GetSymbolName(_api.EnvironmentName, orderInfo.MarketIndex) ?? string.Empty,
                orderInfo.OrderId.ToString(),
                ParseOrderType(orderInfo.OrderType),
                orderInfo.IsAsk ? SharedOrderSide.Sell : SharedOrderSide.Buy,
                ParseOrderStatus(orderInfo.Status),
                orderInfo.CreateTime)
            {
                ClientOrderId = orderInfo.ClientOrderId.ToString(),
                OrderPrice = orderInfo.Price,
                OrderQuantity = new SharedOrderQuantity(orderInfo.InitialBaseQuantity),
                QuantityFilled = new SharedOrderQuantity(orderInfo.QuantityFilled, orderInfo.QuoteQuantityFilled),
                TimeInForce = ParseTimeInForce(orderInfo.TimeInForce),
                UpdateTime = orderInfo.UpdateTime,
                TriggerPrice = orderInfo.TriggerPrice > 0 ? orderInfo.TriggerPrice : null,
                IsTriggerOrder = orderInfo.TriggerPrice > 0
            });

        }

        public GetOpenSpotOrdersOptions GetOpenSpotOrdersOptions { get; }
            = new GetOpenSpotOrdersOptions(_exchangeName, true);
        public async Task<HttpResult<SharedSpotOrder[]>> GetOpenSpotOrdersAsync(GetOpenOrdersRequest request, CancellationToken ct)
        {
            var validationError = GetOpenSpotOrdersOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotOrder[]>(Exchange, validationError);

            var symbol = request.Symbol?.GetSymbol(FormatSymbol);
            var orders = await _api.Trading.GetOpenOrdersAsync(symbol: symbol, ct: ct).ConfigureAwait(false);
            if (!orders.Success)
                return HttpResult.Fail<SharedSpotOrder[]>(orders);

            var spotOrders = orders.Data.Orders.Where(x => x.MarketIndex >= 2048);
            return HttpResult.Ok(orders, spotOrders.Select(x => new SharedSpotOrder(
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
            }).ToArray());

        }

        public GetSpotClosedOrdersOptions GetClosedSpotOrdersOptions { get; } = new GetSpotClosedOrdersOptions(_exchangeName, false, true, false, 100);
        public async Task<HttpResult<SharedSpotOrder[]>> GetClosedSpotOrdersAsync(GetClosedOrdersRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = GetClosedSpotOrdersOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotOrder[]>(Exchange, validationError);

            var direction = DataDirection.Descending;
            var limit = request.Limit ?? 100;
            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var pageParams = Pagination.GetPaginationParameters(
                direction, limit, request.StartTime,
                request.EndTime ?? DateTime.UtcNow,
                pageRequest);

            // Get data
            var orders = await _api.Trading.GetClosedOrdersAsync(
                symbol: symbol,
                limit: limit,
                cursor: pageParams.Cursor,
                ct: ct).ConfigureAwait(false);
            if (!orders.Success)
                return HttpResult.Fail<SharedSpotOrder[]>(orders);

            var spotOrders = orders.Data.Orders.Where(x => x.MarketIndex >= 2048);
            var nextPageRequest = Pagination.GetNextPageRequest(
                   () => orders.Data.NextCursor == null ? null : Pagination.NextPageFromCursor(orders.Data.NextCursor),
                   orders.Data.Orders.Length,
                   orders.Data.Orders.Select(x => x.CreateTime),
                   request.StartTime,
                   request.EndTime ?? DateTime.UtcNow,
                   pageParams);

            return HttpResult.Ok(orders, ExchangeHelpers.ApplyFilter(spotOrders, x => x.CreateTime, request.StartTime, request.EndTime, direction)
                    .Select(x => new SharedSpotOrder(
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
                    }).ToArray(), nextPageRequest);

        }

        public GetSpotOrderTradesOptions GetSpotOrderTradesOptions { get; }
            = new GetSpotOrderTradesOptions(_exchangeName, true);
        public async Task<HttpResult<SharedUserTrade[]>> GetSpotOrderTradesAsync(GetOrderTradesRequest request, CancellationToken ct)
        {
            var validationError = GetSpotOrderTradesOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedUserTrade[]>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedUserTrade[]>(Exchange, ArgumentError.Invalid(nameof(GetOrderTradesRequest.OrderId), "Invalid order id"));

            var orders = await _api.Trading.GetUserTradesAsync(symbol: request.Symbol!.GetSymbol(FormatSymbol), orderIndex: orderId, ct: ct).ConfigureAwait(false);
            if (!orders.Success)
                return HttpResult.Fail<SharedUserTrade[]>(orders);

            return HttpResult.Ok(orders, orders.Data!.Trades.Select(x => new SharedUserTrade(
                request.Symbol,
                LighterUtils.GetSymbolName(_api.EnvironmentName, x.MarketId) ?? string.Empty,
                request.OrderId,
                x.TradeId.ToString(),
                x.AskAccountId == _api.ApiCredentials!.Credential.AccountIndex ? SharedOrderSide.Sell : SharedOrderSide.Buy,
                new SharedOrderQuantity(x.Quantity),
                x.Price,
                x.Timestamp)
            {
                ClientOrderId = (x.BidAccountId == _api.ApiCredentials!.Credential!.AccountIndex ? x.BidClientId : x.AskClientId).ToString(),
                Fee = x.IsMakerAsk == (x.AskAccountId == _api.ApiCredentials!.Credential.AccountIndex) ? x.Quantity * x.MakerFee : x.Quantity * x.TakerFee,
                Role = x.IsMakerAsk == (x.AskAccountId == _api.ApiCredentials!.Credential.AccountIndex) ? SharedRole.Maker : SharedRole.Taker
            }).ToArray());

        }

        Task<HttpResult<SharedUserTrade[]>> ISpotOrderRestClient.GetSpotUserTradesAsync(GetUserTradesRequest request, PageRequest? pageRequest, CancellationToken ct)
            => GetSpotUserTradeHistoryAsync(request, pageRequest, ct);
        GetSpotUserTradeHistoryOptions ISpotOrderRestClient.GetSpotUserTradesOptions => GetSpotUserTradeHistoryOptions;

        public GetSpotUserTradeHistoryOptions GetSpotUserTradeHistoryOptions { get; } = new GetSpotUserTradeHistoryOptions(_exchangeName, false, true, false, 100);
        public async Task<HttpResult<SharedUserTrade[]>> GetSpotUserTradeHistoryAsync(GetUserTradesRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = GetSpotUserTradeHistoryOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedUserTrade[]>(Exchange, validationError);

            var direction = DataDirection.Descending;
            var limit = request.Limit ?? 100;
            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var pageParams = Pagination.GetPaginationParameters(
                direction, limit, request.StartTime,
                request.EndTime ?? DateTime.UtcNow,
                pageRequest);

            // Get data
            var result = await _api.Trading.GetUserTradesAsync(
                symbol: symbol,
                limit: limit,
                cursor: pageParams.Cursor,
                ct: ct
                ).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedUserTrade[]>(result);

            var spotTrades = result.Data.Trades.Where(x => x.MarketId >= 2048);
            var nextPageRequest = Pagination.GetNextPageRequest(
                () => result.Data.NextCursor == null ? null : Pagination.NextPageFromCursor(result.Data.NextCursor),
                result.Data!.Trades.Length,
                result.Data.Trades.Select(x => x.Timestamp),
                request.StartTime,
                request.EndTime ?? DateTime.UtcNow,
                pageParams);

            return HttpResult.Ok(result, ExchangeHelpers.ApplyFilter(spotTrades, x => x.Timestamp, request.StartTime, request.EndTime, direction)
                    .Select(x => new SharedUserTrade(
                        request.Symbol,
                        LighterUtils.GetSymbolName(_api.EnvironmentName, x.MarketId) ?? string.Empty,
                        (x.BidAccountId == _api.ApiCredentials!.Credential!.AccountIndex ? x.BidId : x.AskId).ToString(),
                        x.TradeId.ToString(),
                        x.AskAccountId == _api.ApiCredentials!.Credential.AccountIndex ? SharedOrderSide.Sell : SharedOrderSide.Buy,
                        new SharedOrderQuantity(x.Quantity),
                        x.Price,
                        x.Timestamp)
                    {
                        ClientOrderId = (x.BidAccountId == _api.ApiCredentials.Credential.AccountIndex ? x.BidClientId : x.AskClientId).ToString(),
                        Fee = x.IsMakerAsk == (x.AskAccountId == _api.ApiCredentials.Credential.AccountIndex) ? x.Quantity * x.MakerFee : x.Quantity * x.TakerFee,
                        Role = x.IsMakerAsk == (x.AskAccountId == _api.ApiCredentials.Credential.AccountIndex) ? SharedRole.Maker : SharedRole.Taker
                    }).ToArray(), nextPageRequest);

        }

        public CancelSpotOrderOptions CancelSpotOrderOptions { get; }
            = new CancelSpotOrderOptions(_exchangeName, true);
        public async Task<HttpResult<SharedId>> CancelSpotOrderAsync(CancelOrderRequest request, CancellationToken ct)
        {
            var validationError = CancelSpotOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedId>(Exchange, ArgumentError.Invalid(nameof(CancelOrderRequest.OrderId), "Invalid order id"));

            var order = await _api.Trading.CancelOrderAsync(request.Symbol!.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
            if (!order.Success)
                return HttpResult.Fail<SharedId>(order);

            return HttpResult.Ok(order, new SharedId(request.OrderId));

        }

        private Enums.TimeInForce GetTimeInForce(SharedTimeInForce? tif, SharedOrderType type)
        {
            if (tif == SharedTimeInForce.ImmediateOrCancel) return TimeInForce.ImmediateOrCancel;
            if (tif == SharedTimeInForce.GoodTillCanceled) return TimeInForce.GoodTillTime;
            if (type == SharedOrderType.LimitMaker) return TimeInForce.PostOnly;
            if (type == SharedOrderType.Market) return TimeInForce.ImmediateOrCancel;

            return TimeInForce.GoodTillTime;
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

        #endregion

        #region Spot Client Id Order Client

        public GetSpotOrderByClientOrderIdOptions GetSpotOrderByClientOrderIdOptions { get; }
            = new GetSpotOrderByClientOrderIdOptions(_exchangeName, true);
        public async Task<HttpResult<SharedSpotOrder>> GetSpotOrderByClientOrderIdAsync(GetOrderRequest request, CancellationToken ct)
        {
            var validationError = GetSpotOrderByClientOrderIdOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotOrder>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedSpotOrder>(Exchange, ArgumentError.Invalid(nameof(GetOrderRequest.OrderId), "Invalid order id"));

            LighterOrder? orderInfo = null;
            var openOrders = await _api.Trading.GetOpenOrdersAsync(symbol: request.Symbol!.GetSymbol(FormatSymbol), ct: ct).ConfigureAwait(false);
            if (!openOrders.Success)
                return HttpResult.Fail<SharedSpotOrder>(openOrders);

            orderInfo = openOrders.Data.Orders.SingleOrDefault(x => x.ClientOrderId == orderId);
            if (orderInfo == null)
            {
                var closedOrders = await _api.Trading.GetClosedOrdersAsync(symbol: request.Symbol!.GetSymbol(FormatSymbol), limit: 100, ct: ct).ConfigureAwait(false);
                if (!closedOrders.Success)
                    return HttpResult.Fail<SharedSpotOrder>(closedOrders);
                orderInfo = closedOrders.Data.Orders.SingleOrDefault(x => x.ClientOrderId == orderId);
            }

            if (orderInfo == null)
                return HttpResult.Fail<SharedSpotOrder>(openOrders, new ServerError(ErrorType.UnknownOrder, "Order not found"));

            return HttpResult.Ok(openOrders, new SharedSpotOrder(
                ExchangeSymbolCache.ParseSymbol(_topicSpotId, _api.EnvironmentName, null, LighterUtils.GetSymbolName(_api.EnvironmentName, orderInfo.MarketIndex)),
                LighterUtils.GetSymbolName(_api.EnvironmentName, orderInfo.MarketIndex) ?? string.Empty,
                orderInfo.OrderId.ToString(),
                ParseOrderType(orderInfo.OrderType),
                orderInfo.IsAsk ? SharedOrderSide.Sell : SharedOrderSide.Buy,
                ParseOrderStatus(orderInfo.Status),
                orderInfo.CreateTime)
            {
                ClientOrderId = orderInfo.ClientOrderId.ToString(),
                OrderPrice = orderInfo.Price,
                OrderQuantity = new SharedOrderQuantity(orderInfo.InitialBaseQuantity),
                QuantityFilled = new SharedOrderQuantity(orderInfo.QuantityFilled, orderInfo.QuoteQuantityFilled),
                TimeInForce = ParseTimeInForce(orderInfo.TimeInForce),
                UpdateTime = orderInfo.UpdateTime,
                TriggerPrice = orderInfo.TriggerPrice > 0 ? orderInfo.TriggerPrice : null,
                IsTriggerOrder = orderInfo.TriggerPrice > 0
            });
        }

        public CancelSpotOrderByClientOrderIdOptions CancelSpotOrderByClientOrderIdOptions { get; }
            = new CancelSpotOrderByClientOrderIdOptions(_exchangeName, true);
        public async Task<HttpResult<SharedId>> CancelSpotOrderByClientOrderIdAsync(CancelOrderRequest request, CancellationToken ct)
        {
            var validationError = CancelSpotOrderByClientOrderIdOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedId>(Exchange, ArgumentError.Invalid(nameof(CancelOrderRequest.OrderId), "Invalid order id"));

            var order = await _api.Trading.CancelOrderAsync(request.Symbol!.GetSymbol(FormatSymbol), orderIndex: orderId, ct: ct).ConfigureAwait(false);
            if (!order.Success)
                return HttpResult.Fail<SharedId>(order);

            return HttpResult.Ok(order, new SharedId(request.OrderId));

        }
        #endregion
    }
}
