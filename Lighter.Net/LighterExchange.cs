using CryptoExchange.Net.Objects;
using CryptoExchange.Net.RateLimiting.Interfaces;
using CryptoExchange.Net.RateLimiting;
using System;
using CryptoExchange.Net.SharedApis;
using Lighter.Net.Converters;
using System.Text.Json;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Converters;
using CryptoExchange.Net.RateLimiting.Guards;
using CryptoExchange.Net.RateLimiting.Filters;

namespace Lighter.Net
{
    /// <summary>
    /// Lighter exchange information and configuration
    /// </summary>
    public static class LighterExchange
    {
        /// <summary>
        /// Platform metadata
        /// </summary>
        public static PlatformInfo Metadata { get; } = new PlatformInfo(
                "Lighter",
                "Lighter",
                "https://raw.githubusercontent.com/JKorf/Lighter.Net/main/Lighter.Net/Icon/icon.png",
                "https://app.lighter.xyz",
                ["https://apidocs.lighter.xyz/reference"],
                PlatformType.CryptoCurrencyExchange,
                CentralizationType.Decentralized,
                LighterEnvironment.All
                );

        /// <summary>
        /// Exchange name
        /// </summary>
        public static string ExchangeName => "Lighter";

        /// <summary>
        /// Display name
        /// </summary>
        public static string DisplayName => "Lighter";

        /// <summary>
        /// Url to exchange image
        /// </summary>
        public static string ImageUrl { get; } = "https://raw.githubusercontent.com/JKorf/Lighter.Net/main/Lighter.Net/Icon/icon.png";

        /// <summary>
        /// Url to the main website
        /// </summary>
        public static string Url { get; } = "https://app.lighter.xyz";

        /// <summary>
        /// Urls to the API documentation
        /// </summary>
        public static string[] ApiDocsUrl { get; } = new[] {
            "https://apidocs.lighter.xyz/reference"
            };

        /// <summary>
        /// Type of exchange
        /// </summary>
        public static ExchangeType Type { get; } = ExchangeType.DEX;

        internal static JsonSerializerOptions _serializerContext = SerializerOptions.WithConverters(JsonSerializerContextCache.GetOrCreate<LighterSourceGenerationContext>());
        internal static ParameterSerializationSettings _parameterSerializationSettings = new ParameterSerializationSettings
        {
        };

        /// <summary>
        /// Aliases for Lighter assets
        /// </summary>
        public static AssetAliasConfiguration AssetAliases { get; } = new AssetAliasConfiguration
        {
            Aliases = [
                new AssetAlias("USDC", SharedSymbol.UsdOrStable.ToUpperInvariant(), AliasType.OnlyToExchange)
            ]
        };

        /// <summary>
        /// Format a base and quote asset to an Lighter recognized symbol 
        /// </summary>
        /// <param name="baseAsset">Base asset</param>
        /// <param name="quoteAsset">Quote asset</param>
        /// <param name="tradingMode">Trading mode</param>
        /// <param name="deliverTime">Delivery time for delivery futures</param>
        /// <returns></returns>
        public static string FormatSymbol(string baseAsset, string quoteAsset, TradingMode tradingMode, DateTime? deliverTime = null)
        {
            baseAsset = AssetAliases.CommonToExchangeName(baseAsset.ToUpperInvariant());
            quoteAsset = AssetAliases.CommonToExchangeName(quoteAsset.ToUpperInvariant());

            if (tradingMode == TradingMode.Spot)
                return $"{baseAsset}/{quoteAsset}";

            return baseAsset;
        }

        /// <summary>
        /// Rate limiter configuration for the Lighter API
        /// </summary>
        public static LighterRateLimiters RateLimiter { get; } = new LighterRateLimiters();
    }

    /// <summary>
    /// Rate limiter configuration for the Lighter API
    /// </summary>
    public class LighterRateLimiters
    {
        /// <summary>
        /// Event for when a rate limit is triggered
        /// </summary>
        public event Action<RateLimitEvent> RateLimitTriggered;
        /// <summary>
        /// Event when the rate limit is updated. Note that it's only updated when a request is send, so there are no specific updates when the current usage is decaying.
        /// </summary>
        public event Action<RateLimitUpdateEvent> RateLimitUpdated;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        internal LighterRateLimiters()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            Initialize();
        }

        private void Initialize()
        {
            LighterRest = new RateLimitGate("LighterRest");
            LighterRest.RateLimitTriggered += (x) => RateLimitTriggered?.Invoke(x);
            LighterRest.RateLimitUpdated += (x) => RateLimitUpdated?.Invoke(x);
            LighterSocket = new RateLimitGate("LighterSocket")
                .AddGuard(new RateLimitGuard(RateLimitGuard.PerHost, new LimitItemTypeFilter(RateLimitItemType.Connection), 80, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding))
                .AddGuard(new RateLimitGuard(RateLimitGuard.PerHost, new LimitItemTypeFilter(RateLimitItemType.Request), 200, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            LighterSocket.RateLimitTriggered += (x) => RateLimitTriggered?.Invoke(x);
            LighterSocket.RateLimitUpdated += (x) => RateLimitUpdated?.Invoke(x);
        }

        internal IRateLimitGate LighterRest { get; private set; }
        internal IRateLimitGate LighterSocket { get; private set; }
    }
}
