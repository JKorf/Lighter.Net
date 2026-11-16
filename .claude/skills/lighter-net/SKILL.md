---
name: lighter-net
description: Generate and troubleshoot C#/.NET code consuming Lighter.Net for DEX Spot/perpetual market data, accounts, signed transactions, trading, leverage/margin, WebSocket streams, and Shared API V2 workflows.
---

# Lighter.Net

Install the consumer package:

```sh
dotnet add package JKorf.Lighter.Net
```

Use `LighterRestClient.ExchangeApi` and `LighterSocketClient.ExchangeApi`, each with `ExchangeData`, `Account`, and `Trading`. Spot and perpetuals share this root; do not invent separate SpotApi/FuturesApi clients. Reuse and dispose clients, await calls, and retain successful subscriptions for unsubscribe cleanup.

Credentials are `LighterCredentials(ethKey, accountIndex, apiKeyIndex, apiSecret)` using `CryptoExchange.Net.Web3.EthKey`. Use `EthKey.FromPublicAddress(address)` for API-key signing without an L1 private key, or `EthKey.FromPrivateKey(key)` for operations that require L1 signatures. The private API key secret is distinct from the wallet private key. Public market data needs no credentials. Signer-backed operations require a compatible native signer runtime.

Spot symbols use `ETH/USDC`; perpetuals use `ETH`. Use metadata or `LighterExchange.FormatSymbol` for reusable formatting. Native order quantities and Shared order quantities use base-asset units, including perpetuals; metadata precision and multipliers still matter.

Check `Success` before `Data`: REST returns `HttpResult`, socket transactions `QueryResult`, subscriptions `WebSocketResult<UpdateSubscription>`, local helpers `CallResult`, and some Shared helpers `ExchangeCallResult`, with generic variants where applicable. Transaction acceptance is distinct from order execution. Shared placement can succeed with a null `SharedId.Id`; retain numeric client order IDs and reconcile order updates instead.

Use `.SharedApi` and narrow V2 capabilities for portable code; `ILighterSharedApiClient` groups `Rest` and `Socket`. Use native APIs for signer/account management and Lighter-specific transactions. Leave nonce management to the library unless the application coordinates a specific nonce workflow.

## Read when needed

- [API surfaces](references/api-surfaces.md): endpoints, Shared capabilities, catalogs, quantities, and native signer deployment.
- [Usage](references/usage.md): setup, credentials, account reads, order syntax, subscriptions, Shared API V2, and DI.
- [Operational considerations](references/safety.md): key roles, nonces, integrator fees, execution, and uncertain submissions.

For work on the library itself, use [repository instructions](../../../AGENTS.md) (checkout-only).
