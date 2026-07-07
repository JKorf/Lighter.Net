using CryptoExchange.Net;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using Lighter.Net.Enums;
using Lighter.Net.Interfaces.Clients.ExchangeApi;
using Lighter.Net.Objects.Internal;
using Lighter.Net.Objects.Models;
using Lighter.Net.Objects.Sockets;
using Lighter.Net.Objects.Sockets.Subscriptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Lighter.Net.Clients.ExchangeApi
{
    /// <inheritdoc />
    internal class LighterSocketClientExchangeApiTrading : ILighterSocketClientExchangeApiTrading
    {
        private readonly ILogger _logger;
        private readonly LighterSocketClientExchangeApi _baseClient;
        private LighterRestClient? _restClient;

        internal LighterSocketClientExchangeApiTrading(ILogger logger, LighterSocketClientExchangeApi baseClient)
        {
            _logger = logger;
            _baseClient = baseClient;
        }

        /// <inheritdoc />
        public async Task<WebSocketResult<UpdateSubscription>> SubscribeToOrderUpdatesAsync(
            long? accountIndex, Action<DataEvent<LighterOrderUpdate>> onMessage, CancellationToken ct = default)
        {
            var internalHandler = new Action<DateTime, string?, LighterOrderUpdate>((receiveTime, originalData, data) =>
            {
                onMessage(
                    new DataEvent<LighterOrderUpdate>(LighterExchange.ExchangeName, data, receiveTime, originalData)
                        .WithStreamId(data.Channel)
                        .WithUpdateType(data.Type == "subscribed/account_all_orders" ? SocketUpdateType.Snapshot : SocketUpdateType.Update)
                    );
            });

            var subscription = new LighterSubscription<LighterOrderUpdate>(_baseClient, _logger, "account_all_orders", $"/{accountIndex ?? _baseClient.ApiCredentials?.Credential.AccountIndex}", internalHandler, true);
            return await _baseClient.SubscribeAsync(subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebSocketResult<UpdateSubscription>> SubscribeToUserTradeUpdatesAsync(
            long? accountIndex, Action<DataEvent<LighterUserTradeUpdate>> onMessage, CancellationToken ct = default)
        {
            var internalHandler = new Action<DateTime, string?, LighterUserTradeUpdate>((receiveTime, originalData, data) =>
            {
                onMessage(
                    new DataEvent<LighterUserTradeUpdate>(LighterExchange.ExchangeName, data, receiveTime, originalData)
                        .WithStreamId(data.Channel)
                        .WithUpdateType(data.Type == "subscribed/account_all_trades" ? SocketUpdateType.Snapshot : SocketUpdateType.Update)
                    );
            });

            var subscription = new LighterSubscription<LighterUserTradeUpdate>(_baseClient, _logger, "account_all_trades", $"/{accountIndex ?? _baseClient.ApiCredentials?.Credential.AccountIndex}", internalHandler, false);
            return await _baseClient.SubscribeAsync(subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebSocketResult<UpdateSubscription>> SubscribeToPositionUpdatesAsync(
            long? accountIndex, Action<DataEvent<LighterPositionUpdate>> onMessage, CancellationToken ct = default)
        {
            var internalHandler = new Action<DateTime, string?, LighterPositionUpdate>((receiveTime, originalData, data) =>
            {
                onMessage(
                    new DataEvent<LighterPositionUpdate>(LighterExchange.ExchangeName, data, receiveTime, originalData)
                        .WithStreamId(data.Channel)
                        .WithUpdateType(data.Type == "subscribed/account_all_positions" ? SocketUpdateType.Snapshot : SocketUpdateType.Update)
                    );
            });

            var subscription = new LighterSubscription<LighterPositionUpdate>(_baseClient, _logger, "account_all_positions", $"/{accountIndex ?? _baseClient.ApiCredentials?.Credential.AccountIndex}", internalHandler, false);
            return await _baseClient.SubscribeAsync(subscription, ct).ConfigureAwait(false);
        }


        #region Place Order

        /// <inheritdoc />
        public async Task<QueryResult<LighterSocketTransactionResult>> PlaceOrderAsync(
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
                return QueryResult.Fail<LighterSocketTransactionResult>(_baseClient.Exchange, new NoApiCredentialsError());

            await LighterUtils.CheckBuilderFeeAsync(_restClient ??= new LighterRestClient(opts =>
            {
                opts.ApiCredentials = _baseClient.ApiCredentials;
                opts.Environment = _baseClient.ClientOptions.Environment;
                opts.Proxy = _baseClient.ClientOptions.Proxy;
            })).ConfigureAwait(false);

            var symbolInfo = await _baseClient.GetSymbolInfoAsync(symbol).ConfigureAwait(false);
            if (!symbolInfo.Success)
                return QueryResult.Fail<LighterSocketTransactionResult>(_baseClient.Exchange, symbolInfo.Error!);

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

            var query = new LighterRequestQuery<LighterSocketTransactionResult, LighterTxRequest>(_baseClient, "jsonapi/sendtx", new LighterTxRequest
            {
                TransactionType = signedTx.TxType,
                TransactionInfo = signedTx.TxInfo
            }, false);
            return await _baseClient.QueryAsync(query, ct).ConfigureAwait(false);
        }

        #endregion

        #region Place Multiple Orders

        /// <inheritdoc />
        public async Task<QueryResult<LighterSocketTransactionsResult>> PlaceMultipleOrdersAsync(
            IEnumerable<LighterOrderRequest> orders,
            long? nonce = null,
            CancellationToken ct = default)
        {
            if (!_baseClient.Authenticated)
                return QueryResult.Fail<LighterSocketTransactionsResult>(_baseClient.Exchange, new NoApiCredentialsError());

            await LighterUtils.CheckBuilderFeeAsync(_restClient ??= new LighterRestClient(opts =>
            {
                opts.ApiCredentials = _baseClient.ApiCredentials;
                opts.Environment = _baseClient.ClientOptions.Environment;
                opts.Proxy = _baseClient.ClientOptions.Proxy;
            })).ConfigureAwait(false);

            nonce ??= await _baseClient.GetNoncesAsync(orders.Count()).ConfigureAwait(false);

            var txTypes = new List<int>();
            var txInfos = new List<string>();
            foreach (var order in orders)
            {
                var symbolInfo = await _baseClient.GetSymbolInfoAsync(order.Symbol).ConfigureAwait(false);
                if (!symbolInfo.Success)
                    return QueryResult.Fail<LighterSocketTransactionsResult>(_baseClient.Exchange, symbolInfo.Error!);

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

#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning disable IL2026 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
            var query = new LighterRequestQuery<LighterSocketTransactionsResult, LighterBatchTxRequest>(_baseClient, "jsonapi/sendtx", new LighterBatchTxRequest
            {
                TransactionTypes = JsonSerializer.Serialize(txTypes.ToArray(), LighterExchange._serializerContext),
                TransactionInfos = JsonSerializer.Serialize(txInfos.ToArray(), LighterExchange._serializerContext)
            }, false);
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2026 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
            return await _baseClient.QueryAsync(query, ct).ConfigureAwait(false);
        }

        #endregion

        #region Edit Order

        /// <inheritdoc />
        public async Task<QueryResult<LighterSocketTransactionResult>> EditOrderAsync(
            string symbol,
            long orderIndex,
            decimal quantity,
            decimal price,
            decimal? triggerPrice = null,
            long? nonce = null,
            CancellationToken ct = default)
        {
            if (!_baseClient.Authenticated)
                return QueryResult.Fail<LighterSocketTransactionResult>(_baseClient.Exchange, new NoApiCredentialsError());

            var symbolInfo = await _baseClient.GetSymbolInfoAsync(symbol).ConfigureAwait(false);
            if (!symbolInfo.Success)
                return QueryResult.Fail<LighterSocketTransactionResult>(_baseClient.Exchange, symbolInfo.Error!);

            nonce ??= await _baseClient.GetNonceAsync().ConfigureAwait(false);

            var actPrice = (int)(price * ((decimal)Math.Pow(10, symbolInfo.Data.SupportedPriceDecimals)));
            var actTriggerPrice = (int?)(triggerPrice * ((decimal)Math.Pow(10, symbolInfo.Data.SupportedPriceDecimals)));
            var actQuantity = (long)(quantity * ((decimal)Math.Pow(10, symbolInfo.Data.SupportedQuantityDecimals)));

            var signedTx = LighterUtils.GetSigner(_baseClient.ClientOptions.LibraryPath, _baseClient.BaseAddress, _baseClient.ApiCredentials!).SignModifyOrder(
                symbolInfo.Data.MarketId,
                orderIndex,
                actQuantity,
                actPrice,
                actTriggerPrice ?? 0,
                0x1,
                nonce.Value,
                _baseClient.ApiCredentials!.Credential!.ApiKeyIndex,
                _baseClient.ApiCredentials!.Credential!.AccountIndex);

            var query = new LighterRequestQuery<LighterSocketTransactionResult, LighterTxRequest>(_baseClient, "jsonapi/sendtx", new LighterTxRequest
            {
                TransactionType = signedTx.TxType,
                TransactionInfo = signedTx.TxInfo
            }, false);
            return await _baseClient.QueryAsync(query, ct).ConfigureAwait(false);
        }

        #endregion

        #region Cancel Order

        /// <inheritdoc />
        public async Task<QueryResult<LighterSocketTransactionResult>> CancelOrderAsync(
            string symbol,
            long orderIndex,
            long? nonce,
            CancellationToken ct = default)
        {
            if (!_baseClient.Authenticated)
                return QueryResult.Fail<LighterSocketTransactionResult>(_baseClient.Exchange, new NoApiCredentialsError());

            var symbolInfo = await _baseClient.GetSymbolInfoAsync(symbol).ConfigureAwait(false);
            if (!symbolInfo.Success)
                return QueryResult.Fail<LighterSocketTransactionResult>(_baseClient.Exchange, symbolInfo.Error!);

            nonce ??= await _baseClient.GetNonceAsync().ConfigureAwait(false);

            var signedTx = LighterUtils.GetSigner(_baseClient.ClientOptions.LibraryPath, _baseClient.BaseAddress, _baseClient.ApiCredentials!).SignCancelOrder(
                symbolInfo.Data.MarketId,
                orderIndex,
                0x1,
                nonce.Value,
                _baseClient.ApiCredentials!.Credential!.ApiKeyIndex,
                _baseClient.ApiCredentials!.Credential!.AccountIndex);

            var query = new LighterRequestQuery<LighterSocketTransactionResult, LighterTxRequest>(_baseClient, "jsonapi/sendtx", new LighterTxRequest
            {
                TransactionType = signedTx.TxType,
                TransactionInfo = signedTx.TxInfo
            }, false);
            return await _baseClient.QueryAsync(query, ct).ConfigureAwait(false);
        }

        #endregion

        #region Cancel All Orders

        /// <inheritdoc />
        public async Task<QueryResult<LighterSocketTransactionResult>> CancelAllOrdersAsync(
            TimeInForce timeInForce,
            long? nonce,
            CancellationToken ct = default)
        {
            if (!_baseClient.Authenticated)
                return QueryResult.Fail<LighterSocketTransactionResult>(_baseClient.Exchange, new NoApiCredentialsError());

            nonce ??= await _baseClient.GetNonceAsync().ConfigureAwait(false);

            var signedTx = LighterUtils.GetSigner(_baseClient.ClientOptions.LibraryPath, _baseClient.BaseAddress, _baseClient.ApiCredentials!).SignCancelAllOrders(
                (int)timeInForce,
                0,
                0x1,
                nonce.Value,
                _baseClient.ApiCredentials!.Credential!.ApiKeyIndex,
                _baseClient.ApiCredentials!.Credential!.AccountIndex);

            var query = new LighterRequestQuery<LighterSocketTransactionResult, LighterTxRequest>(_baseClient, "jsonapi/sendtx", new LighterTxRequest
            {
                TransactionType = signedTx.TxType,
                TransactionInfo = signedTx.TxInfo
            }, false);
            return await _baseClient.QueryAsync(query, ct).ConfigureAwait(false);
        }

        #endregion
    }
}
