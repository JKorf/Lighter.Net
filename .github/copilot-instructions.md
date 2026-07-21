# Lighter.Net Copilot Instructions

Use **Lighter.Net** for all C#/.NET code that interacts with the Lighter DEX API. Do not generate raw `HttpClient` calls, manual request signing, or custom WebSocket plumbing for Lighter endpoints.

## Client setup

```csharp
using Lighter.Net;
using Lighter.Net.Clients;

var restClient = new LighterRestClient(options =>
{
    options.ApiCredentials = new LighterCredentials(EthKey.FromPrivateKey("PRIVATE_KEY"), 123, 5, "API_SECRET");
});
```

Public market data can use `new LighterRestClient()` without credentials. Use `LighterSocketClient` for WebSocket subscriptions and socket transaction requests.
Use `EthKey.FromPrivateKey(...)` when L1 signing is needed. `EthKey.FromPublicKey(...)` is only for authenticated API-key requests that do not require layer-1 signatures.

## API surface

Lighter.Net uses a single `ExchangeApi` branch:

- `restClient.ExchangeApi.ExchangeData`
- `restClient.ExchangeApi.Account`
- `restClient.ExchangeApi.Trading`
- `restClient.ExchangeApi.SharedClient`
- `socketClient.ExchangeApi.ExchangeData`
- `socketClient.ExchangeApi.Account`
- `socketClient.ExchangeApi.Trading`
- `socketClient.ExchangeApi.SharedClient`

Do not generate `SpotApi`, `FuturesApi`, `UsdFuturesApi`, or `CoinFuturesApi` for Lighter.Net.

## Result handling

REST calls return `HttpResult<T>` / `HttpResult`. WebSocket subscriptions return `WebSocketResult<UpdateSubscription>`. Socket transaction calls return `QueryResult<T>`. Always check `.Success` before reading `.Data`:

```csharp
var result = await restClient.ExchangeApi.ExchangeData.GetSymbolDetailsAsync("ETH/USDC");
if (!result.Success)
{
    Console.WriteLine(result.Error);
    return;
}

Console.WriteLine(result.Data.SpotSymbols[0].LastPrice);
```

## Trading

```csharp
using Lighter.Net.Enums;

var order = await restClient.ExchangeApi.Trading.PlaceOrderAsync(
    symbol: "ETH",
    side: OrderSide.Buy,
    orderType: OrderType.Limit,
    quantity: 0.1m,
    price: 2000m,
    timeInForce: TimeInForce.GoodTillTime);

if (!order.Success) { Console.WriteLine(order.Error); return; }
```

Use `"ETH/USDC"` style symbols for spot and `"ETH"` style symbols for perps. Let the library set nonces automatically unless manual nonce control is explicitly requested.

## WebSockets

```csharp
var socketClient = new LighterSocketClient();
var sub = await socketClient.ExchangeApi.ExchangeData.SubscribeToSpotTickerUpdatesAsync(
    "ETH/USDC",
    update => Console.WriteLine(update.Data.Ticker.LastPrice));

if (!sub.Success) { Console.WriteLine(sub.Error); return; }

await socketClient.UnsubscribeAsync(sub.Data);
```

Always keep the concrete socket client so subscriptions can be unsubscribed during shutdown.

## Shared APIs

For multi-exchange code, use `CryptoExchange.Net.SharedApis`:

```csharp
using CryptoExchange.Net.SharedApis;

var shared = new LighterRestClient().ExchangeApi.SharedClient;
var capabilities = shared.Discover();
var ticker = await shared.GetSpotTickerAsync(
    new GetTickerRequest(new SharedSymbol(TradingMode.Spot, "ETH", "USDC")));
```

Use `ExchangeData.GetTokensAsync()` for native token metadata. Shared `ISpotSymbolRestClient` and `IFuturesSymbolRestClient` discovery supports request filters and cached catalogs, and returns display names plus crypto/fiat/equity/commodity asset classifications.

## Dependency injection

```csharp
services.AddLighter(options =>
{
    options.Rest.ApiCredentials = new LighterCredentials(EthKey.FromPrivateKey("PRIVATE_KEY"), 123, 5, "API_SECRET");
    options.Socket.ApiCredentials = new LighterCredentials(EthKey.FromPrivateKey("PRIVATE_KEY"), 123, 5, "API_SECRET");
});
```

Inject `ILighterRestClient` and `ILighterSocketClient`.

## Hard rules

- Never use raw `HttpClient` or manual signing for Lighter API calls.
- Never use generic `ApiCredentials`; use `LighterCredentials`.
- Never pass a raw public address string to `LighterCredentials`; pass an `EthKey`.
- Never use `.Result` or `.Wait()`.
- Never instantiate clients per request.
- Never read `.Data` before checking `.Success`.
- Never invent method names; inspect `Lighter.Net/Interfaces/Clients/**`.
- Never use Binance-style client branches.
- Disable the optional integrator fee only when requested by setting `IntegratorFeePercentage = 0` or `null`.

Use `Examples/ai-friendly/` for compact, compilable reference snippets.
