using CryptoExchange.Net;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Errors;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.SharedApis;
using Lighter.Net.Enums;
using Lighter.Net.Interfaces.Clients.ExchangeApi;
using Lighter.Net.Objects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lighter.Net.Clients.ExchangeApi
{
    internal partial class LighterSocketClientExchangeSharedApi :
        SharedApiBase,
        ILighterSocketClientExchangeApiShared,
        ILighterSocketClientExchangeSharedApi
    {
        private readonly LighterSocketClientExchangeApi _api;

        private const string _exchangeName = "Lighter";
        private const string _topicSpotId = "LighterSpot";
        private const string _topicFuturesId = "LighterFutures";

        public override SharedClientInfo Discover() => SharedUtils.GetClientInfo(LighterExchange.Metadata, this);

        public LighterSocketClientExchangeSharedApi(LighterSocketClientExchangeApi api)
            : base(
                  SharedTransport.Socket,
                  api.Exchange,
                  [TradingMode.Spot, TradingMode.PerpetualLinear],
                  () => api.Authenticated,
                  api.FormatSymbol)
        {
            _api = api;

            SetCapabilities(
                SubscribeAllTickersOptions,
                SubscribeTickerOptions,
                SubscribeTradeOptions,
                SubscribeBookTickerOptions,
                SubscribeKlineOptions,
                SubscribeBalanceOptions,
                SubscribeFuturesOrderOptions,
                SubscribeSpotOrderOptions,
                SubscribeUserTradeOptions,
                SubscribePositionOptions,
                PlaceSpotOrderOptions,
                CancelSpotOrderOptions,
                PlaceFuturesOrderOptions,
                CancelFuturesOrderOptions
                );
        }

    }
}
