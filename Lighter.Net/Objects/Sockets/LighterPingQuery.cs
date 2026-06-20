using CryptoExchange.Net.Sockets;
using CryptoExchange.Net.Sockets.Default.Routing;
using Lighter.Net.Objects.Internal;

namespace Lighter.Net.Objects.Sockets
{
    internal class LighterPingQuery : Query<LighterSocketPong>
    {
        public LighterPingQuery() : base("{\"type\": \"ping\"}", false)
        {
            MessageRouter = MessageRouter.CreateVoid<LighterSocketPong>("pong");
        }
    }
}
