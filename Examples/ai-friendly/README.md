# AI-Friendly Examples

These examples are optimized for AI coding assistants and quick onboarding. Each file is:

- **Compilable** - drop into a console project with `dotnet add package JKorf.Lighter.Net` and it builds.
- **Self-contained** - single file, no external setup, no shared helpers.
- **Heavily commented** - explains why each line matters, not just what it does.
- **Idiomatic** - follows current Lighter.Net 1.x and CryptoExchange.Net 12.x patterns.

## Files

| File | What it shows |
|---|---|
| `01-quickstart.cs` | Client setup, public market data, authenticated account calls, safe order skeleton |
| `02-trading.cs` | Leverage, margin, order placement, order lookup, cancellation, and batch order shape |
| `03-websocket.cs` | Public ticker/trade/kline streams and authenticated user streams with proper teardown |
| `04-multi-exchange.cs` | `CryptoExchange.Net.SharedApis` pattern for exchange-agnostic code, capability discovery, and shared subscriptions |
| `05-error-handling.cs` | `HttpResult`, `WebSocketResult`, `QueryResult`, `ExchangeCallResult`, retry, and common error scenarios |

## Running

```bash
dotnet new console -n MyLighterApp
cd MyLighterApp
dotnet add package JKorf.Lighter.Net
# Copy the example .cs file content into Program.cs
# Replace PUBLIC_ADDRESS / ACCOUNT_INDEX / API_KEY_INDEX / API_SECRET placeholders where needed
dotnet run
```

Authenticated examples can place or cancel real orders. Read the comments and use conservative parameters before running trading code against a live account.
