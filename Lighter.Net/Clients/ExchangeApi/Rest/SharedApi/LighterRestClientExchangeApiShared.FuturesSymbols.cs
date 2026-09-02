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
    }
}
