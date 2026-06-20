using System;
using System.Text.Json.Serialization;
using Lighter.Net.Enums;

namespace Lighter.Net.Objects.Models;

/// <summary>
/// Lighter orders
/// </summary>
public record LighterOrders : LighterResponse
{
    /// <summary>
    /// ["<c>next_cursor</c>"] Next cursor
    /// </summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }
    /// <summary>
    /// ["<c>orders</c>"] Orders
    /// </summary>
    [JsonPropertyName("orders")]
    public LighterOrder[] Orders { get; set; } = [];
}

/// <summary>
/// Order info
/// </summary>
public record LighterOrder
{
    /// <summary>
    /// ["<c>order_index</c>"] Order index
    /// </summary>
    [JsonPropertyName("order_index")]
    public long OrderIndex { get; set; }
    /// <summary>
    /// ["<c>client_order_index</c>"] Client order index
    /// </summary>
    [JsonPropertyName("client_order_index")]
    public long ClientOrderIndex { get; set; }
    /// <summary>
    /// ["<c>order_id</c>"] Order id
    /// </summary>
    [JsonPropertyName("order_id")]
    public long OrderId { get; set; }
    /// <summary>
    /// ["<c>client_order_id</c>"] Client order id
    /// </summary>
    [JsonPropertyName("client_order_id")]
    public long ClientOrderId { get; set; }
    /// <summary>
    /// ["<c>market_index</c>"] Market index
    /// </summary>
    [JsonPropertyName("market_index")]
    public int MarketIndex { get; set; }
    /// <summary>
    /// ["<c>owner_account_index</c>"] Owner account index
    /// </summary>
    [JsonPropertyName("owner_account_index")]
    public long OwnerAccountIndex { get; set; }
    /// <summary>
    /// ["<c>initial_base_amount</c>"] Initial base quantity
    /// </summary>
    [JsonPropertyName("initial_base_amount")]
    public decimal InitialBaseQuantity { get; set; }
    /// <summary>
    /// ["<c>price</c>"] Price
    /// </summary>
    [JsonPropertyName("price")]
    public decimal Price { get; set; }
    /// <summary>
    /// ["<c>nonce</c>"] Nonce
    /// </summary>
    [JsonPropertyName("nonce")]
    public long Nonce { get; set; }
    /// <summary>
    /// ["<c>remaining_base_amount</c>"] Remaining base quantity
    /// </summary>
    [JsonPropertyName("remaining_base_amount")]
    public decimal QuantityRemaining { get; set; }
    /// <summary>
    /// ["<c>is_ask</c>"] Is ask
    /// </summary>
    [JsonPropertyName("is_ask")]
    public bool IsAsk { get; set; }
    /// <summary>
    /// ["<c>base_size</c>"] Base quantity
    /// </summary>
    [JsonPropertyName("base_size")]
    public decimal BaseQuantity { get; set; }
    /// <summary>
    /// ["<c>base_price</c>"] Base price
    /// </summary>
    [JsonPropertyName("base_price")]
    public decimal BasePrice { get; set; }
    /// <summary>
    /// ["<c>filled_base_amount</c>"] Filled base quantity
    /// </summary>
    [JsonPropertyName("filled_base_amount")]
    public decimal QuantityFilled { get; set; }
    /// <summary>
    /// ["<c>filled_quote_amount</c>"] Filled quote quantity
    /// </summary>
    [JsonPropertyName("filled_quote_amount")]
    public decimal QuoteQuantityFilled { get; set; }
    /// <summary>
    /// ["<c>side</c>"] Side
    /// </summary>
    [JsonPropertyName("side")]
    public OrderSide? Side { get; set; }
    /// <summary>
    /// ["<c>type</c>"] Order type
    /// </summary>
    [JsonPropertyName("type")]
    public OrderType OrderType { get; set; }
    /// <summary>
    /// ["<c>time_in_force</c>"] Time in force
    /// </summary>
    [JsonPropertyName("time_in_force")]
    public TimeInForce TimeInForce { get; set; }
    /// <summary>
    /// ["<c>reduce_only</c>"] Reduce only
    /// </summary>
    [JsonPropertyName("reduce_only")]
    public bool ReduceOnly { get; set; }
    /// <summary>
    /// ["<c>trigger_price</c>"] Trigger price
    /// </summary>
    [JsonPropertyName("trigger_price")]
    public decimal TriggerPrice { get; set; }
    /// <summary>
    /// ["<c>order_expiry</c>"] Order expiry
    /// </summary>
    [JsonPropertyName("order_expiry")]
    public DateTime OrderExpiry { get; set; }
    /// <summary>
    /// ["<c>status</c>"] Status
    /// </summary>
    [JsonPropertyName("status")]
    public OrderStatus Status { get; set; }
    /// <summary>
    /// ["<c>trigger_status</c>"] Trigger status
    /// </summary>
    [JsonPropertyName("trigger_status")]
    public TriggerStatus? TriggerStatus { get; set; }
    /// <summary>
    /// ["<c>trigger_time</c>"] Trigger time
    /// </summary>
    [JsonPropertyName("trigger_time")]
    public DateTime? TriggerTime { get; set; }
    /// <summary>
    /// ["<c>parent_order_index</c>"] Parent order index
    /// </summary>
    [JsonPropertyName("parent_order_index")]
    public long ParentOrderIndex { get; set; }
    /// <summary>
    /// ["<c>parent_order_id</c>"] Parent order id
    /// </summary>
    [JsonPropertyName("parent_order_id")]
    public long ParentOrderId { get; set; }
    /// <summary>
    /// ["<c>to_trigger_order_id_0</c>"] To trigger order id0
    /// </summary>
    [JsonPropertyName("to_trigger_order_id_0")]
    public long ToTriggerOrderId0 { get; set; }
    /// <summary>
    /// ["<c>to_trigger_order_id_1</c>"] To trigger order id1
    /// </summary>
    [JsonPropertyName("to_trigger_order_id_1")]
    public long ToTriggerOrderId1 { get; set; }
    /// <summary>
    /// ["<c>to_cancel_order_id_0</c>"] To cancel order id0
    /// </summary>
    [JsonPropertyName("to_cancel_order_id_0")]
    public long ToCancelOrderId0 { get; set; }
    /// <summary>
    /// ["<c>block_height</c>"] Block height
    /// </summary>
    [JsonPropertyName("block_height")]
    public long BlockHeight { get; set; }
    /// <summary>
    /// ["<c>timestamp</c>"] Timestamp
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }
    /// <summary>
    /// ["<c>created_at</c>"] Create time
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTime CreateTime { get; set; }
    /// <summary>
    /// ["<c>updated_at</c>"] Update time
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTime UpdateTime { get; set; }
    /// <summary>
    /// ["<c>transaction_time</c>"] Transaction time
    /// </summary>
    [JsonPropertyName("transaction_time")]
    public DateTime? TransactionTime { get; set; }
    /// <summary>
    /// ["<c>integrator_fee_collector_index</c>"] Integrator fee collector index
    /// </summary>
    [JsonPropertyName("integrator_fee_collector_index")]
    public long? IntegratorFeeCollectorIndex { get; set; }
    /// <summary>
    /// ["<c>integrator_maker_fee</c>"] Integrator maker fee
    /// </summary>
    [JsonPropertyName("integrator_maker_fee")]
    public decimal? IntegratorMakerFee { get; set; }
    /// <summary>
    /// ["<c>integrator_taker_fee</c>"] Integrator taker fee
    /// </summary>
    [JsonPropertyName("integrator_taker_fee")]
    public decimal? IntegratorTakerFee { get; set; }
    /// <summary>
    /// ["<c>order_flags</c>"] Order flags
    /// </summary>
    [JsonPropertyName("order_flags")]
    public long? OrderFlags { get; set; }
}

