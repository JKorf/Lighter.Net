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
            string pk = Environment.GetEnvironmentVariable("PUBLICKEY");
            int accountIndex = int.Parse(Environment.GetEnvironmentVariable("ACCOUNTINDEX") ?? "0");
            int apiKeyIndex = int.Parse(Environment.GetEnvironmentVariable("APIKEYINDEX") ?? "0");
            string sec = Environment.GetEnvironmentVariable("APISECRET");

            Authenticated = pk != null && sec != null;
            return new LighterRestClient(null, loggerFactory, Options.Create(new LighterRestOptions
            {
                AutoTimestamp = false,
                OutputOriginalData = true,
                ApiCredentials = pk == null ? null : new LighterCredentials(
                    EthKey.FromPublicKey(pk),
                    accountIndex: accountIndex,
                    apiKeyIndex: apiKeyIndex,
                    apiSecret: sec
                    )
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

            var result = await client.ExchangeApi.ExchangeData.GetKlinesAsync("TSTTST", (KlineInterval)123);

            Assert.That(result.Success, Is.False);
            Assert.That(result.Error.Code, Is.EqualTo(20001));
        }

        [Test]
        public async Task TestAccount()
        {
            await RunAndCheckResult(client => client.ExchangeApi.Account.GetAccountsByL1AddressAsync(default, default, CancellationToken.None), true, true);
            await RunAndCheckResult(client => client.ExchangeApi.Account.GetNonceAsync(default, default, CancellationToken.None), true, true);
            await RunAndCheckResult(client => client.ExchangeApi.Account.GetAccountsAsync(default, default, default, default, CancellationToken.None), true, true, ignoreProperties: ["code"]);
            await RunAndCheckResult(client => client.ExchangeApi.Account.GetAccountLimitsAsync(default, CancellationToken.None), true, true);
            await RunAndCheckResult(client => client.ExchangeApi.Account.GetAccountMetadataAsync(default, default, default, CancellationToken.None), true, true);
            await RunAndCheckResult(client => client.ExchangeApi.Account.GetPnlAsync(Resolution.OneHour, default, default, default, default, default, default, CancellationToken.None), true, true);
            await RunAndCheckResult(client => client.ExchangeApi.Account.GetLiquidationsAsync(default, default, default, default, CancellationToken.None), true, true);
            await RunAndCheckResult(client => client.ExchangeApi.Account.GetFundingHistoryAsync(default, default, default, default, default, default, default, CancellationToken.None), true, true);
            await RunAndCheckResult(client => client.ExchangeApi.Account.GetDepositHistoryAsync(default, default, default, default, CancellationToken.None), true, true);
            await RunAndCheckResult(client => client.ExchangeApi.Account.GetTransferHistoryAsync(default, default, default, CancellationToken.None), true, true);
            await RunAndCheckResult(client => client.ExchangeApi.Account.GetWithdrawHistoryAsync(default, default, default, CancellationToken.None), true, true);
            await RunAndCheckResult(client => client.ExchangeApi.Account.GetApiKeysAsync(default, default, CancellationToken.None), true, true);
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

        [Test]
        public async Task TestTrading()
        {
            await RunAndCheckResult(client => client.ExchangeApi.Trading.GetOpenOrdersAsync(default, default, default, CancellationToken.None), true, true);
            await RunAndCheckResult(client => client.ExchangeApi.Trading.GetClosedOrdersAsync(default, default, default, default, default, default, CancellationToken.None), true, true);
            await RunAndCheckResult(client => client.ExchangeApi.Trading.GetUserTradesAsync(default, default, default, default, default, default, default, default, default, default, default, default, CancellationToken.None), true, true,
                 ignoreProperties: ["trade_id_str", "ask_id_str", "bid_id_str", "ask_client_id_str", "bid_client_id_str"]);
        }
    }
}
