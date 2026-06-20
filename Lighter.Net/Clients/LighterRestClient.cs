using CryptoExchange.Net.Clients;
using Lighter.Net.Clients.ExchangeApi;
using Lighter.Net.Interfaces.Clients;
using Lighter.Net.Interfaces.Clients.ExchangeApi;
using Lighter.Net.Objects.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;


namespace Lighter.Net.Clients
{
    /// <inheritdoc cref="ILighterRestClient" />
    public class LighterRestClient : BaseRestClient<LighterEnvironment, LighterCredentials>, ILighterRestClient
    {
        /// <inheritdoc />
        public new LighterRestOptions ClientOptions => (LighterRestOptions)base.ClientOptions;

        #region Api clients

        /// <inheritdoc />
        public ILighterRestClientExchangeApi ExchangeApi { get; }


        #endregion

        #region constructor/destructor

        /// <summary>
        /// Create a new instance of the LighterRestClient using provided options
        /// </summary>
        /// <param name="optionsDelegate">Option configuration delegate</param>
        public LighterRestClient(Action<LighterRestOptions>? optionsDelegate = null)
            : this(null, null, Options.Create(ApplyOptionsDelegate(optionsDelegate)))
        {
        }

        /// <summary>
        /// Create a new instance of the LighterRestClient using provided options
        /// </summary>
        /// <param name="options">Option configuration</param>
        /// <param name="loggerFactory">The logger factory</param>
        /// <param name="httpClient">Http client for this client</param>
        public LighterRestClient(HttpClient? httpClient, ILoggerFactory? loggerFactory, IOptions<LighterRestOptions> options) : base(loggerFactory, "Lighter")
        {
            Initialize(options.Value);

            ExchangeApi = AddApiClient(new LighterRestClientExchangeApi(this, loggerFactory, httpClient, options.Value));
        }

        #endregion

        /// <summary>
        /// Set the default options to be used when creating new clients
        /// </summary>
        /// <param name="optionsDelegate">Option configuration delegate</param>
        public static void SetDefaultOptions(Action<LighterRestOptions> optionsDelegate)
        {
            LighterRestOptions.Default = ApplyOptionsDelegate(optionsDelegate);
        }
    }
}
