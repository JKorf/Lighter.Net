// 02-trading.cs
//
// Demonstrates: authenticated trading flows - leverage, margin, order placement,
// open/closed order queries, cancellation, and batch order shape.
//
// Setup:
//   dotnet add package JKorf.Lighter.Net
//   Substitute PRIVATE_KEY / ACCOUNT_INDEX / API_KEY_INDEX / API_SECRET

using Lighter.Net;
using Lighter.Net.Clients;
using Lighter.Net.Enums;
using Lighter.Net.Objects.Models;

var client = new LighterRestClient(options =>
{
    options.ApiCredentials = new LighterCredentials(EthKey.FromPrivateKey("PRIVATE_KEY"), 123, 5, "API_SECRET");

    // Lighter.Net uses Lighter's optional integrator-code mechanism by default.
    // Set to 0m or null if you explicitly want to disable it.
    // options.IntegratorFeePercentage = 0m;
});

// ---- 1. SET LEVERAGE FOR A PERP SYMBOL ----
var leverage = await client.ExchangeApi.Account.SetLeverageAsync(
    symbol: "ETH",
    leverage: 5,
    marginMode: MarginMode.CrossMargin);

if (!leverage.Success)
{
    Console.WriteLine($"Failed to set leverage: {leverage.Error}");
    return;
}

Console.WriteLine($"Set leverage transaction: {leverage.Data}");

// ---- 2. UPDATE MARGIN ----
// addOrRemove: true adds margin, false removes margin.
// This is real account-changing code, so keep disabled until intentionally used.
var updateMargin = false;
if (updateMargin)
{
    var margin = await client.ExchangeApi.Account.UpdateMarginAsync(
        symbol: "ETH",
        usdcAmount: 10m,
        addOrRemove: true,
        nonce: null);

    if (!margin.Success)
    {
        Console.WriteLine($"Failed to update margin: {margin.Error}");
        return;
    }
}

// ---- 3. PLACE A LIMIT ORDER ----
// Lighter.Net can auto-populate nonce values. Do not pass nonce manually unless
// you have a specific operational reason.
var placeRealOrder = false;
if (placeRealOrder)
{
    var order = await client.ExchangeApi.Trading.PlaceOrderAsync(
        symbol: "ETH",
        side: OrderSide.Buy,
        orderType: OrderType.Limit,
        quantity: 0.01m,
        price: 2000m,
        timeInForce: TimeInForce.GoodTillTime,
        reduceOnly: false);

    if (!order.Success)
    {
        Console.WriteLine($"Failed to place order: {order.Error}");
        return;
    }

    Console.WriteLine($"Place order transaction: {order.Data}");
}

// ---- 4. QUERY OPEN AND CLOSED ORDERS ----
var openOrders = await client.ExchangeApi.Trading.GetOpenOrdersAsync(symbol: "ETH", marketType: MarketType.Perps);
if (!openOrders.Success)
{
    Console.WriteLine($"Failed to get open orders: {openOrders.Error}");
    return;
}

Console.WriteLine($"Open orders response: {openOrders.Data}");

var closedOrders = await client.ExchangeApi.Trading.GetClosedOrdersAsync(symbol: "ETH", limit: 20);
if (!closedOrders.Success)
{
    Console.WriteLine($"Failed to get closed orders: {closedOrders.Error}");
    return;
}

Console.WriteLine($"Closed orders response: {closedOrders.Data}");

// ---- 5. CANCEL AN ORDER ----
// Use the order index returned by Lighter for the order you want to cancel.
var cancelRealOrder = false;
if (cancelRealOrder)
{
    var cancel = await client.ExchangeApi.Trading.CancelOrderAsync(
        symbol: "ETH",
        orderIndex: 123456789);

    if (!cancel.Success)
    {
        Console.WriteLine($"Failed to cancel order: {cancel.Error}");
        return;
    }
}

// ---- 6. BATCH ORDER SHAPE ----
// Batch orders use LighterOrderRequest. Keep disabled until the account should
// intentionally submit multiple live orders.
var placeBatch = false;
if (placeBatch)
{
    var orders = new[]
    {
        new LighterOrderRequest
        {
            Symbol = "ETH",
            Side = OrderSide.Buy,
            OrderType = OrderType.Limit,
            Quantity = 0.01m,
            Price = 1900m,
            TimeInForce = TimeInForce.GoodTillTime
        }
    };

    var batch = await client.ExchangeApi.Trading.PlaceMultipleOrdersAsync(orders);
    if (!batch.Success)
    {
        Console.WriteLine($"Failed to place batch: {batch.Error}");
        return;
    }
}
