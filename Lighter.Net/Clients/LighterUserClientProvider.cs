using CryptoExchange.Net.Clients;
using Lighter.Net.Interfaces.Clients;
using Lighter.Net.Objects.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;

namespace Lighter.Net.Clients
{
    /// <inheritdoc />
    public class LighterUserClientProvider : UserClientProvider<
        ILighterRestClient,
        ILighterSocketClient,
        LighterRestOptions,
        LighterSocketOptions,
        LighterCredentials,
        LighterEnvironment
        >, ILighterUserClientProvider
    {
        /// <inheritdoc />
        public override string ExchangeName => LighterExchange.Metadata.Id;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="optionsDelegate">Options to use for created clients</param>
        public LighterUserClientProvider(Action<LighterOptions>? optionsDelegate = null)
            : this(null, null, Options.Create(ApplyOptionsDelegate(optionsDelegate).Rest), Options.Create(ApplyOptionsDelegate(optionsDelegate).Socket))
        {
        }
        
        /// <summary>
        /// ctor
        /// </summary>
        public LighterUserClientProvider(
            HttpClient? httpClient,
            ILoggerFactory? loggerFactory,
            IOptions<LighterRestOptions> restOptions,
            IOptions<LighterSocketOptions> socketOptions)
            :base(httpClient, loggerFactory, restOptions, socketOptions)
        {
        }


        /// <inheritdoc />
        protected override ILighterRestClient ConstructRestClient(HttpClient client, ILoggerFactory? loggerFactory, IOptions<LighterRestOptions> options)
            => new LighterRestClient(client, loggerFactory, options);
        /// <inheritdoc />
        protected override ILighterSocketClient ConstructSocketClient(ILoggerFactory? loggerFactory, IOptions<LighterSocketOptions> options)
            => new LighterSocketClient(options, loggerFactory);
    }
}
