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
}
