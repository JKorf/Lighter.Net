# Lighter.Net API surfaces

## Client roots

`LighterRestClient` / `ILighterRestClient` and `LighterSocketClient` / `ILighterSocketClient` expose one `ExchangeApi`, with `ExchangeData`, `Account`, and `Trading`. Both Spot and perpetual workflows use this root. Socket trading/account methods include signed transaction requests in addition to subscriptions.

## REST Exchange Data

| Intent | Method |
| --- | --- |
| Platform status | `client.ExchangeApi.ExchangeData.GetStatusAsync()` |
| System config | `client.ExchangeApi.ExchangeData.GetSystemConfigAsync()` |
| Symbols | `client.ExchangeApi.ExchangeData.GetSymbolsAsync(...)` |
| Assets | `client.ExchangeApi.ExchangeData.GetAssetsAsync(...)` |
| Token metadata and categories | `client.ExchangeApi.ExchangeData.GetTokensAsync()` |
| Order book | `client.ExchangeApi.ExchangeData.GetOrderBookAsync(symbol, limit)` |
| Recent trades | `client.ExchangeApi.ExchangeData.GetRecentTradesAsync(symbol, limit)` |
| Ticker / symbol details | `client.ExchangeApi.ExchangeData.GetSymbolDetailsAsync(symbol)` |
| Klines | `client.ExchangeApi.ExchangeData.GetKlinesAsync(symbol, interval, ...)` |
| Mark-price klines | `client.ExchangeApi.ExchangeData.GetMarkPriceKlinesAsync(symbol, interval, ...)` |
| Funding history | `client.ExchangeApi.ExchangeData.GetFundingRateHistoryAsync(symbol, resolution, ...)` |
| Current funding rates | `client.ExchangeApi.ExchangeData.GetFundingRatesAsync()` |

## REST Account And Trading

| Intent | Method |
| --- | --- |
| Generate API key pair locally | `client.ExchangeApi.Account.GenerateApiKey()` |
| Get nonce | `client.ExchangeApi.Account.GetNonceAsync(...)` |
| Accounts and metadata | `client.ExchangeApi.Account.GetAccountsAsync(...)`, `client.ExchangeApi.Account.GetAccountMetadataAsync(...)` |
| PnL / liquidations / funding | `client.ExchangeApi.Account.GetPnlAsync(...)`, `client.ExchangeApi.Account.GetLiquidationsAsync(...)`, `client.ExchangeApi.Account.GetFundingHistoryAsync(...)` |
| Deposits / withdrawals / transfers | `client.ExchangeApi.Account.GetDepositHistoryAsync(...)`, `client.ExchangeApi.Account.GetWithdrawHistoryAsync(...)`, `client.ExchangeApi.Account.GetTransferHistoryAsync(...)` |
| Set leverage | `client.ExchangeApi.Account.SetLeverageAsync(symbol, leverage, marginMode)` |
| Update margin | `client.ExchangeApi.Account.UpdateMarginAsync(symbol, usdcAmount, addOrRemove, nonce)` |
| Approve integrator | `client.ExchangeApi.Account.ApproveIntegratorAsync(...)`; use `EthKey.FromPrivateKey(...)` for L1 signing |
| Place order | `client.ExchangeApi.Trading.PlaceOrderAsync(...)` |
| Edit order | `client.ExchangeApi.Trading.EditOrderAsync(...)` |
| Cancel order | `client.ExchangeApi.Trading.CancelOrderAsync(symbol, orderIndex)` |
| Cancel all orders | `client.ExchangeApi.Trading.CancelAllOrdersAsync(timeInForce)` |
| Open / closed orders | `client.ExchangeApi.Trading.GetOpenOrdersAsync(...)`, `client.ExchangeApi.Trading.GetClosedOrdersAsync(...)` |
| User trades | `client.ExchangeApi.Trading.GetUserTradesAsync(...)` |

## WebSocket

| Intent | Method |
| --- | --- |
| Public trades | `socketClient.ExchangeApi.ExchangeData.SubscribeToTradeUpdatesAsync(symbol, handler)` |
| Order book | `socketClient.ExchangeApi.ExchangeData.SubscribeToOrderBookUpdatesAsync(symbol, handler)` |
| Book ticker | `socketClient.ExchangeApi.ExchangeData.SubscribeToBookTickerUpdatesAsync(symbol, handler)` |
| Spot tickers | `socketClient.ExchangeApi.ExchangeData.SubscribeToSpotTickerUpdatesAsync(...)` |
| Futures tickers | `socketClient.ExchangeApi.ExchangeData.SubscribeToFuturesTickerUpdatesAsync(...)` |
| Klines | `socketClient.ExchangeApi.ExchangeData.SubscribeToKlineUpdatesAsync(symbol, interval, handler)` |
| Account / balance updates | `socketClient.ExchangeApi.Account.SubscribeToAccountUpdatesAsync(...)`, `socketClient.ExchangeApi.Account.SubscribeToBalanceUpdatesAsync(...)` |
| Order / trade / position updates | `socketClient.ExchangeApi.Trading.SubscribeToOrderUpdatesAsync(...)`, `socketClient.ExchangeApi.Trading.SubscribeToUserTradeUpdatesAsync(...)`, `socketClient.ExchangeApi.Trading.SubscribeToPositionUpdatesAsync(...)` |
| Socket order transactions | `socketClient.ExchangeApi.Trading.PlaceOrderAsync(...)`, `socketClient.ExchangeApi.Trading.EditOrderAsync(...)`, `socketClient.ExchangeApi.Trading.CancelOrderAsync(...)` |
| Shutdown | Store `UpdateSubscription` and call `socketClient.UnsubscribeAsync(subscription.Data)` |

## Shared API V2

`ILighterSharedApiClient` exposes these aggregate properties. The native REST/socket `ExchangeApi` surfaces expose the corresponding `.SharedApi`.

- `Rest` (`ILighterRestClientExchangeSharedApi`): `IGetKlinesRest`, `IGetSpotSymbolsRest`, `IGetFuturesSymbolsRest`, `IGetTickerRest`, `IGetAllTickersRest`, `IGetBookTickerRest`, `IGetRecentTradesRest`, `IGetOrderBookRest`, `IGetAssetRest`, `IGetAllAssetsRest`, `IGetDepositHistoryRest`, `IGetWithdrawalHistoryRest`, `IGetFeesRest`, `IGetBalancesRest`, `IPlaceSpotOrderRest`, `IGetSpotOrderRest`, `IGetOpenSpotOrdersRest`, `IGetClosedSpotOrdersRest`, `IGetSpotUserTradeHistoryRest`, `IGetSpotOrderTradesRest`, `ICancelSpotOrderRest`, `IGetSpotOrderByClientOrderIdRest`, `ICancelSpotOrderByClientOrderIdRest`, `IGetLeverageRest`, `ISetLeverageRest`, `IGetOpenInterestRest`, `IPlaceFuturesOrderRest`, `IGetFuturesOrderRest`, `IGetOpenFuturesOrdersRest`, `IGetClosedFuturesOrdersRest`, `IGetFuturesOrderTradesRest`, `IGetFuturesUserTradeHistoryRest`, `ICancelFuturesOrderRest`, `IGetPositionsRest`, `IGetFuturesOrderByClientOrderIdRest`, `ICancelFuturesOrderByClientOrderIdRest`, `IGetFundingRateHistoryRest`.
- `Socket` (`ILighterSocketClientExchangeSharedApi`): `ISubscribeAllTickersSocket`, `ISubscribeTickerSocket`, `ISubscribeTradesSocket`, `ISubscribeBookTickerSocket`, `ISubscribeKlinesSocket`, `ISubscribeBalancesSocket`, `ISubscribeSpotOrdersSocket`, `ISubscribeFuturesOrdersSocket`, `ISubscribeUserTradesSocket`, `ISubscribePositionsSocket`, `IPlaceSpotOrderSocket`, `ICancelSpotOrderSocket`, `IPlaceFuturesOrderSocket`, `ICancelFuturesOrderSocket`.

For known workflows use aggregate properties or inject narrow capabilities. For dynamic selection call `aggregate.GetCapability(SharedCapabilities.Tickers.GetTicker.Rest, tradingMode: TradingMode.Spot)` and check for null before using the match's `Capability` and `Options`. Descriptors identify operations, not guarantees of support. `GetCapabilities` enumerates matches and `Discover` summarizes metadata. With several exchanges registered, inject `IEnumerable<TCapability>` and select by exchange, supported trading mode, and options.

### Shared order prerequisites

Shared Spot/Futures place-order requests require `Price` even for market orders. Market-order `Price` is a current reference: the adapter applies a 5% buy/sell slippage bound. Client order IDs must be numeric strings accepted as a signed 64-bit integer. Successful REST/socket placement returns `SharedId` with null `Id`; retain the submitted client order index and resolve the exchange order index through queries/updates. Do not pass a null placement ID to cancellation. REST `ClosePositionAsync` also returns a null `SharedId.Id` and uses its required `price` exchange parameter as the slippage reference; reconcile remaining exposure through positions and order/trade updates.

## Symbols, catalogs, and quantities

Spot symbols use `ETH/USDC`; perpetuals use `ETH`. `SharedSymbol(TradingMode.Spot, "ETH", "USDC")` or `SharedSymbol(TradingMode.PerpetualLinear, "ETH", "USDC")` distinguishes products that share one native client root. Inspect the correct `SpotSymbols` or `PerpSymbols` results from native metadata. `GetTokensAsync()` supplies additional asset/category metadata.

Call `IGetSpotSymbolsRest.GetSpotSymbolsAsync(new GetSymbolsRequest())` or `IGetFuturesSymbolsRest.GetFuturesSymbolsAsync(new GetSymbolsRequest())`, check success, then read `SpotSymbolCatalog` or `FuturesSymbolCatalog`. Requests honor base/quote asset type/subtype filters. Returned metadata includes display names and crypto/stablecoin/fiat/TradFi equity/commodity classifications where populated; use it rather than guessing asset categories from names.

Native order quantity and Shared order/book quantities use base assets, including perpetuals. Shared sizing uses `SharedOrderQuantity.QuantityInBaseAsset` and books identify `SharedQuantityType.BaseAsset`. Do not replace these with integer contract counts. Contract metadata includes multiplier, price/quantity precision, and minimum trade constraints; the signer converts values into the required integer wire representation.

## Signer, factories, and checkout-only sources

Signer-backed calls load a native library. The package includes signer assets for win-x64, linux-x64, linux-arm64, and osx-arm64. REST/socket options expose `LibraryPath` for an explicit library file. Verify deployment includes the compatible binary; a successful C# build alone does not validate native signing or browser support.

`ILighterOrderBookFactory` creates local books from native or Shared symbols; check `StartAsync`, then stop the book in cleanup. `ILighterTrackerFactory` supplies kline, trade, and user-data trackers. `ILighterUserClientProvider` caches per-user clients; clear stale clients after credential changes.

- [Public interfaces](../../../../Lighter.Net/Interfaces/Clients/): exact signatures, market/account indexes and cancellation tokens (checkout-only).
- [Implementations](../../../../Lighter.Net/Clients/): Shared options, conversion and signer-backed behavior (checkout-only).
- [Complete examples](../../../../Examples/ai-friendly/): independent consumer programs (checkout-only).
