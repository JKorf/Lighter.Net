using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.SharedApis;
using System;
using Lighter.Net.Objects.Options;

namespace Lighter.Net.Interfaces
{
    /// <summary>
    /// Lighter local order book factory
    /// </summary>
    public interface ILighterOrderBookFactory
    {
        /// <summary>
        /// Exchange order book factory methods
        /// </summary>
        IOrderBookFactory<LighterOrderBookOptions> Exchange { get; }

        /// <summary>
        /// Create a new local order book instance
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <param name="options">Book options</param>
        /// <returns></returns>
        ISymbolOrderBook Create(SharedSymbol symbol, Action<LighterOrderBookOptions>? options = null);

        /// <summary>
        /// Create a new local order book instance
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <param name="options">Book options</param>
        ISymbolOrderBook Create(string symbol, Action<LighterOrderBookOptions>? options = null);

    }
}