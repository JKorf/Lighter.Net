using CryptoExchange.Net.SharedApis;
using Lighter.Net.Interfaces.Clients.ExchangeApi;

namespace Lighter.Net.Interfaces.Clients
{
    /// <summary>
    /// Client for the shared REST and WebSocket API implementations of Lighter
    /// </summary>
    public interface ILighterSharedApiClient : ISharedApiClientBase
    {
        /// <summary>
        /// REST shared API implementations
        /// </summary>
        ILighterRestClientExchangeSharedApi Rest { get; }

        /// <summary>
        /// WebSocket shared API implementations
        /// </summary>
        ILighterSocketClientExchangeSharedApi Socket { get; }
    }
}
