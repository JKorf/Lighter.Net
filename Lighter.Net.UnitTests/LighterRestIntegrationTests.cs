using CryptoExchange.Net.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Lighter.Net.Clients;
using Lighter.Net.Objects.Options;
using Lighter.Net.Enums;
using Lighter.Net.Objects;
using System.Threading;

namespace Lighter.Net.UnitTests
{
    [NonParallelizable]
    public class LighterRestIntegrationTests : RestIntegrationTest<LighterRestClient>
    {
        public override bool Run { get; set; } = false;

        public override LighterRestClient GetClient(ILoggerFactory loggerFactory)
        {
            return new LighterRestClient(null, loggerFactory, Options.Create(new LighterRestOptions
            {
                AutoTimestamp = false,
                OutputOriginalData = true,
            }));
        }

        [Test]
        public async Task TestErrorResponseParsing()
        {
            if (!ShouldRun())
                return;
            
            // Use different client with unit test environment to skip symbol mapping
            var client = new LighterRestClient(null, null, Options.Create(new LighterRestOptions
            {
                AutoTimestamp = false,
                OutputOriginalData = true,
                Environment = LighterEnvironment.CreateCustom("UnitTest", LighterApiAddresses.Default.RestClientAddress, LighterApiAddresses.Default.SocketClientAddress)
            }));

            var result = await CreateClient().ExchangeApi.ExchangeData.GetKlinesAsync("TSTTST", (KlineInterval)123);

            Assert.That(result.Success, Is.False);
            Assert.That(result.Error.Code, Is.EqualTo(20001));
        }

        [Test]
        public async Task TestExchangeData()
        {
            await RunAndCheckResult(client => client.ExchangeApi.ExchangeData.GetStatusAsync(CancellationToken.None), false, true);
            await RunAndCheckResult(client => client.ExchangeApi.ExchangeData.GetSystemConfigAsync(CancellationToken.None), false, true);
            await RunAndCheckResult(client => client.ExchangeApi.ExchangeData.GetSymbolsAsync(default, default, CancellationToken.None), false, true, compareNestedProperty: "order_books");
            await RunAndCheckResult(client => client.ExchangeApi.ExchangeData.GetLayer1BasicInfoAsync(CancellationToken.None), false, true);
            await RunAndCheckResult(client => client.ExchangeApi.ExchangeData.GetAssetsAsync(default, CancellationToken.None), false, true, compareNestedProperty: "asset_details");
            await RunAndCheckResult(client => client.ExchangeApi.ExchangeData.GetOrderBookAsync("ETH", default, CancellationToken.None), false, true, ignoreProperties: ["transaction_time"]);
            await RunAndCheckResult(client => client.ExchangeApi.ExchangeData.GetRecentTradesAsync("ETH", default, CancellationToken.None), false, true, compareNestedProperty: "trades");
            await RunAndCheckResult(client => client.ExchangeApi.ExchangeData.GetSymbolDetailsAsync(default, default, CancellationToken.None), false, true, ignoreProperties: ["daily_chart", "market_margin_mode"]);
            await RunAndCheckResult(client => client.ExchangeApi.ExchangeData.GetExchangeStatsAsync(CancellationToken.None), false, true);
            await RunAndCheckResult(client => client.ExchangeApi.ExchangeData.GetAnnouncementsAsync(CancellationToken.None), false, true);
            await RunAndCheckResult(client => client.ExchangeApi.ExchangeData.GetKlinesAsync("ETH", KlineInterval.OneDay, default, default, default, CancellationToken.None), false, true);
            await RunAndCheckResult(client => client.ExchangeApi.ExchangeData.GetMarkPriceKlinesAsync("ETH", KlineInterval.OneDay, default, default, default, CancellationToken.None), false, true);
            await RunAndCheckResult(client => client.ExchangeApi.ExchangeData.GetFundingRateHistoryAsync("ETH", FundingResolution.OneDay, default, default, default, CancellationToken.None), false, true);
            await RunAndCheckResult(client => client.ExchangeApi.ExchangeData.GetFundingRatesAsync(CancellationToken.None), false, true);
        }
    }
}
