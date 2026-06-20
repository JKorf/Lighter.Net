using CryptoExchange.Net;
using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Converters.MessageParsing.DynamicConverters;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Errors;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.SharedApis;
using Lighter.Net.Clients.MessageHandlers;
using Lighter.Net.Interfaces.Clients.ExchangeApi;
using Lighter.Net.Objects.Internal;
using Lighter.Net.Objects.Models;
using Lighter.Net.Objects.Options;
using Lighter.Net.Objects.Sockets;
using Lighter.Net.Objects.Sockets.Subscriptions;
using Microsoft.Extensions.Logging;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Lighter.Net.Clients.ExchangeApi
{
    /// <summary>
    /// Client providing access to the Lighter Exchange websocket Api
    /// </summary>
    internal partial class LighterSocketClientExchangeApi : SocketApiClient<LighterEnvironment, LighterAuthenticationProvider, LighterCredentials>, ILighterSocketClientExchangeApi
    {
        #region fields
        protected override ErrorMapping ErrorMapping => LighterErrors.Errors;
        #endregion

        /// <inheritdoc />
        public ILighterSocketClientExchangeApiAccount Account { get; }
        /// <inheritdoc />
        public ILighterSocketClientExchangeApiExchangeData ExchangeData { get; }
        /// <inheritdoc />
        public ILighterSocketClientExchangeApiTrading Trading { get; }

        internal new LighterSocketOptions ClientOptions => (LighterSocketOptions)base.ClientOptions;

        #region constructor/destructor

        /// <summary>
        /// ctor
        /// </summary>
        internal LighterSocketClientExchangeApi(ILoggerFactory? loggerFactory, LighterSocketOptions options) :
            base(loggerFactory, LighterExchange.Metadata.Id, options.Environment.SocketClientAddress!, options, options.ExchangeOptions)
        {
            Account = new LighterSocketClientExchangeApiAccount(_logger, this);
            ExchangeData = new LighterSocketClientExchangeApiExchangeData(_logger, this);
            Trading = new LighterSocketClientExchangeApiTrading(_logger, this);

            RateLimiter = LighterExchange.RateLimiter.LighterSocket;
            MaxIndividualSubscriptionsPerConnection = 500;

            AddSystemSubscription(new LighterConnectedSubscription(_logger));

            RegisterPeriodicQuery("Ping",
                TimeSpan.FromSeconds(30),
                x => new LighterPingQuery(),
                (connection, result) =>
                {
                    if (result.Error?.ErrorType == ErrorType.Timeout)
                    {
                        // Ping timeout, reconnect
                        _logger.LogWarning("[Sckt {SocketId}] Ping response timeout, reconnecting", connection.SocketId);
                        _ = connection.TriggerReconnectAsync();
                    }
                });
        }
        #endregion

        /// <inheritdoc />
        protected override IMessageSerializer CreateSerializer() => new SystemTextJsonMessageSerializer(LighterExchange._serializerContext);
        /// <inheritdoc />
        public override ISocketMessageHandler CreateMessageConverter(WebSocketMessageType messageType) => new LighterSocketMessageHandler();

        /// <inheritdoc />
        protected override LighterAuthenticationProvider CreateAuthenticationProvider(LighterCredentials credentials)
            => new LighterAuthenticationProvider(credentials);

        /// <inheritdoc />
        public ILighterSocketClientExchangeApiShared SharedClient => this;

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
                using var restClient = new LighterRestClient(opts =>
                {
                    opts.Environment = ClientOptions.Environment;
                    opts.Proxy = ClientOptions.Proxy;
                });

                var symbolResult = await restClient.ExchangeApi.ExchangeData.GetSymbolsAsync().ConfigureAwait(false);
                if (!symbolResult.Success)
                    return CallResult.Fail<LighterSymbol>(symbolResult.Error!);

                marketInfo = LighterUtils.GetSymbolInfo(EnvironmentName, symbol);
                if (marketInfo == null)
                    return CallResult.Fail<LighterSymbol>(new ServerError(ErrorType.UnknownSymbol, "Symbol not found"));
            }

            return CallResult.Ok(marketInfo);
        }

        internal async Task<long> GetNoncesAsync(int count)
        {
            if (!LighterUtils.NonceSet(ApiCredentials!))
            {
                using var restClient = new LighterRestClient(opts =>
                {
                    opts.Environment = ClientOptions.Environment;
                    opts.Proxy = ClientOptions.Proxy;
                    opts.ApiCredentials = ApiCredentials;
                });

                var nonceResult = await restClient.ExchangeApi.Account.GetNonceAsync().ConfigureAwait(false);
                if (nonceResult.Success)
                    LighterUtils.SetNonce(ApiCredentials!, nonceResult.Data.Nonce);
            }

            return LighterUtils.GetNonce(ApiCredentials!, count);
        }

        internal async Task<long> GetNonceAsync()
        {
            if (!LighterUtils.NonceSet(ApiCredentials!))
            {
                using var restClient = new LighterRestClient(opts =>
                {
                    opts.Environment = ClientOptions.Environment;
                    opts.Proxy = ClientOptions.Proxy;
                    opts.ApiCredentials = ApiCredentials;
                });

                var nonceResult = await restClient.ExchangeApi.Account.GetNonceAsync().ConfigureAwait(false);
                if (nonceResult.Success)
                    LighterUtils.SetNonce(ApiCredentials!, nonceResult.Data.Nonce);
            }

            return LighterUtils.GetNonce(ApiCredentials!);
        }

        internal async Task<WebSocketResult<UpdateSubscription>> SubscribeAsync<T>(
            LighterSubscription<T> subscription, CancellationToken ct)
        {
            return await base.SubscribeAsync(BaseAddress.AppendPath("stream"), subscription, ct).ConfigureAwait(false);
        }

        internal async Task<QueryResult<TResponse>> QueryAsync<TResponse, TRequest>(LighterRequestQuery<TResponse, TRequest> query, CancellationToken ct)
            where TRequest : LighterQueryRequest
            where TResponse: LighterQueryResponse
        {
            return await base.QueryAsync<TResponse>(BaseAddress.AppendPath("stream"), query, ct).ConfigureAwait(false);
        }

    }
}
