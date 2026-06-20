using System;
using System.Threading;
using System.Threading.Tasks;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.OrderBook;
using Microsoft.Extensions.Logging;
using Lighter.Net.Clients;
using Lighter.Net.Interfaces.Clients;
using Lighter.Net.Objects.Options;
using Lighter.Net.Objects.Models;

namespace Lighter.Net.SymbolOrderBooks
{
    /// <summary>
    /// Implementation for a synchronized order book. After calling Start the order book will sync itself and keep up to date with new data. It will automatically try to reconnect and resync in case of a lost/interrupted connection.
    /// Make sure to check the State property to see if the order book is synced.
    /// </summary>
    public class LighterSymbolOrderBook : SymbolOrderBook
    {
        private readonly bool _clientOwner;
        private readonly ILighterSocketClient _socketClient;
        private readonly TimeSpan _initialDataTimeout;

        /// <summary>
        /// Create a new order book instance
        /// </summary>
        /// <param name="symbol">The symbol the order book is for</param>
        /// <param name="optionsDelegate">Option configuration delegate</param>
        public LighterSymbolOrderBook(string symbol, Action<LighterOrderBookOptions>? optionsDelegate = null)
            : this(symbol, optionsDelegate, null, null)
        {
            _clientOwner = true;
        }

        /// <summary>
        /// Create a new order book instance
        /// </summary>
        /// <param name="symbol">The symbol the order book is for</param>
        /// <param name="optionsDelegate">Option configuration delegate</param>
        /// <param name="logger">Logger</param>
        /// <param name="socketClient">Socket client instance</param>
        public LighterSymbolOrderBook(
            string symbol,
            Action<LighterOrderBookOptions>? optionsDelegate,
            ILoggerFactory? logger,
            ILighterSocketClient? socketClient) : base(logger, "Lighter", "Exchange", symbol)
        {
            var options = LighterOrderBookOptions.Default.Copy();
            if (optionsDelegate != null)
                optionsDelegate(options);
            Initialize(options);

            _strictLevels = false;
            _sequencesAreConsecutive = true;

            _initialDataTimeout = options?.InitialDataTimeout ?? TimeSpan.FromSeconds(30);
            _clientOwner = socketClient == null;
            _socketClient = socketClient ?? new LighterSocketClient();
        }

        /// <inheritdoc />
        protected override async Task<CallResult<UpdateSubscription>> DoStartAsync(CancellationToken ct)
        {
            var subscription = await _socketClient.ExchangeApi.ExchangeData.SubscribeToOrderBookUpdatesAsync(Symbol, ProcessUpdate).ConfigureAwait(false);
            if (!subscription.Success)
                return CallResult.Fail<UpdateSubscription>(subscription.Error);

            Status = OrderBookStatus.Syncing;

            var set = await WaitForSetOrderBookAsync(_initialDataTimeout, ct).ConfigureAwait(false);
            if (!set.Success)
            {
                await subscription.Data.CloseAsync().ConfigureAwait(false);
                return CallResult.Fail<UpdateSubscription>(set.Error!);
            }

            Status = OrderBookStatus.Synced;
            return CallResult.Ok(subscription.Data);
        }

        private void ProcessUpdate(DataEvent<LighterOrderBookUpdate> @event)
        {
            if (@event.UpdateType == SocketUpdateType.Snapshot)
                SetSnapshot(@event.Data.OrderBook.Nonce, @event.Data.OrderBook.Bids, @event.Data.OrderBook.Asks);
            else
                UpdateOrderBook(@event.Data.OrderBook.BeginNonce + 1, @event.Data.OrderBook.Nonce, @event.Data.OrderBook.Bids, @event.Data.OrderBook.Asks, @event.DataTime, @event.DataTimeLocal);
        }

        /// <inheritdoc />
        protected override void DoReset()
        {
        }

        /// <inheritdoc />
        protected override async Task<CallResult> DoResyncAsync(CancellationToken ct)
        {
            return await WaitForSetOrderBookAsync(_initialDataTimeout, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (_clientOwner)
            {
                _socketClient?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
