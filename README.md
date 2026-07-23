# ![Lighter.Net](https://raw.githubusercontent.com/JKorf/Lighter.Net/main/Lighter.Net/Icon/icon.png) Lighter.Net  

[![.NET](https://img.shields.io/github/actions/workflow/status/JKorf/Lighter.Net/dotnet.yml?style=for-the-badge)](https://github.com/JKorf/Lighter.Net/actions/workflows/dotnet.yml) ![License](https://img.shields.io/github/license/JKorf/Lighter.Net?style=for-the-badge)

Lighter.Net is a client library for accessing the [Lighter DEX REST and Websocket API](https://apidocs.lighter.xyz/docs/get-started). 

## Features
* Response data is mapped to descriptive models
* Input parameters and response values are mapped to discriptive enum values where possible
* High performance
* Automatic websocket (re)connection management 
* Client side rate limiting 
* Client side order book implementation
* Support for managing different accounts
* Extensive logging
* Support for different environments
* Easy integration with other exchange clients based on the CryptoExchange.Net base library
* Native AOT support

## Supported Frameworks
The library is targeting both `.NET Standard 2.0` and `.NET Standard 2.1` for optimal compatibility, as well as the latest dotnet versions to use the latest framework features.

|.NET implementation|Version Support|
|--|--|
|.NET Core|`2.0` and higher|
|.NET Framework|`4.6.1` and higher|
|Mono|`5.4` and higher|
|Xamarin.iOS|`10.14` and higher|
|Xamarin.Android|`8.0` and higher|
|UWP|`10.0.16299` and higher|
|Unity|`2018.1` and higher|

## Install the library

### NuGet 
[![NuGet version](https://img.shields.io/nuget/v/JKorf.Lighter.net.svg?style=for-the-badge)](https://www.nuget.org/packages/JKorf.Lighter.Net)  [![Nuget downloads](https://img.shields.io/nuget/dt/JKorf.Lighter.Net.svg?style=for-the-badge)](https://www.nuget.org/packages/JKorf.Lighter.Net)

	dotnet add package JKorf.Lighter.Net
	
### GitHub packages
Lighter.Net is available on [GitHub packages](https://github.com/JKorf/Lighter.Net/pkgs/nuget/Lighter.Net). You'll need to add `https://nuget.pkg.github.com/JKorf/index.json` as a NuGet package source.

### Download release
[![GitHub Release](https://img.shields.io/github/v/release/JKorf/Lighter.Net?style=for-the-badge&label=GitHub)](https://github.com/JKorf/Lighter.Net/releases)

The NuGet package files are added along side the source with the latest GitHub release which can found [here](https://github.com/JKorf/Lighter.Net/releases).

## How to use
*Basic request:* 
```csharp
// Get the ETH/USDC ticker via rest request
var restClient = new LighterRestClient();
var tickerResult = await restClient.ExchangeApi.ExchangeData.GetSymbolDetailsAsync("ETH/USDC");
if (!tickerResult.Success)
{
    Console.WriteLine($"Error: {tickerResult.Error}");
    return;
}

var lastPrice = tickerResult.Data.SpotSymbols[0].LastPrice;
```

*Place order:*
```csharp
var restClient = new LighterRestClient(opts => {
    opts.ApiCredentials = new LighterCredentials(EthKey.FromPrivateKey("KEY"), 123, 5, "APISECRET");
});

// Place Limit order to go 0.1 long for ETH at 2000
var orderResult = await restClient.ExchangeApi.Trading.PlaceOrderAsync(
    "ETH",
    OrderSide.Buy,
    OrderType.Limit,
    0.1m,
    2000m,
    timeInForce: TimeInForce.GoodTillTime
    );
```

*WebSocket subscription:* 
```csharp
// Subscribe to ETH/USDC ticker updates via the websocket API
var socketClient = new LighterSocketClient();
var tickerSubscriptionResult = socketClient.ExchangeApi.ExchangeData.SubscribeToSpotTickerUpdatesAsync("ETH/USDC", (update) =>
{
    var lastPrice = update.Data.Ticker.LastPrice;
});
```

For information on the clients, dependency injection, response processing and more see the [documentation](https://cryptoexchange.jkorf.dev/client-libs/getting-started), or have a look at the examples [here](https://github.com/JKorf/Lighter.Net/tree/main/Examples) or [here](https://github.com/JKorf/CryptoExchange.Net/tree/master/Examples).

**NOTE**  
Lighter.Net uses the Integrator Code mechanism for Lighter, which means that an additional 1bps / 0.01% fee is charged on top of orders placed with the library to fund development. This is entirely optional and can be disabled in the client options by setting `IntegratorFeePercentage` to `0` or `null` in the client options.

## AI / LLM documentation

Lighter.Net includes AI-oriented documentation and examples for code generation tools:

|File|Purpose|
|--|--|
|[`AGENTS.md`](AGENTS.md)|Assistant skill with core Lighter.Net patterns, pitfalls, and examples|
|[`llms.txt`](llms.txt)|Short LLM index with links to docs, examples, and critical usage rules|
|[`llms-full.txt`](llms-full.txt)|Detailed LLM context with endpoint routing, code patterns, and anti-hallucination checks|
|[`docs/ai-api-map.md`](docs/ai-api-map.md)|Table-style intent-to-method map|
|[`Examples/ai-friendly`](Examples/ai-friendly)|Compilable single-file examples for common REST, WebSocket, shared API, and error handling workflows|

See [cryptoexchange-skills-hub](https://github.com/JKorf/cryptoexchange-skills-hub) for installable skills.

## CryptoExchange.Net
Lighter.Net is based on the [CryptoExchange.Net](https://github.com/JKorf/CryptoExchange.Net) base library. Other exchange API implementations based on the CryptoExchange.Net base library are available and follow the same logic.

CryptoExchange.Net also allows for [easy access to different exchange API's](https://jkorf.github.io/CryptoExchange.Net#idocs_shared).

|Exchange|Repository|Nuget|
|--|--|--|
|Aster|[JKorf/Aster.Net](https://github.com/JKorf/Aster.Net)|[![Nuget version](https://img.shields.io/nuget/v/JKorf.Aster.net.svg?style=flat-square)](https://www.nuget.org/packages/JKorf.Aster.Net)|
|Binance|[JKorf/Binance.Net](https://github.com/JKorf/Binance.Net)|[![Nuget version](https://img.shields.io/nuget/v/Binance.net.svg?style=flat-square)](https://www.nuget.org/packages/Binance.Net)|
|BingX|[JKorf/BingX.Net](https://github.com/JKorf/BingX.Net)|[![Nuget version](https://img.shields.io/nuget/v/JK.BingX.net.svg?style=flat-square)](https://www.nuget.org/packages/JK.BingX.Net)|
|Bitfinex|[JKorf/Bitfinex.Net](https://github.com/JKorf/Bitfinex.Net)|[![Nuget version](https://img.shields.io/nuget/v/Bitfinex.net.svg?style=flat-square)](https://www.nuget.org/packages/Bitfinex.Net)|
|Bitget|[JKorf/Bitget.Net](https://github.com/JKorf/Bitget.Net)|[![Nuget version](https://img.shields.io/nuget/v/JK.Bitget.net.svg?style=flat-square)](https://www.nuget.org/packages/JK.Bitget.Net)|
|BitMart|[JKorf/BitMart.Net](https://github.com/JKorf/BitMart.Net)|[![Nuget version](https://img.shields.io/nuget/v/BitMart.net.svg?style=flat-square)](https://www.nuget.org/packages/BitMart.Net)|
|BitMEX|[JKorf/BitMEX.Net](https://github.com/JKorf/BitMEX.Net)|[![Nuget version](https://img.shields.io/nuget/v/JKorf.BitMEX.net.svg?style=flat-square)](https://www.nuget.org/packages/JKorf.BitMEX.Net)|
|Bitstamp|[JKorf/Bitstamp.Net](https://github.com/JKorf/Bitstamp.Net)|[![Nuget version](https://img.shields.io/nuget/v/Bitstamp.Net.svg?style=flat-square)](https://www.nuget.org/packages/Bitstamp.Net)|
|BloFin|[JKorf/BloFin.Net](https://github.com/JKorf/BloFin.Net)|[![Nuget version](https://img.shields.io/nuget/v/BloFin.net.svg?style=flat-square)](https://www.nuget.org/packages/BloFin.Net)|
|Bybit|[JKorf/Bybit.Net](https://github.com/JKorf/Bybit.Net)|[![Nuget version](https://img.shields.io/nuget/v/Bybit.net.svg?style=flat-square)](https://www.nuget.org/packages/Bybit.Net)|
|Coinbase|[JKorf/Coinbase.Net](https://github.com/JKorf/Coinbase.Net)|[![Nuget version](https://img.shields.io/nuget/v/JKorf.Coinbase.net.svg?style=flat-square)](https://www.nuget.org/packages/JKorf.Coinbase.Net)|
|CoinEx|[JKorf/CoinEx.Net](https://github.com/JKorf/CoinEx.Net)|[![Nuget version](https://img.shields.io/nuget/v/CoinEx.net.svg?style=flat-square)](https://www.nuget.org/packages/CoinEx.Net)|
|CoinGecko|[JKorf/CoinGecko.Net](https://github.com/JKorf/CoinGecko.Net)|[![Nuget version](https://img.shields.io/nuget/v/CoinGecko.net.svg?style=flat-square)](https://www.nuget.org/packages/CoinGecko.Net)|
|CoinW|[JKorf/CoinW.Net](https://github.com/JKorf/CoinW.Net)|[![Nuget version](https://img.shields.io/nuget/v/CoinW.net.svg?style=flat-square)](https://www.nuget.org/packages/CoinW.Net)|
|Crypto.com|[JKorf/CryptoCom.Net](https://github.com/JKorf/CryptoCom.Net)|[![Nuget version](https://img.shields.io/nuget/v/CryptoCom.net.svg?style=flat-square)](https://www.nuget.org/packages/CryptoCom.Net)|
|DeepCoin|[JKorf/DeepCoin.Net](https://github.com/JKorf/DeepCoin.Net)|[![Nuget version](https://img.shields.io/nuget/v/DeepCoin.net.svg?style=flat-square)](https://www.nuget.org/packages/DeepCoin.Net)|
|Gate.io|[JKorf/GateIo.Net](https://github.com/JKorf/GateIo.Net)|[![Nuget version](https://img.shields.io/nuget/v/GateIo.net.svg?style=flat-square)](https://www.nuget.org/packages/GateIo.Net)|
|HTX|[JKorf/HTX.Net](https://github.com/JKorf/HTX.Net)|[![Nuget version](https://img.shields.io/nuget/v/JKorf.HTX.net.svg?style=flat-square)](https://www.nuget.org/packages/Jkorf.HTX.Net)|
|HyperLiquid|[JKorf/HyperLiquid.Net](https://github.com/JKorf/HyperLiquid.Net)|[![Nuget version](https://img.shields.io/nuget/v/HyperLiquid.Net.svg?style=flat-square)](https://www.nuget.org/packages/HyperLiquid.Net)|
|Kraken|[JKorf/Kraken.Net](https://github.com/JKorf/Kraken.Net)|[![Nuget version](https://img.shields.io/nuget/v/KrakenExchange.net.svg?style=flat-square)](https://www.nuget.org/packages/KrakenExchange.Net)|
|Kucoin|[JKorf/Kucoin.Net](https://github.com/JKorf/Kucoin.Net)|[![Nuget version](https://img.shields.io/nuget/v/Kucoin.net.svg?style=flat-square)](https://www.nuget.org/packages/Kucoin.Net)|
|Mexc|[JKorf/Mexc.Net](https://github.com/JKorf/Mexc.Net)|[![Nuget version](https://img.shields.io/nuget/v/JK.Mexc.net.svg?style=flat-square)](https://www.nuget.org/packages/JK.Mexc.Net)|
|OKX|[JKorf/OKX.Net](https://github.com/JKorf/OKX.Net)|[![Nuget version](https://img.shields.io/nuget/v/JK.OKX.net.svg?style=flat-square)](https://www.nuget.org/packages/JK.OKX.Net)|
|Pionex|[JKorf/Pionex.Net](https://github.com/JKorf/Pionex.Net)|[![Nuget version](https://img.shields.io/nuget/v/Pionex.net.svg?style=flat-square)](https://www.nuget.org/packages/Pionex.Net)|
|Polymarket|[JKorf/Polymarket.Net](https://github.com/JKorf/Polymarket.Net)|[![Nuget version](https://img.shields.io/nuget/v/Polymarket.net.svg?style=flat-square)](https://www.nuget.org/packages/Polymarket.Net)|
|Toobit|[JKorf/Toobit.Net](https://github.com/JKorf/Toobit.Net)|[![Nuget version](https://img.shields.io/nuget/v/Toobit.net.svg?style=flat-square)](https://www.nuget.org/packages/Toobit.Net)|
|Upbit|[JKorf/Upbit.Net](https://github.com/JKorf/Upbit.Net)|[![Nuget version](https://img.shields.io/nuget/v/JKorf.Upbit.net.svg?style=flat-square)](https://www.nuget.org/packages/JKorf.Upbit.Net)|
|Weex|[JKorf/Weex.Net](https://github.com/JKorf/Weex.Net)|[![Nuget version](https://img.shields.io/nuget/v/Weex.net.svg?style=flat-square)](https://www.nuget.org/packages/Weex.Net)|
|WhiteBit|[JKorf/WhiteBit.Net](https://github.com/JKorf/WhiteBit.Net)|[![Nuget version](https://img.shields.io/nuget/v/WhiteBit.net.svg?style=flat-square)](https://www.nuget.org/packages/WhiteBit.Net)|
|XT|[JKorf/XT.Net](https://github.com/JKorf/XT.Net)|[![Nuget version](https://img.shields.io/nuget/v/XT.net.svg?style=flat-square)](https://www.nuget.org/packages/XT.Net)|

When using multiple of these API's the [CryptoClients.Net](https://github.com/JKorf/CryptoClients.Net) package can be used which combines this and the other packages and allows easy access to all exchange API's.

## Discord
[![Nuget version](https://img.shields.io/discord/847020490588422145?style=for-the-badge)](https://discord.gg/MSpeEtSY8t)  
A Discord server is available [here](https://discord.gg/MSpeEtSY8t). For discussion and/or questions around the CryptoExchange.Net and implementation libraries, feel free to join.

## Supported functionality

### REST
|API|Supported|Location|
|--|--:|--|
|Root|✓|`restClient.ExchangeApi.ExchangeData`|
|Account|✓|`restClient.ExchangeApi.Account`|
|Order|✓|`restClient.ExchangeApi.ExchangeData` / `restClient.ExchangeApi.Trading`|
|Transaction|✓|`restClient.ExchangeApi.Account` (sendTx handled internally)|
|Announcement|✓|`restClient.ExchangeApi.ExchangeData`|
|Apikeys|Partial|`restClient.ExchangeApi.Account`|
|Candlestick|✓|`restClient.ExchangeApi.ExchangeData`|
|Bridge|X||
|Funding|✓|`restClient.ExchangeApi.ExchangeData`|
|Notification|X||
|Info|X||
|Referral|X||
|Fee Credits|X||
|RFQ|X||
|API Explorer|X||

### WebSocket
|API|Supported|Location|
|--|--:|--|
|Account subscriptions|✓|`socketClient.FuturesApi.Account`|
|Account requests|✓|`socketClient.FuturesApi.Account`|
|Market data subscriptions|✓|`socketClient.FuturesApi.ExchangeData`|
|Trading subscriptions|✓|`socketClient.FuturesApi.Trading`|
|Order requests|✓|`socketClient.FuturesApi.Trading`|

## Support the project
Any support is greatly appreciated.

### Donate
Make a one time donation in a crypto currency of your choice. If you prefer to donate a currency not listed here please contact me.

**Btc**:  bc1q277a5n54s2l2mzlu778ef7lpkwhjhyvghuv8qf  
**Eth**:  0xcb1b63aCF9fef2755eBf4a0506250074496Ad5b7   
**USDT (TRX)**  TKigKeJPXZYyMVDgMyXxMf17MWYia92Rjd 

### Sponsor
Alternatively, sponsor me on Github using [Github Sponsors](https://github.com/sponsors/JKorf). 

## Release notes
* Version 1.3.0 - 21 Jul 2026
    * Updated CryptoExchange.Net to v12.2.0 
    * Added SpotSymbolCatalog to Shared ISpotSymbolRestClient interface
    * Added FuturesSymbolCatalog to Shared IFuturesSymbolRestClient interface
    * Added BaseAssetType, BaseAssetSubType, QuoteAssetType and QuoteAssetSubType to GetSymbolsRequest model
    * Added DisplayName to SharedSpotSymbol and SharedFuturesSymbol models
    * Added BaseAssetType, BaseAssetSubType, QuoteAssetType and QuoteAssetSubType to SharedSpotSymbol and SharedFuturesSymbol models
    * Added DebuggerDisplay attributes to Shared models
    * Added restClient.ExchangeApi.ExchangeData.GetTokensAsync endpoint

* Version 1.2.2 - 14 Jul 2026
    * Fixed Shared API UserTrades incorrect order side parsing

* Version 1.2.1 - 13 Jul 2026
    * Updated LighterAccountUpdate model fixing deserialization issue when FundingHistories is set
    * Fixed exception during authentication when retrying requests

* Version 1.2.0 - 09 Jul 2026
    * Updated CryptoExchange.Net to v12.1.0

* Version 1.1.6 - 08 Jul 2026
    * Fixed missing parameters in SignModifyOrderDelegate causing EditOrderAsync to fail

* Version 1.1.5 - 07 Jul 2026
    * Fixed hardcoded price scaling logic

* Version 1.1.4 - 06 Jul 2026
    * Fixed hardcoded price scaling logic

* Version 1.1.3 - 04 Jul 2026
    * Fixed automatic refresh of auth tokens after expiry

* Version 1.1.2 - 04 Jul 2026
    * Fixed signer libs not getting included in output

* Version 1.1.1 - 03 Jul 2026
    * Fixed native library finding when using NuGet package

* Version 1.1.0 - 03 Jul 2026
    * Added IFundingRateRestClient Shared API implementation
    * Updated authentication, added L1 signing support
    * Fixed missing namespaces for some enums
    * Fixed libraries not getting included in Nuget/publish

    * Notes for updating:
        * API credentials providing has been updated to now take a EthKey object, for example `new LighterCredentials(EthKey.FromPublicKey(..), ..)`

* Version 1.0.1 - 30 Jun 2026
    * Fixed WebSocket unsubscribe not working correctly

* Version 1.0.0 - 29 Jun 2026
    * Initial release

