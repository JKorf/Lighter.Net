using System;
using System.Text.Json.Serialization;
using CryptoExchange.Net.Interfaces;

namespace Lighter.Net.Objects.Models;

/// <summary>
/// Order book
/// </summary>
public record LighterOrderBook : LighterResponse
{
    /// <summary>
    /// ["<c>total_asks</c>"] Total asks
    /// </summary>
    [JsonPropertyName("total_asks")]
    public int TotalAsks { get; set; }
    /// <summary>
    /// ["<c>asks</c>"] Asks
    /// </summary>
    [JsonPropertyName("asks")]
    public LighterOrderBookEntry[] Asks { get; set; } = [];
    /// <summary>
    /// ["<c>total_bids</c>"] Total bids
    /// </summary>
    [JsonPropertyName("total_bids")]
    public int TotalBids { get; set; }
    /// <summary>
    /// ["<c>bids</c>"] Bids
    /// </summary>
    [JsonPropertyName("bids")]
    public LighterOrderBookEntry[] Bids { get; set; } = [];
}

/// <summary>
/// Order book entry
/// </summary>
public record LighterOrderBookEntry : ISymbolOrderBookEntry
{
    /// <summary>
    /// ["<c>order_index</c>"] Order index
    /// </summary>
    [JsonPropertyName("order_index")]
    public long OrderIndex { get; set; }
    /// <summary>
    /// ["<c>order_id</c>"] Order id
    /// </summary>
    [JsonPropertyName("order_id")]
    public long OrderId { get; set; }
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
    /// ["<c>remaining_base_amount</c>"] Remaining base quantity
    /// </summary>
    [JsonPropertyName("remaining_base_amount")]
    public decimal Quantity { get; set; }
    /// <summary>
    /// ["<c>price</c>"] Price
    /// </summary>
    [JsonPropertyName("price")]
    public decimal Price { get; set; }
    /// <summary>
    /// ["<c>order_expiry</c>"] Order expiry
    /// </summary>
    [JsonPropertyName("order_expiry")]
    public DateTime? OrderExpiry { get; set; }
}