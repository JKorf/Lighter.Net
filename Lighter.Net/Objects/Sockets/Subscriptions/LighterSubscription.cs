using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using CryptoExchange.Net.Sockets.Default;
using CryptoExchange.Net.Sockets.Default.Routing;
using Lighter.Net.Clients.ExchangeApi;
using Microsoft.Extensions.Logging;
using System;

namespace Lighter.Net.Objects.Sockets.Subscriptions
{
    /// <inheritdoc />
    internal class LighterSubscription<T> : Subscription
    {
        private readonly Action<DateTime, string?, T> _handler;

        private readonly string _channel;
        private readonly string? _suffix;
        private readonly LighterSocketClientExchangeApi _client;
        private readonly bool _auth;

        /// <summary>
        /// ctor
        /// </summary>
        public LighterSubscription(LighterSocketClientExchangeApi client, ILogger logger, string channel, string? channelSuffix, Action<DateTime, string?, T> handler, bool auth) : base(logger, false)
        {
            _client = client;
            _handler = handler;
            _auth = auth;

            _channel = channel;
            _suffix = channelSuffix;
            MessageRouter = MessageRouter.CreateForEvent<T>(
                [
                    $"subscribed/{channel}{channel}{_suffix?.Replace('/', ':')}", 
                    $"update/{channel}{channel}{_suffix?.Replace('/', ':')}"
                ], 
                DoHandleMessage);
        }

        /// <inheritdoc />
        protected override Query? GetSubQuery(SocketConnection connection)
        {
            return new LighterSubscriptionQuery(_client, new Internal.LighterSocketRequest
            {
                Type = "subscribe",
                Channel = $"{_channel}{_suffix}",
                Auth = _auth ? LighterUtils.GetAuthToken(_client.ClientOptions.LibraryPath, _client.BaseAddress, _client.ApiCredentials!) : null
            }, false, 1);
        }

        /// <inheritdoc />
        protected override Query? GetUnsubQuery(SocketConnection connection)
        {
            return new LighterUnsubscriptionQuery(_client, new Internal.LighterSocketRequest
            {
                Type = "unsubscribe",
                Channel = $"{_channel}{_suffix}",
            }, false, 1);
        }

        /// <inheritdoc />
        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, T message)
        {
            _handler.Invoke(receiveTime, originalData, message);
            return CallResult.Ok();
        }
    }
}
