using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using CryptoExchange.Net.Sockets.Default;
using CryptoExchange.Net.Sockets.Default.Routing;
using System;
using Lighter.Net.Objects.Internal;
using CryptoExchange.Net.Clients;

namespace Lighter.Net.Objects.Sockets
{
    internal class LighterSubscriptionQuery : Query<LighterSocketError>
    {
        private readonly SocketApiClient _client;

        public LighterSubscriptionQuery(SocketApiClient client, LighterSocketRequest request, bool authenticated, int weight = 1) : base(request, authenticated, weight)
        {
            _client = client;

            RequestTimeout = TimeSpan.FromSeconds(2);
            TimeoutBehavior = TimeoutBehavior.Succeed;
            MessageRouter = MessageRouter.CreateForQuery<LighterSocketError>("error", HandleError);
        }


        private CallResult<LighterSocketError> HandleError(SocketConnection connection, DateTime time, string? originalData, LighterSocketError message)
        {
            // Some way to know if this error is for this query..?
            return CallResult.Fail<LighterSocketError>(new ServerError(message.Error.Code, _client.GetErrorInfo(message.Error.Code, message.Error.Message)), originalData);
        }
    }
}
