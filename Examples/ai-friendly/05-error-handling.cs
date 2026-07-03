// 05-error-handling.cs
//
// Demonstrates: HttpResult, WebSocketResult, QueryResult, ExchangeCallResult,
// retry logic, and common error scenarios.
//
// Setup:
//   dotnet add package JKorf.Lighter.Net

using CryptoExchange.Net.Objects;
using CryptoExchange.Net.SharedApis;
using Lighter.Net;
using Lighter.Net.Clients;

var client = new LighterRestClient(options =>
{
    options.ApiCredentials = new LighterCredentials(EthKey.FromPrivateKey("PRIVATE_KEY"), 123, 5, "API_SECRET");
});

// ---- 1. THE BASIC PATTERN ----
// REST methods return HttpResult<T> or HttpResult.
// Local helpers can return CallResult<T>.
// WebSocket subscriptions return WebSocketResult<UpdateSubscription>.
// WebSocket transaction requests return QueryResult<T>.
// Shared non-I/O symbol/cache helpers return ExchangeCallResult<T>.
// .Success is true/false. .Data is valid only when .Success is true.
// .Error contains structured error info when .Success is false.
// .Error.IsTransient hints if a retry might succeed.
var result = await client.ExchangeApi.ExchangeData.GetSymbolDetailsAsync("ETH/USDC");

if (result.Success)
{
    var spotSymbol = result.Data.SpotSymbols.FirstOrDefault();
    Console.WriteLine($"Price: {spotSymbol?.LastPrice}");
}
else
{
    Console.WriteLine($"Code:      {result.Error?.Code}");
    Console.WriteLine($"Message:   {result.Error?.Message}");
    Console.WriteLine($"Type:      {result.Error?.ErrorType}");
    Console.WriteLine($"Transient: {result.Error?.IsTransient}");
}

// ---- 2. SIMPLE RETRY WITH BACKOFF ----
// Retry only on transient errors, for example a temporary network issue or
// server-side throttling. Do not retry validation errors or insufficient balance.
async Task<HttpResult<T>> WithRetry<T>(
    Func<Task<HttpResult<T>>> call,
    int maxAttempts = 3)
{
    HttpResult<T> last = default!;
    for (var attempt = 1; attempt <= maxAttempts; attempt++)
    {
        last = await call();
        if (last.Success)
            return last;

        if (last.Error?.IsTransient != true)
            return last;

        await Task.Delay(TimeSpan.FromMilliseconds(250 * Math.Pow(2, attempt)));
    }

    return last;
}

var ticker = await WithRetry(
    () => client.ExchangeApi.ExchangeData.GetSymbolDetailsAsync("ETH/USDC"));

if (!ticker.Success)
    Console.WriteLine($"Ticker still failed after retry: {ticker.Error}");

// ---- 3. COMMON LIGHTER ERROR SCENARIOS ----
//
// Missing credentials:
//   Authenticated endpoints need LighterCredentials(publicAddress, accountIndex,
//   apiKeyIndex, apiSecret). Public market data does not.
//
// Wrong account index or API key index:
//   Check the accountIndex and apiKeyIndex values in LighterCredentials.
//
// Nonce conflict:
//   Let Lighter.Net auto-set nonces unless the caller explicitly needs manual
//   control. Passing stale nonce values can cause transaction rejection.
//
// Native signer loading issue:
//   Ensure the packaged lighter-signer native library is present for the current
//   OS and architecture. The NuGet package copies supported native libraries.
//
// Order validation failure:
//   Check symbol format ("ETH/USDC" for spot, "ETH" for perps), quantity, price,
//   time-in-force, trigger price, reduceOnly, leverage, and account margin.

// ---- 4. SHARED API CALL RESULT ----
// Shared APIs use the same .Success/.Error pattern and allow exchange-agnostic code.
var shared = client.ExchangeApi.SharedClient;
var support = await shared.SupportsSpotSymbolAsync(new SharedSymbol(TradingMode.Spot, "ETH", "USDC"));
if (!support.Success)
{
    Console.WriteLine($"Symbol support check failed: {support.Error}");
}
else
{
    Console.WriteLine($"ETH/USDC supported: {support.Data}");
}

// ---- 5. EXCEPTIONS VS ERROR RESULTS ----
// Lighter.Net returns API failures via result.Error, not thrown exceptions.
// Exceptions are for misconfiguration, disposal, cancellation, missing credentials
// on authenticated calls, native signer loading issues, or programmer errors.

// Common variations:
//   With CancellationToken:    pass `ct: cancellationToken` to any method.
//   With timeout per request:  options.RequestTimeout = TimeSpan.FromSeconds(10);
//   Polly integration:         use Error.IsTransient as the retry predicate.
