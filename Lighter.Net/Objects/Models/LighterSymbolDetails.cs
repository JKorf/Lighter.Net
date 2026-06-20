using System;
using System.Text.Json.Serialization;
using Lighter.Net.Enums;

namespace Lighter.Net.Objects.Models;

/// <summary>
/// Symbol details
/// </summary>
public record LighterSymbolDetails : LighterResponse
{
    /// <summary>
    /// ["<c>order_book_details</c>"] Perp futures symbols
    /// </summary>
    [JsonPropertyName("order_book_details")]
    public LighterSymbolPerpDetails[] PerpSymbols { get; set; } = [];
    /// <summary>
    /// ["<c>spot_order_book_details</c>"] Spot symbols
    /// </summary>
    [JsonPropertyName("spot_order_book_details")]
    public LighterSymbolSpotDetails[] SpotSymbols { get; set; } = [];
}

/// <summary>
/// Perp symbol details
/// </summary>
public record LighterSymbolPerpDetails
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
    /// ["<c>market_type</c>"] Symbol type
    /// </summary>
    [JsonPropertyName("market_type")]
    public SymbolType SymbolType { get; set; }
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
    /// ["<c>is_taker_fee_enabled</c>"] Taker fee enabled
    /// </summary>
    [JsonPropertyName("is_taker_fee_enabled")]
    public bool IsTakerFeeEnabled { get; set; }
    /// <summary>
    /// ["<c>maker_fee</c>"] Maker fee
    /// </summary>
    [JsonPropertyName("maker_fee")]
    public decimal MakerFee { get; set; }
    /// <summary>
    /// ["<c>is_maker_fee_enabled</c>"] Maker fee enabled
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
    /// ["<c>order_quote_limit</c>"] Order quote limit
    /// </summary>
    [JsonPropertyName("order_quote_limit")]
    public decimal OrderQuoteLimit { get; set; }
    /// <summary>
    /// ["<c>size_decimals</c>"] Quantity decimals
    /// </summary>
    [JsonPropertyName("size_decimals")]
    public int QuantityDecimals { get; set; }
    /// <summary>
    /// ["<c>price_decimals</c>"] Price decimals
    /// </summary>
    [JsonPropertyName("price_decimals")]
    public int PriceDecimals { get; set; }
    /// <summary>
    /// ["<c>quote_multiplier</c>"] Quote multiplier
    /// </summary>
    [JsonPropertyName("quote_multiplier")]
    public decimal QuoteMultiplier { get; set; }
    /// <summary>
    /// ["<c>default_initial_margin_fraction</c>"] Default initial margin fraction
    /// </summary>
    [JsonPropertyName("default_initial_margin_fraction")]
    public decimal DefaultInitialMarginFraction { get; set; }
    /// <summary>
    /// ["<c>min_initial_margin_fraction</c>"] Min initial margin fraction
    /// </summary>
    [JsonPropertyName("min_initial_margin_fraction")]
    public decimal MinInitialMarginFraction { get; set; }
    /// <summary>
    /// ["<c>maintenance_margin_fraction</c>"] Maintenance margin fraction
    /// </summary>
    [JsonPropertyName("maintenance_margin_fraction")]
    public decimal MaintenanceMarginFraction { get; set; }
    /// <summary>
    /// ["<c>closeout_margin_fraction</c>"] Closeout margin fraction
    /// </summary>
    [JsonPropertyName("closeout_margin_fraction")]
    public decimal CloseoutMarginFraction { get; set; }
    /// <summary>
    /// ["<c>last_trade_price</c>"] Last trade price
    /// </summary>
    [JsonPropertyName("last_trade_price")]
    public decimal LastPrice { get; set; }
    /// <summary>
    /// ["<c>daily_trades_count</c>"] 24h trades count
    /// </summary>
    [JsonPropertyName("daily_trades_count")]
    public int TradeCount { get; set; }
    /// <summary>
    /// ["<c>daily_base_token_volume</c>"] 24h base token volume
    /// </summary>
    [JsonPropertyName("daily_base_token_volume")]
    public decimal Volume { get; set; }
    /// <summary>
    /// ["<c>daily_quote_token_volume</c>"] 24h quote token volume
    /// </summary>
    [JsonPropertyName("daily_quote_token_volume")]
    public decimal QuoteVolume { get; set; }
    /// <summary>
    /// ["<c>daily_price_low</c>"] 24h low price
    /// </summary>
    [JsonPropertyName("daily_price_low")]
    public decimal LowPrice { get; set; }
    /// <summary>
    /// ["<c>daily_price_high</c>"] 24h high price
    /// </summary>
    [JsonPropertyName("daily_price_high")]
    public decimal HighPrice { get; set; }
    /// <summary>
    /// ["<c>daily_price_change</c>"] 24h price change
    /// </summary>
    [JsonPropertyName("daily_price_change")]
    public decimal PriceChangePercentage { get; set; }
    /// <summary>
    /// ["<c>open_interest</c>"] Open interest
    /// </summary>
    [JsonPropertyName("open_interest")]
    public decimal OpenInterest { get; set; }
    /// <summary>
    /// ["<c>market_config</c>"] Market config
    /// </summary>
    [JsonPropertyName("market_config")]
    public LighterSymbolConfig MarketConfig { get; set; } = null!;
    /// <summary>
    /// ["<c>strategy_index</c>"] Strategy index
    /// </summary>
    [JsonPropertyName("strategy_index")]
    public long StrategyIndex { get; set; }
    /// <summary>
    /// ["<c>funding_clamp_small</c>"] Funding clamp small
    /// </summary>
    [JsonPropertyName("funding_clamp_small")]
    public decimal FundingClampSmall { get; set; }
    /// <summary>
    /// ["<c>funding_clamp_big</c>"] Funding clamp big
    /// </summary>
    [JsonPropertyName("funding_clamp_big")]
    public decimal FundingClampBig { get; set; }
    /// <summary>
    /// ["<c>base_interest_rate</c>"] Base interest rate
    /// </summary>
    [JsonPropertyName("base_interest_rate")]
    public decimal BaseInterestRate { get; set; }
    /// <summary>
    /// ["<c>created_at</c>"] Create time
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTime CreateTime { get; set; }
}

/// <summary>
/// Symbol config
/// </summary>
public record LighterSymbolConfig
{
    /// <summary>
    /// ["<c>market_margin_mode</c>"] Margin mode enabled
    /// </summary>
    [JsonPropertyName("market_margin_mode")]
    public bool MarginMode { get; set; }
    /// <summary>
    /// ["<c>insurance_fund_account_index</c>"] Insurance fund account index
    /// </summary>
    [JsonPropertyName("insurance_fund_account_index")]
    public long InsuranceFundAccountIndex { get; set; }
    /// <summary>
    /// ["<c>liquidation_mode</c>"] Liquidation mode
    /// </summary>
    [JsonPropertyName("liquidation_mode")]
    public bool LiquidationMode { get; set; }
    /// <summary>
    /// ["<c>force_reduce_only</c>"] Force reduce only
    /// </summary>
    [JsonPropertyName("force_reduce_only")]
    public bool ForceReduceOnly { get; set; }
    /// <summary>
    /// ["<c>funding_fee_discounts_enabled</c>"] Funding fee discounts enabled
    /// </summary>
    [JsonPropertyName("funding_fee_discounts_enabled")]
    public bool FundingFeeDiscountsEnabled { get; set; }
    /// <summary>
    /// ["<c>trading_hours</c>"] Trading hours
    /// </summary>
    [JsonPropertyName("trading_hours")]
    public string? TradingHours { get; set; }
    /// <summary>
    /// ["<c>hidden</c>"] Hidden
    /// </summary>
    [JsonPropertyName("hidden")]
    public bool Hidden { get; set; }
    /// <summary>
    /// ["<c>rfq_enabled</c>"] RFQ enabled
    /// </summary>
    [JsonPropertyName("rfq_enabled")]
    public bool RfqEnabled { get; set; }
}

/// <summary>
/// Spot symbol details
/// </summary>
public record LighterSymbolSpotDetails
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
    public long MarketId { get; set; }
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
    /// ["<c>is_taker_fee_enabled</c>"] Taker fee enabled
    /// </summary>
    [JsonPropertyName("is_taker_fee_enabled")]
    public bool IsTakerFeeEnabled { get; set; }
    /// <summary>
    /// ["<c>maker_fee</c>"] Maker fee
    /// </summary>
    [JsonPropertyName("maker_fee")]
    public decimal MakerFee { get; set; }
    /// <summary>
    /// ["<c>is_maker_fee_enabled</c>"] Maker fee enabled
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
    /// ["<c>size_decimals</c>"] Quantity decimals
    /// </summary>
    [JsonPropertyName("size_decimals")]
    public int QuantityDecimals { get; set; }
    /// <summary>
    /// ["<c>price_decimals</c>"] Price decimals
    /// </summary>
    [JsonPropertyName("price_decimals")]
    public int PriceDecimals { get; set; }
    /// <summary>
    /// ["<c>last_trade_price</c>"] Last trade price
    /// </summary>
    [JsonPropertyName("last_trade_price")]
    public decimal LastPrice { get; set; }
    /// <summary>
    /// ["<c>daily_trades_count</c>"] 24h trade count
    /// </summary>
    [JsonPropertyName("daily_trades_count")]
    public decimal TradesCount { get; set; }
    /// <summary>
    /// ["<c>daily_base_token_volume</c>"] 24h volume in base asset
    /// </summary>
    [JsonPropertyName("daily_base_token_volume")]
    public decimal Volume { get; set; }
    /// <summary>
    /// ["<c>daily_quote_token_volume</c>"] 24h volume in quote asset
    /// </summary>
    [JsonPropertyName("daily_quote_token_volume")]
    public decimal QuoteVolume { get; set; }
    /// <summary>
    /// ["<c>daily_price_low</c>"] 24h low price
    /// </summary>
    [JsonPropertyName("daily_price_low")]
    public decimal LowPrice { get; set; }
    /// <summary>
    /// ["<c>daily_price_high</c>"] 24h high price
    /// </summary>
    [JsonPropertyName("daily_price_high")]
    public decimal HighPrice { get; set; }
    /// <summary>
    /// ["<c>daily_price_change</c>"] 24h price change
    /// </summary>
    [JsonPropertyName("daily_price_change")]
    public decimal PriceChangePercentage { get; set; }
    /// <summary>
    /// ["<c>created_at</c>"] Create time
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTime CreateTime { get; set; }
}

