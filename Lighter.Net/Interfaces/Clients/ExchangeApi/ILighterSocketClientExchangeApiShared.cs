using CryptoExchange.Net.SharedApis;

namespace Lighter.Net.Interfaces.Clients.ExchangeApi
{
    /// <summary>
    /// Shared interface for Exchange socket API usage
    /// </summary>
    public interface ILighterSocketClientExchangeApiShared :
        ITickerSocketClient,
        ITickersSocketClient,
        ITradeSocketClient,
        IBookTickerSocketClient,
        IKlineSocketClient,
        IBalanceSocketClient,
        ISpotOrderSocketClient,
        IFuturesOrderSocketClient,
        IUserTradeSocketClient,
        IPositionSocketClient,
        ISpotOrderManagementSocketClient,
        IFuturesOrderManagementSocketClient
    {
    }

    /// <summary>
    /// Shared API interface. Shared APIs provide a common,
    /// exchange-independent contract for accessing functionality across different
    /// exchange client libraries.
    /// </summary>
    public interface ILighterSocketClientExchangeSharedApi :
        ISubscribeAllTickersOperation,
        ISubscribeTickerOperation,
        ISubscribeTradesOperation,
        ISubscribeBookTickerOperation,
        ISubscribeKlinesOperation,
        ISubscribeBalancesOperation,
        ISubscribeSpotOrdersOperation,
        ISubscribeFuturesOrdersOperation,
        ISubscribeUserTradesOperation,
        ISubscribePositionsOperation,
        IPlaceSpotOrderOperation,
        ICancelSpotOrderOperation,
        IPlaceFuturesOrderOperation,
        ICancelFuturesOrderOperation
    {
    }
}
