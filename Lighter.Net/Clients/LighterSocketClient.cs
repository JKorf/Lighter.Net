using CryptoExchange.Net.Clients;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using Lighter.Net.Interfaces.Clients;
using Lighter.Net.Objects.Options;
using Lighter.Net.Interfaces.Clients.ExchangeApi;
using Lighter.Net.Clients.ExchangeApi;

namespace Lighter.Net.Clients
{
    /// <inheritdoc cref="ILighterSocketClient" />
    public class LighterSocketClient : BaseSocketClient<LighterEnvironment, LighterCredentials>, ILighterSocketClient
    {
        /// <inheritdoc />
        public new LighterSocketOptions ClientOptions => (LighterSocketOptions)base.ClientOptions;

        #region Api clients

        /// <inheritdoc />
        public ILighterSocketClientExchangeApi ExchangeApi { get; }

        #endregion

        #region constructor/destructor

        /// <summary>
        /// Create a new instance of LighterSocketClient
        /// </summary>
        /// <param name="optionsDelegate">Option configuration delegate</param>
        public LighterSocketClient(Action<LighterSocketOptions>? optionsDelegate = null)
            : this(Options.Create(ApplyOptionsDelegate(optionsDelegate)), null)
        {
        }

        /// <summary>
        /// Create a new instance of LighterSocketClient
        /// </summary>
        /// <param name="loggerFactory">The logger factory</param>
        /// <param name="options">Option configuration</param>
        public LighterSocketClient(IOptions<LighterSocketOptions> options, ILoggerFactory? loggerFactory = null) : base(loggerFactory, "Lighter")
        {
            Initialize(options.Value);

            ExchangeApi = AddApiClient(new LighterSocketClientExchangeApi(loggerFactory, options.Value));
        }
        #endregion

        /// <summary>
        /// Set the default options to be used when creating new clients
        /// </summary>
        /// <param name="optionsDelegate">Option configuration delegate</param>
        public static void SetDefaultOptions(Action<LighterSocketOptions> optionsDelegate)
        {
            LighterSocketOptions.Default = ApplyOptionsDelegate(optionsDelegate);
        }
    }
}
