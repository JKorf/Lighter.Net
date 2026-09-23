using System;
using CryptoExchange.Net.Interfaces.Clients;

namespace Lighter.Net.Interfaces.Clients.ExchangeApi
{
    /// <summary>
    /// Lighter Exchange streams
    /// </summary>
    public interface ILighterSocketClientExchangeApi : ISocketApiClient<LighterCredentials>, IDisposable
    {
        /// <summary>
        /// Account streams and requests. Account endpoints include balance info, withdraw/deposit info and requesting and account settings
        /// </summary>
        ILighterSocketClientExchangeApiAccount Account { get; }
        /// <summary>
        /// Exchange data streams and requests. Exchange data endpoints include market info, order book info, and trade info
        /// </summary>
        ILighterSocketClientExchangeApiExchangeData ExchangeData { get; }
        /// <summary>
        /// Trading streams and requests. Trading endpoints include order placement, order cancellation, and trade execution
        /// </summary>
        ILighterSocketClientExchangeApiTrading Trading { get; }

        /// <summary>
        /// [V1] Get the shared socket requests client. For new implementations prefer <see cref="SharedApi"/>
        /// </summary>
        public ILighterSocketClientExchangeApiShared SharedClient { get; }
        /// <summary>
        /// [V2] Gets the aggregate Shared API interface. Shared APIs provide a common,
        /// exchange-independent contract for accessing functionality across different
        /// exchange client libraries.
        /// </summary>
        public ILighterSocketClientExchangeSharedApi SharedApi { get; }
    }
}
