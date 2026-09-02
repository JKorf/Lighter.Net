using CryptoExchange.Net;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Errors;
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
    internal partial class LighterRestClientExchangeSharedApi :
        SharedApiBase,
        ILighterRestClientExchangeApiShared,
        ILighterRestClientExchangeSharedApi
    {
        private readonly LighterRestClientExchangeApi _api;

        private const string _exchangeName = "Lighter";
        private const string _topicSpotId = "LighterSpot";
        private const string _topicFuturesId = "LighterFutures";

        public override SharedClientInfo Discover() => SharedUtils.GetClientInfo(LighterExchange.Metadata, this);

        public LighterRestClientExchangeSharedApi(LighterRestClientExchangeApi api)
            : base(
                  api.Exchange,
                  [TradingMode.Spot, TradingMode.PerpetualLinear],
                  () => api.Authenticated,
                  api.FormatSymbol)
        {
            _api = api;

            SetCapabilities(
                GetKlinesOptions,
                GetSpotSymbolsOptions,
                GetFuturesSymbolsOptions,
                GetSpotTickerOptions,
                GetAllSpotTickersOptions,
                GetFuturesTickerOptions,
                GetAllFuturesTickersOptions,
                GetBookTickerOptions,
                GetRecentTradesOptions,
                GetOrderBookOptions,
                GetAssetOptions,
                GetAllAssetsOptions,
                GetDepositHistoryOptions,
                GetWithdrawalHistoryOptions,
                GetFeeOptions,
                GetBalancesOptions,
                PlaceSpotOrderOptions,
                GetSpotOrderOptions,
                GetOpenSpotOrdersOptions,
                GetClosedSpotOrdersOptions,
                CancelSpotOrderOptions,
                GetSpotUserTradeHistoryOptions,
                GetSpotOrderTradesOptions,
                GetSpotOrderByClientOrderIdOptions,
                CancelSpotOrderByClientOrderIdOptions,
                PlaceFuturesOrderOptions,
                GetFuturesOrderOptions,
                GetOpenFuturesOrdersOptions,
                GetClosedFuturesOrdersOptions,
                CancelFuturesOrderOptions,
                GetFuturesOrderTradesOptions,
                GetFuturesUserTradeHistoryOptions,
                ClosePositionOptions,
                GetPositionsOptions,
                GetFuturesOrderByClientOrderIdOptions,
                CancelFuturesOrderByClientOrderIdOptions,
                GetLeverageOptions,
                SetLeverageOptions,
                GetOpenFuturesOrdersOptions,
                GetFundingRateHistoryOptions,
                GetOpenInterestOptions
                );
        }

    }
}
