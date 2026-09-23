using CryptoExchange.Net;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Objects;
using Lighter.Net.Enums;
using Lighter.Net.Interfaces.Clients.ExchangeApi;
using Lighter.Net.Objects.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Lighter.Net.Clients.ExchangeApi
{
    /// <inheritdoc />
    internal class LighterRestClientExchangeApiTrading : ILighterRestClientExchangeApiTrading
    {
        private static readonly RequestDefinitionCache _definitions = new RequestDefinitionCache();
        private readonly LighterRestClientExchangeApi _baseClient;
        private readonly ILogger _logger;

        internal LighterRestClientExchangeApiTrading(ILogger logger, LighterRestClientExchangeApi baseClient)
        {
            _baseClient = baseClient;
            _logger = logger;
        }

        #region Place Order

        /// <inheritdoc />
        public async Task<HttpResult<LighterTransactionResult>> PlaceOrderAsync(
            string symbol,
            OrderSide side,
            OrderType orderType,
            decimal quantity,
            decimal price,
            TimeInForce timeInForce,
            long? clientOrderIndex,
            bool? reduceOnly = null,
            decimal? triggerPrice = null,
            DateTime? orderExpiry = null,
            long? nonce = null,
            CancellationToken ct = default)
        {
            if (!_baseClient.Authenticated)
                return HttpResult.Fail<LighterTransactionResult>(_baseClient.Exchange, new NoApiCredentialsError());

            await LighterUtils.CheckBuilderFeeAsync(_baseClient._baseClient).ConfigureAwait(false);

            var symbolInfo = await _baseClient.GetSymbolInfoAsync(symbol).ConfigureAwait(false);
            if (!symbolInfo.Success)
                return HttpResult.Fail<LighterTransactionResult>(_baseClient.Exchange, symbolInfo.Error!);

            nonce ??= await _baseClient.GetNonceAsync().ConfigureAwait(false);

            var actPrice = (int)(price * ((decimal)Math.Pow(10, symbolInfo.Data.SupportedPriceDecimals)));
            var actTriggerPrice = (int?)(triggerPrice * ((decimal)Math.Pow(10, symbolInfo.Data.SupportedPriceDecimals)));
            var actQuantity = (long)(quantity * ((decimal)Math.Pow(10, symbolInfo.Data.SupportedQuantityDecimals)));
            var actClientIndex = clientOrderIndex ?? ExchangeHelpers.RandomLong(9);
            DateTime? actExpiry = timeInForce == TimeInForce.GoodTillTime ? (orderExpiry ?? DateTime.UtcNow.AddDays(28)) : null;

            int integratorFeePercentage = 0;
            long integratorFeeAccountIndex = 0;
            if (_baseClient.ClientOptions.IntegratorFeePercentage > 0
                && _baseClient.ClientOptions.IntegratorAccountIndex != null
                && LighterUtils.IntegratorFeeUsable(_baseClient.ApiCredentials!))
            {
                integratorFeeAccountIndex = _baseClient.ClientOptions.IntegratorAccountIndex.Value;
                integratorFeePercentage = (int)(_baseClient.ClientOptions.IntegratorFeePercentage.Value * 10000);
            }

            var signedTx = LighterUtils.GetSigner(_baseClient.ClientOptions.LibraryPath, _baseClient.BaseAddress, _baseClient.ApiCredentials!).SignCreateOrder(
                symbolInfo.Data.MarketId,
                actClientIndex,
                actQuantity,
                actPrice,
                (int)side,
                (int)orderType,
                (int)timeInForce,
                reduceOnly == true ? 1 : 0,
                actTriggerPrice ?? 0,
                DateTimeConverter.ConvertToMilliseconds(actExpiry) ?? 0,
                integratorFeeAccountIndex,
                integratorFeePercentage,
                integratorFeePercentage,
                0x1,
                nonce.Value,
                _baseClient.ApiCredentials!.Credential!.ApiKeyIndex,
                _baseClient.ApiCredentials!.Credential!.AccountIndex);

            var body = new Parameters(LighterExchange._parameterSerializationSettings);
            body.Add("tx_type", signedTx.TxType);
            body.Add("tx_info", signedTx.TxInfo);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "/api/v1/sendTx", LighterExchange.RateLimiter.LighterRest, 6, false);
            return await _baseClient.SendAsync<LighterTransactionResult>(request, body, ct).ConfigureAwait(false);
        }

        #endregion

        #region Place Multiple Orders

        /// <inheritdoc />
        public async Task<HttpResult<LighterTransactionsResult>> PlaceMultipleOrdersAsync(
            IEnumerable<LighterOrderRequest> orders,
            long? nonce = null,
            CancellationToken ct = default)
        {
            if (!_baseClient.Authenticated)
                return HttpResult.Fail<LighterTransactionsResult>(_baseClient.Exchange, new NoApiCredentialsError());

            await LighterUtils.CheckBuilderFeeAsync(_baseClient._baseClient).ConfigureAwait(false);

            nonce ??= await _baseClient.GetNoncesAsync(orders.Count()).ConfigureAwait(false);

            var txTypes = new List<int>();
            var txInfos = new List<string>();
            foreach (var order in orders)
            {
                var symbolInfo = await _baseClient.GetSymbolInfoAsync(order.Symbol).ConfigureAwait(false);
                if (!symbolInfo.Success)
                    return HttpResult.Fail<LighterTransactionsResult>(_baseClient.Exchange, symbolInfo.Error!);

                var actPrice = (int)(order.Price * ((decimal)Math.Pow(10, symbolInfo.Data.SupportedPriceDecimals)));
                var actTriggerPrice = (int?)(order.TriggerPrice * ((decimal)Math.Pow(10, symbolInfo.Data.SupportedPriceDecimals)));
                var actQuantity = (long)(order.Quantity * ((decimal)Math.Pow(10, symbolInfo.Data.SupportedQuantityDecimals)));
                var actClientIndex = order.ClientOrderIndex ?? ExchangeHelpers.RandomLong(9);
                DateTime? actExpiry = order.TimeInForce == TimeInForce.GoodTillTime ? (order.ExpireTime ?? DateTime.UtcNow.AddDays(28)) : null;

                int integratorFeePercentage = 0;
                long integratorFeeAccountIndex = 0;
                if (_baseClient.ClientOptions.IntegratorFeePercentage > 0
                    && _baseClient.ClientOptions.IntegratorAccountIndex != null
                    && LighterUtils.IntegratorFeeUsable(_baseClient.ApiCredentials!))
                {
                    integratorFeeAccountIndex = _baseClient.ClientOptions.IntegratorAccountIndex.Value;
                    integratorFeePercentage = (int)(_baseClient.ClientOptions.IntegratorFeePercentage.Value * 10000);
                }

                var signedTx = LighterUtils.GetSigner(_baseClient.ClientOptions.LibraryPath, _baseClient.BaseAddress, _baseClient.ApiCredentials!).SignCreateOrder(
                    symbolInfo.Data.MarketId,
                    actClientIndex,
                    actQuantity,
                    actPrice,
                    (int)order.Side,
                    (int)order.OrderType,
                    (int)order.TimeInForce,
                    order.ReduceOnly == true ? 1 : 0,
                    actTriggerPrice ?? 0,
                    DateTimeConverter.ConvertToMilliseconds(actExpiry) ?? 0,
                    integratorFeeAccountIndex,
                    integratorFeePercentage,
                    integratorFeePercentage,
                    0x1,
                    nonce.Value,
                    _baseClient.ApiCredentials!.Credential!.ApiKeyIndex,
                    _baseClient.ApiCredentials!.Credential!.AccountIndex);

                txTypes.Add(signedTx.TxType);
                txInfos.Add(signedTx.TxInfo);
                nonce += 1;
            }

            var body = new Parameters(LighterExchange._parameterSerializationSettings);
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning disable IL2026 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
            body.Add("tx_types", JsonSerializer.Serialize(txTypes.ToArray(), LighterExchange._serializerContext));
            body.Add("tx_infos", JsonSerializer.Serialize(txInfos.ToArray(), LighterExchange._serializerContext));
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2026 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "/api/v1/sendTxBatch", LighterExchange.RateLimiter.LighterRest, 6, false);
            return await _baseClient.SendAsync<LighterTransactionsResult>(request, body, ct).ConfigureAwait(false);
        }

        #endregion

        #region Edit Order

        /// <inheritdoc />
        public async Task<HttpResult<LighterTransactionResult>> EditOrderAsync(
            string symbol,
            long orderIndex,
            decimal quantity,
            decimal price,
            decimal? triggerPrice = null,
            long? nonce = null,
            CancellationToken ct = default)
        {
            if (!_baseClient.Authenticated)
                return HttpResult.Fail<LighterTransactionResult>(_baseClient.Exchange, new NoApiCredentialsError());

            var symbolInfo = await _baseClient.GetSymbolInfoAsync(symbol).ConfigureAwait(false);
            if (!symbolInfo.Success)
                return HttpResult.Fail<LighterTransactionResult>(_baseClient.Exchange, symbolInfo.Error!);

            nonce ??= await _baseClient.GetNonceAsync().ConfigureAwait(false);

            var actPrice = (int)(price * ((decimal)Math.Pow(10, symbolInfo.Data.SupportedPriceDecimals)));
            var actTriggerPrice = (int?)(triggerPrice * ((decimal)Math.Pow(10, symbolInfo.Data.SupportedPriceDecimals)));
            var actQuantity = (long)(quantity * ((decimal)Math.Pow(10, symbolInfo.Data.SupportedQuantityDecimals)));

            int integratorFeePercentage = 0;
            long integratorFeeAccountIndex = 0;
            if (_baseClient.ClientOptions.IntegratorFeePercentage > 0
                && _baseClient.ClientOptions.IntegratorAccountIndex != null
                && LighterUtils.IntegratorFeeUsable(_baseClient.ApiCredentials!))
            {
                integratorFeeAccountIndex = _baseClient.ClientOptions.IntegratorAccountIndex.Value;
                integratorFeePercentage = (int)(_baseClient.ClientOptions.IntegratorFeePercentage.Value * 10000);
            }

            var signedTx = LighterUtils.GetSigner(_baseClient.ClientOptions.LibraryPath, _baseClient.BaseAddress, _baseClient.ApiCredentials!).SignModifyOrder(
                symbolInfo.Data.MarketId,
                orderIndex,
                actQuantity,
                actPrice,
                actTriggerPrice ?? 0,
                integratorFeeAccountIndex,
                integratorFeePercentage,
                integratorFeePercentage,
                0x1,
                nonce.Value,
                _baseClient.ApiCredentials!.Credential!.ApiKeyIndex,
                _baseClient.ApiCredentials!.Credential!.AccountIndex);

            var body = new Parameters(LighterExchange._parameterSerializationSettings);
            body.Add("tx_type", signedTx.TxType);
            body.Add("tx_info", signedTx.TxInfo);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "/api/v1/sendTx", LighterExchange.RateLimiter.LighterRest, 6, false);
            return await _baseClient.SendAsync<LighterTransactionResult>(request, body, ct).ConfigureAwait(false);
        }

        #endregion

        #region Cancel Order

        /// <inheritdoc />
        public async Task<HttpResult<LighterTransactionResult>> CancelOrderAsync(
            string symbol,
            long orderIndex,            
            long? nonce,
            CancellationToken ct = default)
        {
            if (!_baseClient.Authenticated)
                return HttpResult.Fail<LighterTransactionResult>(_baseClient.Exchange, new NoApiCredentialsError());

            var symbolInfo = await _baseClient.GetSymbolInfoAsync(symbol).ConfigureAwait(false);
            if (!symbolInfo.Success)
                return HttpResult.Fail<LighterTransactionResult>(_baseClient.Exchange, symbolInfo.Error!);

            nonce ??= await _baseClient.GetNonceAsync().ConfigureAwait(false);

            var signedTx = LighterUtils.GetSigner(_baseClient.ClientOptions.LibraryPath, _baseClient.BaseAddress, _baseClient.ApiCredentials!).SignCancelOrder(
                symbolInfo.Data.MarketId,
                orderIndex,                
                0x1,
                nonce.Value,
                _baseClient.ApiCredentials!.Credential!.ApiKeyIndex,
                _baseClient.ApiCredentials!.Credential!.AccountIndex);

            var body = new Parameters(LighterExchange._parameterSerializationSettings);
            body.Add("tx_type", signedTx.TxType);
            body.Add("tx_info", signedTx.TxInfo);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "/api/v1/sendTx", LighterExchange.RateLimiter.LighterRest, 6, false);
            return await _baseClient.SendAsync<LighterTransactionResult>(request, body, ct).ConfigureAwait(false);
        }

        #endregion

        #region Cancel All Orders

        /// <inheritdoc />
        public async Task<HttpResult<LighterTransactionResult>> CancelAllOrdersAsync(
            TimeInForce timeInForce,
            long? nonce,
            CancellationToken ct = default)
        {
            if (!_baseClient.Authenticated)
                return HttpResult.Fail<LighterTransactionResult>(_baseClient.Exchange, new NoApiCredentialsError());

            nonce ??= await _baseClient.GetNonceAsync().ConfigureAwait(false);

            var signedTx = LighterUtils.GetSigner(_baseClient.ClientOptions.LibraryPath, _baseClient.BaseAddress, _baseClient.ApiCredentials!).SignCancelAllOrders(
                (int)timeInForce,
                0,
                0x1,
                nonce.Value,
                _baseClient.ApiCredentials!.Credential!.ApiKeyIndex,
                _baseClient.ApiCredentials!.Credential!.AccountIndex);

            var body = new Parameters(LighterExchange._parameterSerializationSettings);
            body.Add("tx_type", signedTx.TxType);
            body.Add("tx_info", signedTx.TxInfo);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "/api/v1/sendTx", LighterExchange.RateLimiter.LighterRest, 6, false);
            return await _baseClient.SendAsync<LighterTransactionResult>(request, body, ct).ConfigureAwait(false);
        }

        #endregion

        #region Get Open Orders

        /// <inheritdoc />
        public async Task<HttpResult<LighterOrders>> GetOpenOrdersAsync(
            long? accountIndex = null,
            string? symbol = null,
            MarketType? marketType = null,
            CancellationToken ct = default)
        {
            LighterSymbol? symbolInfo = null;
            if (symbol != null)
            {
                var symbolInfoResult = await _baseClient.GetSymbolInfoAsync(symbol).ConfigureAwait(false);
                if (!symbolInfoResult.Success)
                    return HttpResult.Fail<LighterOrders>(_baseClient.Exchange, symbolInfoResult.Error!);

                symbolInfo = symbolInfoResult.Data;
            }

            var parameters = new Parameters(LighterExchange._parameterSerializationSettings);
            parameters.Add("market_id", symbolInfo?.MarketId);
            parameters.Add("market_type", marketType);
            parameters.Add("account_index", accountIndex ?? _baseClient.ApiCredentials?.Credential?.AccountIndex);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "/api/v1/accountActiveOrders", LighterExchange.RateLimiter.LighterRest, 300, true);
            var result = await _baseClient.SendAsync<LighterOrders>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Closed Orders

        /// <inheritdoc />
        public async Task<HttpResult<LighterOrders>> GetClosedOrdersAsync(
            long? accountIndex = null,
            string? symbol = null,
            TradeMode? mode = null,
            OrderSide? side = null,
            // Unclear how to send the timestamps
            //DateTime? startTime = null,
            //DateTime? endTime = null,
            int? limit = null,
            string? cursor = null,
            CancellationToken ct = default)
        {
            LighterSymbol? symbolInfo = null;
            if (symbol != null)
            {
                var symbolInfoResult = await _baseClient.GetSymbolInfoAsync(symbol).ConfigureAwait(false);
                if (!symbolInfoResult.Success)
                    return HttpResult.Fail<LighterOrders>(_baseClient.Exchange, symbolInfoResult.Error!);

                symbolInfo = symbolInfoResult.Data;
            }

            var parameters = new Parameters(LighterExchange._parameterSerializationSettings);
            parameters.Add("account_index", accountIndex ?? _baseClient.ApiCredentials?.Credential?.AccountIndex);
            parameters.Add("market_id", symbolInfo?.MarketId);
            parameters.Add("market_type", mode);
            parameters.Add("ask_filter", side == null ? null : side == OrderSide.Buy ? 1 : 0);
            // Unclear how to add
            //if (startTime != null && endTime != null)
            //    parameters.Add("between_timestamps", $"{DateTimeConverter.ConvertToMilliseconds(startTime)} {DateTimeConverter.ConvertToMilliseconds(endTime)}");
            parameters.Add("limit", limit ?? 100);
            parameters.Add("cursor", cursor);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "api/v1/accountInactiveOrders", LighterExchange.RateLimiter.LighterRest, 100, true);
            var result = await _baseClient.SendAsync<LighterOrders>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get User Trades

        /// <inheritdoc />
        public async Task<HttpResult<LighterUserTrades>> GetUserTradesAsync(
            long? accountIndex = null,
            string? symbol = null,
            MarketType? marketType = null,
            long? orderIndex = null,
            long? fromId = null,
            bool? aggregate = null,
            TradeType? tradeType = null,
            OrderSide? side = null,
            TradeRole? role = null,
            string? cursor = null,
            TradeSort? sort = null,
            int? limit = null,
            CancellationToken ct = default)
        {
            LighterSymbol? symbolInfo = null;
            if (symbol != null)
            {
                var symbolInfoResult = await _baseClient.GetSymbolInfoAsync(symbol).ConfigureAwait(false);
                if (!symbolInfoResult.Success)
                    return HttpResult.Fail<LighterUserTrades>(_baseClient.Exchange, symbolInfoResult.Error!);

                symbolInfo = symbolInfoResult.Data;
            }

            var parameters = new Parameters(LighterExchange._parameterSerializationSettings);
            parameters.Add("account_index", accountIndex ?? _baseClient.ApiCredentials?.Credential?.AccountIndex);
            parameters.Add("market_id", symbolInfo?.MarketId);
            parameters.Add("market_type", marketType);
            parameters.Add("order_index", orderIndex);
            parameters.Add("from", fromId);
            parameters.Add("aggregate", aggregate);
            parameters.Add("type", tradeType);
            parameters.Add("ask_filter", side == null ? null : side == OrderSide.Buy ? 1 : 0);
            parameters.Add("role", role);
            parameters.Add("cursor", cursor);
            parameters.Add("sort_by", sort ?? TradeSort.Timestamp);
            parameters.Add("limit", limit ?? 100);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "api/v1/trades", LighterExchange.RateLimiter.LighterRest, 600, true);
            var result = await _baseClient.SendAsync<LighterUserTrades>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

    }
}
