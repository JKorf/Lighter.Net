using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CryptoExchange.Net.Objects;
using Microsoft.Extensions.Logging;
using Lighter.Net.Interfaces.Clients.ExchangeApi;
using Lighter.Net.Objects.Models;
using Lighter.Net.Enums;

namespace Lighter.Net.Clients.ExchangeApi
{
    /// <inheritdoc />
    internal class LighterRestClientExchangeApiExchangeData : ILighterRestClientExchangeApiExchangeData
    {
        private readonly LighterRestClientExchangeApi _baseClient;
        private static readonly RequestDefinitionCache _definitions = new RequestDefinitionCache();

        internal LighterRestClientExchangeApiExchangeData(ILogger logger, LighterRestClientExchangeApi baseClient)
        {
            _baseClient = baseClient;
        }

        #region Get Status

        /// <inheritdoc />
        public async Task<HttpResult<LighterStatus>> GetStatusAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "/", LighterExchange.RateLimiter.LighterRest, 300, false);
            return await _baseClient.SendAsync<LighterStatus>(request, null, ct).ConfigureAwait(false);
        }

        #endregion

        #region Get System Config

        /// <inheritdoc />
        public async Task<HttpResult<LighterSystemConfig>> GetSystemConfigAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "api/v1/systemConfig", LighterExchange.RateLimiter.LighterRest, 300, false);
            var result = await _baseClient.SendAsync<LighterSystemConfig>(request, null, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Symbols

        /// <inheritdoc />
        public async Task<HttpResult<LighterSymbol[]>> GetSymbolsAsync(
            string? symbol = null,
            SymbolTypeFilter? symbolType = null,
            CancellationToken ct = default)
        {
            LighterSymbol? symbolInfo = null;
            if (symbol != null)
            {
                var symbolInfoResult = await _baseClient.GetSymbolInfoAsync(symbol).ConfigureAwait(false);
                if (!symbolInfoResult.Success)
                    return HttpResult.Fail<LighterSymbol[]>(_baseClient.Exchange, symbolInfoResult.Error!);

                symbolInfo = symbolInfoResult.Data;
            }

            var parameters = new Parameters(LighterExchange._parameterSerializationSettings);
            parameters.Add("market_id", symbolInfo?.MarketId);
            parameters.Add("filter", symbolType);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "/api/v1/orderBooks", LighterExchange.RateLimiter.LighterRest, 300, false);
            var result = await _baseClient.SendAsync<LighterSymbols>(request, parameters, ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<LighterSymbol[]>(result);

            LighterUtils.SetSymbolsCache(_baseClient.EnvironmentName, result.Data);
            return HttpResult.Ok(result, result.Data.OrderBooks);
        }

        #endregion

        #region Get Tokens

        /// <inheritdoc />
        public async Task<HttpResult<LighterToken[]>> GetTokensAsync(
            CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "/api/v1/tokenlist", LighterExchange.RateLimiter.LighterRest, 300, false);
            var result = await _baseClient.SendAsync<LighterTokens>(request, null, ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<LighterToken[]>(result);

            return HttpResult.Ok(result, result.Data.Tokens);
        }

        #endregion

        #region Get Layer1 Basic Info

        /// <inheritdoc />
        public async Task<HttpResult<LighterL1Info>> GetLayer1BasicInfoAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "api/v1/layer1BasicInfo", LighterExchange.RateLimiter.LighterRest, 300, false);
            var result = await _baseClient.SendAsync<LighterL1Info>(request, null, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Assets

        /// <inheritdoc />
        public async Task<HttpResult<LighterAsset[]>> GetAssetsAsync(long? assetId = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(LighterExchange._parameterSerializationSettings);
            parameters.Add("asset_id", assetId);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "/api/v1/assetDetails", LighterExchange.RateLimiter.LighterRest, 300, false);
            var result = await _baseClient.SendAsync<LighterAssets>(request, parameters, ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<LighterAsset[]>(result);

            return HttpResult.Ok(result, result.Data.AssetDetails);
        }

        #endregion

        #region Get Order Book

        /// <inheritdoc />
        public async Task<HttpResult<LighterOrderBook>> GetOrderBookAsync(
            string symbol,
            int? limit = null,
            CancellationToken ct = default)
        {
            var symbolInfo = await _baseClient.GetSymbolInfoAsync(symbol).ConfigureAwait(false);
            if (!symbolInfo.Success)
                return HttpResult.Fail<LighterOrderBook>(_baseClient.Exchange, symbolInfo.Error!);

            var parameters = new Parameters(LighterExchange._parameterSerializationSettings);
            parameters.Add("market_id", symbolInfo.Data.MarketId);
            parameters.Add("limit", limit ?? 20);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "/api/v1/orderBookOrders", LighterExchange.RateLimiter.LighterRest, 300, false);
            var result = await _baseClient.SendAsync<LighterOrderBook>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Recent Trades

        /// <inheritdoc />
        public async Task<HttpResult<LighterTrade[]>> GetRecentTradesAsync(
            string symbol,
            int? limit = null,
            CancellationToken ct = default)
        {
            var symbolInfo = await _baseClient.GetSymbolInfoAsync(symbol).ConfigureAwait(false);
            if (!symbolInfo.Success)
                return HttpResult.Fail<LighterTrade[]>(_baseClient.Exchange, symbolInfo.Error!);

            var parameters = new Parameters(LighterExchange._parameterSerializationSettings);
            parameters.Add("market_id", symbolInfo.Data.MarketId);
            parameters.Add("limit", limit ?? 100);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "/api/v1/recentTrades", LighterExchange.RateLimiter.LighterRest, 600, false);
            var result = await _baseClient.SendAsync<LighterTrades>(request, parameters, ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<LighterTrade[]>(result);

            return HttpResult.Ok(result, result.Data.Trades);
        }

        #endregion

        #region Get Symbol Details

        /// <inheritdoc />
        public async Task<HttpResult<LighterSymbolDetails>> GetSymbolDetailsAsync(
            string? symbol = null,
            SymbolTypeFilter? symbolType = null,
            CancellationToken ct = default)
        {
            LighterSymbol? symbolInfo = null;
            if (symbol != null)
            {
                var symbolInfoResult = await _baseClient.GetSymbolInfoAsync(symbol).ConfigureAwait(false);
                if (!symbolInfoResult.Success)
                    return HttpResult.Fail<LighterSymbolDetails>(_baseClient.Exchange, symbolInfoResult.Error!);

                symbolInfo = symbolInfoResult.Data;
            }

            var parameters = new Parameters(LighterExchange._parameterSerializationSettings);
            parameters.Add("market_id", symbolInfo?.MarketId);
            parameters.Add("filter", symbolType);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "/api/v1/orderBookDetails", LighterExchange.RateLimiter.LighterRest, 300, false);
            var result = await _baseClient.SendAsync<LighterSymbolDetails>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Exchange Stats

        /// <inheritdoc />
        public async Task<HttpResult<LighterExchangeStats>> GetExchangeStatsAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "/api/v1/exchangeStats", LighterExchange.RateLimiter.LighterRest, 300, false);
            var result = await _baseClient.SendAsync<LighterExchangeStats>(request, null, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Announcements

        /// <inheritdoc />
        public async Task<HttpResult<LighterAnnouncements>> GetAnnouncementsAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "api/v1/announcement", LighterExchange.RateLimiter.LighterRest, 300, false);
            var result = await _baseClient.SendAsync<LighterAnnouncements>(request, null, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Klines

        /// <inheritdoc />
        public async Task<HttpResult<LighterKlines>> GetKlinesAsync(
            string symbol,
            KlineInterval interval,
            DateTime? startTime = null,
            DateTime? endTime = null,
            //int? limit = null,
            bool? setTimestampToEnd = null,
            CancellationToken ct = default)
        {
            var symbolInfo = await _baseClient.GetSymbolInfoAsync(symbol).ConfigureAwait(false);
            if (!symbolInfo.Success)
                return HttpResult.Fail<LighterKlines>(_baseClient.Exchange, symbolInfo.Error!);

            var parameters = new Parameters(LighterExchange._parameterSerializationSettings);
            parameters.Add("market_id", symbolInfo.Data.MarketId);
            parameters.Add("resolution", interval);
            parameters.Add("start_timestamp", startTime ?? DateTime.UtcNow.AddDays(-7), DateTimeSerialization.SecondsNumber);
            parameters.Add("end_timestamp", endTime ?? DateTime.UtcNow, DateTimeSerialization.SecondsNumber);
            // unclear what count_back parameter does; doesn't seem to limit number of results
            parameters.Add("count_back", 100);
            parameters.Add("set_timestamp_to_end", setTimestampToEnd);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "/api/v1/candles", LighterExchange.RateLimiter.LighterRest, 300, false);
            var result = await _baseClient.SendAsync<LighterKlines>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Mark Price Klines

        /// <inheritdoc />
        public async Task<HttpResult<LighterMarkKlines>> GetMarkPriceKlinesAsync(
            string symbol,
            KlineInterval interval,
            DateTime? startTime = null,
            DateTime? endTime = null,
            int? limit = null,
            CancellationToken ct = default)
        {
            var symbolInfo = await _baseClient.GetSymbolInfoAsync(symbol).ConfigureAwait(false);
            if (!symbolInfo.Success)
                return HttpResult.Fail<LighterMarkKlines>(_baseClient.Exchange, symbolInfo.Error!);

            var parameters = new Parameters(LighterExchange._parameterSerializationSettings);
            parameters.Add("market_id", symbolInfo.Data.MarketId);
            parameters.Add("resolution", interval);
            parameters.Add("start_timestamp", startTime ?? DateTime.UtcNow.AddDays(-7), DateTimeSerialization.SecondsNumber);
            parameters.Add("end_timestamp", endTime ?? DateTime.UtcNow, DateTimeSerialization.SecondsNumber);
            parameters.Add("count_back", limit ?? 100);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "/api/v1/markPriceCandles", LighterExchange.RateLimiter.LighterRest, 300, false);
            var result = await _baseClient.SendAsync<LighterMarkKlines>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Funding Rate History

        /// <inheritdoc />
        public async Task<HttpResult<LighterFundingRateHistory>> GetFundingRateHistoryAsync(
            string symbol,
            FundingResolution resolution,
            DateTime? startTime = null,
            DateTime? endTime = null,
            int? limit = null,
            CancellationToken ct = default)
        {
            var symbolInfo = await _baseClient.GetSymbolInfoAsync(symbol).ConfigureAwait(false);
            if (!symbolInfo.Success)
                return HttpResult.Fail<LighterFundingRateHistory>(_baseClient.Exchange, symbolInfo.Error!);

            var parameters = new Parameters(LighterExchange._parameterSerializationSettings);
            parameters.Add("market_id", symbolInfo.Data.MarketId);
            parameters.Add("resolution", resolution);
            parameters.Add("start_timestamp", startTime ?? DateTime.UtcNow.AddDays(-7), DateTimeSerialization.SecondsNumber);
            parameters.Add("end_timestamp", endTime ?? DateTime.UtcNow, DateTimeSerialization.SecondsNumber);
            parameters.Add("count_back", limit ?? 100);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "api/v1/fundings", LighterExchange.RateLimiter.LighterRest, 300, false);
            var result = await _baseClient.SendAsync<LighterFundingRateHistory>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

        #region Get Funding Rates

        /// <inheritdoc />
        public async Task<HttpResult<LighterCurrentFundingRates>> GetFundingRatesAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "api/v1/funding-rates", LighterExchange.RateLimiter.LighterRest, 300, false);
            var result = await _baseClient.SendAsync<LighterCurrentFundingRates>(request, null, ct).ConfigureAwait(false);
            return result;
        }

        #endregion

    }
}
