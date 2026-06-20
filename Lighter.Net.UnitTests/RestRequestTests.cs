using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Testing;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lighter.Net.Clients;
using Lighter.Net.Enums;
using Lighter.Net.Objects;

namespace Lighter.Net.UnitTests
{
    [TestFixture]
    public class RestRequestTests
    {

        [Test]
        public async Task ValidateAccountCalls()
        {
            var client = new LighterRestClient(opts =>
            {
                opts.AutoTimestamp = false;
                opts.ApiCredentials = new LighterCredentials("123", 0, 1, "456");
                opts.Environment = LighterEnvironment.CreateCustom("UnitTest", LighterApiAddresses.Default.RestClientAddress, LighterApiAddresses.Default.SocketClientAddress);
            });
            var tester = new RestRequestValidator<LighterRestClient>(client, "Endpoints/Exchange/Account", "https://mainnet.zklighter.elliot.ai", IsAuthenticated);
            await tester.ValidateAsync(client => client.ExchangeApi.Account.GetAccountsAsync(AccountBy.AccountIndex, "123"), "GetAccounts");
            await tester.ValidateAsync(client => client.ExchangeApi.Account.GetAccountLimitsAsync(), "GetAccountLimits");
            await tester.ValidateAsync(client => client.ExchangeApi.Account.GetPnlAsync(Resolution.OneMinute), "GetPnl");
            await tester.ValidateAsync(client => client.ExchangeApi.Account.SetAccountTierAsync(null, AccountTier.Standard), "SetAccountTier");
            await tester.ValidateAsync(client => client.ExchangeApi.Account.GetLiquidationsAsync(), "GetLiquidations");
            await tester.ValidateAsync(client => client.ExchangeApi.Account.GetFundingHistoryAsync(), "GetFundingHistory");
            await tester.ValidateAsync(client => client.ExchangeApi.Account.GetDepositHistoryAsync(), "GetDepositHistory");
            await tester.ValidateAsync(client => client.ExchangeApi.Account.GetWithdrawHistoryAsync(), "GetWithdrawHistory");
            await tester.ValidateAsync(client => client.ExchangeApi.Account.GetApiKeysAsync(), "GetApiKeys");
        }

        [Test]
        public async Task ValidateExchangeDataCalls()
        {
            var client = new LighterRestClient(opts =>
            {
                opts.AutoTimestamp = false;
                opts.Environment = LighterEnvironment.CreateCustom("UnitTest", LighterApiAddresses.Default.RestClientAddress, LighterApiAddresses.Default.SocketClientAddress);
            });
            var tester = new RestRequestValidator<LighterRestClient>(client, "Endpoints/Exchange/ExchangeData", "https://mainnet.zklighter.elliot.ai", IsAuthenticated);
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetSystemConfigAsync(), "GetSystemConfig");
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetSymbolsAsync(), "GetSymbols", nestedJsonProperty: "order_books");
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetLayer1BasicInfoAsync(), "GetLayer1BasicInfo");
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetAssetsAsync(), "GetAssets", "asset_details");
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetOrderBookAsync("ETH"), "GetOrderBook", ignoreProperties: ["transaction_time"]);
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetRecentTradesAsync("ETH"), "GetRecentTrades", "trades", ["daily_chart"]);
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetSymbolDetailsAsync(), "GetSymbolDetails", ignoreProperties: ["daily_chart"]);
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetExchangeStatsAsync(), "GetExchangeStats");
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetAnnouncementsAsync(), "GetAnnouncements");
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetKlinesAsync("ETHUSDT", KlineInterval.OneMinute), "GetKlines", ignoreProperties: ["C", "H", "L", "O"]);
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetFundingRateHistoryAsync("ETH/USDC", FundingResolution.OneDay), "GetFundingRateHistory");
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetFundingRatesAsync(), "GetFundingRates");
        }

        [Test]
        public async Task ValidateTradingCalls()
        {
            var client = new LighterRestClient(opts =>
            {
                opts.AutoTimestamp = false;
                opts.ApiCredentials = new LighterCredentials("123", 0, 1, "456");
                opts.Environment = LighterEnvironment.CreateCustom("UnitTest", LighterApiAddresses.Default.RestClientAddress, LighterApiAddresses.Default.SocketClientAddress);
            });
            var tester = new RestRequestValidator<LighterRestClient>(client, "Endpoints/Exchange/Trading", "https://mainnet.zklighter.elliot.ai", IsAuthenticated);
            await tester.ValidateAsync(client => client.ExchangeApi.Trading.GetOpenOrdersAsync(), "GetOpenOrders");
            await tester.ValidateAsync(client => client.ExchangeApi.Trading.GetClosedOrdersAsync(), "GetClosedOrders");
            await tester.ValidateAsync(client => client.ExchangeApi.Trading.GetUserTradesAsync(), "GetUserTrades", ignoreProperties: ["ask_client_id_str", "bid_client_id_str", "ask_id_str", "bid_id_str", "trade_id_str"]);
        }

        private bool IsAuthenticated(IHttpResult result)
        {
            return result.RequestUrl?.Contains("auth") == true;
        }
    }
}
