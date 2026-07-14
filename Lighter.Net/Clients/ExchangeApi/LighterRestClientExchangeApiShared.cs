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
    internal partial class LighterRestClientExchangeApi : ILighterRestClientExchangeApiShared
    {
        private const string _exchangeName = "Lighter";
        private const string _topicSpotId = "LighterSpot";
        private const string _topicFuturesId = "LighterFutures";

        public TradingMode[] SupportedTradingModes => new[] { TradingMode.Spot, TradingMode.PerpetualLinear };

        public void SetDefaultExchangeParameter(string key, object value) => ExchangeParameters.SetStaticParameter(Exchange, key, value);
        public void ResetDefaultExchangeParameters() => ExchangeParameters.ResetStaticParameters();
        public SharedClientInfo Discover() => SharedUtils.GetClientInfo(LighterExchange.Metadata, this);

        #region Klines Client

        GetKlinesOptions IKlineRestClient.GetKlinesOptions { get; } = new GetKlinesOptions(_exchangeName, false, true, true, 500, false, [
            SharedKlineInterval.OneMinute,
            SharedKlineInterval.FiveMinutes,
            SharedKlineInterval.FifteenMinutes,
            SharedKlineInterval.ThirtyMinutes,
            SharedKlineInterval.OneHour,
            SharedKlineInterval.FourHours,
            SharedKlineInterval.TwelveHours,
            SharedKlineInterval.OneDay
            ]);
        async Task<HttpResult<SharedKline[]>> IKlineRestClient.GetKlinesAsync(GetKlinesRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = SharedClient.GetKlinesOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedKline[]>(Exchange, validationError);

            var direction = DataDirection.Descending;
            var symbol = request.SymbolName(FormatSymbol);
            var limit = request.Limit ?? SharedClient.GetKlinesOptions.MaxLimit;
            var pageParams = Pagination.GetPaginationParameters(
                direction,
                limit,
                request.StartTime ?? (request.EndTime ?? DateTime.UtcNow).AddSeconds(-((int)request.Interval * 1000)),
                request.EndTime ?? DateTime.UtcNow,
                pageRequest);

            // Get data
            var result = await ExchangeData.GetKlinesAsync(
                symbol,
                (KlineInterval)request.Interval,
                pageParams.StartTime,
                pageParams.EndTime,
                ct: ct
                ).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedKline[]>(result);

            var nextPageRequest = Pagination.GetNextPageRequest(
                    () => Pagination.NextPageFromTime(pageParams, result.Data!.Klines.Min(x => x.OpenTime)),
                    result.Data!.Klines.Length,
                    result.Data.Klines.Select(x => x.OpenTime),
                    request.StartTime,
                    request.EndTime ?? DateTime.UtcNow,
                    pageParams);

            if ((nextPageRequest?.EndTime - nextPageRequest?.StartTime)?.TotalSeconds < (int)request.Interval)
                nextPageRequest = null;

            // Return
            return HttpResult.Ok(result,
                ExchangeHelpers.ApplyFilter(result.Data.Klines, x => x.OpenTime, request.StartTime, request.EndTime, direction)
                    .Select(x =>
                        new SharedKline(request.Symbol, symbol, x.OpenTime, x.ClosePrice, x.HighPrice, x.LowPrice, x.OpenPrice, x.Volume))
                    .ToArray(), nextPageRequest);

        }

        #endregion

        #region Spot Symbol client
        GetSpotSymbolsOptions ISpotSymbolRestClient.GetSpotSymbolsOptions { get; }
            = new GetSpotSymbolsOptions(_exchangeName, false);

        async Task<HttpResult<SharedSpotSymbol[]>> ISpotSymbolRestClient.GetSpotSymbolsAsync(GetSymbolsRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetSpotSymbolsOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotSymbol[]>(Exchange, validationError);

            var resultAssets = ExchangeData.GetAssetsAsync();
            var resultSymbols = ExchangeData.GetSymbolsAsync(symbolType: SymbolTypeFilter.Spot, ct: ct);
            await Task.WhenAll(resultAssets, resultSymbols).ConfigureAwait(false);
            if (!resultAssets.Result.Success)
                return HttpResult.Fail<SharedSpotSymbol[]>(resultAssets.Result);
            if (!resultSymbols.Result.Success)
                return HttpResult.Fail<SharedSpotSymbol[]>(resultSymbols.Result);

            var resultData = resultSymbols.Result.Data!.Select(s => {
                var baseAsset = resultAssets.Result.Data.SingleOrDefault(x => x.AssetId == s.BaseAssetId);
                var quoteAsset = resultAssets.Result.Data.SingleOrDefault(x => x.AssetId == s.QuoteAssetId);
                if (baseAsset == null || quoteAsset == null)
                    return null;

                return new SharedSpotSymbol(baseAsset.Symbol, quoteAsset.Symbol, s.Symbol, s.Status == SymbolStatus.Active)
                {
                    MinTradeQuantity = s.MinBaseQuantity,
                    MinNotionalValue = s.MinQuoteQuantity,
                    PriceDecimals = s.SupportedPriceDecimals,
                    QuantityDecimals = s.SupportedQuantityDecimals
                };
            }).Where(x => x != null).ToArray();

            ExchangeSymbolCache.UpdateSymbolInfo(_topicSpotId, EnvironmentName, null, resultData!);
            return HttpResult.Ok<SharedSpotSymbol[]>(resultSymbols.Result, resultData!);
        }

        async Task<ExchangeCallResult<SharedSymbol[]>> ISpotSymbolRestClient.GetSpotSymbolsForBaseAssetAsync(string baseAsset)
        {
            if (!ExchangeSymbolCache.HasCached(_topicSpotId, EnvironmentName, null))
            {
                var symbols = await ((ISpotSymbolRestClient)this).GetSpotSymbolsAsync(new GetSymbolsRequest()).ConfigureAwait(false);
                if (!symbols.Success)
                    return ExchangeCallResult<SharedSymbol[]>.Fail(Exchange, symbols.Error!);
            }

            return ExchangeCallResult<SharedSymbol[]>.Ok(Exchange, ExchangeSymbolCache.GetSymbolsForBaseAsset(_topicSpotId, EnvironmentName, null, baseAsset));
        }

        async Task<ExchangeCallResult<bool>> ISpotSymbolRestClient.SupportsSpotSymbolAsync(SharedSymbol symbol)
        {
            if (symbol.TradingMode != TradingMode.Spot)
                throw new ArgumentException(nameof(symbol), "Only Spot symbols allowed");

            if (!ExchangeSymbolCache.HasCached(_topicSpotId, EnvironmentName, null))
            {
                var symbols = await ((ISpotSymbolRestClient)this).GetSpotSymbolsAsync(new GetSymbolsRequest()).ConfigureAwait(false);
                if (!symbols.Success)
                    return ExchangeCallResult<bool>.Fail(Exchange, symbols.Error!);
            }

            return ExchangeCallResult<bool>.Ok(Exchange, ExchangeSymbolCache.SupportsSymbol(_topicSpotId, EnvironmentName, null, symbol));
        }

        async Task<ExchangeCallResult<bool>> ISpotSymbolRestClient.SupportsSpotSymbolAsync(string symbolName)
        {
            if (!ExchangeSymbolCache.HasCached(_topicSpotId, EnvironmentName, null))
            {
                var symbols = await ((ISpotSymbolRestClient)this).GetSpotSymbolsAsync(new GetSymbolsRequest()).ConfigureAwait(false);
                if (!symbols.Success)
                    return ExchangeCallResult<bool>.Fail(Exchange, symbols.Error!);
            }

            return ExchangeCallResult<bool>.Ok(Exchange, ExchangeSymbolCache.SupportsSymbol(_topicSpotId, EnvironmentName, null, symbolName));
        }
        #endregion

        #region Futures Symbol client

        GetFuturesSymbolsOptions IFuturesSymbolRestClient.GetFuturesSymbolsOptions { get; } = new GetFuturesSymbolsOptions(_exchangeName, false);
        async Task<HttpResult<SharedFuturesSymbol[]>> IFuturesSymbolRestClient.GetFuturesSymbolsAsync(GetSymbolsRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetSpotSymbolsOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFuturesSymbol[]>(Exchange, validationError);

            var resultSymbols = await ExchangeData.GetSymbolsAsync(symbolType: SymbolTypeFilter.Perp, ct: ct).ConfigureAwait(false);
            if (!resultSymbols.Success)
                return HttpResult.Fail<SharedFuturesSymbol[]>(resultSymbols);

            var resultData = resultSymbols.Data!.Select(ParseSymbol).ToArray();

            // Register both LIT/USDC and LIT as symbol names
            var symbolRegistrations = resultData
                .Concat(resultSymbols.Data.Select(x => new SharedFuturesSymbol(TradingMode.PerpetualLinear, x.Symbol, "USDC", x.Symbol, true))).ToArray();

            ExchangeSymbolCache.UpdateSymbolInfo(_topicFuturesId, EnvironmentName, null, symbolRegistrations);

            return HttpResult.Ok(resultSymbols, resultData!);
        }

        private SharedFuturesSymbol ParseSymbol(LighterSymbol s)
        {
            return new SharedFuturesSymbol(TradingMode.PerpetualLinear, s.Symbol, "USDC", $"{s.Symbol}/USDC", s.Status == SymbolStatus.Active)
            {
                MinTradeQuantity = s.MinBaseQuantity,
                MinNotionalValue = s.MinQuoteQuantity,
                PriceDecimals = s.SupportedPriceDecimals,
                QuantityDecimals = s.SupportedQuantityDecimals,
            };
        }

        async Task<ExchangeCallResult<SharedSymbol[]>> IFuturesSymbolRestClient.GetFuturesSymbolsForBaseAssetAsync(string baseAsset)
        {
            if (!ExchangeSymbolCache.HasCached(_topicFuturesId, EnvironmentName, null))
            {
                var symbols = await ((IFuturesSymbolRestClient)this).GetFuturesSymbolsAsync(new GetSymbolsRequest()).ConfigureAwait(false);
                if (!symbols.Success)
                    return ExchangeCallResult<SharedSymbol[]>.Fail(Exchange, symbols.Error!);
            }

            return ExchangeCallResult<SharedSymbol[]>.Ok(Exchange, ExchangeSymbolCache.GetSymbolsForBaseAsset(_topicFuturesId, EnvironmentName, null, baseAsset));
        }

        async Task<ExchangeCallResult<bool>> IFuturesSymbolRestClient.SupportsFuturesSymbolAsync(SharedSymbol symbol)
        {
            if (symbol.TradingMode == TradingMode.Spot)
                throw new ArgumentException(nameof(symbol), "Spot symbols not allowed");

            if (!ExchangeSymbolCache.HasCached(_topicFuturesId, EnvironmentName, null))
            {
                var symbols = await ((IFuturesSymbolRestClient)this).GetFuturesSymbolsAsync(new GetSymbolsRequest()).ConfigureAwait(false);
                if (!symbols.Success)
                    return ExchangeCallResult<bool>.Fail(Exchange, symbols.Error!);
            }

            return ExchangeCallResult<bool>.Ok(Exchange, ExchangeSymbolCache.SupportsSymbol(_topicFuturesId, EnvironmentName, null, symbol));
        }

        async Task<ExchangeCallResult<bool>> IFuturesSymbolRestClient.SupportsFuturesSymbolAsync(string symbolName)
        {
            if (!ExchangeSymbolCache.HasCached(_topicFuturesId, EnvironmentName, null))
            {
                var symbols = await ((IFuturesSymbolRestClient)this).GetFuturesSymbolsAsync(new GetSymbolsRequest()).ConfigureAwait(false);
                if (!symbols.Success)
                    return ExchangeCallResult<bool>.Fail(Exchange, symbols.Error!);
            }

            return ExchangeCallResult<bool>.Ok(Exchange, ExchangeSymbolCache.SupportsSymbol(_topicFuturesId, EnvironmentName, null, symbolName));
        }
        #endregion

        #region Spot Ticker client

        GetSpotTickerOptions ISpotTickerRestClient.GetSpotTickerOptions { get; } = new GetSpotTickerOptions(_exchangeName);
        async Task<HttpResult<SharedSpotTicker>> ISpotTickerRestClient.GetSpotTickerAsync(GetTickerRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetSpotTickerOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotTicker>(Exchange, validationError);

            var result = await ExchangeData.GetSymbolDetailsAsync(request.SymbolName(FormatSymbol), ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedSpotTicker>(result);

            return HttpResult.Ok(result, new SharedSpotTicker(
                    request.Symbol,
                    result.Data.SpotSymbols[0].Symbol,
                    result.Data.SpotSymbols[0].LastPrice,
                    result.Data.SpotSymbols[0].HighPrice,
                    result.Data.SpotSymbols[0].LowPrice,
                    result.Data.SpotSymbols[0].Volume,
                    result.Data.SpotSymbols[0].PriceChangePercentage)
            {
                QuoteVolume = result.Data.SpotSymbols[0].QuoteVolume
            });

        }

        GetSpotTickersOptions ISpotTickerRestClient.GetSpotTickersOptions { get; } = new GetSpotTickersOptions(_exchangeName);
        async Task<HttpResult<SharedSpotTicker[]>> ISpotTickerRestClient.GetSpotTickersAsync(GetTickersRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetSpotTickersOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotTicker[]>(Exchange, validationError);

            var result = await ExchangeData.GetSymbolDetailsAsync(symbolType: SymbolTypeFilter.Spot, ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedSpotTicker[]>(result);

            return HttpResult.Ok(result, result.Data!.SpotSymbols.Select(x =>
                    new SharedSpotTicker(
                        ExchangeSymbolCache.ParseSymbol(_topicSpotId, EnvironmentName, null, x.Symbol),
                        x.Symbol,
                        x.LastPrice,
                        x.HighPrice,
                        x.LowPrice,
                        x.Volume,
                        x.PriceChangePercentage)
                    {
                        QuoteVolume = x.QuoteVolume
                    }).ToArray());

        }

        #endregion

        #region Futures Ticker client

        GetFuturesTickerOptions IFuturesTickerRestClient.GetFuturesTickerOptions { get; } = new GetFuturesTickerOptions(_exchangeName);
        async Task<HttpResult<SharedFuturesTicker>> IFuturesTickerRestClient.GetFuturesTickerAsync(GetTickerRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetFuturesTickerOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFuturesTicker>(Exchange, validationError);

            var result = await ExchangeData.GetSymbolDetailsAsync(request.SymbolName(FormatSymbol), ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedFuturesTicker>(result);

            return HttpResult.Ok(result, new SharedFuturesTicker(
                    request.Symbol,
                    result.Data.PerpSymbols[0].Symbol,
                    result.Data.PerpSymbols[0].LastPrice,
                    result.Data.PerpSymbols[0].HighPrice,
                    result.Data.PerpSymbols[0].LowPrice,
                    result.Data.PerpSymbols[0].Volume,
                    result.Data.PerpSymbols[0].PriceChangePercentage)
            {
            });

        }

        GetFuturesTickersOptions IFuturesTickerRestClient.GetFuturesTickersOptions { get; } = new GetFuturesTickersOptions(_exchangeName);
        async Task<HttpResult<SharedFuturesTicker[]>> IFuturesTickerRestClient.GetFuturesTickersAsync(GetTickersRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetFuturesTickersOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFuturesTicker[]>(Exchange, validationError);

            var result = await ExchangeData.GetSymbolDetailsAsync(symbolType: SymbolTypeFilter.Perp, ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedFuturesTicker[]>(result);

            return HttpResult.Ok(result, result.Data!.PerpSymbols.Select(x =>
                    new SharedFuturesTicker(
                        ExchangeSymbolCache.ParseSymbol(_topicFuturesId, EnvironmentName, null, x.Symbol),
                        x.Symbol,
                        x.LastPrice,
                        x.HighPrice,
                        x.LowPrice,
                        x.Volume,
                        x.PriceChangePercentage)
                    {
                    }).ToArray());

        }

        #endregion

        #region Book Ticker client

        GetBookTickerOptions IBookTickerRestClient.GetBookTickerOptions { get; }
            = new GetBookTickerOptions(_exchangeName, false);
        async Task<HttpResult<SharedBookTicker>> IBookTickerRestClient.GetBookTickerAsync(GetBookTickerRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetBookTickerOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedBookTicker>(Exchange, validationError);

            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var resultTicker = await ExchangeData.GetOrderBookAsync(symbol, 1, ct: ct).ConfigureAwait(false);
            if (!resultTicker.Success)
                return HttpResult.Fail<SharedBookTicker>(resultTicker);

            return HttpResult.Ok(resultTicker, new SharedBookTicker(
                request.Symbol,
                symbol,
                resultTicker.Data.Asks[0].Price,
                resultTicker.Data.Asks[0].Quantity,
                resultTicker.Data.Bids[0].Price,
                resultTicker.Data.Bids[0].Quantity));

        }

        #endregion

        #region Recent Trades client
        GetRecentTradesOptions IRecentTradeRestClient.GetRecentTradesOptions { get; } = new GetRecentTradesOptions(_exchangeName, 100, false);

        async Task<HttpResult<SharedTrade[]>> IRecentTradeRestClient.GetRecentTradesAsync(GetRecentTradesRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetRecentTradesOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedTrade[]>(Exchange, validationError);

            // Get data
            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var result = await ExchangeData.GetRecentTradesAsync(
                symbol,
                limit: request.Limit,
                ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedTrade[]>(result);

            // Return
            return HttpResult.Ok(result, result.Data!.Select(x =>
                new SharedTrade(request.Symbol, symbol, x.Quantity, x.Price, x.Timestamp)
                {
                    Side = x.IsMakerAsk ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                }).ToArray());

        }
        #endregion

        #region Order Book client
        GetOrderBookOptions IOrderBookRestClient.GetOrderBookOptions { get; } = new GetOrderBookOptions(_exchangeName, 1, 250, false)
        {
            RequestNotes = "When specifying the limit parameter less entries might be returned as individual orders are combined into aggregated levels client side"
        };
        async Task<HttpResult<SharedOrderBook>> IOrderBookRestClient.GetOrderBookAsync(GetOrderBookRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetOrderBookOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedOrderBook>(Exchange, validationError);

            var result = await ExchangeData.GetOrderBookAsync(
                request.Symbol!.GetSymbol(FormatSymbol),
                limit: request.Limit ?? 50,
                ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedOrderBook>(result);

            var asks = result.Data.Asks.GroupBy(x => x.Price);
            var bids = result.Data.Bids.GroupBy(x => x.Price);

            return HttpResult.Ok(result, 
                new SharedOrderBook(
                    asks.Select(x => new CombinedEntry { Price = x.Key, Quantity = x.Sum(y => y.Quantity) }).ToArray(),
                    bids.Select(x => new CombinedEntry { Price = x.Key, Quantity = x.Sum(y => y.Quantity) }).ToArray()
                    ));

        }

        class CombinedEntry : ISymbolOrderBookEntry
        {
            public decimal Quantity { get; set; }
            public decimal Price { get; set; }
        }
        #endregion

        #region Asset client
        GetAssetsOptions IAssetsRestClient.GetAssetsOptions { get; }
            = new GetAssetsOptions(_exchangeName, false);

        async Task<HttpResult<SharedAsset[]>> IAssetsRestClient.GetAssetsAsync(GetAssetsRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetAssetsOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedAsset[]>(Exchange, validationError);

            var assets = await ExchangeData.GetAssetsAsync(ct: ct).ConfigureAwait(false);
            if (!assets.Success)
                return HttpResult.Fail<SharedAsset[]>(assets);

            return HttpResult.Ok(assets, assets.Data!.Select(x => new SharedAsset(x.Symbol)
            {
                FullName = x.Symbol,
                Networks = [ new SharedAssetNetwork(x.Symbol)
                {
                    ContractAddress = x.L1Address,
                    MinWithdrawQuantity = x.MinWithdrawalQuantity
                }]
            }).ToArray());

        }

        GetAssetOptions IAssetsRestClient.GetAssetOptions { get; } = new GetAssetOptions(_exchangeName, false);
        async Task<HttpResult<SharedAsset>> IAssetsRestClient.GetAssetAsync(GetAssetRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetAssetOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedAsset>(Exchange, validationError);

            var assets = await ExchangeData.GetAssetsAsync(ct: ct).ConfigureAwait(false);
            if (!assets.Success)
                return HttpResult.Fail<SharedAsset>(assets);

            var asset = assets.Data!.SingleOrDefault(x => x.Symbol.Equals(request.Asset, StringComparison.InvariantCultureIgnoreCase));
            if (asset == null)
                return HttpResult.Fail<SharedAsset>(Exchange, new ServerError(new ErrorInfo(ErrorType.UnknownAsset, false, "Asset not found")));

            return HttpResult.Ok(assets, new SharedAsset(asset.Symbol)
            {
                FullName = asset.Symbol,
                Networks = [ new SharedAssetNetwork(asset.Symbol)
                {
                    ContractAddress = asset.L1Address,
                    MinWithdrawQuantity = asset.MinWithdrawalQuantity
                }]
            });

        }

        #endregion

        #region Deposit client

        GetDepositAddressesOptions IDepositRestClient.GetDepositAddressesOptions { get; }
            = new GetDepositAddressesOptions(_exchangeName, true)
            {
                Supported = false
            };
        Task<HttpResult<SharedDepositAddress[]>> IDepositRestClient.GetDepositAddressesAsync(GetDepositAddressesRequest request, CancellationToken ct)
        {
            return Task.FromResult(HttpResult.Fail<SharedDepositAddress[]>(ExchangeName, new InvalidOperationError("GetDepositAddresses is not support on " + _exchangeName)));
        }

        GetDepositsOptions IDepositRestClient.GetDepositsOptions { get; } = new GetDepositsOptions(_exchangeName, false, true, false, 100);
        async Task<HttpResult<SharedDeposit[]>> IDepositRestClient.GetDepositsAsync(GetDepositsRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = SharedClient.GetDepositsOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedDeposit[]>(Exchange, validationError);

            var limit = request.Limit ?? 100;
            var direction = DataDirection.Descending;
            var pageParams = Pagination.GetPaginationParameters(direction, limit, request.StartTime, request.EndTime ?? DateTime.UtcNow, pageRequest, true);

            var assetsData = await ExchangeData.GetAssetsAsync(ct: ct).ConfigureAwait(false);
            if (!assetsData.Success)
                return HttpResult.Fail<SharedDeposit[]>(assetsData);

            string? l1Address = null;
            if (request.Asset != null)
                l1Address = assetsData.Data.SingleOrDefault(x => x.Symbol.Equals(request.Asset, StringComparison.InvariantCultureIgnoreCase))?.L1Address;
            
            var result = await Account.GetDepositHistoryAsync(
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

        #endregion

        #region Withdrawal client

        GetWithdrawalsOptions IWithdrawalRestClient.GetWithdrawalsOptions { get; } = new GetWithdrawalsOptions(_exchangeName, false, true, false, 100);
        async Task<HttpResult<SharedWithdrawal[]>> IWithdrawalRestClient.GetWithdrawalsAsync(GetWithdrawalsRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = SharedClient.GetWithdrawalsOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedWithdrawal[]>(Exchange, validationError);

            var limit = request.Limit ?? 100;
            var direction = DataDirection.Descending;
            var pageParams = Pagination.GetPaginationParameters(direction, limit, request.StartTime, request.EndTime ?? DateTime.UtcNow, pageRequest, true);

            var assetsData = await ExchangeData.GetAssetsAsync(ct: ct).ConfigureAwait(false);
            if (!assetsData.Success)
                return HttpResult.Fail<SharedWithdrawal[]>(assetsData);

            var result = await Account.GetWithdrawHistoryAsync(
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
        #endregion

        #region Fee Client
        GetFeeOptions IFeeRestClient.GetFeeOptions { get; } = new GetFeeOptions(_exchangeName, true);

        async Task<HttpResult<SharedFee>> IFeeRestClient.GetFeesAsync(GetFeeRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetFeeOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFee>(Exchange, validationError);

            // Get data
            var result = await Account.GetAccountLimitsAsync(ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedFee>(result);

            // Return
            return HttpResult.Ok(result, new SharedFee(result.Data.CurrentMakerFeeTick * 100, result.Data.CurrentTakerFeeTick * 100));

        }
        #endregion

        #region Balance Client
        GetBalancesOptions IBalanceRestClient.GetBalancesOptions { get; } = new GetBalancesOptions(_exchangeName, AccountTypeFilter.Futures, AccountTypeFilter.Spot);

        async Task<HttpResult<SharedBalance[]>> IBalanceRestClient.GetBalancesAsync(GetBalancesRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetBalancesOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedBalance[]>(Exchange, validationError);

            var result = await Account.GetAccountsAsync(ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedBalance[]>(result);

            var tradingMode = request.TradingMode ?? TradingMode.Spot;
            var account = result.Data.Accounts.Single(x => x.AccountIndex == ApiCredentials!.Credential.AccountIndex);

            return HttpResult.Ok(result, account.Assets.Select(x =>
                new SharedBalance(
                    tradingMode,
                    x.Symbol,
                    tradingMode == TradingMode.Spot ? (x.Balance - x.LockedBalance) : x.MarginBalance,
                    tradingMode == TradingMode.Spot ? x.Balance : x.MarginBalance)).ToArray());

        }

        #endregion

        #region Spot Order Client

        SharedFeeDeductionType ISpotOrderRestClient.SpotFeeDeductionType => SharedFeeDeductionType.DeductFromOutput;
        SharedFeeAssetType ISpotOrderRestClient.SpotFeeAssetType => SharedFeeAssetType.OutputAsset;
        SharedOrderType[] ISpotOrderRestClient.SpotSupportedOrderTypes { get; } = new[] { SharedOrderType.Limit, SharedOrderType.Market, SharedOrderType.LimitMaker };
        SharedTimeInForce[] ISpotOrderRestClient.SpotSupportedTimeInForce { get; } = new[] { SharedTimeInForce.GoodTillCanceled, SharedTimeInForce.ImmediateOrCancel };
        SharedQuantitySupport ISpotOrderRestClient.SpotSupportedOrderQuantity { get; } = new SharedQuantitySupport(
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset);

        string ISpotOrderRestClient.GenerateClientOrderId() => ExchangeHelpers.RandomLong(9).ToString();

        PlaceSpotOrderOptions ISpotOrderRestClient.PlaceSpotOrderOptions { get; } = new PlaceSpotOrderOptions(_exchangeName)
        {
            RequiredOptionalParameters = new List<ParameterDescription>
            {
                new ParameterDescription(nameof(PlaceSpotOrderRequest.Price), typeof(decimal), "Price for the order. For market orders this should be the current symbol price to calculate max slippage", 21.5m)
            },
        };
        async Task<HttpResult<SharedId>> ISpotOrderRestClient.PlaceSpotOrderAsync(PlaceSpotOrderRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.PlaceSpotOrderOptions.ValidateRequest(request, this);
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
                cid = long.Parse(((ISpotOrderRestClient)SharedClient).GenerateClientOrderId());
            }

            var result = await Trading.PlaceOrderAsync(
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

            return HttpResult.Ok(result, new SharedId(cid.ToString()));

        }

        private decimal GetSlippagePrice(PlaceSpotOrderRequest request)
        {
            // Calculate 5% max slippage
            if (request.Side == SharedOrderSide.Buy)
                return request.Price!.Value * 1.05m;

            return request.Price!.Value * 0.95m;
        }

        GetSpotOrderOptions ISpotOrderRestClient.GetSpotOrderOptions { get; } = new GetSpotOrderOptions(_exchangeName, true);
        async Task<HttpResult<SharedSpotOrder>> ISpotOrderRestClient.GetSpotOrderAsync(GetOrderRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetSpotOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotOrder>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedSpotOrder>(Exchange, ArgumentError.Invalid(nameof(GetOrderRequest.OrderId), "Invalid order id"));

            LighterOrder? orderInfo = null;
            var openOrders = await Trading.GetOpenOrdersAsync(symbol: request.Symbol!.GetSymbol(FormatSymbol), ct: ct).ConfigureAwait(false);
            if (!openOrders.Success)
                return HttpResult.Fail<SharedSpotOrder>(openOrders);

            orderInfo = openOrders.Data.Orders.SingleOrDefault(x => x.OrderId == orderId || x.ClientOrderId == orderId);
            if (orderInfo == null)
            {
                var closedOrders = await Trading.GetClosedOrdersAsync(symbol: request.Symbol!.GetSymbol(FormatSymbol), limit: 100, ct: ct).ConfigureAwait(false);
                if (!closedOrders.Success)
                    return HttpResult.Fail<SharedSpotOrder>(closedOrders);
                orderInfo = closedOrders.Data.Orders.SingleOrDefault(x => x.OrderId == orderId || x.ClientOrderId == orderId);
            }

            if (orderInfo == null)
                return HttpResult.Fail<SharedSpotOrder>(openOrders, new ServerError(ErrorType.UnknownOrder, "Order not found"));

            return HttpResult.Ok(openOrders, new SharedSpotOrder(
                ExchangeSymbolCache.ParseSymbol(_topicSpotId, EnvironmentName, null, LighterUtils.GetSymbolName(EnvironmentName, orderInfo.MarketIndex)),
                LighterUtils.GetSymbolName(EnvironmentName, orderInfo.MarketIndex) ?? string.Empty,
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

        GetOpenSpotOrdersOptions ISpotOrderRestClient.GetOpenSpotOrdersOptions { get; }
            = new GetOpenSpotOrdersOptions(_exchangeName, true);
        async Task<HttpResult<SharedSpotOrder[]>> ISpotOrderRestClient.GetOpenSpotOrdersAsync(GetOpenOrdersRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetOpenSpotOrdersOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotOrder[]>(Exchange, validationError);

            var symbol = request.Symbol?.GetSymbol(FormatSymbol);
            var orders = await Trading.GetOpenOrdersAsync(symbol: symbol, ct: ct).ConfigureAwait(false);
            if (!orders.Success)
                return HttpResult.Fail<SharedSpotOrder[]>(orders);

            var spotOrders = orders.Data.Orders.Where(x => x.MarketIndex >= 2048);
            return HttpResult.Ok(orders, spotOrders.Select(x => new SharedSpotOrder(
                ExchangeSymbolCache.ParseSymbol(_topicSpotId, EnvironmentName, null, LighterUtils.GetSymbolName(EnvironmentName, x.MarketIndex)),
                LighterUtils.GetSymbolName(EnvironmentName, x.MarketIndex) ?? string.Empty,
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

        GetSpotClosedOrdersOptions ISpotOrderRestClient.GetClosedSpotOrdersOptions { get; } = new GetSpotClosedOrdersOptions(_exchangeName, false, true, false, 100);
        async Task<HttpResult<SharedSpotOrder[]>> ISpotOrderRestClient.GetClosedSpotOrdersAsync(GetClosedOrdersRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = SharedClient.GetClosedSpotOrdersOptions.ValidateRequest(request, this);
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
            var orders = await Trading.GetClosedOrdersAsync(
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
                        ExchangeSymbolCache.ParseSymbol(_topicSpotId, EnvironmentName, null, LighterUtils.GetSymbolName(EnvironmentName, x.MarketIndex)),
                        LighterUtils.GetSymbolName(EnvironmentName, x.MarketIndex) ?? string.Empty,
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

        GetSpotOrderTradesOptions ISpotOrderRestClient.GetSpotOrderTradesOptions { get; }
            = new GetSpotOrderTradesOptions(_exchangeName, true);
        async Task<HttpResult<SharedUserTrade[]>> ISpotOrderRestClient.GetSpotOrderTradesAsync(GetOrderTradesRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetSpotOrderTradesOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedUserTrade[]>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedUserTrade[]>(Exchange, ArgumentError.Invalid(nameof(GetOrderTradesRequest.OrderId), "Invalid order id"));

            var orders = await Trading.GetUserTradesAsync(symbol: request.Symbol!.GetSymbol(FormatSymbol), orderIndex: orderId, ct: ct).ConfigureAwait(false);
            if (!orders.Success)
                return HttpResult.Fail<SharedUserTrade[]>(orders);

            return HttpResult.Ok(orders, orders.Data!.Trades.Select(x => new SharedUserTrade(
                request.Symbol,
                LighterUtils.GetSymbolName(EnvironmentName, x.MarketId) ?? string.Empty,
                request.OrderId,
                x.TradeId.ToString(),
                x.AskAccountId == ApiCredentials!.Credential.AccountIndex ? SharedOrderSide.Sell : SharedOrderSide.Buy,
                x.Quantity,
                x.Price,
                x.Timestamp)
            {
                ClientOrderId = (x.BidAccountId == ApiCredentials!.Credential!.AccountIndex ? x.BidClientId : x.AskClientId).ToString(),
                Fee = x.IsMakerAsk == (x.AskAccountId == ApiCredentials.Credential.AccountIndex) ? x.Quantity * x.MakerFee : x.Quantity * x.TakerFee,
                Role = x.IsMakerAsk == (x.AskAccountId == ApiCredentials.Credential.AccountIndex) ? SharedRole.Maker : SharedRole.Taker
            }).ToArray());

        }

        GetSpotUserTradesOptions ISpotOrderRestClient.GetSpotUserTradesOptions { get; } = new GetSpotUserTradesOptions(_exchangeName, false, true, false, 100);
        async Task<HttpResult<SharedUserTrade[]>> ISpotOrderRestClient.GetSpotUserTradesAsync(GetUserTradesRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = SharedClient.GetSpotUserTradesOptions.ValidateRequest(request, this);
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
            var result = await Trading.GetUserTradesAsync(
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
                        LighterUtils.GetSymbolName(EnvironmentName, x.MarketId) ?? string.Empty,
                        (x.BidAccountId == ApiCredentials!.Credential!.AccountIndex ? x.BidId : x.AskId).ToString(),
                        x.TradeId.ToString(),
                        x.AskAccountId == ApiCredentials!.Credential.AccountIndex ? SharedOrderSide.Sell : SharedOrderSide.Buy,
                        x.Quantity,
                        x.Price,
                        x.Timestamp)
                    {
                        ClientOrderId = (x.BidAccountId == ApiCredentials.Credential.AccountIndex ? x.BidClientId : x.AskClientId).ToString(),
                        Fee = x.IsMakerAsk == (x.AskAccountId == ApiCredentials.Credential.AccountIndex) ? x.Quantity * x.MakerFee : x.Quantity * x.TakerFee,
                        Role = x.IsMakerAsk == (x.AskAccountId == ApiCredentials.Credential.AccountIndex) ? SharedRole.Maker : SharedRole.Taker
                    }).ToArray(), nextPageRequest);

        }

        CancelSpotOrderOptions ISpotOrderRestClient.CancelSpotOrderOptions { get; }
            = new CancelSpotOrderOptions(_exchangeName, true);
        async Task<HttpResult<SharedId>> ISpotOrderRestClient.CancelSpotOrderAsync(CancelOrderRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.CancelSpotOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedId>(Exchange, ArgumentError.Invalid(nameof(CancelOrderRequest.OrderId), "Invalid order id"));

            var order = await Trading.CancelOrderAsync(request.Symbol!.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
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

        GetSpotOrderByClientOrderIdOptions ISpotOrderClientIdRestClient.GetSpotOrderByClientOrderIdOptions { get; }
            = new GetSpotOrderByClientOrderIdOptions(_exchangeName, true);
        async Task<HttpResult<SharedSpotOrder>> ISpotOrderClientIdRestClient.GetSpotOrderByClientOrderIdAsync(GetOrderRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetSpotOrderByClientOrderIdOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotOrder>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedSpotOrder>(Exchange, ArgumentError.Invalid(nameof(GetOrderRequest.OrderId), "Invalid order id"));

            LighterOrder? orderInfo = null;
            var openOrders = await Trading.GetOpenOrdersAsync(symbol: request.Symbol!.GetSymbol(FormatSymbol), ct: ct).ConfigureAwait(false);
            if (!openOrders.Success)
                return HttpResult.Fail<SharedSpotOrder>(openOrders);

            orderInfo = openOrders.Data.Orders.SingleOrDefault(x => x.ClientOrderId == orderId);
            if (orderInfo == null)
            {
                var closedOrders = await Trading.GetClosedOrdersAsync(symbol: request.Symbol!.GetSymbol(FormatSymbol), limit: 100, ct: ct).ConfigureAwait(false);
                if (!closedOrders.Success)
                    return HttpResult.Fail<SharedSpotOrder>(closedOrders);
                orderInfo = closedOrders.Data.Orders.SingleOrDefault(x => x.ClientOrderId == orderId);
            }

            if (orderInfo == null)
                return HttpResult.Fail<SharedSpotOrder>(openOrders, new ServerError(ErrorType.UnknownOrder, "Order not found"));

            return HttpResult.Ok(openOrders, new SharedSpotOrder(
                ExchangeSymbolCache.ParseSymbol(_topicSpotId, EnvironmentName, null, LighterUtils.GetSymbolName(EnvironmentName, orderInfo.MarketIndex)),
                LighterUtils.GetSymbolName(EnvironmentName, orderInfo.MarketIndex) ?? string.Empty,
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

        CancelSpotOrderByClientOrderIdOptions ISpotOrderClientIdRestClient.CancelSpotOrderByClientOrderIdOptions { get; }
            = new CancelSpotOrderByClientOrderIdOptions(_exchangeName, true);
        async Task<HttpResult<SharedId>> ISpotOrderClientIdRestClient.CancelSpotOrderByClientOrderIdAsync(CancelOrderRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.CancelSpotOrderByClientOrderIdOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedId>(Exchange, ArgumentError.Invalid(nameof(CancelOrderRequest.OrderId), "Invalid order id"));

            var order = await Trading.CancelOrderAsync(request.Symbol!.GetSymbol(FormatSymbol), orderIndex: orderId, ct: ct).ConfigureAwait(false);
            if (!order.Success)
                return HttpResult.Fail<SharedId>(order);

            return HttpResult.Ok(order, new SharedId(request.OrderId));

        }
        #endregion

        #region Futures Order Client

        SharedFeeDeductionType IFuturesOrderRestClient.FuturesFeeDeductionType => SharedFeeDeductionType.AddToCost;
        SharedFeeAssetType IFuturesOrderRestClient.FuturesFeeAssetType => SharedFeeAssetType.QuoteAsset;

        SharedOrderType[] IFuturesOrderRestClient.FuturesSupportedOrderTypes { get; } = new[] { SharedOrderType.Limit, SharedOrderType.Market, SharedOrderType.LimitMaker };
        SharedTimeInForce[] IFuturesOrderRestClient.FuturesSupportedTimeInForce { get; } = new[] { SharedTimeInForce.GoodTillCanceled, SharedTimeInForce.ImmediateOrCancel };
        SharedQuantitySupport IFuturesOrderRestClient.FuturesSupportedOrderQuantity { get; } = new SharedQuantitySupport(
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset);

        string IFuturesOrderRestClient.GenerateClientOrderId() => ExchangeHelpers.RandomLong(9).ToString();

        PlaceFuturesOrderOptions IFuturesOrderRestClient.PlaceFuturesOrderOptions { get; } = new PlaceFuturesOrderOptions(_exchangeName, false);
        async Task<HttpResult<SharedId>> IFuturesOrderRestClient.PlaceFuturesOrderAsync(PlaceFuturesOrderRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.PlaceFuturesOrderOptions.ValidateRequest(request, this);
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
                cid = long.Parse(((IFuturesOrderRestClient)SharedClient).GenerateClientOrderId());
            }

            var result = await Trading.PlaceOrderAsync(
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

            return HttpResult.Ok(result, new SharedId(cid.ToString()));

        }

        private decimal GetSlippagePrice(PlaceFuturesOrderRequest request)
        {
            // Calculate 5% max slippage
            if (request.Side == SharedOrderSide.Buy)
                return request.Price!.Value * 1.05m;

            return request.Price!.Value * 0.95m;
        }

        GetFuturesOrderOptions IFuturesOrderRestClient.GetFuturesOrderOptions { get; } = new GetFuturesOrderOptions(_exchangeName, true);
        async Task<HttpResult<SharedFuturesOrder>> IFuturesOrderRestClient.GetFuturesOrderAsync(GetOrderRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetFuturesOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFuturesOrder>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedFuturesOrder>(Exchange, ArgumentError.Invalid(nameof(GetOrderRequest.OrderId), "Invalid order id"));

            LighterOrder? orderInfo = null;
            var openOrders = await Trading.GetOpenOrdersAsync(symbol: request.Symbol!.GetSymbol(FormatSymbol), ct: ct).ConfigureAwait(false);
            if (!openOrders.Success)
                return HttpResult.Fail<SharedFuturesOrder>(openOrders);

            orderInfo = openOrders.Data.Orders.SingleOrDefault(x => x.OrderId == orderId || x.ClientOrderId == orderId);
            if (orderInfo == null)
            {
                var closedOrders = await Trading.GetClosedOrdersAsync(symbol: request.Symbol!.GetSymbol(FormatSymbol), limit: 100, ct: ct).ConfigureAwait(false);
                if (!closedOrders.Success)
                    return HttpResult.Fail<SharedFuturesOrder>(closedOrders);
                orderInfo = closedOrders.Data.Orders.SingleOrDefault(x => x.OrderId == orderId || x.ClientOrderId == orderId);
            }

            if (orderInfo == null)
                return HttpResult.Fail<SharedFuturesOrder>(openOrders, new ServerError(ErrorType.UnknownOrder, "Order not found"));

            return HttpResult.Ok(openOrders, new SharedFuturesOrder(
                ExchangeSymbolCache.ParseSymbol(_topicFuturesId, EnvironmentName, null, LighterUtils.GetSymbolName(EnvironmentName, orderInfo.MarketIndex)),
                LighterUtils.GetSymbolName(EnvironmentName, orderInfo.MarketIndex) ?? string.Empty,
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
                IsTriggerOrder = orderInfo.TriggerPrice > 0,
                ReduceOnly = orderInfo.ReduceOnly
            });

        }

        GetOpenFuturesOrdersOptions IFuturesOrderRestClient.GetOpenFuturesOrdersOptions { get; } = new GetOpenFuturesOrdersOptions(_exchangeName, true);
        async Task<HttpResult<SharedFuturesOrder[]>> IFuturesOrderRestClient.GetOpenFuturesOrdersAsync(GetOpenOrdersRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetOpenFuturesOrdersOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFuturesOrder[]>(Exchange, validationError);

            var symbol = request.Symbol?.GetSymbol(FormatSymbol);
            var orders = await Trading.GetOpenOrdersAsync(symbol: symbol, ct: ct).ConfigureAwait(false);
            if (!orders.Success)
                return HttpResult.Fail<SharedFuturesOrder[]>(orders);

            var futuresOrders = orders.Data.Orders.Where(x => x.MarketIndex < 2048);
            return HttpResult.Ok(orders, futuresOrders.Select(x => new SharedFuturesOrder(
                ExchangeSymbolCache.ParseSymbol(_topicFuturesId, EnvironmentName, null, LighterUtils.GetSymbolName(EnvironmentName, x.MarketIndex)),
                LighterUtils.GetSymbolName(EnvironmentName, x.MarketIndex) ?? string.Empty,
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
            }).ToArray());

        }

        GetFuturesClosedOrdersOptions IFuturesOrderRestClient.GetClosedFuturesOrdersOptions { get; } = new GetFuturesClosedOrdersOptions(_exchangeName, true, true, true, 100);
        async Task<HttpResult<SharedFuturesOrder[]>> IFuturesOrderRestClient.GetClosedFuturesOrdersAsync(GetClosedOrdersRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = SharedClient.GetClosedFuturesOrdersOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFuturesOrder[]>(Exchange, validationError);

            var direction = DataDirection.Descending;
            var limit = request.Limit ?? 100;
            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var pageParams = Pagination.GetPaginationParameters(
                direction, limit, request.StartTime,
                request.EndTime ?? DateTime.UtcNow,
                pageRequest);

            // Get data
            var orders = await Trading.GetClosedOrdersAsync(
                symbol: symbol,
                limit: limit,
                cursor: pageParams.Cursor,
                ct: ct).ConfigureAwait(false);
            if (!orders.Success)
                return HttpResult.Fail<SharedFuturesOrder[]>(orders);

            var futuresOrders = orders.Data.Orders.Where(x => x.MarketIndex < 2048);
            var nextPageRequest = Pagination.GetNextPageRequest(
                   () => orders.Data.NextCursor == null ? null : Pagination.NextPageFromCursor(orders.Data.NextCursor),
                   orders.Data.Orders.Length,
                   orders.Data.Orders.Select(x => x.CreateTime),
                   request.StartTime,
                   request.EndTime ?? DateTime.UtcNow,
                   pageParams);

            return HttpResult.Ok(orders, ExchangeHelpers.ApplyFilter(futuresOrders, x => x.CreateTime, request.StartTime, request.EndTime, direction)
                    .Select(x => new SharedFuturesOrder(
                        ExchangeSymbolCache.ParseSymbol(_topicFuturesId, EnvironmentName, null, LighterUtils.GetSymbolName(EnvironmentName, x.MarketIndex)),
                        LighterUtils.GetSymbolName(EnvironmentName, x.MarketIndex) ?? string.Empty,
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
                    }).ToArray(), nextPageRequest);
        }

        GetFuturesOrderTradesOptions IFuturesOrderRestClient.GetFuturesOrderTradesOptions { get; } = new GetFuturesOrderTradesOptions(_exchangeName, true);
        async Task<HttpResult<SharedUserTrade[]>> IFuturesOrderRestClient.GetFuturesOrderTradesAsync(GetOrderTradesRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetFuturesOrderTradesOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedUserTrade[]>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedUserTrade[]>(Exchange, ArgumentError.Invalid(nameof(GetOrderTradesRequest.OrderId), "Invalid order id"));

            var orders = await Trading.GetUserTradesAsync(symbol: request.Symbol!.GetSymbol(FormatSymbol), orderIndex: orderId, ct: ct).ConfigureAwait(false);
            if (!orders.Success)
                return HttpResult.Fail<SharedUserTrade[]>(orders);

            return HttpResult.Ok(orders, orders.Data!.Trades.Select(x => new SharedUserTrade(
                request.Symbol,
                LighterUtils.GetSymbolName(EnvironmentName, x.MarketId) ?? string.Empty,
                request.OrderId,
                x.TradeId.ToString(),
                x.AskAccountId == ApiCredentials!.Credential.AccountIndex ? SharedOrderSide.Sell : SharedOrderSide.Buy,
                x.Quantity,
                x.Price,
                x.Timestamp)
            {
                ClientOrderId = (x.BidAccountId == ApiCredentials!.Credential!.AccountIndex ? x.BidClientId : x.AskClientId).ToString(),
                Fee = x.IsMakerAsk == (x.AskAccountId == ApiCredentials.Credential.AccountIndex) ? x.Quantity * x.MakerFee : x.Quantity * x.TakerFee,
                Role = x.IsMakerAsk == (x.AskAccountId == ApiCredentials.Credential.AccountIndex) ? SharedRole.Maker : SharedRole.Taker
            }).ToArray());

        }

        GetFuturesUserTradesOptions IFuturesOrderRestClient.GetFuturesUserTradesOptions { get; } = new GetFuturesUserTradesOptions(_exchangeName, true, true, true, 100);
        async Task<HttpResult<SharedUserTrade[]>> IFuturesOrderRestClient.GetFuturesUserTradesAsync(GetUserTradesRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = SharedClient.GetFuturesUserTradesOptions.ValidateRequest(request, this);
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
            var result = await Trading.GetUserTradesAsync(
                symbol: symbol,
                limit: limit,
                cursor: pageParams.Cursor,
                ct: ct
                ).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedUserTrade[]>(result);

            var futuresTrades = result.Data.Trades.Where(x => x.MarketId < 2048);
            var nextPageRequest = Pagination.GetNextPageRequest(
                () => result.Data.NextCursor == null ? null : Pagination.NextPageFromCursor(result.Data.NextCursor),
                result.Data!.Trades.Length,
                result.Data.Trades.Select(x => x.Timestamp),
                request.StartTime,
                request.EndTime ?? DateTime.UtcNow,
                pageParams);

            return HttpResult.Ok(result, ExchangeHelpers.ApplyFilter(futuresTrades, x => x.Timestamp, request.StartTime, request.EndTime, direction)
                    .Select(x => new SharedUserTrade(
                        request.Symbol,
                        LighterUtils.GetSymbolName(EnvironmentName, x.MarketId) ?? string.Empty,
                        (x.BidAccountId == ApiCredentials!.Credential!.AccountIndex ? x.BidId : x.AskId).ToString(),
                        x.TradeId.ToString(),
                        x.AskAccountId == ApiCredentials!.Credential.AccountIndex ? SharedOrderSide.Sell : SharedOrderSide.Buy,
                        x.Quantity,
                        x.Price,
                        x.Timestamp)
                    {
                        ClientOrderId = (x.BidAccountId == ApiCredentials.Credential.AccountIndex ? x.BidClientId : x.AskClientId).ToString(),
                        Fee = x.IsMakerAsk == (x.AskAccountId == ApiCredentials.Credential.AccountIndex) ? x.Quantity * x.MakerFee : x.Quantity * x.TakerFee,
                        Role = x.IsMakerAsk == (x.AskAccountId == ApiCredentials.Credential.AccountIndex) ? SharedRole.Maker : SharedRole.Taker
                    }).ToArray(), nextPageRequest);

        }

        CancelFuturesOrderOptions IFuturesOrderRestClient.CancelFuturesOrderOptions { get; } = new CancelFuturesOrderOptions(_exchangeName, true);
        async Task<HttpResult<SharedId>> IFuturesOrderRestClient.CancelFuturesOrderAsync(CancelOrderRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.CancelFuturesOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedId>(Exchange, ArgumentError.Invalid(nameof(CancelOrderRequest.OrderId), "Invalid order id"));

            var order = await Trading.CancelOrderAsync(request.Symbol!.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
            if (!order.Success)
                return HttpResult.Fail<SharedId>(order);

            return HttpResult.Ok(order, new SharedId(request.OrderId));

        }

        GetPositionsOptions IFuturesOrderRestClient.GetPositionsOptions { get; } = new GetPositionsOptions(_exchangeName, true);
        async Task<HttpResult<SharedPosition[]>> IFuturesOrderRestClient.GetPositionsAsync(GetPositionsRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetPositionsOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedPosition[]>(Exchange, validationError);

            var result = await Account.GetAccountsAsync(ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedPosition[]>(result);

            var account = result.Data.Accounts.Single(x => x.AccountIndex == ApiCredentials!.Credential!.AccountIndex);

            return HttpResult.Ok(result, account.Positions.Select(x =>
                new SharedPosition(
                    ExchangeSymbolCache.ParseSymbol(_topicFuturesId, EnvironmentName, null, x.Symbol),
                    x.Symbol,
                    Math.Abs(x.Position),
                    null)
                {
                    AverageOpenPrice = x.AverageEntryPrice,
                    PositionMode = SharedPositionMode.OneWay,
                    PositionSide = x.PositionSide == Enums.PositionSide.Short ? SharedPositionSide.Short : SharedPositionSide.Long,
                    UnrealizedPnl = x.UnrealizedPnl,
                    LiquidationPrice = x.LiquidationPrice != 0 ? x.LiquidationPrice : null
                }).ToArray());

        }

        ClosePositionOptions IFuturesOrderRestClient.ClosePositionOptions { get; } = new ClosePositionOptions(_exchangeName, true)
        {
            RequiredOptionalParameters = new List<ParameterDescription>
            {
                new ParameterDescription(nameof(ClosePositionRequest.PositionSide), typeof(SharedPositionSide), "The position side to close", SharedPositionSide.Long),
                new ParameterDescription(nameof(ClosePositionRequest.Quantity), typeof(decimal), "Quantity of the position is required", 0.1m)
            },
            RequiredExchangeParameters = new List<ParameterDescription>
            {
                new ParameterDescription("Price", typeof(decimal), "The current price of the symbol. Required to calculate max slippage.", 21.5m)
            },
        };
        async Task<HttpResult<SharedId>> IFuturesOrderRestClient.ClosePositionAsync(ClosePositionRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.ClosePositionOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            long cid = long.Parse(((IFuturesOrderRestClient)SharedClient).GenerateClientOrderId());

            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var result = await Trading.PlaceOrderAsync(
                symbol,
                request.PositionSide == SharedPositionSide.Long ? OrderSide.Sell : OrderSide.Buy,
                OrderType.Market,
                request.Quantity!.Value,
                price: GetSlippagePrice(request.PositionSide!.Value, request.GetParamValue<decimal>(_exchangeName, "price")),
                timeInForce: TimeInForce.ImmediateOrCancel,
                reduceOnly: true,
                clientOrderIndex: cid,
                ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedId>(result);

            return HttpResult.Ok(result, new SharedId(cid.ToString()));

        }

        private decimal GetSlippagePrice(SharedPositionSide side, decimal price)
        {
            // Calculate 5% max slippage
            if (side == SharedPositionSide.Short)
                return price * 1.05m;

            return price * 0.95m;
        }
        #endregion

        #region Futures Client Id Order Client

        GetFuturesOrderByClientOrderIdOptions IFuturesOrderClientIdRestClient.GetFuturesOrderByClientOrderIdOptions { get; } = new GetFuturesOrderByClientOrderIdOptions(_exchangeName, true);
        async Task<HttpResult<SharedFuturesOrder>> IFuturesOrderClientIdRestClient.GetFuturesOrderByClientOrderIdAsync(GetOrderRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetFuturesOrderByClientOrderIdOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFuturesOrder>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedFuturesOrder>(Exchange, ArgumentError.Invalid(nameof(GetOrderRequest.OrderId), "Invalid order id"));

            LighterOrder? orderInfo = null;
            var openOrders = await Trading.GetOpenOrdersAsync(symbol: request.Symbol!.GetSymbol(FormatSymbol), ct: ct).ConfigureAwait(false);
            if (!openOrders.Success)
                return HttpResult.Fail<SharedFuturesOrder>(openOrders);

            orderInfo = openOrders.Data.Orders.SingleOrDefault(x => x.ClientOrderId == orderId);
            if (orderInfo == null)
            {
                var closedOrders = await Trading.GetClosedOrdersAsync(symbol: request.Symbol!.GetSymbol(FormatSymbol), limit: 100, ct: ct).ConfigureAwait(false);
                if (!closedOrders.Success)
                    return HttpResult.Fail<SharedFuturesOrder>(closedOrders);
                orderInfo = closedOrders.Data.Orders.SingleOrDefault(x => x.ClientOrderId == orderId);
            }

            if (orderInfo == null)
                return HttpResult.Fail<SharedFuturesOrder>(openOrders, new ServerError(ErrorType.UnknownOrder, "Order not found"));

            return HttpResult.Ok(openOrders, new SharedFuturesOrder(
                ExchangeSymbolCache.ParseSymbol(_topicFuturesId, EnvironmentName, null, LighterUtils.GetSymbolName(EnvironmentName, orderInfo.MarketIndex)),
                LighterUtils.GetSymbolName(EnvironmentName, orderInfo.MarketIndex) ?? string.Empty,
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
                IsTriggerOrder = orderInfo.TriggerPrice > 0,
                ReduceOnly = orderInfo.ReduceOnly
            });

        }

        CancelFuturesOrderByClientOrderIdOptions IFuturesOrderClientIdRestClient.CancelFuturesOrderByClientOrderIdOptions { get; } = new CancelFuturesOrderByClientOrderIdOptions(_exchangeName, true);
        async Task<HttpResult<SharedId>> IFuturesOrderClientIdRestClient.CancelFuturesOrderByClientOrderIdAsync(CancelOrderRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.CancelFuturesOrderByClientOrderIdOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedId>(Exchange, ArgumentError.Invalid(nameof(CancelOrderRequest.OrderId), "Invalid order id"));

            var order = await Trading.CancelOrderAsync(request.Symbol!.GetSymbol(FormatSymbol), orderIndex: orderId, ct: ct).ConfigureAwait(false);
            if (!order.Success)
                return HttpResult.Fail<SharedId>(order);

            return HttpResult.Ok(order, new SharedId(request.OrderId));

        }
        #endregion

        #region Leverage client
        SharedLeverageSettingMode ILeverageRestClient.LeverageSettingType => SharedLeverageSettingMode.PerSymbol;

        GetLeverageOptions ILeverageRestClient.GetLeverageOptions { get; } = new GetLeverageOptions(_exchangeName, true);
        async Task<HttpResult<SharedLeverage>> ILeverageRestClient.GetLeverageAsync(GetLeverageRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetLeverageOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedLeverage>(Exchange, validationError);

            var result = await Account.GetAccountsAsync().ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedLeverage>(result);

            var account = result.Data.Accounts.Single(x => x.AccountIndex == ApiCredentials!.Credential!.AccountIndex);
            var position = account.Positions.SingleOrDefault(x => x.Symbol.Equals(request.Symbol!.GetSymbol(FormatSymbol), StringComparison.InvariantCultureIgnoreCase));
            if (position == null)
                return HttpResult.Fail<SharedLeverage>(Exchange, new ServerError(new ErrorInfo(ErrorType.Unknown, false, "Position not found")));

            return HttpResult.Ok(result, new SharedLeverage(100 / position.InitialMarginFraction));
        }

        SetLeverageOptions ILeverageRestClient.SetLeverageOptions { get; } = new SetLeverageOptions(_exchangeName)
        {
            RequiredOptionalParameters = new List<ParameterDescription>
            {
                new ParameterDescription(nameof(SetLeverageRequest.MarginMode), typeof(SharedMarginMode), "The margin mode to change leverage for", SharedMarginMode.Cross)
            }
        };
        async Task<HttpResult<SharedLeverage>> ILeverageRestClient.SetLeverageAsync(SetLeverageRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.SetLeverageOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedLeverage>(Exchange, validationError);

            var result = await Account.SetLeverageAsync(
                symbol: request.Symbol!.GetSymbol(FormatSymbol), 
                (int)request.Leverage, 
                request.MarginMode == SharedMarginMode.Isolated ? MarginMode.IsolatedMargin : MarginMode.CrossMargin,
                ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedLeverage>(result);

            return HttpResult.Ok(result, new SharedLeverage(request.Leverage));

        }
        #endregion

        #region Open Interest client

        GetOpenInterestOptions IOpenInterestRestClient.GetOpenInterestOptions { get; } = new GetOpenInterestOptions(_exchangeName, true);
        async Task<HttpResult<SharedOpenInterest>> IOpenInterestRestClient.GetOpenInterestAsync(GetOpenInterestRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetOpenInterestOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedOpenInterest>(Exchange, validationError);

            var result = await ExchangeData.GetSymbolDetailsAsync(request.Symbol!.GetSymbol(FormatSymbol), ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedOpenInterest>(result);

            if (result.Data.PerpSymbols.Length == 0)
                return HttpResult.Fail<SharedOpenInterest>(result, new ServerError(ErrorType.UnknownSymbol, "Symbol not found"));

            return HttpResult.Ok(result, new SharedOpenInterest(result.Data.PerpSymbols[0].OpenInterest));

        }

        #endregion

        #region Funding Rate client
        GetFundingRateHistoryOptions IFundingRateRestClient.GetFundingRateHistoryOptions { get; } = 
            new GetFundingRateHistoryOptions(_exchangeName, false, true, true, 100, false)
            {
                OptionalExchangeParameters = new List<ParameterDescription>
                {
                    new ParameterDescription("Resolution", typeof(FundingResolution), "The resolution of the data, by default 1H", FundingResolution.OneDay)
                }
            };

        async Task<HttpResult<SharedFundingRate[]>> IFundingRateRestClient.GetFundingRateHistoryAsync(GetFundingRateHistoryRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = SharedClient.GetFundingRateHistoryOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFundingRate[]>(Exchange, validationError);

            int limit = request.Limit ?? 100;
            var direction = DataDirection.Descending;
            var pageParams = Pagination.GetPaginationParameters(direction, limit, request.StartTime, request.EndTime ?? DateTime.UtcNow, pageRequest, false);

            // Get data
            var result = await ExchangeData.GetFundingRateHistoryAsync(
                request.Symbol!.GetSymbol(FormatSymbol),
                request.GetParamValue<FundingResolution?>(_exchangeName, "Resolution") ?? FundingResolution.OneHour,
                startTime: pageParams.StartTime,
                endTime: pageParams.EndTime,
                limit: pageParams.Limit,
                ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedFundingRate[]>(result);

            var nextPageRequest = Pagination.GetNextPageRequest(
                     () => Pagination.NextPageFromTime(pageParams, result.Data.Fundings.Min(x => x.Timestamp)),
                     result.Data.Fundings.Length,
                     result.Data.Fundings.Select(x => x.Timestamp),
                     request.StartTime,
                     request.EndTime ?? DateTime.UtcNow,
                     pageParams);

            return HttpResult.Ok(result, ExchangeHelpers.ApplyFilter(result.Data.Fundings, x => x.Timestamp, request.StartTime, request.EndTime, direction)
                    .Select(x =>
                        new SharedFundingRate(x.Rate, x.Timestamp))
                    .ToArray(), nextPageRequest);
        }
        #endregion
    }
}
