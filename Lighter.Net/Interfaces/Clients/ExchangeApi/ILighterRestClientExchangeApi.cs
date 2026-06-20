using CryptoExchange.Net.Interfaces.Clients;
using System;

namespace Lighter.Net.Interfaces.Clients.ExchangeApi
{
    /// <summary>
    /// Lighter Exchange API endpoints
    /// </summary>
    public interface ILighterRestClientExchangeApi : IRestApiClient<LighterCredentials>, IDisposable
    {
        /// <summary>
        /// Endpoints related to account settings, info or actions
        /// </summary>
        /// <see cref="ILighterRestClientExchangeApiAccount" />
        public ILighterRestClientExchangeApiAccount Account { get; }

        /// <summary>
        /// Endpoints related to retrieving market and system data
        /// </summary>
        /// <see cref="ILighterRestClientExchangeApiExchangeData" />
        public ILighterRestClientExchangeApiExchangeData ExchangeData { get; }

        /// <summary>
        /// Endpoints related to orders and trades
        /// </summary>
        /// <see cref="ILighterRestClientExchangeApiTrading" />
        public ILighterRestClientExchangeApiTrading Trading { get; }

        /// <summary>
        /// Get the shared rest requests client. This interface is shared with other exchanges to allow for a common implementation for different exchanges.
        /// </summary>
        public ILighterRestClientExchangeApiShared SharedClient { get; }
    }
}
