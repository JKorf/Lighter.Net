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

        GetDepositAddressesOptions IDepositRestClient.GetDepositAddressesOptions { get; }
            = new GetDepositAddressesOptions(_exchangeName, true)
            {
                Supported = false
            };
        Task<HttpResult<SharedDepositAddress[]>> IDepositRestClient.GetDepositAddressesAsync(GetDepositAddressesRequest request, CancellationToken ct)
        {
            return Task.FromResult(HttpResult.Fail<SharedDepositAddress[]>(_exchangeName, new InvalidOperationError("GetDepositAddresses is not support on " + _exchangeName)));
        }

        #region Get Deposit History

        async Task<ICallResult<SharedDeposit[]>> IGetDepositHistory.GetDepositHistoryAsync(GetDepositsRequest request, PageRequest? pageRequest, CancellationToken ct)
            => await GetDepositHistoryAsync(request, pageRequest, ct).ConfigureAwait(false);

        Task<HttpResult<SharedDeposit[]>> IDepositRestClient.GetDepositsAsync(GetDepositsRequest request, PageRequest? pageRequest, CancellationToken ct)
            => GetDepositHistoryAsync(request, pageRequest, ct);
        GetDepositHistoryOptions IDepositRestClient.GetDepositsOptions => GetDepositHistoryOptions;

        public GetDepositHistoryOptions GetDepositHistoryOptions { get; } = new GetDepositHistoryOptions(_exchangeName, false, true, false, 100)
        {
            ParameterRuleOverwrites = [
                RequestParameterRuleOverride<GetDepositsRequest>.NotSupported(x => x.StartTime),
                RequestParameterRuleOverride<GetDepositsRequest>.NotSupported(x => x.EndTime)
                ]
        };
        public async Task<HttpResult<SharedDeposit[]>> GetDepositHistoryAsync(GetDepositsRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = GetDepositHistoryOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedDeposit[]>(Exchange, validationError);

            var limit = request.Limit ?? 100;
            var direction = DataDirection.Descending;
            var pageParams = Pagination.GetPaginationParameters(direction, limit, request.StartTime, request.EndTime ?? DateTime.UtcNow, pageRequest, true);

            var assetsData = await _api.ExchangeData.GetAssetsAsync(ct: ct).ConfigureAwait(false);
            if (!assetsData.Success)
                return HttpResult.Fail<SharedDeposit[]>(assetsData);

            string? l1Address = null;
            if (request.Asset != null)
                l1Address = assetsData.Data.SingleOrDefault(x => x.Symbol.Equals(request.Asset, StringComparison.InvariantCultureIgnoreCase))?.L1Address;
            
            var result = await _api.Account.GetDepositHistoryAsync(
                l1Address: l1Address,
                cursor: pageParams.Cursor,
                ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedDeposit[]>(result);

            var nextPageRequest = Pagination.GetNextPageRequest(
                () => Pagination.NextPageFromCursor(result.Data.Cursor),
                result.Data!.Deposits.Length,
                result.Data.Deposits.Select(x => x.Timestamp),
                request.StartTime,
                request.EndTime ?? DateTime.UtcNow,
                pageParams);

            return HttpResult.Ok(result, ExchangeHelpers.ApplyFilter(result.Data.Deposits, x => x.Timestamp, request.StartTime, request.EndTime, direction)
                .Select(x =>
                    new SharedDeposit(
                        assetsData.Data.Single(y => y.AssetId == x.AssetId).Symbol,
                        x.Quantity,
                        x.Status == DepositStatus.Completed,
                        x.Timestamp,
                        ParseTransferStatus(x.Status))
                    {
                        TransactionId = x.L1TransactionHash,
                        Id = x.Id.ToString()
                    }).ToArray(), nextPageRequest);
        }

        #endregion

        private SharedTransferStatus ParseTransferStatus(DepositStatus status)
        {
            if (status == DepositStatus.Completed)
                return SharedTransferStatus.Completed;
            if (status == DepositStatus.Claimable || status == DepositStatus.Pending)
                return SharedTransferStatus.InProgress;
            if (status == DepositStatus.Failed)
                return SharedTransferStatus.Failed;

            return SharedTransferStatus.Unknown;
        }

    }
}
