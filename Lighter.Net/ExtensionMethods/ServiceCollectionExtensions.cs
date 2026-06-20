using CryptoExchange.Net;
using CryptoExchange.Net.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading;
using Lighter.Net;
using Lighter.Net.Clients;
using Lighter.Net.Interfaces;
using Lighter.Net.Interfaces.Clients;
using Lighter.Net.Objects.Options;
using Lighter.Net.SymbolOrderBooks;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extensions for DI
    /// </summary>
    public static class ServiceCollectionExtensions
    {

        /// <summary>
        /// Add services such as the ILighterRestClient and ILighterSocketClient. Configures the services based on the provided configuration.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">The configuration(section) containing the options</param>
        /// <returns></returns>
        public static IServiceCollection AddLighter(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var options = new LighterOptions();
            // Reset environment so we know if they're overridden
            options.Rest.Environment = null!;
            options.Socket.Environment = null!;
            try
            {
                configuration.Bind(options);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("Invalid configuration provided", ex);
            }

            if (options.Rest == null || options.Socket == null)
                throw new ArgumentException("Options null");

            var restEnvName = options.Rest.Environment?.Name ?? options.Environment?.Name ?? LighterEnvironment.Live.Name;
            var socketEnvName = options.Socket.Environment?.Name ?? options.Environment?.Name ?? LighterEnvironment.Live.Name;
            options.Rest.Environment = LighterEnvironment.GetEnvironmentByName(restEnvName) ?? options.Rest.Environment!;
            options.Rest.ApiCredentials = options.Rest.ApiCredentials ?? options.ApiCredentials;
            options.Socket.Environment = LighterEnvironment.GetEnvironmentByName(socketEnvName) ?? options.Socket.Environment!;
            options.Socket.ApiCredentials = options.Socket.ApiCredentials ?? options.ApiCredentials;


            services.AddSingleton(x => Options.Options.Create(options.Rest));
            services.AddSingleton(x => Options.Options.Create(options.Socket));

            return AddLighterCore(services, options.SocketClientLifeTime);
        }

        /// <summary>
        /// Add services such as the ILighterRestClient and ILighterSocketClient. Services will be configured based on the provided options.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="optionsDelegate">Set options for the Lighter services</param>
        /// <returns></returns>
        public static IServiceCollection AddLighter(
            this IServiceCollection services,
            Action<LighterOptions>? optionsDelegate = null)
        {
            var options = new LighterOptions();
            // Reset environment so we know if they're overridden
            options.Rest.Environment = null!;
            options.Socket.Environment = null!;
            optionsDelegate?.Invoke(options);
            if (options.Rest == null || options.Socket == null)
                throw new ArgumentException("Options null");

            options.Rest.Environment = options.Rest.Environment ?? options.Environment ?? LighterEnvironment.Live;
            options.Rest.ApiCredentials = options.Rest.ApiCredentials ?? options.ApiCredentials;
            options.Socket.Environment = options.Socket.Environment ?? options.Environment ?? LighterEnvironment.Live;
            options.Socket.ApiCredentials = options.Socket.ApiCredentials ?? options.ApiCredentials;

            services.AddSingleton(x => Options.Options.Create(options.Rest));
            services.AddSingleton(x => Options.Options.Create(options.Socket));

            return AddLighterCore(services, options.SocketClientLifeTime);
        }

        private static IServiceCollection AddLighterCore(
            this IServiceCollection services,
            ServiceLifetime? socketClientLifeTime = null)
        {
            services.AddHttpClient<ILighterRestClient, LighterRestClient>((client, serviceProvider) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<LighterRestOptions>>().Value;
                client.Timeout = options.RequestTimeout;
                return new LighterRestClient(client, serviceProvider.GetRequiredService<ILoggerFactory>(), serviceProvider.GetRequiredService<IOptions<LighterRestOptions>>());
            }).ConfigurePrimaryHttpMessageHandler((serviceProvider) => {
                var options = serviceProvider.GetRequiredService<IOptions<LighterRestOptions>>().Value;
                return LibraryHelpers.CreateHttpClientMessageHandler(options);
            }).SetHandlerLifetime(Timeout.InfiniteTimeSpan);
            services.Add(new ServiceDescriptor(typeof(ILighterSocketClient), x => { return new LighterSocketClient(x.GetRequiredService<IOptions<LighterSocketOptions>>(), x.GetRequiredService<ILoggerFactory>()); }, socketClientLifeTime ?? ServiceLifetime.Singleton));

            services.AddTransient<ILighterOrderBookFactory, LighterOrderBookFactory>();
            services.AddTransient<ITrackerFactory, LighterTrackerFactory>();
            services.AddTransient<ILighterTrackerFactory, LighterTrackerFactory>();
            services.AddSingleton<ILighterUserClientProvider, LighterUserClientProvider>(x =>
                new LighterUserClientProvider(
                    x.GetRequiredService<IHttpClientFactory>().CreateClient(typeof(ILighterRestClient).Name),
                    x.GetRequiredService<ILoggerFactory>(),
                    x.GetRequiredService<IOptions<LighterRestOptions>>(),
                    x.GetRequiredService<IOptions<LighterSocketOptions>>()));

            services.RegisterSharedRestInterfaces(x => x.GetRequiredService<ILighterRestClient>().ExchangeApi.SharedClient);
            services.RegisterSharedSocketInterfaces(x => x.GetRequiredService<ILighterSocketClient>().ExchangeApi.SharedClient);

            return services;
        }
    }
}
