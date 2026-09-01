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
    internal class LighterRestClientExchangeSharedApi :
        SharedApiBase,
        ILighterRestClientExchangeApiShared,
        ILighterRestClientExchangeSharedApi
    {
        private readonly LighterRestClientExchangeApi _api;

        private const string _exchangeName = "Lighter";
        private const string _topicSpotId = "LighterSpot";
        private const string _topicFuturesId = "LighterFutures";

        public override SharedClientInfo Discover() => SharedUtils.GetClientInfo(LighterExchange.Metadata, this);

        public LighterRestClientExchangeSharedApi(LighterRestClientExchangeApi api)
            : base(
                  api.Exchange,
                  [TradingMode.Spot, TradingMode.PerpetualLinear],
                  () => api.Authenticated,
                  api.FormatSymbol)
        {
            _api = api;

            SetCapabilities(
                GetKlinesOptions,
                GetSpotSymbolsOptions,
                GetFuturesSymbolsOptions,
                GetSpotTickerOptions,
                GetAllSpotTickersOptions,
                GetFuturesTickerOptions,
                GetAllFuturesTickersOptions,
                GetBookTickerOptions,
                GetRecentTradesOptions,
                GetOrderBookOptions,
                GetAssetOptions,
                GetAllAssetsOptions,
                GetDepositHistoryOptions,
                GetWithdrawalHistoryOptions,
                GetFeeOptions,
                GetBalancesOptions,
                PlaceSpotOrderOptions,
                GetSpotOrderOptions,
                GetOpenSpotOrdersOptions,
                GetClosedSpotOrdersOptions,
                CancelSpotOrderOptions,
                GetSpotUserTradeHistoryOptions,
                GetSpotOrderTradesOptions,
                GetSpotOrderByClientOrderIdOptions,
                CancelSpotOrderByClientOrderIdOptions,
                PlaceFuturesOrderOptions,
                GetFuturesOrderOptions,
                GetOpenFuturesOrdersOptions,
                GetClosedFuturesOrdersOptions,
                CancelFuturesOrderOptions,
                GetFuturesOrderTradesOptions,
                GetFuturesUserTradeHistoryOptions,
                ClosePositionOptions,
                GetPositionsOptions,
                GetFuturesOrderByClientOrderIdOptions,
                CancelFuturesOrderByClientOrderIdOptions,
                GetLeverageOptions,
                SetLeverageOptions,
                GetOpenFuturesOrdersOptions,
                GetFundingRateHistoryOptions,
                GetOpenInterestOptions
                );
        }

        #region Klines Client

        public GetKlinesOptions GetKlinesOptions { get; } = new GetKlinesOptions(_exchangeName, false, true, true, 500, false, [
            SharedKlineInterval.OneMinute,
            SharedKlineInterval.FiveMinutes,
            SharedKlineInterval.FifteenMinutes,
            SharedKlineInterval.ThirtyMinutes,
            SharedKlineInterval.OneHour,
            SharedKlineInterval.FourHours,
            SharedKlineInterval.TwelveHours,
            SharedKlineInterval.OneDay
            ]);
        public async Task<HttpResult<SharedKline[]>> GetKlinesAsync(GetKlinesRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = GetKlinesOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedKline[]>(Exchange, validationError);

            var direction = DataDirection.Descending;
            var symbol = request.SymbolName(FormatSymbol);
            var limit = request.Limit ?? GetKlinesOptions.MaxLimit;
            var pageParams = Pagination.GetPaginationParameters(
                direction,
                limit,
                request.StartTime ?? (request.EndTime ?? DateTime.UtcNow).AddSeconds(-((int)request.Interval * 1000)),
                request.EndTime ?? DateTime.UtcNow,
                pageRequest);

            // Get data
            var result = await _api.ExchangeData.GetKlinesAsync(
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
                        new SharedKline(
                            request.Symbol,
                            symbol,
                            x.OpenTime,
                            x.ClosePrice,
                            x.HighPrice,
                            x.LowPrice,
                            x.OpenPrice,
                            new SharedOrderQuantity(x.Volume, x.QuoteVolume)))
                    .ToArray(), nextPageRequest);

        }

        #endregion

        #region Spot Symbol client

        public SharedSymbolCatalog? SpotSymbolCatalog => ExchangeSymbolCache.GetSymbolCatalog(_exchangeName, _topicSpotId, _api.EnvironmentName, null);
        public GetSpotSymbolsOptions GetSpotSymbolsOptions { get; }
            = new GetSpotSymbolsOptions(_exchangeName, false);

        public async Task<HttpResult<SharedSpotSymbol[]>> GetSpotSymbolsAsync(GetSymbolsRequest request, CancellationToken ct)
        {
            var validationError = GetSpotSymbolsOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotSymbol[]>(Exchange, validationError);

            var assetTask = _api.ExchangeData.GetAssetsAsync();
            var symbolTask = _api.ExchangeData.GetSymbolsAsync(symbolType: SymbolTypeFilter.Spot, ct: ct);
            await Task.WhenAll(assetTask, symbolTask).ConfigureAwait(false);
            var assetsResult = assetTask.Result;
            var symbolsResult = symbolTask.Result;
            if (!assetsResult.Success)
                return HttpResult.Fail<SharedSpotSymbol[]>(assetsResult);
            if (!symbolsResult.Success)
                return HttpResult.Fail<SharedSpotSymbol[]>(symbolsResult);

            var data = symbolsResult.Data
               .Select(x => ParseSpotSymbol(x, assetsResult.Data)!)
               .Where(x => x != null)
               .ToArray();

            ExchangeSymbolCache.UpdateSymbolInfo(_topicSpotId, _api.EnvironmentName, null, data);
            return HttpResult.Ok(symbolsResult, SharedUtils.ApplySymbolFilter(data, request));
        }

        private SharedSpotSymbol? ParseSpotSymbol(LighterSymbol s, LighterAsset[] assets)
        {
            var baseAsset = assets.SingleOrDefault(x => x.AssetId == s.BaseAssetId);
            var quoteAsset = assets.SingleOrDefault(x => x.AssetId == s.QuoteAssetId);
            if (baseAsset == null || quoteAsset == null)
                return null;

            var result = new SharedSpotSymbol(baseAsset.Symbol, quoteAsset.Symbol, s.Symbol, s.Status == SymbolStatus.Active)
            {
                MinTradeQuantity = s.MinBaseQuantity,
                MinNotionalValue = s.MinQuoteQuantity,
                PriceDecimals = s.SupportedPriceDecimals,
                QuantityDecimals = s.SupportedQuantityDecimals,
                DisplayName = baseAsset.Symbol,
                BaseAssetType = SharedAssetType.Crypto,
                QuoteAssetType = SharedAssetType.Crypto,
                QuoteAssetSubType = SharedAssetSubType.StableCoin,
                TakerFeePercentage = s.TakerFee,
                MakerFeePercentage = s.MakerFee
            };

            return result;
        }

        public async Task<ExchangeCallResult<SharedSymbol[]>> GetSpotSymbolsForBaseAssetAsync(string baseAsset)
        {
            if (!ExchangeSymbolCache.HasCached(_topicSpotId, _api.EnvironmentName, null))
            {
                var symbols = await GetSpotSymbolsAsync(new GetSymbolsRequest(), default).ConfigureAwait(false);
                if (!symbols.Success)
                    return ExchangeCallResult<SharedSymbol[]>.Fail(Exchange, symbols.Error!);
            }

            return ExchangeCallResult<SharedSymbol[]>.Ok(Exchange, ExchangeSymbolCache.GetSymbolsForBaseAsset(_topicSpotId, _api.EnvironmentName, null, baseAsset));
        }

        public async Task<ExchangeCallResult<bool>> SupportsSpotSymbolAsync(SharedSymbol symbol)
        {
            if (symbol.TradingMode != TradingMode.Spot)
                throw new ArgumentException(nameof(symbol), "Only Spot symbols allowed");

            if (!ExchangeSymbolCache.HasCached(_topicSpotId, _api.EnvironmentName, null))
            {
                var symbols = await GetSpotSymbolsAsync(new GetSymbolsRequest(), default).ConfigureAwait(false);
                if (!symbols.Success)
                    return ExchangeCallResult<bool>.Fail(Exchange, symbols.Error!);
            }

            return ExchangeCallResult<bool>.Ok(Exchange, ExchangeSymbolCache.SupportsSymbol(_topicSpotId, _api.EnvironmentName, null, symbol));
        }

        public async Task<ExchangeCallResult<bool>> SupportsSpotSymbolAsync(string symbolName)
        {
            if (!ExchangeSymbolCache.HasCached(_topicSpotId, _api.EnvironmentName, null))
            {
                var symbols = await GetSpotSymbolsAsync(new GetSymbolsRequest(), default).ConfigureAwait(false);
                if (!symbols.Success)
                    return ExchangeCallResult<bool>.Fail(Exchange, symbols.Error!);
            }

            return ExchangeCallResult<bool>.Ok(Exchange, ExchangeSymbolCache.SupportsSymbol(_topicSpotId, _api.EnvironmentName, null, symbolName));
        }
        #endregion

        #region Futures Symbol client

        public SharedSymbolCatalog? FuturesSymbolCatalog => ExchangeSymbolCache.GetSymbolCatalog(_exchangeName, _topicFuturesId, _api.EnvironmentName, null);
        public GetFuturesSymbolsOptions GetFuturesSymbolsOptions { get; } = new GetFuturesSymbolsOptions(_exchangeName, false);
        public async Task<HttpResult<SharedFuturesSymbol[]>> GetFuturesSymbolsAsync(GetSymbolsRequest request, CancellationToken ct)
        {
            var validationError = GetSpotSymbolsOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFuturesSymbol[]>(Exchange, validationError);

            var symbolsTask = _api.ExchangeData.GetSymbolsAsync(symbolType: SymbolTypeFilter.Perp, ct: ct);
            var tokensTask = _api.ExchangeData.GetTokensAsync(ct: ct);
            await Task.WhenAll(symbolsTask, tokensTask).ConfigureAwait(false);
            var resultSymbols = symbolsTask.Result;
            var resultTokens = tokensTask.Result;
            if (!resultSymbols.Success)
                return HttpResult.Fail<SharedFuturesSymbol[]>(resultSymbols);
            if (!resultTokens.Success)
                return HttpResult.Fail<SharedFuturesSymbol[]>(resultTokens);

            var resultData =
                resultSymbols.Data!
                .Select(x => ParseFuturesSymbol(x, resultTokens.Data))
                .ToArray();

            // Register both LIT/USDC and LIT as symbol names
            var symbolRegistrations = resultData
                .Concat(resultSymbols.Data.Select(x => new SharedFuturesSymbol(TradingMode.PerpetualLinear, x.Symbol, "USDC", x.Symbol, true))).ToArray();

            ExchangeSymbolCache.UpdateSymbolInfo(_topicFuturesId, _api.EnvironmentName, null, symbolRegistrations);
            return HttpResult.Ok(resultSymbols, SharedUtils.ApplySymbolFilter(resultData, request));
        }

        private SharedFuturesSymbol ParseFuturesSymbol(LighterSymbol s, LighterToken[] tokens)
        {
            var result = new SharedFuturesSymbol(TradingMode.PerpetualLinear, s.Symbol, "USDC", $"{s.Symbol}/USDC", s.Status == SymbolStatus.Active)
            {
                MinTradeQuantity = s.MinBaseQuantity,
                MinNotionalValue = s.MinQuoteQuantity,
                PriceDecimals = s.SupportedPriceDecimals,
                QuantityDecimals = s.SupportedQuantityDecimals,
                DisplayName = s.Symbol,
                ContractSize = s.Multiplier,
                QuoteAssetType = SharedAssetType.Crypto,
                QuoteAssetSubType = SharedAssetSubType.StableCoin,
                TakerFeePercentage = s.TakerFee,
                MakerFeePercentage = s.MakerFee
            };

            var compareName = s.Symbol.StartsWith("1000") ? "k" + s.Symbol.Substring(4) : s.Symbol;
            var tokenInfo = tokens.SingleOrDefault(x => x.Symbol == compareName
                && ((x.MarketType == MarketType.Spot && s.MarketType == SymbolType.Spot) 
                || (x.MarketType == MarketType.Perps && s.MarketType == SymbolType.Perp)));
            if (tokenInfo != null)
            {
                if (tokenInfo.AssetType == AssetType.Crypto)
                {
                    result.BaseAssetType = SharedAssetType.Crypto;
                }
                else 
                {
                    if (tokenInfo.Categories.Contains("FX", StringComparer.OrdinalIgnoreCase))
                    {
                        result.BaseAssetType = SharedAssetType.Fiat;
                    }
                    else if (tokenInfo.Categories.Contains("STOCK", StringComparer.OrdinalIgnoreCase)
                        || tokenInfo.Categories.Contains("ETF", StringComparer.OrdinalIgnoreCase)
                        || tokenInfo.Categories.Contains("PRE_IPO", StringComparer.OrdinalIgnoreCase))
                    {
                        result.BaseAssetType = SharedAssetType.TradFi;
                        result.BaseAssetSubType = SharedAssetSubType.Equity;
                    }
                    else if (tokenInfo.Categories.Contains("COMMODITIES", StringComparer.OrdinalIgnoreCase))
                    {
                        result.BaseAssetType = SharedAssetType.TradFi;
                        result.BaseAssetSubType = SharedAssetSubType.Commodity;
                    }
                    else
                    {
                        result.BaseAssetType = SharedAssetType.TradFi;
                    }
                }
            }

            return result;
        }

        public async Task<ExchangeCallResult<SharedSymbol[]>> GetFuturesSymbolsForBaseAssetAsync(string baseAsset)
        {
            if (!ExchangeSymbolCache.HasCached(_topicFuturesId, _api.EnvironmentName, null))
            {
                var symbols = await GetFuturesSymbolsAsync(new GetSymbolsRequest(), default).ConfigureAwait(false);
                if (!symbols.Success)
                    return ExchangeCallResult<SharedSymbol[]>.Fail(Exchange, symbols.Error!);
            }

            return ExchangeCallResult<SharedSymbol[]>.Ok(Exchange, ExchangeSymbolCache.GetSymbolsForBaseAsset(_topicFuturesId, _api.EnvironmentName, null, baseAsset));
        }

        public async Task<ExchangeCallResult<bool>> SupportsFuturesSymbolAsync(SharedSymbol symbol)
        {
            if (symbol.TradingMode == TradingMode.Spot)
                throw new ArgumentException(nameof(symbol), "Spot symbols not allowed");

            if (!ExchangeSymbolCache.HasCached(_topicFuturesId, _api.EnvironmentName, null))
            {
                var symbols = await GetFuturesSymbolsAsync(new GetSymbolsRequest(), default).ConfigureAwait(false);
                if (!symbols.Success)
                    return ExchangeCallResult<bool>.Fail(Exchange, symbols.Error!);
            }

            return ExchangeCallResult<bool>.Ok(Exchange, ExchangeSymbolCache.SupportsSymbol(_topicFuturesId, _api.EnvironmentName, null, symbol));
        }

        public async Task<ExchangeCallResult<bool>> SupportsFuturesSymbolAsync(string symbolName)
        {
            if (!ExchangeSymbolCache.HasCached(_topicFuturesId, _api.EnvironmentName, null))
            {
                var symbols = await GetFuturesSymbolsAsync(new GetSymbolsRequest(), default).ConfigureAwait(false);
                if (!symbols.Success)
                    return ExchangeCallResult<bool>.Fail(Exchange, symbols.Error!);
            }

            return ExchangeCallResult<bool>.Ok(Exchange, ExchangeSymbolCache.SupportsSymbol(_topicFuturesId, _api.EnvironmentName, null, symbolName));
        }
        #endregion

        #region Spot Ticker client

        public GetSpotTickerOptions GetSpotTickerOptions { get; } = new GetSpotTickerOptions(_exchangeName);
        public async Task<HttpResult<SharedSpotTicker>> GetSpotTickerAsync(GetTickerRequest request, CancellationToken ct)
        {
            var validationError = GetSpotTickerOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotTicker>(Exchange, validationError);

            var result = await _api.ExchangeData.GetSymbolDetailsAsync(request.SymbolName(FormatSymbol), ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedSpotTicker>(result);

            return HttpResult.Ok(result, new SharedSpotTicker(
                    request.Symbol,
                    result.Data.SpotSymbols[0].Symbol,
                    result.Data.SpotSymbols[0].LastPrice,
                    result.Data.SpotSymbols[0].HighPrice,
                    result.Data.SpotSymbols[0].LowPrice,
                    new SharedOrderQuantity(result.Data.SpotSymbols[0].Volume, result.Data.SpotSymbols[0].QuoteVolume),
                    result.Data.SpotSymbols[0].PriceChangePercentage)
            {
            });

        }

        Task<HttpResult<SharedSpotTicker[]>> ISpotTickerRestClient.GetSpotTickersAsync(GetTickersRequest request, CancellationToken ct)
            => GetAllSpotTickersAsync(request, ct);
        GetAllSpotTickersOptions ISpotTickerRestClient.GetSpotTickersOptions => GetAllSpotTickersOptions;

        public GetAllSpotTickersOptions GetAllSpotTickersOptions { get; } = new GetAllSpotTickersOptions(_exchangeName);
        public async Task<HttpResult<SharedSpotTicker[]>> GetAllSpotTickersAsync(GetTickersRequest request, CancellationToken ct)
        {
            var validationError = GetAllSpotTickersOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotTicker[]>(Exchange, validationError);

            var result = await _api.ExchangeData.GetSymbolDetailsAsync(symbolType: SymbolTypeFilter.Spot, ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedSpotTicker[]>(result);

            return HttpResult.Ok(result, result.Data!.SpotSymbols.Select(x =>
                    new SharedSpotTicker(
                        ExchangeSymbolCache.ParseSymbol(_topicSpotId, _api.EnvironmentName, null, x.Symbol),
                        x.Symbol,
                        x.LastPrice,
                        x.HighPrice,
                        x.LowPrice,
                        new SharedOrderQuantity(x.Volume, x.QuoteVolume),
                        x.PriceChangePercentage)
                    {
                    }).ToArray());

        }

        #endregion

        #region Futures Ticker client

        public GetFuturesTickerOptions GetFuturesTickerOptions { get; } = new GetFuturesTickerOptions(_exchangeName);
        public async Task<HttpResult<SharedFuturesTicker>> GetFuturesTickerAsync(GetTickerRequest request, CancellationToken ct)
        {
            var validationError = GetFuturesTickerOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFuturesTicker>(Exchange, validationError);

            var result = await _api.ExchangeData.GetSymbolDetailsAsync(request.SymbolName(FormatSymbol), ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedFuturesTicker>(result);

            return HttpResult.Ok(result, new SharedFuturesTicker(
                    request.Symbol,
                    result.Data.PerpSymbols[0].Symbol,
                    result.Data.PerpSymbols[0].LastPrice,
                    result.Data.PerpSymbols[0].HighPrice,
                    result.Data.PerpSymbols[0].LowPrice,
                    new SharedOrderQuantity(result.Data.PerpSymbols[0].Volume, result.Data.PerpSymbols[0].QuoteVolume),
                    result.Data.PerpSymbols[0].PriceChangePercentage)
            {
                MarkPrice = result.Data.PerpSymbols[0].MarkPrice,
                IndexPrice = result.Data.PerpSymbols[0].IndexPrice,
            });

        }

        Task<HttpResult<SharedFuturesTicker[]>> IFuturesTickerRestClient.GetFuturesTickersAsync(GetTickersRequest request, CancellationToken ct)
            => GetAllFuturesTickersAsync(request, ct);
        GetAllFuturesTickersOptions IFuturesTickerRestClient.GetFuturesTickersOptions => GetAllFuturesTickersOptions;

        public GetAllFuturesTickersOptions GetAllFuturesTickersOptions { get; } = new GetAllFuturesTickersOptions(_exchangeName);
        public async Task<HttpResult<SharedFuturesTicker[]>> GetAllFuturesTickersAsync(GetTickersRequest request, CancellationToken ct)
        {
            var validationError = GetAllFuturesTickersOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFuturesTicker[]>(Exchange, validationError);

            var result = await _api.ExchangeData.GetSymbolDetailsAsync(symbolType: SymbolTypeFilter.Perp, ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedFuturesTicker[]>(result);

            return HttpResult.Ok(result, result.Data!.PerpSymbols.Select(x =>
                    new SharedFuturesTicker(
                        ExchangeSymbolCache.ParseSymbol(_topicFuturesId, _api.EnvironmentName, null, x.Symbol),
                        x.Symbol,
                        x.LastPrice,
                        x.HighPrice,
                        x.LowPrice,
                        new SharedOrderQuantity(x.Volume, x.QuoteVolume),
                        x.PriceChangePercentage)
                    {
                        MarkPrice = x.MarkPrice,
                        IndexPrice = x.IndexPrice,
                    }).ToArray());

        }

        #endregion

        #region Book Ticker client

        public GetBookTickerOptions GetBookTickerOptions { get; }
            = new GetBookTickerOptions(_exchangeName, false);
        public async Task<HttpResult<SharedBookTicker>> GetBookTickerAsync(GetBookTickerRequest request, CancellationToken ct)
        {
            var validationError = GetBookTickerOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedBookTicker>(Exchange, validationError);

            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var resultTicker = await _api.ExchangeData.GetOrderBookAsync(symbol, 1, ct: ct).ConfigureAwait(false);
            if (!resultTicker.Success)
                return HttpResult.Fail<SharedBookTicker>(resultTicker);

            return HttpResult.Ok(resultTicker, new SharedBookTicker(
                request.Symbol,
                symbol,
                resultTicker.Data.Asks[0].Price,
                new SharedOrderQuantity(resultTicker.Data.Asks[0].Quantity),
                resultTicker.Data.Bids[0].Price,
                new SharedOrderQuantity(resultTicker.Data.Bids[0].Quantity)));

        }

        #endregion

        #region Recent Trades client
        public GetRecentTradesOptions GetRecentTradesOptions { get; } = new GetRecentTradesOptions(_exchangeName, 100, false);

        public async Task<HttpResult<SharedTrade[]>> GetRecentTradesAsync(GetRecentTradesRequest request, CancellationToken ct)
        {
            var validationError = GetRecentTradesOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedTrade[]>(Exchange, validationError);

            // Get data
            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var result = await _api.ExchangeData.GetRecentTradesAsync(
                symbol,
                limit: request.Limit,
                ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedTrade[]>(result);

            // Return
            return HttpResult.Ok(result, result.Data!.Select(x =>
                new SharedTrade(request.Symbol, symbol, new SharedOrderQuantity(x.Quantity), x.Price, x.Timestamp)
                {
                    Side = x.IsMakerAsk ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                }).ToArray());

        }
        #endregion

        #region Order Book client
        public GetOrderBookOptions GetOrderBookOptions { get; } = new GetOrderBookOptions(_exchangeName, 1, 250, false)
        {
            RequestNotes = "When specifying the limit parameter less entries might be returned as individual orders are combined into aggregated levels client side"
        };
        public async Task<HttpResult<SharedOrderBook>> GetOrderBookAsync(GetOrderBookRequest request, CancellationToken ct)
        {
            var validationError = GetOrderBookOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedOrderBook>(Exchange, validationError);

            var result = await _api.ExchangeData.GetOrderBookAsync(
                request.Symbol!.GetSymbol(FormatSymbol),
                limit: request.Limit ?? 50,
                ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedOrderBook>(result);

            var asks = result.Data.Asks.GroupBy(x => x.Price);
            var bids = result.Data.Bids.GroupBy(x => x.Price);

            return HttpResult.Ok(result, 
                new SharedOrderBook(
                    SharedQuantityType.BaseAsset,
                    null,
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
        Task<HttpResult<SharedAsset[]>> IAssetsRestClient.GetAssetsAsync(GetAssetsRequest request, CancellationToken ct)
            => GetAllAssetsAsync(request, ct);
        GetAllAssetsOptions IAssetsRestClient.GetAssetsOptions => GetAllAssetsOptions;

        public GetAllAssetsOptions GetAllAssetsOptions { get; }
            = new GetAllAssetsOptions(_exchangeName, false);

        public async Task<HttpResult<SharedAsset[]>> GetAllAssetsAsync(GetAssetsRequest request, CancellationToken ct)
        {
            var validationError = GetAllAssetsOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedAsset[]>(Exchange, validationError);

            var assets = await _api.ExchangeData.GetAssetsAsync(ct: ct).ConfigureAwait(false);
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

        public GetAssetOptions GetAssetOptions { get; } = new GetAssetOptions(_exchangeName, false);
        public async Task<HttpResult<SharedAsset>> GetAssetAsync(GetAssetRequest request, CancellationToken ct)
        {
            var validationError = GetAssetOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedAsset>(Exchange, validationError);

            var assets = await _api.ExchangeData.GetAssetsAsync(ct: ct).ConfigureAwait(false);
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
            return Task.FromResult(HttpResult.Fail<SharedDepositAddress[]>(_exchangeName, new InvalidOperationError("GetDepositAddresses is not support on " + _exchangeName)));
        }


        Task<HttpResult<SharedDeposit[]>> IDepositRestClient.GetDepositsAsync(GetDepositsRequest request, PageRequest? pageRequest, CancellationToken ct)
            => GetDepositHistoryAsync(request, pageRequest, ct);
        GetDepositHistoryOptions IDepositRestClient.GetDepositsOptions => GetDepositHistoryOptions;

        public GetDepositHistoryOptions GetDepositHistoryOptions { get; } = new GetDepositHistoryOptions(_exchangeName, false, true, false, 100);
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
        public GetFeeOptions GetFeeOptions { get; } = new GetFeeOptions(_exchangeName, true);

        public async Task<HttpResult<SharedFee>> GetFeesAsync(GetFeeRequest request, CancellationToken ct)
        {
            var validationError = GetFeeOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFee>(Exchange, validationError);

            // Get data
            var result = await _api.Account.GetAccountLimitsAsync(ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedFee>(result);

            // Return
            return HttpResult.Ok(result, new SharedFee(result.Data.CurrentMakerFeeTick * 100, result.Data.CurrentTakerFeeTick * 100));

        }
        #endregion

        #region Balance Client
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

        #region Futures Order Client

        public SharedFeeDeductionType FuturesFeeDeductionType => SharedFeeDeductionType.AddToCost;
        public SharedFeeAssetType FuturesFeeAssetType => SharedFeeAssetType.QuoteAsset;

        public SharedOrderType[] FuturesSupportedOrderTypes { get; } = new[] { SharedOrderType.Limit, SharedOrderType.Market, SharedOrderType.LimitMaker };
        public SharedTimeInForce[] FuturesSupportedTimeInForce { get; } = new[] { SharedTimeInForce.GoodTillCanceled, SharedTimeInForce.ImmediateOrCancel };
        public SharedQuantitySupport FuturesSupportedOrderQuantity { get; } = new SharedQuantitySupport(
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset);

        public PlaceFuturesOrderOptions PlaceFuturesOrderOptions { get; } = new PlaceFuturesOrderOptions(_exchangeName, false);
        public async Task<HttpResult<SharedId>> PlaceFuturesOrderAsync(PlaceFuturesOrderRequest request, CancellationToken ct)
        {
            var validationError = PlaceFuturesOrderOptions.ValidateRequest(request, this);
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

        private decimal GetSlippagePrice(PlaceFuturesOrderRequest request)
        {
            // Calculate 5% max slippage
            if (request.Side == SharedOrderSide.Buy)
                return request.Price!.Value * 1.05m;

            return request.Price!.Value * 0.95m;
        }

        public GetFuturesOrderOptions GetFuturesOrderOptions { get; } = new GetFuturesOrderOptions(_exchangeName, true);
        public async Task<HttpResult<SharedFuturesOrder>> GetFuturesOrderAsync(GetOrderRequest request, CancellationToken ct)
        {
            var validationError = GetFuturesOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFuturesOrder>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedFuturesOrder>(Exchange, ArgumentError.Invalid(nameof(GetOrderRequest.OrderId), "Invalid order id"));

            LighterOrder? orderInfo = null;
            var openOrders = await _api.Trading.GetOpenOrdersAsync(symbol: request.Symbol!.GetSymbol(FormatSymbol), ct: ct).ConfigureAwait(false);
            if (!openOrders.Success)
                return HttpResult.Fail<SharedFuturesOrder>(openOrders);

            orderInfo = openOrders.Data.Orders.SingleOrDefault(x => x.OrderId == orderId || x.ClientOrderId == orderId);
            if (orderInfo == null)
            {
                var closedOrders = await _api.Trading.GetClosedOrdersAsync(symbol: request.Symbol!.GetSymbol(FormatSymbol), limit: 100, ct: ct).ConfigureAwait(false);
                if (!closedOrders.Success)
                    return HttpResult.Fail<SharedFuturesOrder>(closedOrders);
                orderInfo = closedOrders.Data.Orders.SingleOrDefault(x => x.OrderId == orderId || x.ClientOrderId == orderId);
            }

            if (orderInfo == null)
                return HttpResult.Fail<SharedFuturesOrder>(openOrders, new ServerError(ErrorType.UnknownOrder, "Order not found"));

            return HttpResult.Ok(openOrders, new SharedFuturesOrder(
                ExchangeSymbolCache.ParseSymbol(_topicFuturesId, _api.EnvironmentName, null, LighterUtils.GetSymbolName(_api.EnvironmentName, orderInfo.MarketIndex)),
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
                IsTriggerOrder = orderInfo.TriggerPrice > 0,
                ReduceOnly = orderInfo.ReduceOnly
            });

        }

        public GetOpenFuturesOrdersOptions GetOpenFuturesOrdersOptions { get; } = new GetOpenFuturesOrdersOptions(_exchangeName, true);
        public async Task<HttpResult<SharedFuturesOrder[]>> GetOpenFuturesOrdersAsync(GetOpenOrdersRequest request, CancellationToken ct)
        {
            var validationError = GetOpenFuturesOrdersOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFuturesOrder[]>(Exchange, validationError);

            var symbol = request.Symbol?.GetSymbol(FormatSymbol);
            var orders = await _api.Trading.GetOpenOrdersAsync(symbol: symbol, ct: ct).ConfigureAwait(false);
            if (!orders.Success)
                return HttpResult.Fail<SharedFuturesOrder[]>(orders);

            var futuresOrders = orders.Data.Orders.Where(x => x.MarketIndex < 2048);
            return HttpResult.Ok(orders, futuresOrders.Select(x => new SharedFuturesOrder(
                ExchangeSymbolCache.ParseSymbol(_topicFuturesId, _api.EnvironmentName, null, LighterUtils.GetSymbolName(_api.EnvironmentName, x.MarketIndex)),
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
                IsTriggerOrder = x.TriggerPrice > 0,
                ReduceOnly = x.ReduceOnly
            }).ToArray());

        }

        public GetFuturesClosedOrdersOptions GetClosedFuturesOrdersOptions { get; } = new GetFuturesClosedOrdersOptions(_exchangeName, true, true, true, 100);
        public async Task<HttpResult<SharedFuturesOrder[]>> GetClosedFuturesOrdersAsync(GetClosedOrdersRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = GetClosedFuturesOrdersOptions.ValidateRequest(request, this);
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
            var orders = await _api.Trading.GetClosedOrdersAsync(
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
                        ExchangeSymbolCache.ParseSymbol(_topicFuturesId, _api.EnvironmentName, null, LighterUtils.GetSymbolName(_api.EnvironmentName, x.MarketIndex)),
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
                        IsTriggerOrder = x.TriggerPrice > 0,
                        ReduceOnly = x.ReduceOnly
                    }).ToArray(), nextPageRequest);
        }

        public GetFuturesOrderTradesOptions GetFuturesOrderTradesOptions { get; } = new GetFuturesOrderTradesOptions(_exchangeName, true);
        public async Task<HttpResult<SharedUserTrade[]>> GetFuturesOrderTradesAsync(GetOrderTradesRequest request, CancellationToken ct)
        {
            var validationError = GetFuturesOrderTradesOptions.ValidateRequest(request, this);
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
                Fee = x.IsMakerAsk == (x.AskAccountId == _api.ApiCredentials.Credential.AccountIndex) ? x.Quantity * x.MakerFee : x.Quantity * x.TakerFee,
                Role = x.IsMakerAsk == (x.AskAccountId == _api.ApiCredentials.Credential.AccountIndex) ? SharedRole.Maker : SharedRole.Taker
            }).ToArray());

        }

        Task<HttpResult<SharedUserTrade[]>> IFuturesOrderRestClient.GetFuturesUserTradesAsync(GetUserTradesRequest request, PageRequest? pageRequest, CancellationToken ct)
            => GetFuturesUserTradeHistoryAsync(request, pageRequest, ct);
        GetFuturesUserTradeHistoryOptions IFuturesOrderRestClient.GetFuturesUserTradesOptions => GetFuturesUserTradeHistoryOptions;

        public GetFuturesUserTradeHistoryOptions GetFuturesUserTradeHistoryOptions { get; } = new GetFuturesUserTradeHistoryOptions(_exchangeName, true, true, true, 100);
        public async Task<HttpResult<SharedUserTrade[]>> GetFuturesUserTradeHistoryAsync(GetUserTradesRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = GetFuturesUserTradeHistoryOptions.ValidateRequest(request, this);
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

        public CancelFuturesOrderOptions CancelFuturesOrderOptions { get; } = new CancelFuturesOrderOptions(_exchangeName, true);
        public async Task<HttpResult<SharedId>> CancelFuturesOrderAsync(CancelOrderRequest request, CancellationToken ct)
        {
            var validationError = CancelFuturesOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedId>(Exchange, ArgumentError.Invalid(nameof(CancelOrderRequest.OrderId), "Invalid order id"));

            var order = await _api.Trading.CancelOrderAsync(request.Symbol!.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
            if (!order.Success)
                return HttpResult.Fail<SharedId>(order);

            return HttpResult.Ok(order, new SharedId(request.OrderId));

        }

        public GetPositionsOptions GetPositionsOptions { get; } = new GetPositionsOptions(_exchangeName, true);
        public async Task<HttpResult<SharedPosition[]>> GetPositionsAsync(GetPositionsRequest request, CancellationToken ct)
        {
            var validationError = GetPositionsOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedPosition[]>(Exchange, validationError);

            var result = await _api.Account.GetAccountsAsync(ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedPosition[]>(result);

            var account = result.Data.Accounts.Single(x => x.AccountIndex == _api.ApiCredentials!.Credential!.AccountIndex);

            return HttpResult.Ok(result, account.Positions.Select(x =>
                new SharedPosition(
                    ExchangeSymbolCache.ParseSymbol(_topicFuturesId, _api.EnvironmentName, null, x.Symbol),
                    x.Symbol,
                    new SharedOrderQuantity(Math.Abs(x.Position)),
                    null)
                {
                    AverageOpenPrice = x.AverageEntryPrice,
                    PositionMode = SharedPositionMode.OneWay,
                    PositionSide = x.PositionSide == Enums.PositionSide.Short ? SharedPositionSide.Short : SharedPositionSide.Long,
                    UnrealizedPnl = x.UnrealizedPnl,
                    LiquidationPrice = x.LiquidationPrice != 0 ? x.LiquidationPrice : null
                }).ToArray());

        }

        public ClosePositionOptions ClosePositionOptions { get; } = new ClosePositionOptions(_exchangeName, true)
        {
            RequiredRequestParameters = new List<ParameterDescription>
            {
                new ParameterDescription(nameof(ClosePositionRequest.PositionSide), typeof(SharedPositionSide), "The position side to close", SharedPositionSide.Long),
                new ParameterDescription(nameof(ClosePositionRequest.Quantity), typeof(decimal), "Quantity of the position is required", 0.1m)
            },
            RequiredExchangeParameters = new List<ParameterDescription>
            {
                new ParameterDescription("Price", typeof(decimal), "The current price of the symbol. Required to calculate max slippage.", 21.5m)
            },
        };
        public async Task<HttpResult<SharedId>> ClosePositionAsync(ClosePositionRequest request, CancellationToken ct)
        {
            var validationError = ClosePositionOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            long cid = long.Parse(GenerateClientOrderId());

            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var result = await _api.Trading.PlaceOrderAsync(
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

            return HttpResult.Ok(result, new SharedId(null));

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

        public GetFuturesOrderByClientOrderIdOptions GetFuturesOrderByClientOrderIdOptions { get; } = new GetFuturesOrderByClientOrderIdOptions(_exchangeName, true);
        public async Task<HttpResult<SharedFuturesOrder>> GetFuturesOrderByClientOrderIdAsync(GetOrderRequest request, CancellationToken ct)
        {
            var validationError = GetFuturesOrderByClientOrderIdOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFuturesOrder>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedFuturesOrder>(Exchange, ArgumentError.Invalid(nameof(GetOrderRequest.OrderId), "Invalid order id"));

            LighterOrder? orderInfo = null;
            var openOrders = await _api.Trading.GetOpenOrdersAsync(symbol: request.Symbol!.GetSymbol(FormatSymbol), ct: ct).ConfigureAwait(false);
            if (!openOrders.Success)
                return HttpResult.Fail<SharedFuturesOrder>(openOrders);

            orderInfo = openOrders.Data.Orders.SingleOrDefault(x => x.ClientOrderId == orderId);
            if (orderInfo == null)
            {
                var closedOrders = await _api.Trading.GetClosedOrdersAsync(symbol: request.Symbol!.GetSymbol(FormatSymbol), limit: 100, ct: ct).ConfigureAwait(false);
                if (!closedOrders.Success)
                    return HttpResult.Fail<SharedFuturesOrder>(closedOrders);
                orderInfo = closedOrders.Data.Orders.SingleOrDefault(x => x.ClientOrderId == orderId);
            }

            if (orderInfo == null)
                return HttpResult.Fail<SharedFuturesOrder>(openOrders, new ServerError(ErrorType.UnknownOrder, "Order not found"));

            return HttpResult.Ok(openOrders, new SharedFuturesOrder(
                ExchangeSymbolCache.ParseSymbol(_topicFuturesId, _api.EnvironmentName, null, LighterUtils.GetSymbolName(_api.EnvironmentName, orderInfo.MarketIndex)),
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
                IsTriggerOrder = orderInfo.TriggerPrice > 0,
                ReduceOnly = orderInfo.ReduceOnly
            });

        }

        public CancelFuturesOrderByClientOrderIdOptions CancelFuturesOrderByClientOrderIdOptions { get; } = new CancelFuturesOrderByClientOrderIdOptions(_exchangeName, true);
        public async Task<HttpResult<SharedId>> CancelFuturesOrderByClientOrderIdAsync(CancelOrderRequest request, CancellationToken ct)
        {
            var validationError = CancelFuturesOrderByClientOrderIdOptions.ValidateRequest(request, this);
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

        #region Leverage client
        public SharedLeverageSettingMode LeverageSettingType => SharedLeverageSettingMode.PerSymbol;

        public GetLeverageOptions GetLeverageOptions { get; } = new GetLeverageOptions(_exchangeName, true);
        public async Task<HttpResult<SharedLeverage>> GetLeverageAsync(GetLeverageRequest request, CancellationToken ct)
        {
            var validationError = GetLeverageOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedLeverage>(Exchange, validationError);

            var result = await _api.Account.GetAccountsAsync().ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedLeverage>(result);

            var account = result.Data.Accounts.Single(x => x.AccountIndex == _api.ApiCredentials!.Credential!.AccountIndex);
            var position = account.Positions.SingleOrDefault(x => x.Symbol.Equals(request.Symbol!.GetSymbol(FormatSymbol), StringComparison.InvariantCultureIgnoreCase));
            if (position == null)
                return HttpResult.Fail<SharedLeverage>(Exchange, new ServerError(new ErrorInfo(ErrorType.Unknown, false, "Position not found")));

            return HttpResult.Ok(result, new SharedLeverage(100 / position.InitialMarginFraction));
        }

        public SetLeverageOptions SetLeverageOptions { get; } = new SetLeverageOptions(_exchangeName)
        {
            RequiredRequestParameters = new List<ParameterDescription>
            {
                new ParameterDescription(nameof(SetLeverageRequest.MarginMode), typeof(SharedMarginMode), "The margin mode to change leverage for", SharedMarginMode.Cross)
            }
        };
        public async Task<HttpResult<SharedLeverage>> SetLeverageAsync(SetLeverageRequest request, CancellationToken ct)
        {
            var validationError = SetLeverageOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedLeverage>(Exchange, validationError);

            var result = await _api.Account.SetLeverageAsync(
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

        public GetOpenInterestOptions GetOpenInterestOptions { get; } = new GetOpenInterestOptions(_exchangeName, false);
        public async Task<HttpResult<SharedOpenInterest>> GetOpenInterestAsync(GetOpenInterestRequest request, CancellationToken ct)
        {
            var validationError = GetOpenInterestOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedOpenInterest>(Exchange, validationError);

            var result = await _api.ExchangeData.GetSymbolDetailsAsync(request.Symbol!.GetSymbol(FormatSymbol), ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedOpenInterest>(result);

            if (result.Data.PerpSymbols.Length == 0)
                return HttpResult.Fail<SharedOpenInterest>(result, new ServerError(ErrorType.UnknownSymbol, "Symbol not found"));

            return HttpResult.Ok(result, new SharedOpenInterest(new SharedOrderQuantity(result.Data.PerpSymbols[0].OpenInterest)));

        }

        #endregion

        #region Funding Rate client
        public GetFundingRateHistoryOptions GetFundingRateHistoryOptions { get; } = 
            new GetFundingRateHistoryOptions(_exchangeName, false, true, true, 100, false)
            {
                OptionalExchangeParameters = new List<ParameterDescription>
                {
                    new ParameterDescription("Resolution", typeof(FundingResolution), "The resolution of the data, by default 1H", FundingResolution.OneDay)
                }
            };

        public async Task<HttpResult<SharedFundingRate[]>> GetFundingRateHistoryAsync(GetFundingRateHistoryRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = GetFundingRateHistoryOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFundingRate[]>(Exchange, validationError);

            int limit = request.Limit ?? 100;
            var direction = DataDirection.Descending;
            var pageParams = Pagination.GetPaginationParameters(direction, limit, request.StartTime, request.EndTime ?? DateTime.UtcNow, pageRequest, false);

            // Get data
            var result = await _api.ExchangeData.GetFundingRateHistoryAsync(
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
