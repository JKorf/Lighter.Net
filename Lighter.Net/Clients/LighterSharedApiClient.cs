using Lighter.Net.Interfaces.Clients;
using Lighter.Net.Interfaces.Clients.ExchangeApi;

namespace Lighter.Net.Clients
{
    /// <inheritdoc />
    public class LighterSharedApiClient : ILighterSharedApiClient
    {
        /// <inheritdoc />
        public ILighterRestClientExchangeSharedApi Rest { get; }
        /// <inheritdoc />
        public ILighterSocketClientExchangeSharedApi Socket { get; }

        /// <summary>
        /// ctor
        /// </summary>
        public LighterSharedApiClient(
            ILighterRestClient restClient,
            ILighterSocketClient socketClient)
        {
            Rest = restClient.ExchangeApi.SharedApi;
            Socket = socketClient.ExchangeApi.SharedApi;
        }
    }
}
