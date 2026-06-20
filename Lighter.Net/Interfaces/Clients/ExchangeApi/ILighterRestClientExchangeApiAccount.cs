using CryptoExchange.Net.Objects;
using Lighter.Net.Enums;
using Lighter.Net.Objects.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lighter.Net.Interfaces.Clients.ExchangeApi
{
    /// <summary>
    /// Lighter Exchange account endpoints. Account endpoints include balance info, withdraw/deposit info and requesting and account settings
    /// </summary>
    public interface ILighterRestClientExchangeApiAccount
    {
        /// <summary>
        /// Generate an API key
        /// </summary>
        /// <returns></returns>
        CallResult<(string PublicKey, string PrivateKey)> GenerateApiKey();

        /// <summary>
        /// Get the next nonce
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/reference/nextnonce" /><br />
        /// Endpoint:<br />
        /// GET api/v1/nextNonce<br />
        /// </para>
        /// </summary>
        /// <param name="accountIndex">Account index. If not provided will be taken from credentials</param>
        /// <param name="apiKeyIndex">API key index. If not provided will be taken from credentials</param>
        /// <param name="ct">Cancellation token</param>
        Task<HttpResult<LighterNonce>> GetNonceAsync(
            long? accountIndex = null,
            long? apiKeyIndex = null,
            CancellationToken ct = default);

        /// <summary>
        /// Get account by L1 address
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/reference/accountsbyl1address" /><br />
        /// Endpoint:<br />
        /// GET api/v1/accountsByL1Address<br />
        /// </para>
        /// </summary>
        /// <param name="address">["<c>l1_address</c>"] The layer 1 address. If not provided will be taken from the credentials</param>
        /// <param name="cursor">["<c>cursor</c>"] Pagination cursor</param>
        /// <param name="ct">Cancellation token</param>
        Task<HttpResult<LighterAccount>> GetAccountsByL1AddressAsync(
            string? address = null,
            string? cursor = null,
            CancellationToken ct = default);

        /// <summary>
        /// Get account details
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/reference/account-1" /><br />
        /// Endpoint:<br />
        /// GET api/v1/account<br />
        /// </para>
        /// </summary>
        /// <param name="by">["<c>by</c>"] By what value to retrieve the accounts. If not provided the account index from credentials will be used</param>
        /// <param name="value">["<c>value</c>"] The value to retrieve accounts by. If not provided the account index from credentials will be used</param>
        /// <param name="activeOnly">["<c>active_only</c>"] Only active accounts</param>
        /// <param name="cursor">["<c>cursor</c>"] Pagination cursor</param>
        /// <param name="ct">Cancellation token</param>
        Task<HttpResult<LighterAccounts>> GetAccountsAsync(
            AccountBy? by = null,
            string? value = null,
            bool? activeOnly = null,
            string? cursor = null,
            CancellationToken ct = default);

        /// <summary>
        /// Get account limits
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/reference/accountlimits" /><br />
        /// Endpoint:<br />
        /// GET api/v1/accountLimits<br />
        /// </para>
        /// </summary>
        /// <param name="accountIndex">["<c>account_index</c>"] Account index. If not provided will be taken from credentials</param>
        /// <param name="ct">Cancellation token</param>
        Task<HttpResult<LighterAccountLimits>> GetAccountLimitsAsync(long? accountIndex = null, CancellationToken ct = default);

        /// <summary>
        /// Get account metadata
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/reference/accountmetadata" /><br />
        /// Endpoint:<br />
        /// GET api/v1/accountMetadata<br />
        /// </para>
        /// </summary>
        /// <param name="by">["<c>by</c>"] By what value to retrieve the accounts. If not provided the account index from credentials will be used</param>
        /// <param name="value">["<c>value</c>"] The value to retrieve accounts by. If not provided the account index from credentials will be used</param>
        /// <param name="cursor">["<c>cursor</c>"] Pagination cursor</param>
        /// <param name="ct">Cancellation token</param>
        Task<HttpResult<LighterAccountMetadata>> GetAccountMetadataAsync(
            AccountBy? by = null,
            string? value = null,
            string? cursor = null,
            CancellationToken ct = default);

        /// <summary>
        /// Get profit and loss info
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/reference/pnl" /><br />
        /// Endpoint:<br />
        /// GET api/v1/pnl<br />
        /// </para>
        /// </summary>
        /// <param name="resolution">["<c>resolution</c>"] Period</param>
        /// <param name="by">["<c>by</c>"] By what value to retrieve the accounts. If not provided the account index from credentials will be used</param>
        /// <param name="value">["<c>value</c>"] The value to retrieve accounts by. If not provided the account index from credentials will be used</param>
        /// <param name="ignoreTransfers">["<c>ignore_transfers</c>"] Ignore transfers</param>
        /// <param name="startTime">["<c>start_timestamp</c>"] Filter by start time</param>
        /// <param name="endTime">["<c>end_timestamp</c>"] Filter by end time</param>
        /// <param name="limit">["<c>count_back</c>"] Max number of results</param>
        /// <param name="ct">Cancellation token</param>
        Task<HttpResult<LighterPnl>> GetPnlAsync(
            Resolution resolution,
            AccountBy? by = null,
            string? value = null,
            bool? ignoreTransfers = null,
            DateTime? startTime = null,
            DateTime? endTime = null,
            int? limit = null,
            CancellationToken ct = default);

        /// <summary>
        /// Set account tier
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/reference/changeaccounttier" /><br />
        /// Endpoint:<br />
        /// POST api/v1/changeAccountTier<br />
        /// </para>
        /// </summary>
        /// <param name="accountIndex">["<c>account_index</c>"] Account index. If not provided will be taken from credentials</param>
        /// <param name="newTier">["<c>new_tier</c>"] New tier</param>
        /// <param name="ct">Cancellation token</param>
        Task<HttpResult<LighterResponse>> SetAccountTierAsync(
            long? accountIndex,
            AccountTier newTier,
            CancellationToken ct = default);

        /// <summary>
        /// Get account liquidation history
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/reference/liquidations" /><br />
        /// Endpoint:<br />
        /// GET api/v1/liquidations<br />
        /// </para>
        /// </summary>
        /// <param name="accountIndex">["<c>account_index</c>"] Account index. If not provided will be taken from the credentials</param>
        /// <param name="marketId">["<c>market_id</c>"] Filter by market id</param>
        /// <param name="limit">["<c>limit</c>"] Max number of results, max 100</param>
        /// <param name="cursor">["<c>cursor</c>"] Pagination cursor</param>
        /// <param name="ct">Cancellation token</param>
        Task<HttpResult<LighterLiquidations>> GetLiquidationsAsync(
            long? accountIndex = null,
            long? marketId = null,
            int? limit = null,
            string? cursor = null,
            CancellationToken ct = default);

        /// <summary>
        /// Get position funding history
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/reference/positionfunding" /><br />
        /// Endpoint:<br />
        /// GET api/v1/positionFunding<br />
        /// </para>
        /// </summary>
        /// <param name="accountIndex">["<c>account_index</c>"] Account index. If not provided will be taken from credentials</param>
        /// <param name="marketId">["<c>market_id</c>"] Market id</param>
        /// <param name="side">["<c>side</c>"] Filter by side</param>
        /// <param name="startTime">["<c>start_timestamp</c>"] Filter by start time</param>
        /// <param name="endTime">["<c>end_timestamp</c>"] Filter by end time</param>
        /// <param name="limit">["<c>limit</c>"] Max number of results, max 100</param>
        /// <param name="cursor">["<c>cursor</c>"] Pagination cursor</param>
        /// <param name="ct">Cancellation token</param>
        Task<HttpResult<LighterFundingHistory>> GetFundingHistoryAsync(
            long? accountIndex = null,
            long? marketId = null,
            PositionSide? side = null,
            DateTime? startTime = null,
            DateTime? endTime = null,
            int? limit = null,
            string? cursor = null,
            CancellationToken ct = default);

        /// <summary>
        /// Get deposit history
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/reference/deposit_history" /><br />
        /// Endpoint:<br />
        /// GET api/v1/deposit/history<br />
        /// </para>
        /// </summary>
        /// <param name="accountIndex">["<c>account_index</c>"] Account index. If not provided will be taken from credentials</param>
        /// <param name="l1Address">["<c>l1_address</c>"] Layer 1 address. If not provided will be taken from credentials</param>
        /// <param name="filter">["<c>filter</c>"] Filter results</param>
        /// <param name="cursor">["<c>cursor</c>"] Pagination cursor</param>
        /// <param name="ct">Cancellation token</param>
        Task<HttpResult<LighterDepositHistory>> GetDepositHistoryAsync(
            long? accountIndex = null,
            string? l1Address = null,
            WithdrawDepositFilter? filter = null,
            string? cursor = null,
            CancellationToken ct = default);

        /// <summary>
        /// Get transfer history
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/reference/transfer_history" /><br />
        /// Endpoint:<br />
        /// GET api/v1/transfer/history<br />
        /// </para>
        /// </summary>
        /// <param name="accountIndex">["<c>account_index</c>"] Account index. If not provided will be taken from credentials</param>
        /// <param name="filter">["<c>type</c>"] Type filter</param>
        /// <param name="cursor">["<c>cursor</c>"] Pagination cursor</param>
        /// <param name="ct">Cancellation token</param>
        Task<HttpResult<LighterTransferHistory>> GetTransferHistoryAsync(
            long? accountIndex = null,
            TransferFilterType? filter = null,
            string? cursor = null, 
            CancellationToken ct = default);

        /// <summary>
        /// Get withdrawal history
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/reference/withdraw_history" /><br />
        /// Endpoint:<br />
        /// GET api/v1/withdraw/history<br />
        /// </para>
        /// </summary>
        /// <param name="accountIndex">["<c>account_index</c>"] </param>
        /// <param name="filter">["<c>filter</c>"] Filter by status</param>
        /// <param name="cursor">["<c>cursor</c>"] Pagination cursor</param>
        /// <param name="ct">Cancellation token</param>
        Task<HttpResult<LighterWithdrawalHistory>> GetWithdrawHistoryAsync(
            long? accountIndex = null,
            WithdrawDepositFilter? filter = null,
            string? cursor = null,
            CancellationToken ct = default);

        /// <summary>
        /// Get API keys
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/reference/apikeys" /><br />
        /// Endpoint:<br />
        /// GET api/v1/apikeys<br />
        /// </para>
        /// </summary>
        /// <param name="accountIndex">["<c>account_index</c>"] Account index. If not provided will be taken from credentials</param>
        /// <param name="apiKeyIndex">["<c>api_key_index</c>"] Filter by index</param>
        /// <param name="ct">Cancellation token</param>
        Task<HttpResult<LighterApiKeys>> GetApiKeysAsync(
            long? accountIndex = null,
            int? apiKeyIndex = null,
            CancellationToken ct = default);

        /// <summary>
        /// Approve integrator
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/reference/sendtx" /><br />
        /// Endpoint:<br />
        /// GET api/v1/sendTx<br />
        /// </para>
        /// </summary>
        /// <param name="integratorAccountIndex">Integrator account index</param>
        /// <param name="integratorTakerFee">Integrator max taker fee, 1000 = 10 bps / 0.1%</param>
        /// <param name="integratorMakerFee">Integrator max maker fee, 1000 = 10 bps / 0.1%</param>
        /// <param name="expireTime">Expire time</param>
        /// <param name="nonce">The nonce, will be set automatically if not provided.</param>
        /// <param name="ct">Cancellation token</param>
        Task<HttpResult<LighterTransactionResult>> ApproveIntegratorAsync(
            long integratorAccountIndex,
            int integratorTakerFee,
            int integratorMakerFee,
            DateTime expireTime,
            long? nonce = null,
            CancellationToken ct = default);

        /// <summary>
        /// Set leverage for a specific symbol
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/reference/sendtx" /><br />
        /// Endpoint:<br />
        /// GET api/v1/sendTx<br />
        /// </para>
        /// </summary>
        /// <param name="symbol">Symbol name, for example `ETH`</param>
        /// <param name="leverage">New leverage</param>
        /// <param name="marginMode">Margin mode</param>
        /// <param name="nonce">The nonce, will be set automatically if not provided.</param>
        /// <param name="ct">Cancellation token</param>
        Task<HttpResult<LighterTransactionResult>> SetLeverageAsync(
            string symbol,
            int leverage,
            MarginMode marginMode,
            long? nonce = null,
            CancellationToken ct = default);

        /// <summary>
        /// Update margin for a symbol
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/reference/sendtx" /><br />
        /// Endpoint:<br />
        /// GET api/v1/sendTx<br />
        /// </para>
        /// </summary>
        /// <param name="symbol">Symbol name, for example `ETH`</param>
        /// <param name="usdcAmount">Amount in USDC to add or remove</param>
        /// <param name="addOrRemove">True to add, false to remove</param>
        /// <param name="nonce">The nonce, will be set automatically if not provided.</param>
        /// <param name="ct">Cancellation token</param>
        Task<HttpResult<LighterTransactionResult>> UpdateMarginAsync(
            string symbol,
            decimal usdcAmount,
            bool addOrRemove,
            long? nonce,
            CancellationToken ct = default);
    }
}
