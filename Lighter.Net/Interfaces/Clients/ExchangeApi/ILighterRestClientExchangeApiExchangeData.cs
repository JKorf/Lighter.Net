using System;
using System.Threading;
using System.Threading.Tasks;
using CryptoExchange.Net.Objects;
using Lighter.Net.Enums;
using Lighter.Net.Objects.Models;

namespace Lighter.Net.Interfaces.Clients.ExchangeApi
{
    /// <summary>
    /// Lighter Exchange exchange data endpoints. Exchange data includes market data (tickers, order books, etc) and system status.
    /// </summary>
    public interface ILighterRestClientExchangeApiExchangeData
    {
        /// <summary>
        /// Get platform status
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/reference/status" /><br />
        /// Endpoint:<br />
        /// GET api/v1/status<br />
        /// </para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<HttpResult<LighterStatus>> GetStatusAsync(CancellationToken ct = default);

        /// <summary>
        /// Get system configuration
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/reference/systemconfig" /><br />
        /// Endpoint:<br />
        /// GET api/v1/systemConfig<br />
        /// </para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        Task<HttpResult<LighterSystemConfig>> GetSystemConfigAsync(CancellationToken ct = default);

        /// <summary>
        /// Get supported symbols
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/reference/orderbooks" /><br />
        /// Endpoint:<br />
        /// GET /api/v1/orderBooks<br />
        /// </para>
        /// </summary>
        /// <param name="symbol">Symbol name, for example `ETH/USDC` for spot or `ETH` for perps</param>
        /// <param name="symbolType">["<c>filter</c>"] Filter by symbol type</param>
        /// <param name="ct">Cancellation token</param>
        Task<HttpResult<LighterSymbol[]>> GetSymbolsAsync(
            string? symbol = null,
            SymbolTypeFilter? symbolType = null,
            CancellationToken ct = default);

        /// <summary>
        /// Get layer 1 basic info
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/reference/layer1basicinfo" /><br />
        /// Endpoint:<br />
        /// GET api/v1/layer1BasicInfo<br />
        /// </para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        Task<HttpResult<LighterL1Info>> GetLayer1BasicInfoAsync(CancellationToken ct = default);

        /// <summary>
        /// Get asset details
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/reference/assetdetails" /><br />
        /// Endpoint:<br />
        /// GET /api/v1/assetDetails<br />
        /// </para>
        /// </summary>
        /// <param name="assetId">["<c>asset_id</c>"] Filter by asset id</param>
        /// <param name="ct">Cancellation token</param>
        Task<HttpResult<LighterAsset[]>> GetAssetsAsync(long? assetId = null, CancellationToken ct = default);

        /// <summary>
        /// Get order book snapshot
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/reference/orderbookorders" /><br />
        /// Endpoint:<br />
        /// GET /api/v1/orderBookOrders<br />
        /// </para>
        /// </summary>
        /// <param name="symbol">Symbol name, for example `ETH/USDC` for spot or `ETH` for perps</param>
        /// <param name="limit">["<c>limit</c>"] Max depth</param>
        /// <param name="ct">Cancellation token</param>
        Task<HttpResult<LighterOrderBook>> GetOrderBookAsync(
            string symbol,
            int? limit = null,
            CancellationToken ct = default);

        /// <summary>
        /// Get recent trades
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/reference/recenttrades" /><br />
        /// Endpoint:<br />
        /// GET /api/v1/recentTrades<br />
        /// </para>
        /// </summary>
        /// <param name="symbol">Symbol name, for example `ETH/USDC` for spot or `ETH` for perps</param>
        /// <param name="limit">["<c>limit</c>"] Max number of trades</param>
        /// <param name="ct">Cancellation token</param>
        Task<HttpResult<LighterTrade[]>> GetRecentTradesAsync(
            string symbol,
            int? limit = null,
            CancellationToken ct = default);

        /// <summary>
        /// Get symbols details and ticker data
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/reference/orderbookdetails" /><br />
        /// Endpoint:<br />
        /// GET /api/v1/orderBookDetails<br />
        /// </para>
        /// </summary>
        /// <param name="symbol">Symbol name, for example `ETH/USDC` for spot or `ETH` for perps</param>
        /// <param name="symbolType">["<c>filter</c>"] Filter by symbol type</param>
        /// <param name="ct">Cancellation token</param>
        Task<HttpResult<LighterSymbolDetails>> GetSymbolDetailsAsync(
            string? symbol = null,
            SymbolTypeFilter? symbolType = null,
            CancellationToken ct = default);

        /// <summary>
        /// Get exchange statistics
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/reference/exchangestats" /><br />
        /// Endpoint:<br />
        /// GET /api/v1/exchangeStats<br />
        /// </para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        Task<HttpResult<LighterExchangeStats>> GetExchangeStatsAsync(CancellationToken ct = default);

        /// <summary>
        /// Get announcements
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/reference/announcement-1" /><br />
        /// Endpoint:<br />
        /// GET api/v1/announcement<br />
        /// </para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        Task<HttpResult<LighterAnnouncements>> GetAnnouncementsAsync(CancellationToken ct = default);

        /// <summary>
        /// Get kline/candlestick data
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/reference/candles" /><br />
        /// Endpoint:<br />
        /// GET /api/v1/candles<br />
        /// </para>
        /// </summary>
        /// <param name="symbol">Symbol name, for example `ETH/USDC` for spot or `ETH` for perps</param>
        /// <param name="interval">["<c>resolution</c>"] Kline interval</param>
        /// <param name="startTime">["<c>start_timestamp</c>"] Filter by start time</param>
        /// <param name="endTime">["<c>end_timestamp</c>"] Filter by end time</param>
        /// <param name="setTimestampToEnd">["<c>set_timestamp_to_end</c>"] </param>
        /// <param name="ct">Cancellation token</param>
        Task<HttpResult<LighterKlines>> GetKlinesAsync(
            string symbol,
            KlineInterval interval,
            DateTime? startTime = null,
            DateTime? endTime = null,
            bool? setTimestampToEnd = null,
            CancellationToken ct = default);

        /// <summary>
        /// Get mark price kline/candlestick data
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/reference/markpricecandles" /><br />
        /// Endpoint:<br />
        /// GET /api/v1/markPriceCandles<br />
        /// </para>
        /// </summary>
        /// <param name="symbol">Symbol name, for example `ETH/USDC` for spot or `ETH` for perps</param>
        /// <param name="interval">["<c>resolution</c>"] Kline interval</param>
        /// <param name="startTime">["<c>start_timestamp</c>"] Filter by start time</param>
        /// <param name="endTime">["<c>end_timestamp</c>"] Filter by end time</param>
        /// <param name="limit">["<c>count_back</c>"] Max number of results</param>
        /// <param name="ct">Cancellation token</param>
        Task<HttpResult<LighterMarkKlines>> GetMarkPriceKlinesAsync(
            string symbol,
            KlineInterval interval,
            DateTime? startTime = null,
            DateTime? endTime = null,
            int? limit = null,
            CancellationToken ct = default);

        /// <summary>
        /// Get funding rate history
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/reference/fundings" /><br />
        /// Endpoint:<br />
        /// GET api/v1/fundings<br />
        /// </para>
        /// </summary>
        /// <param name="symbol">Symbol name, for example `ETH/USDC` for spot or `ETH` for perps</param>
        /// <param name="resolution">["<c>resolution</c>"] Resolution</param>
        /// <param name="startTime">["<c>start_timestamp</c>"] Filter by start time</param>
        /// <param name="endTime">["<c>end_timestamp</c>"] Filter by end time</param>
        /// <param name="limit">["<c>count_back</c>"] Max number of results</param>
        /// <param name="ct">Cancellation token</param>
        Task<HttpResult<LighterFundingRateHistory>> GetFundingRateHistoryAsync(
            string symbol,
            FundingResolution resolution,
            DateTime? startTime = null,
            DateTime? endTime = null,
            int? limit = null,
            CancellationToken ct = default);

        /// <summary>
        /// Get current funding rates
        /// <para>
        /// Docs:<br />
        /// <a href="https://apidocs.lighter.xyz/reference/funding-rates" /><br />
        /// Endpoint:<br />
        /// GET api/v1/funding-rates<br />
        /// </para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        Task<HttpResult<LighterCurrentFundingRates>> GetFundingRatesAsync(CancellationToken ct = default);

    }
}
