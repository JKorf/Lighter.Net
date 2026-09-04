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

        #region Get Withdrawal History

        async Task<ICallResult<SharedWithdrawal[]>> IGetWithdrawalHistory.GetWithdrawalHistoryAsync(GetWithdrawalsRequest request, PageRequest? pageRequest, CancellationToken ct)
            => await GetWithdrawalHistoryAsync(request, pageRequest, ct).ConfigureAwait(false);

        Task<HttpResult<SharedWithdrawal[]>> IWithdrawalRestClient.GetWithdrawalsAsync(GetWithdrawalsRequest request, PageRequest? pageRequest, CancellationToken ct)
            => GetWithdrawalHistoryAsync(request, pageRequest, ct);
        GetWithdrawalHistoryOptions IWithdrawalRestClient.GetWithdrawalsOptions => GetWithdrawalHistoryOptions;

        public GetWithdrawalHistoryOptions GetWithdrawalHistoryOptions { get; } = new GetWithdrawalHistoryOptions(_exchangeName, false, true, false, 100);
        public async Task<HttpResult<SharedWithdrawal[]>> GetWithdrawalHistoryAsync(GetWithdrawalsRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = GetWithdrawalHistoryOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedWithdrawal[]>(Exchange, validationError);

            var limit = request.Limit ?? 100;
            var direction = DataDirection.Descending;
            var pageParams = Pagination.GetPaginationParameters(direction, limit, request.StartTime, request.EndTime ?? DateTime.UtcNow, pageRequest, true);

            var assetsData = await _api.ExchangeData.GetAssetsAsync(ct: ct).ConfigureAwait(false);
            if (!assetsData.Success)
                return HttpResult.Fail<SharedWithdrawal[]>(assetsData);

            var result = await _api.Account.GetWithdrawHistoryAsync(
                ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedWithdrawal[]>(result);

            var nextPageRequest = Pagination.GetNextPageRequest(
                () => direction == DataDirection.Ascending
                    ? Pagination.NextPageFromId(result.Data!.Withdraws.Max(x => x.Id) + 1)
                    : Pagination.NextPageFromTime(pageParams, result.Data!.Withdraws.Min(x => x.Timestamp), false),
                result.Data!.Withdraws.Length,
                result.Data.Withdraws.Select(x => x.Timestamp),
                request.StartTime,
                request.EndTime ?? DateTime.UtcNow,
                pageParams);

            return HttpResult.Ok(result, ExchangeHelpers.ApplyFilter(result.Data.Withdraws, x => x.Timestamp, request.StartTime, request.EndTime, direction)
                .Select(x =>
                    new SharedWithdrawal(
                        assetsData.Data.Single(y => y.AssetId == x.AssetId).Symbol,
                        "-",
                        x.Quantity,
                        x.Status == WithdrawalStatus.Completed,
                        x.Timestamp,
                        ParseTransferStatus(x.Status))
                    {
                        TransactionId = x.L1TransactionHash,
                        Id = x.Id.ToString()
                    })
                .ToArray(), nextPageRequest);
        }

        #endregion

        private SharedTransferStatus ParseTransferStatus(WithdrawalStatus status)
        {
            if (status == WithdrawalStatus.Completed)
                return SharedTransferStatus.Completed;
            if (status == WithdrawalStatus.Claimable || status == WithdrawalStatus.Pending)
                return SharedTransferStatus.InProgress;
            if (status == WithdrawalStatus.Failed || status == WithdrawalStatus.Refunded)
                return SharedTransferStatus.Failed;

            return SharedTransferStatus.Unknown;
        }
    }
}
