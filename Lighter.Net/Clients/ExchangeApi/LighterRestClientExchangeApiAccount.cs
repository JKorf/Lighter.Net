using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Objects;
using Lighter.Net.Enums;
using Lighter.Net.Interfaces.Clients.ExchangeApi;
using Lighter.Net.Objects.Models;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Lighter.Net.Clients.ExchangeApi
{
    /// <inheritdoc />
    internal class LighterRestClientExchangeApiAccount : ILighterRestClientExchangeApiAccount
    {
        private static readonly RequestDefinitionCache _definitions = new RequestDefinitionCache();
        private readonly LighterRestClientExchangeApi _baseClient;

        internal LighterRestClientExchangeApiAccount(LighterRestClientExchangeApi baseClient)
        {
            _baseClient = baseClient;
        }

        #region Get Accounts

        /// <inheritdoc />
        public async Task<HttpResult<LighterAccount>> GetAccountsByL1AddressAsync(
            string? address = null,
            string? cursor = null,
            CancellationToken ct = default)
        {
            var parameters = new Parameters(LighterExchange._parameterSerializationSettings);
            parameters.Add("l1_address", address ?? _baseClient.ApiCredentials?.Credential.Key);
            parameters.Add("cursor", cursor);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "/api/v1/accountsByL1Address", LighterExchange.RateLimiter.LighterRest, 300, false);
            var result = await _baseClient.SendAsync<LighterAccount>(request, parameters, ct).ConfigureAwait(false);
            if (result.Success && result.Data.Code != 200)
                return HttpResult.Fail<LighterAccount>(result, new ServerError(result.Data.Code, _baseClient.GetErrorInfo(result.Data.Code, result.Data.Message)));

            return result;
        }

        #endregion

        #region Generate Api key

        /// <inheritdoc />
        public CallResult<(string PublicKey, string PrivateKey)> GenerateApiKey()
        {
            var (apiKey, secret) = LighterUtils.GetSigner(_baseClient.ClientOptions.LibraryPath, _baseClient.BaseAddress, _baseClient.ApiCredentials!).GenerateAPIKey();
            return CallResult.Ok((apiKey, secret));
        }

        #endregion

        #region Get Nonce

        /// <inheritdoc />
        public async Task<HttpResult<LighterNonce>> GetNonceAsync(
            long? accountIndex = null,
            long? apiKeyIndex = null, 
            CancellationToken ct = default)
        {
            var parameters = new Parameters(LighterExchange._parameterSerializationSettings);
            parameters.Add("account_index", accountIndex ?? _baseClient.ApiCredentials?.Credential.AccountIndex);
            parameters.Add("api_key_index", apiKeyIndex ?? _baseClient.ApiCredentials?.Credential.ApiKeyIndex);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "/api/v1/nextNonce", LighterExchange.RateLimiter.LighterRest, 6, false);
            return await _baseClient.SendAsync<LighterNonce>(request, parameters, ct).ConfigureAwait(false);
        }

        #endregion

        #region Get Accounts

        /// <inheritdoc />
        public async Task<HttpResult<LighterAccounts>> GetAccountsAsync(
            AccountBy? by = null,
            string? value = null,
            bool? activeOnly = null,
            string? cursor = null,
            CancellationToken ct = default)
        {
            var parameters = new Parameters(LighterExchange._parameterSerializationSettings);
            parameters.Add("by", by ?? AccountBy.AccountIndex);
            parameters.Add("value", value ?? _baseClient.ApiCredentials?.Credential.AccountIndex.ToString());
            parameters.Add("active_only", activeOnly);
            parameters.Add("cursor", cursor);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "api/v1/account", LighterExchange.RateLimiter.LighterRest, 300, false);
            var result = await _baseClient.SendAsync<LighterAccounts>(request, parameters, ct, skipCheck: true).ConfigureAwait(false);
            if (result.Success && result.Data.Code != 200)
                return HttpResult.Fail<LighterAccounts>(result, new ServerError(result.Data.Code, _baseClient.GetErrorInfo(result.Data.Code, result.Data.Message)));

            return result;
        }

        #endregion

        #region Get Account Limits

        /// <inheritdoc />
        public async Task<HttpResult<LighterAccountLimits>> GetAccountLimitsAsync(long? accountIndex = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(LighterExchange._parameterSerializationSettings);
            parameters.Add("account_index", accountIndex ?? _baseClient.ApiCredentials?.Credential.AccountIndex);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "api/v1/accountLimits", LighterExchange.RateLimiter.LighterRest, 300, true);
            var result = await _baseClient.SendAsync<LighterAccountLimits>(request, parameters, ct).ConfigureAwait(false);
            if (result.Success && result.Data.Code != 200)
                return HttpResult.Fail<LighterAccountLimits>(result, new ServerError(result.Data.Code, _baseClient.GetErrorInfo(result.Data.Code, result.Data.Message)));

            return result;
        }

        #endregion

        #region Get Account Metadata

        /// <inheritdoc />
        public async Task<HttpResult<LighterAccountMetadata>> GetAccountMetadataAsync(
            AccountBy? by = null,
            string? value = null,
            string? cursor = null,
            CancellationToken ct = default)
        {
            var parameters = new Parameters(LighterExchange._parameterSerializationSettings);
            parameters.Add("by", by ?? AccountBy.AccountIndex);
            parameters.Add("value", value ?? _baseClient.ApiCredentials?.Credential.AccountIndex.ToString());
            parameters.Add("cursor", cursor);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "api/v1/accountMetadata", LighterExchange.RateLimiter.LighterRest, 300, true);
            var result = await _baseClient.SendAsync<LighterAccountMetadata>(request, parameters, ct).ConfigureAwait(false);
            if (result.Success && result.Data.Code != 200)
                return HttpResult.Fail<LighterAccountMetadata>(result, new ServerError(result.Data.Code, _baseClient.GetErrorInfo(result.Data.Code, result.Data.Message)));

            return result;
        }

        #endregion

        #region Get Pnl

        /// <inheritdoc />
        public async Task<HttpResult<LighterPnl>> GetPnlAsync(
            Resolution resolution,
            AccountBy? by = null,
            string? value = null,
            bool? ignoreTransfers = null,
            DateTime? startTime = null,
            DateTime? endTime = null,
            int? limit = null,
            CancellationToken ct = default)
        {
            var parameters = new Parameters(LighterExchange._parameterSerializationSettings);
            parameters.Add("by", by ?? AccountBy.AccountIndex);
            parameters.Add("value", value ?? _baseClient.ApiCredentials?.Credential.AccountIndex.ToString());
            parameters.Add("resolution", resolution);
            parameters.Add("ignore_transfers", ignoreTransfers);
            parameters.Add("start_timestamp", startTime ?? DateTime.UtcNow.AddDays(-7));
            parameters.Add("end_timestamp", endTime ?? DateTime.UtcNow);
            parameters.Add("count_back", limit ?? 100);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "api/v1/pnl", LighterExchange.RateLimiter.LighterRest, 300, true);
            var result = await _baseClient.SendAsync<LighterPnl>(request, parameters, ct).ConfigureAwait(false);
            if (result.Success && result.Data.Code != 200)
                return HttpResult.Fail<LighterPnl>(result, new ServerError(result.Data.Code, _baseClient.GetErrorInfo(result.Data.Code, result.Data.Message)));

            return result;
        }

        #endregion

        #region Set Account Tier

        /// <inheritdoc />
        public async Task<HttpResult<LighterResponse>> SetAccountTierAsync(
            long? accountIndex,
            AccountTier newTier,
            CancellationToken ct = default)
        {
            var parameters = new Parameters(LighterExchange._parameterSerializationSettings);
            parameters.Add("account_index", accountIndex ?? _baseClient.ApiCredentials?.Credential.AccountIndex);
            parameters.Add("new_tier", newTier);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "api/v1/changeAccountTier", LighterExchange.RateLimiter.LighterRest, 3000, true);
            var result = await _baseClient.SendAsync<LighterResponse>(request, parameters, ct).ConfigureAwait(false);
            if (result.Success && result.Data.Code != 200)
                return HttpResult.Fail<LighterResponse>(result, new ServerError(result.Data.Code, _baseClient.GetErrorInfo(result.Data.Code, result.Data.Message)));

            return result;
        }

        #endregion

        #region Get Liquidations

        /// <inheritdoc />
        public async Task<HttpResult<LighterLiquidations>> GetLiquidationsAsync(
            long? accountIndex = null,
            long? marketId = null,
            int? limit = null,
            string? cursor = null,
            CancellationToken ct = default)
        {
            var parameters = new Parameters(LighterExchange._parameterSerializationSettings);
            parameters.Add("account_index", accountIndex ?? _baseClient.ApiCredentials?.Credential.AccountIndex);
            parameters.Add("market_id", marketId);
            parameters.Add("limit", limit ?? 100);
            parameters.Add("cursor", cursor);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "api/v1/liquidations", LighterExchange.RateLimiter.LighterRest, 300, true);
            var result = await _baseClient.SendAsync<LighterLiquidations>(request, parameters, ct).ConfigureAwait(false);
            if (result.Success && result.Data.Code != 200)
                return HttpResult.Fail<LighterLiquidations>(result, new ServerError(result.Data.Code, _baseClient.GetErrorInfo(result.Data.Code, result.Data.Message)));

            return result;
        }

        #endregion

        #region Get Funding History

        /// <inheritdoc />
        public async Task<HttpResult<LighterFundingHistory>> GetFundingHistoryAsync(
            long? accountIndex = null,
            long? marketId = null,
            PositionSide? side = null,
            DateTime? startTime = null,
            DateTime? endTime = null,
            int? limit = null,
            string? cursor = null,
            CancellationToken ct = default)
        {
            var parameters = new Parameters(LighterExchange._parameterSerializationSettings);
            parameters.Add("account_index", accountIndex ?? _baseClient.ApiCredentials?.Credential.AccountIndex);
            parameters.Add("market_id", marketId);
            parameters.Add("side", side);
            parameters.Add("start_timestamp", startTime);
            parameters.Add("end_timestamp", endTime);
            parameters.Add("limit", limit ?? 100);
            parameters.Add("cursor", cursor);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "api/v1/positionFunding", LighterExchange.RateLimiter.LighterRest, 300, true);
            var result = await _baseClient.SendAsync<LighterFundingHistory>(request, parameters, ct).ConfigureAwait(false);
            if (result.Success && result.Data.Code != 200)
                return HttpResult.Fail<LighterFundingHistory>(result, new ServerError(result.Data.Code, _baseClient.GetErrorInfo(result.Data.Code, result.Data.Message)));
            return result;
        }

        #endregion

        #region Get Deposit History

        /// <inheritdoc />
        public async Task<HttpResult<LighterDepositHistory>> GetDepositHistoryAsync(
            long? accountIndex = null,
            string? l1Address = null,
            WithdrawDepositFilter? filter = null,
            string? cursor = null,
            CancellationToken ct = default)
        {
            var parameters = new Parameters(LighterExchange._parameterSerializationSettings);
            parameters.Add("account_index", accountIndex ?? _baseClient.ApiCredentials?.Credential.AccountIndex);
            parameters.Add("l1_address", l1Address ?? _baseClient.ApiCredentials?.Credential.Key);
            parameters.Add("filter", filter);
            parameters.Add("cursor", cursor);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "api/v1/deposit/history", LighterExchange.RateLimiter.LighterRest, 300, true);
            var result = await _baseClient.SendAsync<LighterDepositHistory>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Transfer History

        /// <inheritdoc />
        public async Task<HttpResult<LighterTransferHistory>> GetTransferHistoryAsync(
            long? accountIndex = null,
            TransferFilterType? filter = null,
            string? cursor = null,
            CancellationToken ct = default)
        {
            var parameters = new Parameters(LighterExchange._parameterSerializationSettings);
            parameters.Add("account_index", accountIndex ?? _baseClient.ApiCredentials?.Credential.AccountIndex);
            parameters.Add("filter", filter);
            parameters.Add("cursor", cursor);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "api/v1/transfer/history", LighterExchange.RateLimiter.LighterRest, 300, true);
            var result = await _baseClient.SendAsync<LighterTransferHistory>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Withdraw History

        /// <inheritdoc />
        public async Task<HttpResult<LighterWithdrawalHistory>> GetWithdrawHistoryAsync(
            long? accountIndex = null,
            WithdrawDepositFilter? filter = null,
            string? cursor = null,
            CancellationToken ct = default)
        {
            var parameters = new Parameters(LighterExchange._parameterSerializationSettings);
            parameters.Add("account_index", accountIndex ?? _baseClient.ApiCredentials?.Credential.AccountIndex);
            parameters.Add("filter", filter);
            parameters.Add("cursor", cursor);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "api/v1/withdraw/history", LighterExchange.RateLimiter.LighterRest, 300, true);
            var result = await _baseClient.SendAsync<LighterWithdrawalHistory>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Api Keys

        /// <inheritdoc />
        public async Task<HttpResult<LighterApiKeys>> GetApiKeysAsync(
            long? accountIndex = null,
            int? apiKeyIndex = null,
            CancellationToken ct = default)
        {
            var parameters = new Parameters(LighterExchange._parameterSerializationSettings);
            parameters.Add("account_index", accountIndex ?? _baseClient.ApiCredentials?.Credential.AccountIndex);
            parameters.Add("api_key_index", apiKeyIndex ?? 255);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "api/v1/apikeys", LighterExchange.RateLimiter.LighterRest, 150, false);
            var result = await _baseClient.SendAsync<LighterApiKeys>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Approve Integrator

        /// <inheritdoc />
        public async Task<HttpResult<LighterTransactionResult>> ApproveIntegratorAsync(
            long integratorAccountIndex,
            int integratorTakerFee,
            int integratorMakerFee,
            DateTime expireTime,
            long? nonce,
            CancellationToken ct = default)
        {
            if (!_baseClient.Authenticated)
                return HttpResult.Fail<LighterTransactionResult>(_baseClient.Exchange, new NoApiCredentialsError());

            nonce ??= await _baseClient.GetNonceAsync().ConfigureAwait(false);

            var signedTx = LighterUtils.GetSigner(_baseClient.ClientOptions.LibraryPath, _baseClient.BaseAddress, _baseClient.ApiCredentials!).SignApproveIntegrator(
                integratorAccountIndex,
                integratorTakerFee,
                integratorMakerFee,
                DateTimeConverter.ConvertToMilliseconds(expireTime).Value,
                0x1,
                nonce.Value,
                _baseClient.ApiCredentials!.Credential!.ApiKeyIndex,
                _baseClient.ApiCredentials!.Credential!.AccountIndex);

            var body = new Parameters(LighterExchange._parameterSerializationSettings);
            body.Add("tx_type", signedTx.TxType);
            body.Add("tx_info", signedTx.TxInfo);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "/api/v1/sendTx", LighterExchange.RateLimiter.LighterRest, 6, false);
            return await _baseClient.SendAsync<LighterTransactionResult>(request, body, ct, skipCheck: true).ConfigureAwait(false);
        }

        #endregion

        #region Update Margin
        /// <inheritdoc />
        public async Task<HttpResult<LighterTransactionResult>> UpdateMarginAsync(
            string symbol,
            decimal usdcAmount,
            bool addOrRemove,
            long? nonce,
            CancellationToken ct = default)
        {
            if (!_baseClient.Authenticated)
                return HttpResult.Fail<LighterTransactionResult>(_baseClient.Exchange, new NoApiCredentialsError());

            var symbolInfo = await _baseClient.GetSymbolInfoAsync(symbol).ConfigureAwait(false);
            if (!symbolInfo.Success)
                return HttpResult.Fail<LighterTransactionResult>(_baseClient.Exchange, symbolInfo.Error!);

            nonce ??= await _baseClient.GetNonceAsync().ConfigureAwait(false);

            var signedTx = LighterUtils.GetSigner(_baseClient.ClientOptions.LibraryPath, _baseClient.BaseAddress, _baseClient.ApiCredentials!).SignUpdateMargin(
                symbolInfo.Data.MarketId,
                (int)(usdcAmount * LighterUtils.UsdcMultiplier),
                addOrRemove ? 1: 0,
                0x1,
                nonce.Value,
                _baseClient.ApiCredentials!.Credential!.ApiKeyIndex,
                _baseClient.ApiCredentials!.Credential!.AccountIndex);

            var body = new Parameters(LighterExchange._parameterSerializationSettings);
            body.Add("tx_type", signedTx.TxType);
            body.Add("tx_info", signedTx.TxInfo);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "/api/v1/sendTx", LighterExchange.RateLimiter.LighterRest, 6, false);
            return await _baseClient.SendAsync<LighterTransactionResult>(request, body, ct, skipCheck: true).ConfigureAwait(false);
        }

        #endregion

        #region Set Leverage
        /// <inheritdoc />
        public async Task<HttpResult<LighterTransactionResult>> SetLeverageAsync(
            string symbol,
            int leverage,
            MarginMode marginMode,
            long? nonce,
            CancellationToken ct = default)
        {
            if (!_baseClient.Authenticated)
                return HttpResult.Fail<LighterTransactionResult>(_baseClient.Exchange, new NoApiCredentialsError());

            var symbolInfo = await _baseClient.GetSymbolInfoAsync(symbol).ConfigureAwait(false);
            if (!symbolInfo.Success)
                return HttpResult.Fail<LighterTransactionResult>(_baseClient.Exchange, symbolInfo.Error!);

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

            var body = new Parameters(LighterExchange._parameterSerializationSettings);
            body.Add("tx_type", signedTx.TxType);
            body.Add("tx_info", signedTx.TxInfo);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "/api/v1/sendTx", LighterExchange.RateLimiter.LighterRest, 6, false);
            return await _baseClient.SendAsync<LighterTransactionResult>(request, body, ct, skipCheck: true).ConfigureAwait(false);
        }

        #endregion
    }
}
