using CryptoExchange.Net.Trackers.UserData;
using CryptoExchange.Net.Trackers.UserData.Objects;
using Microsoft.Extensions.Logging;
using Lighter.Net.Interfaces.Clients;

namespace Lighter.Net
{
    /// <inheritdoc />
    public class LighterUserSpotDataTracker : UserSpotDataTracker
    {
        /// <summary>
        /// ctor
        /// </summary>
        public LighterUserSpotDataTracker(
            ILogger<LighterUserSpotDataTracker> logger,
            ILighterRestClient restClient,
            ILighterSocketClient socketClient,
            string? userIdentifier,
            SpotUserDataTrackerConfig? config = null) : base(
                logger,
                restClient.ExchangeApi.SharedClient,
                restClient.ExchangeApi.SharedClient,
                socketClient.ExchangeApi.SharedClient,
                restClient.ExchangeApi.SharedClient,
                socketClient.ExchangeApi.SharedClient,
                socketClient.ExchangeApi.SharedClient,
                userIdentifier,
                config ?? new SpotUserDataTrackerConfig())
        {
        }
    }

    /// <inheritdoc />
    public class LighterUserFuturesDataTracker : UserFuturesDataTracker
    {
        /// <inheritdoc />
        protected override bool WebsocketPositionUpdatesAreFullSnapshots => false;

        /// <summary>
        /// ctor
        /// </summary>
        public LighterUserFuturesDataTracker(
            ILogger<LighterUserFuturesDataTracker> logger,
            ILighterRestClient restClient,
            ILighterSocketClient socketClient,
            string? userIdentifier,
            FuturesUserDataTrackerConfig? config = null) :
            base(logger,
                restClient.ExchangeApi.SharedClient,
                restClient.ExchangeApi.SharedClient,
                socketClient.ExchangeApi.SharedClient,
                restClient.ExchangeApi.SharedClient,
                socketClient.ExchangeApi.SharedClient,
                socketClient.ExchangeApi.SharedClient,
                socketClient.ExchangeApi.SharedClient,
                userIdentifier,
                config ?? new FuturesUserDataTrackerConfig())
        {

        }
    }
}
