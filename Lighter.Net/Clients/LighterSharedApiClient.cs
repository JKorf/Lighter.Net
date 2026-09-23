using CryptoExchange.Net.SharedApis;
using Lighter.Net.Interfaces.Clients;
using Lighter.Net.Interfaces.Clients.ExchangeApi;
using Lighter.Net.Objects.Options;
using Microsoft.Extensions.Options;

namespace Lighter.Net.Clients
{
    /// <inheritdoc />
    public class LighterSharedApiClient : SharedApiClientBase, ILighterSharedApiClient
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
            ILighterSocketClient socketClient,
            IOptions<LighterOptions> options)
            : base(options.Value.SharedApi.PreferredTransport,
                  restClient.ExchangeApi.SharedApi,
                  socketClient.ExchangeApi.SharedApi)
        {
            Rest = restClient.ExchangeApi.SharedApi;
            Socket = socketClient.ExchangeApi.SharedApi;
        }
    }
}
