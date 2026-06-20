using CryptoExchange.Net.Sockets;
using CryptoExchange.Net.Sockets.Default.Routing;
using Lighter.Net.Objects.Internal;
using CryptoExchange.Net.Clients;

namespace Lighter.Net.Objects.Sockets
{
    internal class LighterUnsubscriptionQuery : Query<LighterSocketUpdate>
    {
        private readonly SocketApiClient _client;

        public LighterUnsubscriptionQuery(SocketApiClient client, LighterSocketRequest request, bool authenticated, int weight = 1) : base(request, authenticated, weight)
        {
            _client = client;

            MessageRouter = MessageRouter.CreateVoid<LighterSocketUpdate>("unsubscribed" + request.Channel.Replace('/', ':'));
        }
    }
}
