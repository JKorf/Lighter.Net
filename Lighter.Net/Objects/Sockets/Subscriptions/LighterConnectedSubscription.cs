using CryptoExchange.Net.Sockets.Default;
using CryptoExchange.Net.Sockets.Default.Routing;
using Microsoft.Extensions.Logging;

namespace Lighter.Net.Objects.Sockets.Subscriptions
{
    internal class LighterConnectedSubscription : SystemSubscription
    {
        public LighterConnectedSubscription(ILogger logger) : base(logger, false)
        {
            MessageRouter = MessageRouter.CreateVoid<string>("connected");
        }
    }
}
