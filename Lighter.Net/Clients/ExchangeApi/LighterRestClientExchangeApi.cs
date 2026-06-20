using CryptoExchange.Net.Objects;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Lighter.Net.Interfaces.Clients.ExchangeApi;
using Lighter.Net.Objects.Options;
using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.SharedApis;
using CryptoExchange.Net.Objects.Errors;
using CryptoExchange.Net.Converters.MessageParsing.DynamicConverters;
using Lighter.Net.Clients.MessageHandlers;
using Lighter.Net.Objects.Models;

namespace Lighter.Net.Clients.ExchangeApi
{
    /// <inheritdoc cref="ILighterRestClientExchangeApi" />
    internal partial class LighterRestClientExchangeApi : RestApiClient<LighterEnvironment, LighterAuthenticationProvider, LighterCredentials>, ILighterRestClientExchangeApi
    {
        #region fields 
        protected override ErrorMapping ErrorMapping => LighterErrors.Errors;

        internal new LighterRestOptions ClientOptions => (LighterRestOptions)base.ClientOptions;

        /// <inheritdoc />
        protected override IRestMessageHandler MessageHandler { get; } = new LighterRestMessageHandler(LighterErrors.Errors);

        internal readonly LighterRestClient _baseClient;
        #endregion

        #region Api clients
        /// <inheritdoc />
        public ILighterRestClientExchangeApiAccount Account { get; }
        /// <inheritdoc />
        public ILighterRestClientExchangeApiExchangeData ExchangeData { get; }
        /// <inheritdoc />
        public ILighterRestClientExchangeApiTrading Trading { get; }
        /// <inheritdoc />
        public string ExchangeName => "Lighter";
        #endregion

        #region constructor/destructor
        internal LighterRestClientExchangeApi(LighterRestClient client, ILoggerFactory? loggerFactory, HttpClient? httpClient, LighterRestOptions options)
            : base(loggerFactory, LighterExchange.Metadata.Id, httpClient, options.Environment.RestClientAddress, options, options.ExchangeOptions)
        {
            _baseClient = client;

            RequestBodyFormat = RequestBodyFormat.FormData;

            Account = new LighterRestClientExchangeApiAccount(this);
            ExchangeData = new LighterRestClientExchangeApiExchangeData(_logger, this);
            Trading = new LighterRestClientExchangeApiTrading(_logger, this);
        }
        #endregion

        /// <inheritdoc />
        protected override IMessageSerializer CreateSerializer() => new SystemTextJsonMessageSerializer(LighterExchange._serializerContext);

        /// <inheritdoc />
        protected override LighterAuthenticationProvider CreateAuthenticationProvider(LighterCredentials credentials)
            => new LighterAuthenticationProvider(credentials);

        internal async Task<HttpResult> SendAsync(RequestDefinition definition, Parameters? parameters, CancellationToken cancellationToken, int? weight = null)
        {
            var result = await base.SendAsync<Unit>(definition, parameters, cancellationToken, null, weight).ConfigureAwait(false);
            return result;
        }

        internal async Task<HttpResult<T>> SendAsync<T>(RequestDefinition definition, Parameters? parameters, CancellationToken cancellationToken, int? weight = null, bool skipCheck = false)
        {
            if (!skipCheck)
                await LighterUtils.CheckBuilderFeeAsync(_baseClient).ConfigureAwait(false);

            var result = await base.SendAsync<T>(definition, parameters, cancellationToken, null, weight).ConfigureAwait(false);
            return result;
        }

        /// <inheritdoc />
        protected override async Task<HttpResult<DateTime>> GetServerTimestampAsync()
        {
            var result = await ExchangeData.GetStatusAsync().ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<DateTime>(result);

            return HttpResult.Ok(result, result.Data.Timestamp);
        }

        internal async Task<long> GetNoncesAsync(int count)
        {
            if (!LighterUtils.NonceSet(ApiCredentials!))
            {
                var nonceResult = await Account.GetNonceAsync().ConfigureAwait(false);
                if (nonceResult.Success)
                    LighterUtils.SetNonce(ApiCredentials!, nonceResult.Data.Nonce);
            }

            return LighterUtils.GetNonce(ApiCredentials!, count);
        }

        internal async Task<long> GetNonceAsync()
        {
            if (!LighterUtils.NonceSet(ApiCredentials!))
            {
                var nonceResult = await Account.GetNonceAsync().ConfigureAwait(false);
                if (nonceResult.Success)
                    LighterUtils.SetNonce(ApiCredentials!, nonceResult.Data.Nonce);
            }

            return LighterUtils.GetNonce(ApiCredentials!);
        }

        /// <inheritdoc />
        public override string FormatSymbol(string baseAsset, string quoteAsset, TradingMode tradingMode, DateTime? deliverDate = null) 
            => LighterExchange.FormatSymbol(baseAsset, quoteAsset, tradingMode, deliverDate);

        internal async Task<CallResult<LighterSymbol>> GetSymbolInfoAsync(string symbol)
        {
            if (EnvironmentName == "UnitTest")
                return CallResult.Ok(new LighterSymbol { MarketId = 0, SupportedQuantityDecimals = 4 });

            var marketInfo = LighterUtils.GetSymbolInfo(EnvironmentName, symbol);
            if (marketInfo == null)
            {
                var symbolResult = await ExchangeData.GetSymbolsAsync().ConfigureAwait(false);
                if (!symbolResult.Success)
                    return CallResult.Fail<LighterSymbol>(symbolResult.Error!);

                marketInfo = LighterUtils.GetSymbolInfo(EnvironmentName, symbol);
                if (marketInfo == null)
                    return CallResult.Fail<LighterSymbol>(new ServerError(ErrorType.UnknownSymbol, "Symbol not found"));
            }

            return CallResult.Ok(marketInfo);
        }

        /// <inheritdoc />
        public ILighterRestClientExchangeApiShared SharedClient => this;
    }
}
