using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using CryptoExchange.Net.Sockets.Default;
using CryptoExchange.Net.Sockets.Default.Routing;
using Lighter.Net.Objects.Internal;
using System;

namespace Lighter.Net.Objects.Sockets
{
    internal class LighterRequestQuery<TResponse, TRequest> : Query<TResponse>
       where TRequest : LighterQueryRequest
       where TResponse: LighterQueryResponse
    {
        private readonly SocketApiClient _client;

        public LighterRequestQuery(SocketApiClient client, string type, TRequest request, bool authenticated, int weight = 1) : base(new LighterRequestWrapper<TRequest>
        {
            Type = type,
            Data = request
        }, authenticated, weight)
        {
            _client = client;

            MessageRouter = MessageRouter.CreateForQuery<TResponse>(request.Id, HandleMessage);
        }

        private CallResult<TResponse> HandleMessage(SocketConnection connection, DateTime time, string? originalData, TResponse data)
        {
            if (data.Error != null)
                return CallResult.Fail<TResponse>(new ServerError(data.Error.Code, _client.GetErrorInfo(data.Error.Code, data.Error.Message)), originalData);

            return CallResult.Ok(data, originalData);
        }
    }
}
