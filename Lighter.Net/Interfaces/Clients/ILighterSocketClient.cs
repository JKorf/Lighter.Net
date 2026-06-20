using CryptoExchange.Net.Interfaces.Clients;
using Lighter.Net.Interfaces.Clients.ExchangeApi;

namespace Lighter.Net.Interfaces.Clients
{
    /// <summary>
    /// Client for accessing the Lighter websocket API
    /// </summary>
    public interface ILighterSocketClient : ISocketClient<LighterCredentials>
    {
        /// <summary>
        /// Exchange API endpoints
        /// </summary>
        /// <see cref="ILighterSocketClientExchangeApi"/>
        public ILighterSocketClientExchangeApi ExchangeApi { get; }
    }
}
