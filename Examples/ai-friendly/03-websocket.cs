// 03-websocket.cs
//
// Demonstrates: WebSocket subscriptions - public ticker/trade/kline streams
// and authenticated account/order streams. Includes proper teardown.
//
// Setup:
//   dotnet add package JKorf.Lighter.Net

using Lighter.Net;
using Lighter.Net.Clients;
using Lighter.Net.Enums;

// ---- 1. PUBLIC SOCKET CLIENT - for market data streams ----
// Reuse a single client instance across subscriptions. The client handles
// connection management and automatic reconnects.
var publicSocket = new LighterSocketClient();

var tickerSub = await publicSocket.ExchangeApi.ExchangeData.SubscribeToSpotTickerUpdatesAsync(
    "ETH/USDC",
    update =>
    {
        // Keep handlers fast. Offload heavy processing to a queue/channel.
        Console.WriteLine($"ETH/USDC last price: {update.Data.Ticker.LastPrice}");
    });

if (!tickerSub.Success)
{
    Console.WriteLine($"Failed to subscribe spot ticker: {tickerSub.Error}");
    return;
}

var tradeSub = await publicSocket.ExchangeApi.ExchangeData.SubscribeToTradeUpdatesAsync(
    "ETH/USDC",
    update => Console.WriteLine($"Trade update: {update.Data}"));

if (!tradeSub.Success)
{
    Console.WriteLine($"Failed to subscribe trades: {tradeSub.Error}");
    await publicSocket.UnsubscribeAsync(tickerSub.Data);
    return;
}

var klineSub = await publicSocket.ExchangeApi.ExchangeData.SubscribeToKlineUpdatesAsync(
    "ETH/USDC",
    KlineInterval.OneMinute,
    update => Console.WriteLine($"Kline update: {update.Data}"));

if (!klineSub.Success)
{
    Console.WriteLine($"Failed to subscribe klines: {klineSub.Error}");
    await publicSocket.UnsubscribeAsync(tickerSub.Data);
    await publicSocket.UnsubscribeAsync(tradeSub.Data);
    return;
}

// ---- 2. AUTHENTICATED SOCKET CLIENT - for user data ----
var authSocket = new LighterSocketClient(options =>
{
    options.ApiCredentials = new LighterCredentials(EthKey.FromPrivateKey("PRIVATE_KEY"), 123, 5, "API_SECRET");
});

var accountSub = await authSocket.ExchangeApi.Account.SubscribeToAccountUpdatesAsync(
    accountIndex: null,
    update => Console.WriteLine($"Account update: {update.Data}"));

if (!accountSub.Success)
{
    Console.WriteLine($"Failed to subscribe account updates: {accountSub.Error}");
    await publicSocket.UnsubscribeAsync(tickerSub.Data);
    await publicSocket.UnsubscribeAsync(tradeSub.Data);
    await publicSocket.UnsubscribeAsync(klineSub.Data);
    return;
}

var orderSub = await authSocket.ExchangeApi.Trading.SubscribeToOrderUpdatesAsync(
    accountIndex: null,
    update => Console.WriteLine($"Order update: {update.Data}"));

if (!orderSub.Success)
{
    Console.WriteLine($"Failed to subscribe order updates: {orderSub.Error}");
    await publicSocket.UnsubscribeAsync(tickerSub.Data);
    await publicSocket.UnsubscribeAsync(tradeSub.Data);
    await publicSocket.UnsubscribeAsync(klineSub.Data);
    await authSocket.UnsubscribeAsync(accountSub.Data);
    return;
}

Console.WriteLine("All subscriptions active. Press Enter to teardown...");
Console.ReadLine();

// ---- 3. TEARDOWN - IMPORTANT ----
await publicSocket.UnsubscribeAsync(tickerSub.Data);
await publicSocket.UnsubscribeAsync(tradeSub.Data);
await publicSocket.UnsubscribeAsync(klineSub.Data);
await authSocket.UnsubscribeAsync(accountSub.Data);
await authSocket.UnsubscribeAsync(orderSub.Data);

Console.WriteLine("Clean shutdown complete.");

// Common variations:
//   Perp ticker:             SubscribeToFuturesTickerUpdatesAsync("ETH", handler)
//   All spot tickers:        SubscribeToSpotTickerUpdatesAsync(handler)
//   Order book stream:       SubscribeToOrderBookUpdatesAsync("ETH/USDC", handler)
//   Book ticker stream:      SubscribeToBookTickerUpdatesAsync("ETH/USDC", handler)
//   Balance updates:         authSocket.ExchangeApi.Account.SubscribeToBalanceUpdatesAsync(null, handler)
//   Position updates:        authSocket.ExchangeApi.Trading.SubscribeToPositionUpdatesAsync(null, handler)
