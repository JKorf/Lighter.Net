using CryptoExchange.Net.Objects.Options;
using System;

namespace Lighter.Net.Objects.Options
{
    /// <summary>
    /// Options for the Lighter SymbolOrderBook
    /// </summary>
    public class LighterOrderBookOptions : OrderBookOptions
    {
        /// <summary>
        /// Default options for new clients
        /// </summary>
        public static LighterOrderBookOptions Default { get; set; } = new LighterOrderBookOptions();

        /// <summary>
        /// After how much time we should consider the connection dropped if no data is received for this time after the initial subscriptions
        /// </summary>
        public TimeSpan? InitialDataTimeout { get; set; }

        internal LighterOrderBookOptions Copy()
        {
            var result = Copy<LighterOrderBookOptions>();
            result.InitialDataTimeout = InitialDataTimeout;
            return result;
        }
    }
}
