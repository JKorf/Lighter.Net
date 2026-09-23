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
        IGetKlinesRest,
        IGetSpotSymbolsRest,
        IGetFuturesSymbolsRest,
        IGetTickerRest,
        IGetAllTickersRest,
        IGetBookTickerRest,
        IGetRecentTradesRest,
        IGetOrderBookRest,
        IGetAssetRest,
        IGetAllAssetsRest,
        IGetDepositHistoryRest,
        IGetWithdrawalHistoryRest,
        IGetFeesRest,
        IGetBalancesRest,
        IPlaceSpotOrderRest,
        IGetSpotOrderRest,
        IGetOpenSpotOrdersRest,
        IGetClosedSpotOrdersRest,
        IGetSpotUserTradeHistoryRest,
        IGetSpotOrderTradesRest,
        ICancelSpotOrderRest,
        IGetSpotOrderByClientOrderIdRest,
        ICancelSpotOrderByClientOrderIdRest,
        IGetLeverageRest,
        ISetLeverageRest,
        IGetOpenInterestRest,
        IPlaceFuturesOrderRest,
        IGetFuturesOrderRest,
        IGetOpenFuturesOrdersRest,
        IGetClosedFuturesOrdersRest,
        IGetFuturesOrderTradesRest,
        IGetFuturesUserTradeHistoryRest,
        ICancelFuturesOrderRest,
        IGetPositionsRest,
        IGetFuturesOrderByClientOrderIdRest,
        ICancelFuturesOrderByClientOrderIdRest,
        IGetFundingRateHistoryRest
    {
    }
}
