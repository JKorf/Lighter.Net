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
using System.Threading;
using System.Threading.Tasks;

namespace Lighter.Net.Clients.ExchangeApi
{
    /// <inheritdoc />
    internal class LighterSocketClientExchangeApiAccount : ILighterSocketClientExchangeApiAccount
    {
        private readonly LighterSocketClientExchangeApi _baseClient;
        private readonly ILogger _logger;

        internal LighterSocketClientExchangeApiAccount(ILogger logger, LighterSocketClientExchangeApi baseClient)
        {
            _logger = logger;
            _baseClient = baseClient;
        }

        /// <inheritdoc />
        public async Task<WebSocketResult<UpdateSubscription>> SubscribeToAccountUpdatesAsync(
            long? accountIndex, Action<DataEvent<LighterAccountUpdate>> onMessage, CancellationToken ct = default)
        {
            var internalHandler = new Action<DateTime, string?, LighterAccountUpdate>((receiveTime, originalData, data) =>
            {
                onMessage(
                    new DataEvent<LighterAccountUpdate>(LighterExchange.ExchangeName, data, receiveTime, originalData)
                        .WithStreamId(data.Channel)
                        .WithUpdateType(data.Type == "subscribed/account_all" ? SocketUpdateType.Snapshot : SocketUpdateType.Update)
                    );
            });

            var subscription = new LighterSubscription<LighterAccountUpdate>(_baseClient, _logger, "account_all", $"/{accountIndex ?? _baseClient.ApiCredentials?.Credential.AccountIndex}", internalHandler, false);
            return await _baseClient.SubscribeAsync(subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebSocketResult<UpdateSubscription>> SubscribeToUserStatsUpdatesAsync(
            long? accountIndex, Action<DataEvent<LighterUserStatsUpdate>> onMessage, CancellationToken ct = default)
        {
            var internalHandler = new Action<DateTime, string?, LighterUserStatsUpdate>((receiveTime, originalData, data) =>
            {
                _baseClient.UpdateTimeOffset(data.Timestamp);

                onMessage(
                    new DataEvent<LighterUserStatsUpdate>(LighterExchange.ExchangeName, data, receiveTime, originalData)
                        .WithStreamId(data.Channel)
                        .WithUpdateType(data.Type == "subscribed/user_stats" ? SocketUpdateType.Snapshot : SocketUpdateType.Update)
                        .WithDataTimestamp(data.Timestamp, _baseClient.GetTimeOffset())
                    );
            });

            var subscription = new LighterSubscription<LighterUserStatsUpdate>(_baseClient, _logger, "user_stats", $"/{accountIndex ?? _baseClient.ApiCredentials?.Credential.AccountIndex}", internalHandler, false);
            return await _baseClient.SubscribeAsync(subscription, ct).ConfigureAwait(false);
        }


        /// <inheritdoc />
        public async Task<WebSocketResult<UpdateSubscription>> SubscribeToBalanceUpdatesAsync(
            long? accountIndex, Action<DataEvent<LighterBalancesUpdate>> onMessage, CancellationToken ct = default)
        {
            var internalHandler = new Action<DateTime, string?, LighterBalancesUpdate>((receiveTime, originalData, data) =>
            {
                _baseClient.UpdateTimeOffset(data.Timestamp);

                onMessage(
                    new DataEvent<LighterBalancesUpdate>(LighterExchange.ExchangeName, data, receiveTime, originalData)
                        .WithStreamId(data.Channel)
                        .WithUpdateType(data.Type == "subscribed/account_all_assets" ? SocketUpdateType.Snapshot : SocketUpdateType.Update)
                        .WithDataTimestamp(data.Timestamp, _baseClient.GetTimeOffset())
                    );
            });

            var subscription = new LighterSubscription<LighterBalancesUpdate>(_baseClient, _logger, "account_all_assets", $"/{accountIndex ?? _baseClient.ApiCredentials?.Credential.AccountIndex}", internalHandler, true);
            return await _baseClient.SubscribeAsync(subscription, ct).ConfigureAwait(false);
        }

        #region Set Leverage
        /// <inheritdoc />
        public async Task<QueryResult<LighterSocketTransactionResult>> SetLeverageAsync(
            string symbol,
            int leverage,
            MarginMode marginMode,
            long? nonce = null,
            CancellationToken ct = default)
        {
            if (!_baseClient.Authenticated)
                return QueryResult.Fail<LighterSocketTransactionResult>(_baseClient.Exchange, new NoApiCredentialsError());

            var symbolInfo = await _baseClient.GetSymbolInfoAsync(symbol).ConfigureAwait(false);
            if (!symbolInfo.Success)
                return QueryResult.Fail<LighterSocketTransactionResult>(_baseClient.Exchange, symbolInfo.Error!);

            nonce ??= await _baseClient.GetNonceAsync().ConfigureAwait(false);
            var imf = (int)(10_000 / leverage);
            var signedTx = LighterUtils.GetSigner(_baseClient.ClientOptions.LibraryPath, _baseClient.BaseAddress, _baseClient.ApiCredentials!).SignUpdateLeverage(
                symbolInfo.Data.MarketId,
                imf,
                (int)marginMode,
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

        #region Update Margin
        /// <inheritdoc />
        public async Task<QueryResult<LighterSocketTransactionResult>> UpdateMarginAsync(
            string symbol,
            decimal usdcAmount,
            bool addOrRemove,
            long? nonce,
            CancellationToken ct = default)
        {
            if (!_baseClient.Authenticated)
                return QueryResult.Fail<LighterSocketTransactionResult>(_baseClient.Exchange, new NoApiCredentialsError());

            var symbolInfo = await _baseClient.GetSymbolInfoAsync(symbol).ConfigureAwait(false);
            if (!symbolInfo.Success)
                return QueryResult.Fail<LighterSocketTransactionResult>(_baseClient.Exchange, symbolInfo.Error!);

            nonce ??= await _baseClient.GetNonceAsync().ConfigureAwait(false);

            var signedTx = LighterUtils.GetSigner(_baseClient.ClientOptions.LibraryPath, _baseClient.BaseAddress, _baseClient.ApiCredentials!).SignUpdateMargin(
                symbolInfo.Data.MarketId,
                (int)(usdcAmount * LighterUtils.UsdcMultiplier),
                addOrRemove ? 1 : 0,
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
