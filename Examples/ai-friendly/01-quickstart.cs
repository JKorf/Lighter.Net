// 01-quickstart.cs
//
// Demonstrates: client setup, public market data, authenticated account calls,
// and the safe shape for placing an order.
//
// Setup:
//   dotnet new console -n LighterQuickstart && cd LighterQuickstart
//   dotnet add package JKorf.Lighter.Net
//   Copy this file content into Program.cs
//   Substitute PUBLIC_ADDRESS / ACCOUNT_INDEX / API_KEY_INDEX / API_SECRET below
//   dotnet run

using Lighter.Net;
using Lighter.Net.Clients;
using Lighter.Net.Enums;

// ---- 1. PUBLIC CLIENT (no credentials needed for market data) ----
// Reuse this client across the application. Do not create a new client per request.
var publicClient = new LighterRestClient();

// Lighter uses symbols such as "ETH/USDC" for spot and "ETH" for perps.
var symbolDetails = await publicClient.ExchangeApi.ExchangeData.GetSymbolDetailsAsync("ETH/USDC");
if (!symbolDetails.Success)
{
    // .Error contains Code, Message, ErrorType, and IsTransient.
    Console.WriteLine($"Failed to get symbol details: {symbolDetails.Error}");
    return;
}

var spotSymbol = symbolDetails.Data.SpotSymbols.FirstOrDefault();
if (spotSymbol != null)
    Console.WriteLine($"ETH/USDC last price: {spotSymbol.LastPrice}");

var orderBook = await publicClient.ExchangeApi.ExchangeData.GetOrderBookAsync("ETH/USDC", limit: 10);
if (!orderBook.Success)
{
    Console.WriteLine($"Failed to get order book: {orderBook.Error}");
    return;
}

Console.WriteLine($"Order book snapshot received: {orderBook.Data}");

// ---- 2. AUTHENTICATED CLIENT (for account / trading) ----
// Lighter credentials include the L1 public address, account index, API key index,
// and API secret. Keep secrets out of source control in real applications.
var tradingClient = new LighterRestClient(options =>
{
    options.ApiCredentials = new LighterCredentials(
        publicAddress: "PUBLIC_ADDRESS",
        accountIndex: 123,
        apiKeyIndex: 5,
        apiSecret: "API_SECRET");
});

var accounts = await tradingClient.ExchangeApi.Account.GetAccountsAsync();
if (!accounts.Success)
{
    Console.WriteLine($"Failed to get accounts: {accounts.Error}");
    return;
}

Console.WriteLine($"Account response received: {accounts.Data}");

// ---- 3. PLACE A LIMIT BUY ORDER ----
// This is real trading code. Keep it disabled until credentials and parameters
// are intentionally configured for your account.
var placeRealOrder = false;
if (placeRealOrder)
{
    var order = await tradingClient.ExchangeApi.Trading.PlaceOrderAsync(
        symbol: "ETH",
        side: OrderSide.Buy,
        orderType: OrderType.Limit,
        quantity: 0.01m,
        price: 2000m,
        timeInForce: TimeInForce.GoodTillTime);

    if (!order.Success)
    {
        Console.WriteLine($"Failed to place order: {order.Error}");
        return;
    }

    Console.WriteLine($"Order transaction result: {order.Data}");
}

// Common variations:
//   Public status:       publicClient.ExchangeApi.ExchangeData.GetStatusAsync()
//   Recent trades:       publicClient.ExchangeApi.ExchangeData.GetRecentTradesAsync("ETH/USDC", 50)
//   Perp ticker/details: publicClient.ExchangeApi.ExchangeData.GetSymbolDetailsAsync("ETH")
//   Open orders:         tradingClient.ExchangeApi.Trading.GetOpenOrdersAsync(symbol: "ETH")
//   Disable integrator fee: options.IntegratorFeePercentage = 0m
