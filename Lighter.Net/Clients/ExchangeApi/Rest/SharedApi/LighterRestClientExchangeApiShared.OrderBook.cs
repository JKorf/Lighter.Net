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
    internal partial class LighterRestClientExchangeSharedApi
    {
        #region Get Order Book

        async Task<ICallResult<SharedOrderBook>> IGetOrderBook.GetOrderBookAsync(GetOrderBookRequest request, CancellationToken ct)
            => await GetOrderBookAsync(request, ct).ConfigureAwait(false);

        public GetOrderBookOptions GetOrderBookOptions { get; } = new GetOrderBookOptions(_exchangeName, 1, 250, false)
        {
            RequestNotes = "When specifying the limit parameter less entries might be returned as individual orders are combined into aggregated levels client side"
        };
        public async Task<HttpResult<SharedOrderBook>> GetOrderBookAsync(GetOrderBookRequest request, CancellationToken ct)
        {
            var validationError = GetOrderBookOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedOrderBook>(Exchange, validationError);

            var result = await _api.ExchangeData.GetOrderBookAsync(
                request.Symbol!.GetSymbol(FormatSymbol),
                limit: request.Limit ?? 50,
                ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedOrderBook>(result);

            var asks = result.Data.Asks.GroupBy(x => x.Price);
            var bids = result.Data.Bids.GroupBy(x => x.Price);

            return HttpResult.Ok(result, 
                new SharedOrderBook(
                    SharedQuantityType.BaseAsset,
                    null,
                    asks.Select(x => new CombinedEntry { Price = x.Key, Quantity = x.Sum(y => y.Quantity) }).ToArray(),
                    bids.Select(x => new CombinedEntry { Price = x.Key, Quantity = x.Sum(y => y.Quantity) }).ToArray()
                    ));

        }

        #endregion

        class CombinedEntry : ISymbolOrderBookEntry
        {
            public decimal Quantity { get; set; }
            public decimal Price { get; set; }
        }
    }
}
