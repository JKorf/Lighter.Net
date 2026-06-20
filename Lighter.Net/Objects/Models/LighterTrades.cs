using System;
using System.Text.Json.Serialization;
using Lighter.Net.Enums;

namespace Lighter.Net.Objects.Models;

/// <summary>
/// Recent trades
/// </summary>
public record LighterTrades : LighterResponse
{
    /// <summary>
    /// ["<c>trades</c>"] Trades
    /// </summary>
    [JsonPropertyName("trades")]
    public LighterTrade[] Trades { get; set; } = [];
}

/// <summary>
/// Trade info
/// </summary>
public record LighterTrade
{
    /// <summary>
    /// ["<c>trade_id</c>"] Trade id
    /// </summary>
    [JsonPropertyName("trade_id")]
    public long TradeId { get; set; }
    /// <summary>
    /// ["<c>tx_hash</c>"] Transaction hash
    /// </summary>
    [JsonPropertyName("tx_hash")]
    public string TransactionHash { get; set; } = string.Empty;
    /// <summary>
    /// ["<c>type</c>"] Type
    /// </summary>
    [JsonPropertyName("type")]
    public TradeType Type { get; set; }
    /// <summary>
    /// ["<c>market_id</c>"] Market id
    /// </summary>
    [JsonPropertyName("market_id")]
    public int MarketId { get; set; }
    /// <summary>
    /// ["<c>size</c>"] Quantity
    /// </summary>
    [JsonPropertyName("size")]
    public decimal Quantity { get; set; }
    /// <summary>
    /// ["<c>price</c>"] Price
    /// </summary>
    [JsonPropertyName("price")]
    public decimal Price { get; set; }
    /// <summary>
    /// ["<c>usd_amount</c>"] Usd quantity
    /// </summary>
    [JsonPropertyName("usd_amount")]
    public decimal UsdQuantity { get; set; }
    /// <summary>
    /// ["<c>ask_id</c>"] Ask id
    /// </summary>
    [JsonPropertyName("ask_id")]
    public long AskId { get; set; }
    /// <summary>
    /// ["<c>bid_id</c>"] Bid id
    /// </summary>
    [JsonPropertyName("bid_id")]
    public long BidId { get; set; }
    /// <summary>
    /// ["<c>ask_account_id</c>"] Ask account id
    /// </summary>
    [JsonPropertyName("ask_account_id")]
    public long AskAccountId { get; set; }
    /// <summary>
    /// ["<c>bid_account_id</c>"] Bid account id
    /// </summary>
    [JsonPropertyName("bid_account_id")]
    public long BidAccountId { get; set; }
    /// <summary>
    /// ["<c>is_maker_ask</c>"] Is maker ask
    /// </summary>
    [JsonPropertyName("is_maker_ask")]
    public bool IsMakerAsk { get; set; }
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
    /// ["<c>taker_fee</c>"] Taker fee
    /// </summary>
    [JsonPropertyName("taker_fee")]
    public decimal TakerFee { get; set; }
    /// <summary>
    /// ["<c>taker_position_size_before</c>"] Taker position quantity before
    /// </summary>
    [JsonPropertyName("taker_position_size_before")]
    public decimal TakerPositionQuantityBefore { get; set; }
    /// <summary>
    /// ["<c>taker_entry_quote_before</c>"] Taker entry quote before
    /// </summary>
    [JsonPropertyName("taker_entry_quote_before")]
    public decimal TakerEntryQuoteBefore { get; set; }
    /// <summary>
    /// ["<c>taker_initial_margin_fraction_before</c>"] Taker initial margin fraction before
    /// </summary>
    [JsonPropertyName("taker_initial_margin_fraction_before")]
    public decimal TakerInitialMarginFractionBefore { get; set; }
    /// <summary>
    /// ["<c>taker_allocated_margin_usdc_before</c>"] Taker allocated margin USDC before
    /// </summary>
    [JsonPropertyName("taker_allocated_margin_usdc_before")]
    public decimal TakerAllocatedMarginUsdcBefore { get; set; }
    /// <summary>
    /// ["<c>taker_allocated_margin_usdc_after</c>"] Taker allocated margin USDC after
    /// </summary>
    [JsonPropertyName("taker_allocated_margin_usdc_after")]
    public decimal TakerAllocatedMarginUsdcAfter { get; set; }
    /// <summary>
    /// ["<c>taker_position_sign_changed</c>"] Taker position sign changed
    /// </summary>
    [JsonPropertyName("taker_position_sign_changed")]
    public bool TakerPositionSignChanged { get; set; }
    /// <summary>
    /// ["<c>maker_fee</c>"] Maker fee
    /// </summary>
    [JsonPropertyName("maker_fee")]
    public decimal MakerFee { get; set; }
    /// <summary>
    /// ["<c>maker_position_size_before</c>"] Maker position quantity before
    /// </summary>
    [JsonPropertyName("maker_position_size_before")]
    public decimal MakerPositionQuantityBefore { get; set; }
    /// <summary>
    /// ["<c>maker_entry_quote_before</c>"] Maker entry quote before
    /// </summary>
    [JsonPropertyName("maker_entry_quote_before")]
    public decimal MakerEntryQuoteBefore { get; set; }
    /// <summary>
    /// ["<c>maker_initial_margin_fraction_before</c>"] Maker initial margin fraction before
    /// </summary>
    [JsonPropertyName("maker_initial_margin_fraction_before")]
    public decimal MakerInitialMarginFractionBefore { get; set; }
    /// <summary>
    /// ["<c>maker_position_sign_changed</c>"] Maker position sign changed
    /// </summary>
    [JsonPropertyName("maker_position_sign_changed")]
    public bool MakerPositionSignChanged { get; set; }
    /// <summary>
    /// ["<c>transaction_time</c>"] Transaction time
    /// </summary>
    [JsonPropertyName("transaction_time")]
    public DateTime TransactionTime { get; set; }
    /// <summary>
    /// ["<c>bid_account_pnl</c>"] Bid account profit and loss
    /// </summary>
    [JsonPropertyName("bid_account_pnl")]
    public decimal BidAccountPnl { get; set; }
    /// <summary>
    /// ["<c>ask_account_pnl</c>"] Ask account profit and loss
    /// </summary>
    [JsonPropertyName("ask_account_pnl")]
    public decimal AskAccountPnl { get; set; }
    /// <summary>
    /// ["<c>ask_client_id</c>"] Ask client id
    /// </summary>
    [JsonPropertyName("ask_client_id")]
    public long AskClientId { get; set; }
    /// <summary>
    /// ["<c>bid_client_id</c>"] Bid client id
    /// </summary>
    [JsonPropertyName("bid_client_id")]
    public long BidClientId { get; set; }
    /// <summary>
    /// ["<c>ask_client_id_str</c>"] Ask client id str
    /// </summary>
    [JsonPropertyName("ask_client_id_str")]
    public string AskClientIdStr { get; set; } = string.Empty;
    /// <summary>
    /// ["<c>bid_client_id_str</c>"] Bid client id str
    /// </summary>
    [JsonPropertyName("bid_client_id_str")]
    public string BidClientIdStr { get; set; } = string.Empty;
    /// <summary>
    /// ["<c>ask_id_str</c>"] Ask id str
    /// </summary>
    [JsonPropertyName("ask_id_str")]
    public string AskIdStr { get; set; } = string.Empty;
    /// <summary>
    /// ["<c>bid_id_str</c>"] Bid id str
    /// </summary>
    [JsonPropertyName("bid_id_str")]
    public string BidIdStr { get; set; } = string.Empty;
    /// <summary>
    /// ["<c>trade_id_str</c>"] Trade id str
    /// </summary>
    [JsonPropertyName("trade_id_str")]
    public string TradeIdStr { get; set; } = string.Empty;
    /// <summary>
    /// ["<c>integrator_maker_fee</c>"] Integrator maker fee
    /// </summary>
    [JsonPropertyName("integrator_maker_fee")]
    public decimal IntegratorMakerFee { get; set; }
    /// <summary>
    /// ["<c>integrator_maker_fee_collector_index</c>"] Integrator maker fee collector index
    /// </summary>
    [JsonPropertyName("integrator_maker_fee_collector_index")]
    public decimal IntegratorMakerFeeCollectorIndex { get; set; }
    /// <summary>
    /// ["<c>integrator_taker_fee</c>"] Integrator taker fee
    /// </summary>
    [JsonPropertyName("integrator_taker_fee")]
    public decimal IntegratorTakerFee { get; set; }
    /// <summary>
    /// ["<c>integrator_taker_fee_collector_index</c>"] Integrator taker fee collector index
    /// </summary>
    [JsonPropertyName("integrator_taker_fee_collector_index")]
    public decimal IntegratorTakerFeeCollectorIndex { get; set; }
}

