// 04-multi-exchange.cs
//
// Demonstrates: writing exchange-agnostic code using CryptoExchange.Net.SharedApis.
// The same pattern works against Lighter and other CryptoExchange.Net-family
// exchange libraries when the feature is supported.
//
// Setup:
//   dotnet add package JKorf.Lighter.Net
//   dotnet add package Binance.Net   // optional, for another exchange
//   dotnet add package Bybit.Net     // optional, for another exchange

using CryptoExchange.Net.SharedApis;
using Lighter.Net.Clients;

// ---- THE PATTERN ----
// Each exchange client exposes a `.SharedClient` property on its API surfaces.
// SharedClient implements interfaces like ISpotTickerRestClient, ISpotOrderRestClient,
// IBalanceRestClient, etc. - a common abstraction across exchanges.
// Call SharedClient.Discover() before routing optional shared features.
var lighterRest = new LighterRestClient();
ISpotTickerRestClient lighterShared = lighterRest.ExchangeApi.SharedClient;

var sharedInfo = lighterRest.ExchangeApi.SharedClient.Discover();
var supportedFeatures = sharedInfo.Features
    .Where(x => x.Supported)
    .Select(x => x.EndpointName);
Console.WriteLine($"{sharedInfo.Exchange} {sharedInfo.TypeName}: {string.Join(", ", supportedFeatures)}");

// To add other exchanges, install their packages and use the appropriate shared client:
//   ISpotTickerRestClient binanceShared = new BinanceRestClient().SpotApi.SharedClient;
//   ISpotTickerRestClient bybitShared   = new BybitRestClient().V5Api.SharedClient;

// SharedSymbol normalizes exchange-specific symbol formatting.
// Lighter native spot format is "ETH/USDC"; the shared layer uses base/quote assets.
var ethusdc = new SharedSymbol(TradingMode.Spot, "ETH", "USDC");

await PrintTicker(lighterShared, ethusdc);

// ---- AGNOSTIC METHOD - works against any exchange implementing ISpotTickerRestClient ----
async Task PrintTicker(ISpotTickerRestClient client, SharedSymbol symbol)
{
    var result = await client.GetSpotTickerAsync(new GetTickerRequest(symbol));
    if (!result.Success)
    {
        Console.WriteLine($"[{client.Exchange}] Failed: {result.Error}");
        return;
    }

    Console.WriteLine($"[{client.Exchange}] {result.Data.Symbol}: {result.Data.LastPrice}");
}

// ---- WEBSOCKET EXAMPLE - SHARED SUBSCRIPTION ----
var lighterSocket = new LighterSocketClient();
ITickerSocketClient lighterTickerSocket = lighterSocket.ExchangeApi.SharedClient;

var sub = await lighterTickerSocket.SubscribeToTickerUpdatesAsync(
    new SubscribeTickerRequest(ethusdc),
    update => Console.WriteLine($"[{lighterTickerSocket.Exchange}] {update.Data.Symbol}: {update.Data.LastPrice}"));

if (!sub.Success)
{
    Console.WriteLine($"Subscribe failed: {sub.Error}");
    return;
}

Console.WriteLine("Press Enter to exit");
Console.ReadLine();

// Shared socket interfaces do not expose UnsubscribeAsync. Keep the concrete
// socket client and unsubscribe through it.
await lighterSocket.UnsubscribeAsync(sub.Data);

// Common variations:
//   Multi-exchange scanners: loop over List<ISpotTickerRestClient>.
//   Perp tickers: use IFuturesTickerRestClient with TradingMode.PerpetualLinear.
//   Shared order placement: use ISpotOrderRestClient or IFuturesOrderRestClient.
//   Capability checks: inspect SharedClient.Discover().Features before using optional endpoints.
