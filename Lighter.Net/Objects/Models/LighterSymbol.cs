using System;
using System.Text.Json.Serialization;
using Lighter.Net.Enums;

namespace Lighter.Net.Objects.Models;

/// <summary>
/// Symbols
/// </summary>
public record LighterSymbols : LighterResponse
{
    /// <summary>
    /// ["<c>order_books</c>"] Order books
    /// </summary>
    [JsonPropertyName("order_books")]
    public LighterSymbol[] OrderBooks { get; set; } = [];
}

/// <summary>
/// Symbol info
/// </summary>
public record LighterSymbol
{
    /// <summary>
    /// ["<c>symbol</c>"] Symbol
    /// </summary>
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;
    /// <summary>
    /// ["<c>market_id</c>"] Market id
    /// </summary>
    [JsonPropertyName("market_id")]
    public int MarketId { get; set; }
    /// <summary>
    /// ["<c>market_type</c>"] Market type
    /// </summary>
    [JsonPropertyName("market_type")]
    public SymbolType MarketType { get; set; }
    /// <summary>
    /// ["<c>base_asset_id</c>"] Base asset id
    /// </summary>
    [JsonPropertyName("base_asset_id")]
    public long BaseAssetId { get; set; }
    /// <summary>
    /// ["<c>quote_asset_id</c>"] Quote asset id
    /// </summary>
    [JsonPropertyName("quote_asset_id")]
    public long QuoteAssetId { get; set; }
    /// <summary>
    /// ["<c>status</c>"] Status
    /// </summary>
    [JsonPropertyName("status")]
    public SymbolStatus Status { get; set; }
    /// <summary>
    /// ["<c>taker_fee</c>"] Taker fee
    /// </summary>
    [JsonPropertyName("taker_fee")]
    public decimal TakerFee { get; set; }
    /// <summary>
    /// ["<c>is_taker_fee_enabled</c>"] Is taker fee enabled
    /// </summary>
    [JsonPropertyName("is_taker_fee_enabled")]
    public bool IsTakerFeeEnabled { get; set; }
    /// <summary>
    /// ["<c>maker_fee</c>"] Maker fee
    /// </summary>
    [JsonPropertyName("maker_fee")]
    public decimal MakerFee { get; set; }
    /// <summary>
    /// ["<c>is_maker_fee_enabled</c>"] Is maker fee enabled
    /// </summary>
    [JsonPropertyName("is_maker_fee_enabled")]
    public bool IsMakerFeeEnabled { get; set; }
    /// <summary>
    /// ["<c>liquidation_fee</c>"] Liquidation fee
    /// </summary>
    [JsonPropertyName("liquidation_fee")]
    public decimal LiquidationFee { get; set; }
    /// <summary>
    /// ["<c>min_base_amount</c>"] Min base quantity
    /// </summary>
    [JsonPropertyName("min_base_amount")]
    public decimal MinBaseQuantity { get; set; }
    /// <summary>
    /// ["<c>min_quote_amount</c>"] Min quote quantity
    /// </summary>
    [JsonPropertyName("min_quote_amount")]
    public decimal MinQuoteQuantity { get; set; }
    /// <summary>
    /// ["<c>order_quote_limit</c>"] Order quote limit
    /// </summary>
    [JsonPropertyName("order_quote_limit")]
    public decimal OrderQuoteLimit { get; set; }
    /// <summary>
    /// ["<c>supported_size_decimals</c>"] Supported quantity decimals
    /// </summary>
    [JsonPropertyName("supported_size_decimals")]
    public int SupportedQuantityDecimals { get; set; }
    /// <summary>
    /// ["<c>supported_price_decimals</c>"] Supported price decimals
    /// </summary>
    [JsonPropertyName("supported_price_decimals")]
    public int SupportedPriceDecimals { get; set; }
    /// <summary>
    /// ["<c>supported_quote_decimals</c>"] Supported quote decimals
    /// </summary>
    [JsonPropertyName("supported_quote_decimals")]
    public int SupportedQuoteDecimals { get; set; }
    /// <summary>
    /// ["<c>created_at</c>"] Create time
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTime CreateTime { get; set; }
}

