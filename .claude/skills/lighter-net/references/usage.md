# Lighter.Net usage

## Imports

```csharp
using System;
using System.Linq;
using System.Threading.Tasks;
using CryptoExchange.Net.SharedApis;
using CryptoExchange.Net.Web3;
using Lighter.Net;
using Lighter.Net.Clients;
using Lighter.Net.Enums;
using Lighter.Net.Interfaces.Clients;
using Microsoft.Extensions.DependencyInjection;
```

## Public setup and results

```csharp
using var client = new LighterRestClient();
var details = await client.ExchangeApi.ExchangeData.GetSymbolDetailsAsync("ETH/USDC");
if (!details.Success)
{
    Console.WriteLine(details.Error);
    return;
}
var spot = details.Data.SpotSymbols.FirstOrDefault();
Console.WriteLine(spot?.LastPrice);
```

## API-key credentials and account read

API-key signing does not need an L1 private key for every operation:

```csharp
var address = Environment.GetEnvironmentVariable("LIGHTER_WALLET_ADDRESS")
    ?? throw new InvalidOperationException("Wallet address is required");
var secret = Environment.GetEnvironmentVariable("LIGHTER_API_SECRET")
    ?? throw new InvalidOperationException("API secret is required");
var accountIndex = long.Parse(Environment.GetEnvironmentVariable("LIGHTER_ACCOUNT_INDEX")
    ?? throw new InvalidOperationException("Account index is required"));
var keyIndex = int.Parse(Environment.GetEnvironmentVariable("LIGHTER_API_KEY_INDEX")
    ?? throw new InvalidOperationException("API key index is required"));
using var client = new LighterRestClient(options =>
{
    options.ApiCredentials = new LighterCredentials(
        EthKey.FromPublicAddress(address), accountIndex, keyIndex, secret);
});
var accounts = await client.ExchangeApi.Account.GetAccountsAsync();
if (!accounts.Success)
{
    Console.WriteLine(accounts.Error);
    return;
}
Console.WriteLine(accounts.Data.Total);
```

For operations requiring an L1 signature, use wallet private-key material separately from the API secret:

```csharp
var walletKey = Environment.GetEnvironmentVariable("LIGHTER_WALLET_PRIVATE_KEY")
    ?? throw new InvalidOperationException("Wallet private key is required");
var apiSecret = Environment.GetEnvironmentVariable("LIGHTER_API_SECRET")
    ?? throw new InvalidOperationException("API secret is required");
var accountIndex = long.Parse(Environment.GetEnvironmentVariable("LIGHTER_ACCOUNT_INDEX")
    ?? throw new InvalidOperationException("Account index is required"));
var keyIndex = int.Parse(Environment.GetEnvironmentVariable("LIGHTER_API_KEY_INDEX")
    ?? throw new InvalidOperationException("API key index is required"));
using var client = new LighterRestClient(options =>
{
    options.ApiCredentials = new LighterCredentials(
        EthKey.FromPrivateKey(walletKey), accountIndex, keyIndex, apiSecret);
});
```

See [signing considerations](safety.md#wallet-and-api-signing-roles) for native signer deployment and credential roles.

## Perpetual order book

```csharp
var result = await client.ExchangeApi.ExchangeData.GetOrderBookAsync("ETH", limit: 50);
if (!result.Success)
{
    Console.WriteLine(result.Error);
    return;
}
foreach (var bid in result.Data.Bids.Take(5))
    Console.WriteLine($"{bid.Price}: {bid.Quantity}");
```

## Live order submission

This assumes a configured authenticated client and submits a real transaction. Quantity is in base assets. Read [transaction considerations](safety.md#nonces-and-transaction-outcomes) before executing.

```csharp
var order = await client.ExchangeApi.Trading.PlaceOrderAsync(
    symbol: "ETH", side: OrderSide.Buy, orderType: OrderType.Limit,
    quantity: 0.01m, price: 2000m, timeInForce: TimeInForce.GoodTillTime);
if (!order.Success)
{
    Console.WriteLine(order.Error);
    return;
}
Console.WriteLine($"Submitted transaction: {order.Data.TransactionHash}");
```

Order queries require checking their own result; do not use the transaction hash as an order index:

```csharp
var orders = await client.ExchangeApi.Trading.GetOpenOrdersAsync(symbol: "ETH", marketType: MarketType.Perps);
if (!orders.Success)
{
    Console.WriteLine(orders.Error);
    return;
}
foreach (var order in orders.Data.Orders)
    Console.WriteLine($"{order.OrderIndex}: {order.Status}");
```

## Public WebSocket lifecycle

```csharp
using var socketClient = new LighterSocketClient();
var subscription = await socketClient.ExchangeApi.ExchangeData.SubscribeToSpotTickerUpdatesAsync(
    "ETH/USDC", update => Console.WriteLine(update.Data.Ticker.LastPrice));
if (!subscription.Success)
{
    Console.WriteLine(subscription.Error);
    return;
}
try
{
    await Task.Delay(TimeSpan.FromSeconds(30));
}
finally
{
    await socketClient.UnsubscribeAsync(subscription.Data);
}
```

Authenticated streams use credentialed socket options with the same credential shape. Passing null for an optional account index selects the configured account where documented. Socket transaction methods return `QueryResult<T>` rather than subscription handles; apply the same success check and reconciliation as REST transactions.

## Shared API V2

```csharp
IGetTickerRest ticker = client.ExchangeApi.SharedApi;
var result = await ticker.GetTickerAsync(new GetTickerRequest(
    new SharedSymbol(TradingMode.Spot, "ETH", "USDC")));
if (!result.Success)
{
    Console.WriteLine(result.Error);
    return;
}
Console.WriteLine(result.Data.LastPrice);
```

Load symbols before reading the catalog:

```csharp
IGetFuturesSymbolsRest symbolsClient = client.ExchangeApi.SharedApi;
var result = await symbolsClient.GetFuturesSymbolsAsync(new GetSymbolsRequest());
if (!result.Success)
{
    Console.WriteLine(result.Error);
    return;
}
foreach (var symbol in result.Data.Take(5))
    Console.WriteLine($"{symbol.Name}: {symbol.BaseAsset}/{symbol.QuoteAsset}");
var catalog = symbolsClient.FuturesSymbolCatalog;
```

## Dependency injection and dynamic lookup

```csharp
var services = new ServiceCollection();
services.AddLighter();
using var provider = services.BuildServiceProvider();
var aggregate = provider.GetRequiredService<ILighterSharedApiClient>();
var match = aggregate.GetCapability(
    SharedCapabilities.Tickers.GetTicker.Rest, tradingMode: TradingMode.Spot);
if (match is null)
    return;
var result = await match.Capability.GetTickerAsync(new GetTickerRequest(
    new SharedSymbol(TradingMode.Spot, "ETH", "USDC")));
if (!result.Success)
{
    Console.WriteLine(result.Error);
    return;
}
Console.WriteLine(result.Data.LastPrice);
```

Configure `options.ApiCredentials` for authenticated consumers, or `options.Rest.ApiCredentials` and `options.Socket.ApiCredentials` independently. Inject `ILighterRestClient`, `ILighterSocketClient`, the aggregate, or registered narrow capabilities. [API routing](api-surfaces.md#shared-api-v2) describes selection when several exchanges are registered.

## Complete programs (checkout-only)

[Examples](../../../../Examples/ai-friendly/README.md) contains independent public/account, trading, socket, Shared API V2, and public-read retry programs. These links require a checkout; the snippets above work with the installed package.
