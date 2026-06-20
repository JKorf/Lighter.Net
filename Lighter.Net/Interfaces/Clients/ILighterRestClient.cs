using CryptoExchange.Net.Interfaces.Clients;
using Lighter.Net.Interfaces.Clients.ExchangeApi;

namespace Lighter.Net.Interfaces.Clients
{
    /// <summary>
    /// Client for accessing the Lighter Rest API. 
    /// </summary>
    public interface ILighterRestClient : IRestClient<LighterCredentials>
    {
        /// <summary>
        /// Exchange API endpoints
        /// </summary>
        /// <see cref="ILighterRestClientExchangeApi"/>
        public ILighterRestClientExchangeApi ExchangeApi { get; }

    }
}
