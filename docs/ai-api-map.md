# Lighter.Net AI API Map

Use this map when generating C#/.NET code for Lighter with `JKorf.Lighter.Net`. Prefer the typed clients and shared API interfaces instead of raw HTTP or custom WebSocket code.

## Setup

| Intent | Use |
| --- | --- |
| REST client | `new LighterRestClient()` |
| WebSocket client | `new LighterSocketClient()` |
| Authenticated REST/socket calls | `options.ApiCredentials = new LighterCredentials(EthKey.FromPrivateKey("PRIVATE_KEY"), accountIndex, apiKeyIndex, "API_SECRET")` |
| Auth without L1 signing | `EthKey.FromPublicKey("PUBLIC_ADDRESS")` |
| Dependency injection | `services.AddLighter(...)`, then inject `ILighterRestClient` / `ILighterSocketClient` |
| Disable optional integrator fee | `IntegratorFeePercentage = 0m` or `null` |

## REST Exchange Data

| Intent | Method |
| --- | --- |
| Platform status | `restClient.ExchangeApi.ExchangeData.GetStatusAsync()` |
| System config | `restClient.ExchangeApi.ExchangeData.GetSystemConfigAsync()` |
| Symbols | `restClient.ExchangeApi.ExchangeData.GetSymbolsAsync(...)` |
| Assets | `restClient.ExchangeApi.ExchangeData.GetAssetsAsync(...)` |
| Order book | `restClient.ExchangeApi.ExchangeData.GetOrderBookAsync(symbol, limit)` |
| Recent trades | `restClient.ExchangeApi.ExchangeData.GetRecentTradesAsync(symbol, limit)` |
| Ticker / symbol details | `restClient.ExchangeApi.ExchangeData.GetSymbolDetailsAsync(symbol)` |
| Klines | `restClient.ExchangeApi.ExchangeData.GetKlinesAsync(symbol, interval, ...)` |
| Mark-price klines | `restClient.ExchangeApi.ExchangeData.GetMarkPriceKlinesAsync(symbol, interval, ...)` |
| Funding history | `restClient.ExchangeApi.ExchangeData.GetFundingRateHistoryAsync(symbol, resolution, ...)` |
| Current funding rates | `restClient.ExchangeApi.ExchangeData.GetFundingRatesAsync()` |

## REST Account And Trading

| Intent | Method |
| --- | --- |
| Generate API key pair locally | `restClient.ExchangeApi.Account.GenerateApiKey()` |
| Get nonce | `restClient.ExchangeApi.Account.GetNonceAsync(...)` |
| Accounts and metadata | `restClient.ExchangeApi.Account.GetAccountsAsync(...)`, `GetAccountMetadataAsync(...)` |
| PnL / liquidations / funding | `GetPnlAsync(...)`, `GetLiquidationsAsync(...)`, `GetFundingHistoryAsync(...)` |
| Deposits / withdrawals / transfers | `GetDepositHistoryAsync(...)`, `GetWithdrawHistoryAsync(...)`, `GetTransferHistoryAsync(...)` |
| Set leverage | `restClient.ExchangeApi.Account.SetLeverageAsync(symbol, leverage, marginMode)` |
| Update margin | `restClient.ExchangeApi.Account.UpdateMarginAsync(symbol, usdcAmount, addOrRemove, nonce)` |
| Approve integrator | `restClient.ExchangeApi.Account.ApproveIntegratorAsync(...)`; use `EthKey.FromPrivateKey(...)` for L1 signing |
| Place order | `restClient.ExchangeApi.Trading.PlaceOrderAsync(...)` |
| Edit order | `restClient.ExchangeApi.Trading.EditOrderAsync(...)` |
| Cancel order | `restClient.ExchangeApi.Trading.CancelOrderAsync(symbol, orderIndex)` |
| Cancel all orders | `restClient.ExchangeApi.Trading.CancelAllOrdersAsync(timeInForce)` |
| Open / closed orders | `GetOpenOrdersAsync(...)`, `GetClosedOrdersAsync(...)` |
| User trades | `restClient.ExchangeApi.Trading.GetUserTradesAsync(...)` |

## WebSocket

| Intent | Method |
| --- | --- |
| Public trades | `socketClient.ExchangeApi.ExchangeData.SubscribeToTradeUpdatesAsync(symbol, handler)` |
| Order book | `socketClient.ExchangeApi.ExchangeData.SubscribeToOrderBookUpdatesAsync(symbol, handler)` |
| Book ticker | `socketClient.ExchangeApi.ExchangeData.SubscribeToBookTickerUpdatesAsync(symbol, handler)` |
| Spot tickers | `socketClient.ExchangeApi.ExchangeData.SubscribeToSpotTickerUpdatesAsync(...)` |
| Futures tickers | `socketClient.ExchangeApi.ExchangeData.SubscribeToFuturesTickerUpdatesAsync(...)` |
| Klines | `socketClient.ExchangeApi.ExchangeData.SubscribeToKlineUpdatesAsync(symbol, interval, handler)` |
| Account / balance updates | `socketClient.ExchangeApi.Account.SubscribeToAccountUpdatesAsync(...)`, `SubscribeToBalanceUpdatesAsync(...)` |
| Order / trade / position updates | `socketClient.ExchangeApi.Trading.SubscribeToOrderUpdatesAsync(...)`, `SubscribeToUserTradeUpdatesAsync(...)`, `SubscribeToPositionUpdatesAsync(...)` |
| Socket order transactions | `socketClient.ExchangeApi.Trading.PlaceOrderAsync(...)`, `EditOrderAsync(...)`, `CancelOrderAsync(...)` |
| Shutdown | Store `UpdateSubscription` and call `socketClient.UnsubscribeAsync(subscription.Data)` |

## Shared APIs

Call `client.ExchangeApi.SharedClient.Discover()` before routing optional multi-exchange features.

| Intent | Shared API |
| --- | --- |
| Spot ticker | `ISpotTickerRestClient.GetSpotTickerAsync(...)` |
| Futures ticker | `IFuturesTickerRestClient.GetFuturesTickerAsync(...)` |
| Order book | `IOrderBookRestClient.GetOrderBookAsync(...)` |
| Recent trades | `IRecentTradeRestClient.GetRecentTradesAsync(...)` |
| Balances | `IBalanceRestClient.GetBalancesAsync(...)` |
| Spot/futures orders | `ISpotOrderRestClient`, `IFuturesOrderRestClient` |
| Funding-rate history | `IFundingRateRestClient.GetFundingRateHistoryAsync(...)` |
| Shared sockets | `ITickerSocketClient`, `ITickersSocketClient`, `ITradeSocketClient`, `IBookTickerSocketClient`, `IKlineSocketClient`, `IBalanceSocketClient`, order/user-trade/position socket interfaces |

## Result Rules

REST methods return `HttpResult<T>` / `HttpResult`. WebSocket subscriptions return `WebSocketResult<UpdateSubscription>`. WebSocket request/transaction methods return `QueryResult<T>`. Always check `.Success` before reading `.Data`.
