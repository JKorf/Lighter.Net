using CryptoExchange.Net.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Lighter.Net.Clients;
using Lighter.Net.Objects.Options;
using Lighter.Net.Objects.Models;

namespace Lighter.Net.UnitTests
{
    [NonParallelizable]
    internal class LighterSocketIntegrationTests : SocketIntegrationTest<LighterSocketClient>
    {
        public override bool Run { get; set; } = false;

        public LighterSocketIntegrationTests()
        {
        }

        public override LighterSocketClient GetClient(ILoggerFactory loggerFactory)
        {
            var key = Environment.GetEnvironmentVariable("APIKEY");
            var sec = Environment.GetEnvironmentVariable("APISECRET");

            Authenticated = key != null && sec != null;
            return new LighterSocketClient(Options.Create(new LighterSocketOptions
            {
                OutputOriginalData = true,
                //ApiCredentials = Authenticated ? new LighterCredentials(key, sec) : null
            }), loggerFactory);
        }

        [TestCase]
        public async Task TestSubscriptions()
        {
            await RunAndCheckUpdate<LighterAllTickerUpdate>((client, updateHandler) => client.ExchangeApi.ExchangeData.SubscribeToFuturesTickerUpdatesAsync(updateHandler, default), true, false);
        } 
    }
}
