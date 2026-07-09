using CryptoExchange.Net.SharedApis;
using CryptoExchange.Net.Trackers.Klines;
using CryptoExchange.Net.Trackers.Trades;
using CryptoExchange.Net.Trackers.UserData.Interfaces;
using CryptoExchange.Net.Trackers.UserData.Objects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using Lighter.Net.Clients;
using Lighter.Net.Interfaces;
using Lighter.Net.Interfaces.Clients;

namespace Lighter.Net
{
    /// <inheritdoc />
    public class LighterTrackerFactory : ILighterTrackerFactory
    {
        private readonly IServiceProvider? _serviceProvider;

        /// <summary>
        /// ctor
        /// </summary>
        public LighterTrackerFactory()
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="serviceProvider">Service provider for resolving logging and clients</param>
        public LighterTrackerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc />
        public bool CanCreateKlineTracker(SharedSymbol symbol, SharedKlineInterval interval)
        {
            var client = _serviceProvider?.GetRequiredService<ILighterSocketClient>() ?? new LighterSocketClient();
            SubscribeKlineOptions klineOptions = client.ExchangeApi.SharedClient.SubscribeKlineOptions;
            return klineOptions.IsSupported(interval); 
        }

        /// <inheritdoc />
        public bool CanCreateTradeTracker(SharedSymbol symbol) => true;

        /// <inheritdoc />
        public IKlineTracker CreateKlineTracker(SharedSymbol symbol, SharedKlineInterval interval, int? limit = null, TimeSpan? period = null, ExchangeParameters? exchangeParameters = null)
        {
            var restClient = _serviceProvider?.GetRequiredService<ILighterRestClient>() ?? new LighterRestClient();
            var socketClient = _serviceProvider?.GetRequiredService<ILighterSocketClient>() ?? new LighterSocketClient();

            var sharedRestClient = restClient.ExchangeApi.SharedClient;
            var sharedSocketClient = socketClient.ExchangeApi.SharedClient;
            return new KlineTracker(
                _serviceProvider?.GetRequiredService<ILoggerFactory>().CreateLogger(restClient.Exchange),
                sharedRestClient,
                sharedSocketClient,
                symbol,
                interval,
                limit,
                period,
                exchangeParameters
                );
        }
        /// <inheritdoc />
        public ITradeTracker CreateTradeTracker(SharedSymbol symbol, int? limit = null, TimeSpan? period = null, ExchangeParameters? exchangeParameters = null)
        {
            var restClient = _serviceProvider?.GetRequiredService<ILighterRestClient>() ?? new LighterRestClient();
            var socketClient = _serviceProvider?.GetRequiredService<ILighterSocketClient>() ?? new LighterSocketClient();

            var sharedRestClient = restClient.ExchangeApi.SharedClient;
            var sharedSocketClient = socketClient.ExchangeApi.SharedClient;
            return new TradeTracker(
                _serviceProvider?.GetRequiredService<ILoggerFactory>().CreateLogger(restClient.Exchange),
                sharedRestClient,
                null,
                sharedSocketClient,
                symbol,
                limit,
                period,
                exchangeParameters
                );
        }

        /// <inheritdoc />
        public IUserSpotDataTracker CreateUserSpotDataTracker(SpotUserDataTrackerConfig? config = null)
        {
            var restClient = _serviceProvider?.GetRequiredService<ILighterRestClient>() ?? new LighterRestClient();
            var socketClient = _serviceProvider?.GetRequiredService<ILighterSocketClient>() ?? new LighterSocketClient();
            return new LighterUserSpotDataTracker(
                _serviceProvider?.GetRequiredService<ILogger<LighterUserSpotDataTracker>>() ?? new NullLogger<LighterUserSpotDataTracker>(),
                restClient,
                socketClient,
                null,
                config
                );
        }

        /// <inheritdoc />
        public IUserSpotDataTracker CreateUserSpotDataTracker(string userIdentifier, LighterCredentials credentials, SpotUserDataTrackerConfig? config = null, LighterEnvironment? environment = null)
        {
            var clientProvider = _serviceProvider?.GetRequiredService<ILighterUserClientProvider>() ?? new LighterUserClientProvider();
            var restClient = clientProvider.GetRestClient(userIdentifier, credentials, environment);
            var socketClient = clientProvider.GetSocketClient(userIdentifier, credentials, environment);
            return new LighterUserSpotDataTracker(
                _serviceProvider?.GetRequiredService<ILogger<LighterUserSpotDataTracker>>() ?? new NullLogger<LighterUserSpotDataTracker>(),
                restClient,
                socketClient,
                userIdentifier,
                config
                );
        }

        /// <inheritdoc />
        public IUserFuturesDataTracker CreateUserFuturesDataTracker(FuturesUserDataTrackerConfig? config = null)
        {
            var restClient = _serviceProvider?.GetRequiredService<ILighterRestClient>() ?? new LighterRestClient();
            var socketClient = _serviceProvider?.GetRequiredService<ILighterSocketClient>() ?? new LighterSocketClient();
            return new LighterUserFuturesDataTracker(
                _serviceProvider?.GetRequiredService<ILogger<LighterUserFuturesDataTracker>>() ?? new NullLogger<LighterUserFuturesDataTracker>(),
                restClient,
                socketClient,
                null,
                config
                );
        }

        /// <inheritdoc />
        public IUserFuturesDataTracker CreateUserFuturesDataTracker(string userIdentifier, LighterCredentials credentials, FuturesUserDataTrackerConfig? config = null, LighterEnvironment? environment = null)
        {
            var clientProvider = _serviceProvider?.GetRequiredService<ILighterUserClientProvider>() ?? new LighterUserClientProvider();
            var restClient = clientProvider.GetRestClient(userIdentifier, credentials, environment);
            var socketClient = clientProvider.GetSocketClient(userIdentifier, credentials, environment);
            return new LighterUserFuturesDataTracker(
                _serviceProvider?.GetRequiredService<ILogger<LighterUserFuturesDataTracker>>() ?? new NullLogger<LighterUserFuturesDataTracker>(),
                restClient,
                socketClient,
                userIdentifier,
                config
                );
        }
    }
}
