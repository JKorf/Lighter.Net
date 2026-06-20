using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System.Threading.Tasks;
using Lighter.Net.Clients;
using Lighter.Net.Objects.Models;
using Lighter.Net.Objects.Options;
using Lighter.Net.Objects;

namespace Lighter.Net.UnitTests
{
    [TestFixture]
    public class SocketSubscriptionTests
    {
        [Test]
        public async Task ValidateSubscriptions()
        {
            var logger = new LoggerFactory();
            logger.AddProvider(new TraceLoggerProvider());

            var client = new LighterSocketClient(Options.Create(new LighterSocketOptions
            {
                //opts.ApiCredentials = new LighterCredentials("123", "456");
                Environment = LighterEnvironment.CreateCustom("UnitTest", LighterApiAddresses.Default.RestClientAddress, LighterApiAddresses.Default.SocketClientAddress)
            }), logger);
            var tester = new SocketSubscriptionValidator<LighterSocketClient>(client, "Subscriptions/Exchange", "wss://mainnet.zklighter.elliot.ai/stream");
            await tester.ValidateAsync<LighterSpotTickerUpdate>((client, handler) => client.ExchangeApi.ExchangeData.SubscribeToSpotTickerUpdatesAsync("ETH/USDC", handler), "SpotTicker");
            await tester.ValidateAsync<LighterKlineUpdate>((client, handler) => client.ExchangeApi.ExchangeData.SubscribeToKlineUpdatesAsync("ETH/USDC", Enums.KlineInterval.OneHour, handler), "Kline");
        }

        [TestCase]
        public async Task ValidateConcurrentSpotSubscriptions()
        {
            var logger = new LoggerFactory();
            logger.AddProvider(new TraceLoggerProvider());

            var client = new LighterSocketClient(Options.Create(new LighterSocketOptions
            {
                Environment = LighterEnvironment.CreateCustom("UnitTest", LighterApiAddresses.Default.RestClientAddress, LighterApiAddresses.Default.SocketClientAddress),
                OutputOriginalData = true
            }), logger);

            var tester = new SocketSubscriptionValidator<LighterSocketClient>(client, "Subscriptions/Exchange", "wss://mainnet.zklighter.elliot.ai/stream");
            await tester.ValidateConcurrentAsync<LighterKlineUpdate>(
                (client, handler) => client.ExchangeApi.ExchangeData.SubscribeToKlineUpdatesAsync("ETH/USDC", Enums.KlineInterval.OneDay, handler),
                (client, handler) => client.ExchangeApi.ExchangeData.SubscribeToKlineUpdatesAsync("ETH/USDC", Enums.KlineInterval.OneHour, handler),
                "Concurrent");
        }
    }
}
