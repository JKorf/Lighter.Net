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
        #region Get Balances

        async Task<ICallResult<SharedBalance[]>> IGetBalances.GetBalancesAsync(GetBalancesRequest request, CancellationToken ct)
            => await GetBalancesAsync(request, ct).ConfigureAwait(false);

        public GetBalancesOptions GetBalancesOptions { get; } = new GetBalancesOptions(_exchangeName, AccountTypeFilter.Futures, AccountTypeFilter.Spot);

        public async Task<HttpResult<SharedBalance[]>> GetBalancesAsync(GetBalancesRequest request, CancellationToken ct)
        {
            var validationError = GetBalancesOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedBalance[]>(Exchange, validationError);

            var result = await _api.Account.GetAccountsAsync(ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedBalance[]>(result);

            var tradingMode = request.TradingMode ?? TradingMode.Spot;
            var account = result.Data.Accounts.Single(x => x.AccountIndex == _api.ApiCredentials!.Credential.AccountIndex);

            return HttpResult.Ok(result, account.Assets.Select(x =>
                new SharedBalance(
                    tradingMode,
                    x.Symbol,
                    tradingMode == TradingMode.Spot ? (x.Balance - x.LockedBalance) : x.MarginBalance,
                    tradingMode == TradingMode.Spot ? x.Balance : x.MarginBalance)).ToArray());

        }

        #endregion

    }
}
