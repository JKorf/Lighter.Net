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
    }
}
