using CryptoExchange.Net.SharedApis;

namespace Lighter.Net.Interfaces.Clients.ExchangeApi
{
    /// <summary>
    /// Shared interface for Exchange rest API usage
    /// </summary>
    public interface ILighterRestClientExchangeApiShared :
        IKlineRestClient,
        ISpotSymbolRestClient,
        IFuturesSymbolRestClient,
        ISpotTickerRestClient,
        IFuturesTickerRestClient,
        IBookTickerRestClient,
        IRecentTradeRestClient,
        IOrderBookRestClient,
        IAssetsRestClient,
        IDepositRestClient,
        IWithdrawalRestClient,
        IFeeRestClient,
        IBalanceRestClient,
        ISpotOrderRestClient,
        ISpotOrderClientIdRestClient,
        ILeverageRestClient,
        IOpenInterestRestClient,
        IFuturesOrderRestClient,
        IFuturesOrderClientIdRestClient,
        IFundingRateRestClient
    {
    }

    /// <summary>
    /// Shared API interface. Shared APIs provide a common,
    /// exchange-independent contract for accessing functionality across different
    /// exchange client libraries.
    /// </summary>
    public interface ILighterRestClientExchangeSharedApi :
        IGetKlinesEndpoint,
        IGetSpotSymbolsEndpoint,
        IGetFuturesSymbolsEndpoint,
        IGetSpotTickerEndpoint,
        IGetAllSpotTickersEndpoint,
        IGetFuturesTickerEndpoint,
        IGetAllFuturesTickersEndpoint,
        IGetBookTickerEndpoint,
        IGetRecentTradesEndpoint,
        IGetOrderBookEndpoint,
        IGetAssetEndpoint,
        IGetAllAssetsEndpoint,
        IGetDepositHistoryEndpoint,
        IGetWithdrawalHistoryEndpoint,
        IGetFeesEndpoint,
        IGetBalancesEndpoint,
        IPlaceSpotOrderEndpoint,
        IGetSpotOrderEndpoint,
        IGetOpenSpotOrdersEndpoint,
        IGetClosedSpotOrdersEndpoint,
        IGetSpotUserTradeHistoryEndpoint,
        IGetSpotOrderTradesEndpoint,
        ICancelSpotOrderEndpoint,
        IGetSpotOrderByClientOrderIdEndpoint,
        ICancelSpotOrderByClientOrderIdEndpoint,
        IGetLeverageEndpoint,
        ISetLeverageEndpoint,
        IGetOpenInterestEndpoint,
        IPlaceFuturesOrderEndpoint,
        IGetFuturesOrderEndpoint,
        IGetOpenFuturesOrdersEndpoint,
        IGetClosedFuturesOrdersEndpoint,
        IGetFuturesOrderTradesEndpoint,
        IGetFuturesUserTradeHistoryEndpoint,
        ICancelFuturesOrderEndpoint,
        IGetPositionsEndpoint,
        IClosePositionEndpoint,
        IGetFuturesOrderByClientOrderIdEndpoint,
        ICancelFuturesOrderByClientOrderIdEndpoint,
        IGetFundingRateHistoryEndpoint
    {
    }
}
