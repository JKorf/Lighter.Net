using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.SharedApis;
using CryptoExchange.Net.OrderBook;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Lighter.Net.Interfaces;
using Lighter.Net.Interfaces.Clients;
using Lighter.Net.Objects.Options;

namespace Lighter.Net.SymbolOrderBooks
{
    /// <summary>
    /// Lighter order book factory
    /// </summary>
    public class LighterOrderBookFactory : ILighterOrderBookFactory
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="serviceProvider">Service provider for resolving logging and clients</param>
        public LighterOrderBookFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            
            Exchange = new OrderBookFactory<LighterOrderBookOptions>(Create, Create);
        }

                 /// <inheritdoc />
        public IOrderBookFactory<LighterOrderBookOptions> Exchange { get; }


        /// <inheritdoc />
        public ISymbolOrderBook Create(SharedSymbol symbol, Action<LighterOrderBookOptions>? options = null)
        {
            var symbolName = symbol.GetSymbol(LighterExchange.FormatSymbol);
            return Create(symbolName, options);
        }

                 /// <inheritdoc />
        public ISymbolOrderBook Create(string symbol, Action<LighterOrderBookOptions>? options = null)
            => new LighterSymbolOrderBook(symbol, options, 
                                                          _serviceProvider.GetRequiredService<ILoggerFactory>(),
                                                          _serviceProvider.GetRequiredService<ILighterSocketClient>());


    }
}
