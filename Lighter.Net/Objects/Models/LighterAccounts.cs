using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Lighter.Net.Enums;

namespace Lighter.Net.Objects.Models;

/// <summary>
/// Accounts
/// </summary>
public record LighterAccounts : LighterResponse
{
    /// <summary>
    /// ["<c>total</c>"] Total
    /// </summary>
    [JsonPropertyName("total")]
    public int Total { get; set; }
    /// <summary>
    /// ["<c>accounts</c>"] Accounts
    /// </summary>
    [JsonPropertyName("accounts")]
    public LighterAccountDetails[] Accounts { get; set; } = [];
    /// <summary>
    /// ["<c>next_cursor</c>"] Next cursor
    /// </summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }
}

/// <summary>
/// Account info
/// </summary>
public record LighterAccountDetails
{
    /// <summary>
    /// ["<c>account_type</c>"] Account type
    /// </summary>
    [JsonPropertyName("account_type")]
    public int AccountType { get; set; }
    /// <summary>
    /// ["<c>account_trading_mode</c>"] Account trading mode
    /// </summary>
    [JsonPropertyName("account_trading_mode")]
    public TradeMode TradingMode { get; set; }
    /// <summary>
    /// ["<c>index</c>"] Index
    /// </summary>
    [JsonPropertyName("index")]
    public long Index { get; set; }
    /// <summary>
    /// ["<c>l1_address</c>"] L1 address
    /// </summary>
    [JsonPropertyName("l1_address")]
    public string L1Address { get; set; } = string.Empty;
    /// <summary>
    /// ["<c>cancel_all_time</c>"] Cancel all time
    /// </summary>
    [JsonPropertyName("cancel_all_time")]
    public DateTime? CancelAllTime { get; set; }
    /// <summary>
    /// ["<c>total_order_count</c>"] Total order count
    /// </summary>
    [JsonPropertyName("total_order_count")]
    public int TotalOrderCount { get; set; }
    /// <summary>
    /// ["<c>total_isolated_order_count</c>"] Total isolated order count
    /// </summary>
    [JsonPropertyName("total_isolated_order_count")]
    public int TotalIsolatedOrderCount { get; set; }
    /// <summary>
    /// ["<c>pending_order_count</c>"] Pending order count
    /// </summary>
    [JsonPropertyName("pending_order_count")]
    public int PendingOrderCount { get; set; }
    /// <summary>
    /// ["<c>available_balance</c>"] Available balance
    /// </summary>
    [JsonPropertyName("available_balance")]
    public decimal AvailableBalance { get; set; }
    /// <summary>
    /// ["<c>status</c>"] Active
    /// </summary>
    [JsonPropertyName("status")]
    public bool Active { get; set; }
    /// <summary>
    /// ["<c>collateral</c>"] Collateral
    /// </summary>
    [JsonPropertyName("collateral")]
    public decimal Collateral { get; set; }
    /// <summary>
    /// ["<c>account_index</c>"] Account index
    /// </summary>
    [JsonPropertyName("account_index")]
    public long AccountIndex { get; set; }
    /// <summary>
    /// ["<c>name</c>"] Name
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// ["<c>description</c>"] Description
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
    /// <summary>
    /// ["<c>can_invite</c>"] Can invite
    /// </summary>
    [JsonPropertyName("can_invite")]
    public bool CanInvite { get; set; }
    /// <summary>
    /// ["<c>referral_points_percentage</c>"] Referral points percentage
    /// </summary>
    [JsonPropertyName("referral_points_percentage")]
    public decimal? ReferralPointsPercentage { get; set; }
    /// <summary>
    /// ["<c>positions</c>"] Positions
    /// </summary>
    [JsonPropertyName("positions")]
    public LighterPosition[] Positions { get; set; } = [];
    /// <summary>
    /// ["<c>assets</c>"] Assets
    /// </summary>
    [JsonPropertyName("assets")]
    public LighterAccountBalance[] Assets { get; set; } = [];
    /// <summary>
    /// ["<c>total_asset_value</c>"] Total asset value
    /// </summary>
    [JsonPropertyName("total_asset_value")]
    public decimal TotalAssetValue { get; set; }
    /// <summary>
    /// ["<c>cross_asset_value</c>"] Cross asset value
    /// </summary>
    [JsonPropertyName("cross_asset_value")]
    public decimal CrossAssetValue { get; set; }
    /// <summary>
    /// ["<c>pool_info</c>"] Pool info
    /// </summary>
    [JsonPropertyName("pool_info")]
    public LighterAccountPoolInfo PoolInfo { get; set; } = null!;
    /// <summary>
    /// ["<c>shares</c>"] Shares
    /// </summary>
    [JsonPropertyName("shares")]
    public LighterAccountsShare[] Shares { get; set; } = [];
    /// <summary>
    /// ["<c>created_at</c>"] Create time
    /// </summary>
    [JsonPropertyName("created_at")]
    public decimal CreateTime { get; set; }
    /// <summary>
    /// ["<c>transaction_time</c>"] Transaction time
    /// </summary>
    [JsonPropertyName("transaction_time")]
    public DateTime TransactionTime { get; set; }
    /// <summary>
    /// ["<c>pending_unlocks</c>"] Pending unlocks
    /// </summary>
    [JsonPropertyName("pending_unlocks")]
    public LighterAccountPendingUnlock[] PendingUnlocks { get; set; } = [];
    /// <summary>
    /// ["<c>approved_integrators</c>"] Approved integrators
    /// </summary>
    [JsonPropertyName("approved_integrators")]
    public LighterAccountsIntegrator[] ApprovedIntegrators { get; set; } = [];
    /// <summary>
    /// ["<c>can_rfq</c>"] Can rfq
    /// </summary>
    [JsonPropertyName("can_rfq")]
    public bool CanRfq { get; set; }
    /// <summary>
    /// ["<c>cross_initial_margin_requirement</c>"] Cross initial margin requirement
    /// </summary>
    [JsonPropertyName("cross_initial_margin_requirement")]
    public decimal CrossInitialMarginRequirement { get; set; }
    /// <summary>
    /// ["<c>cross_maintenance_margin_requirement</c>"] Cross maintenance margin requirement
    /// </summary>
    [JsonPropertyName("cross_maintenance_margin_requirement")]
    public decimal CrossMaintenanceMarginRequirement { get; set; }
    /// <summary>
    /// ["<c>can_rfq_market_ids</c>"] Can rfq market ids
    /// </summary>
    [JsonPropertyName("can_rfq_market_ids")]
    public string[] CanRfqMarketIds { get; set; } = [];
    /// <summary>
    /// ["<c>metadata</c>"] Metadata
    /// </summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object> Metadata { get; set; } = null!;
}

/// <summary>
/// Position info
/// </summary>
public record LighterPosition
{
    /// <summary>
    /// ["<c>market_id</c>"] Market id
    /// </summary>
    [JsonPropertyName("market_id")]
    public int MarketId { get; set; }
    /// <summary>
    /// ["<c>symbol</c>"] Symbol
    /// </summary>
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;
    /// <summary>
    /// ["<c>initial_margin_fraction</c>"] Initial margin fraction
    /// </summary>
    [JsonPropertyName("initial_margin_fraction")]
    public decimal InitialMarginFraction { get; set; }
    /// <summary>
    /// ["<c>open_order_count</c>"] Open order count
    /// </summary>
    [JsonPropertyName("open_order_count")]
    public int OpenOrderCount { get; set; }
    /// <summary>
    /// ["<c>pending_order_count</c>"] Pending order count
    /// </summary>
    [JsonPropertyName("pending_order_count")]
    public int PendingOrderCount { get; set; }
    /// <summary>
    /// ["<c>position_tied_order_count</c>"] Position tied order count
    /// </summary>
    [JsonPropertyName("position_tied_order_count")]
    public int PositionTiedOrderCount { get; set; }
    /// <summary>
    /// ["<c>sign</c>"] Position side
    /// </summary>
    [JsonPropertyName("sign")]
    public PositionSide PositionSide { get; set; }
    /// <summary>
    /// ["<c>position</c>"] Position
    /// </summary>
    [JsonPropertyName("position")]
    public decimal Position { get; set; }
    /// <summary>
    /// ["<c>avg_entry_price</c>"] Average entry price
    /// </summary>
    [JsonPropertyName("avg_entry_price")]
    public decimal AverageEntryPrice { get; set; }
    /// <summary>
    /// ["<c>position_value</c>"] Position value
    /// </summary>
    [JsonPropertyName("position_value")]
    public decimal PositionValue { get; set; }
    /// <summary>
    /// ["<c>unrealized_pnl</c>"] Unrealized pnl
    /// </summary>
    [JsonPropertyName("unrealized_pnl")]
    public decimal UnrealizedPnl { get; set; }
    /// <summary>
    /// ["<c>realized_pnl</c>"] Realized pnl
    /// </summary>
    [JsonPropertyName("realized_pnl")]
    public decimal RealizedPnl { get; set; }
    /// <summary>
    /// ["<c>liquidation_price</c>"] Liquidation price
    /// </summary>
    [JsonPropertyName("liquidation_price")]
    public decimal LiquidationPrice { get; set; }
    /// <summary>
    /// ["<c>total_funding_paid_out</c>"] Total funding paid out
    /// </summary>
    [JsonPropertyName("total_funding_paid_out")]
    public decimal TotalFundingPaidOut { get; set; }
    /// <summary>
    /// ["<c>margin_mode</c>"] Margin mode
    /// </summary>
    [JsonPropertyName("margin_mode")]
    public MarginMode MarginMode { get; set; }
    /// <summary>
    /// ["<c>allocated_margin</c>"] Allocated margin
    /// </summary>
    [JsonPropertyName("allocated_margin")]
    public decimal AllocatedMargin { get; set; }
    /// <summary>
    /// ["<c>total_discount</c>"] Total discount
    /// </summary>
    [JsonPropertyName("total_discount")]
    public decimal? TotalDiscount { get; set; }
}

/// <summary>
/// Account balance
/// </summary>
public record LighterAccountBalance
{
    /// <summary>
    /// ["<c>symbol</c>"] Symbol
    /// </summary>
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;
    /// <summary>
    /// ["<c>asset_id</c>"] Asset id
    /// </summary>
    [JsonPropertyName("asset_id")]
    public long AssetId { get; set; }
    /// <summary>
    /// ["<c>balance</c>"] Balance
    /// </summary>
    [JsonPropertyName("balance")]
    public decimal Balance { get; set; }
    /// <summary>
    /// ["<c>locked_balance</c>"] Locked balance
    /// </summary>
    [JsonPropertyName("locked_balance")]
    public decimal LockedBalance { get; set; }
    /// <summary>
    /// ["<c>margin_balance</c>"] Margin balance
    /// </summary>
    [JsonPropertyName("margin_balance")]
    public decimal MarginBalance { get; set; }
    /// <summary>
    /// ["<c>margin_mode</c>"] Margin mode
    /// </summary>
    [JsonPropertyName("margin_mode")]
    public bool? MarginMode { get; set; }
}

/// <summary>
/// Pool info
/// </summary>
public record LighterAccountPoolInfo
{
    /// <summary>
    /// ["<c>status</c>"] Status
    /// </summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }
    /// <summary>
    /// ["<c>operator_fee</c>"] Operator fee
    /// </summary>
    [JsonPropertyName("operator_fee")]
    public decimal OperatorFee { get; set; }
    /// <summary>
    /// ["<c>min_operator_share_rate</c>"] Min operator share rate
    /// </summary>
    [JsonPropertyName("min_operator_share_rate")]
    public decimal MinOperatorShareRate { get; set; }
    /// <summary>
    /// ["<c>total_shares</c>"] Total shares
    /// </summary>
    [JsonPropertyName("total_shares")]
    public decimal TotalShares { get; set; }
    /// <summary>
    /// ["<c>operator_shares</c>"] Operator shares
    /// </summary>
    [JsonPropertyName("operator_shares")]
    public decimal OperatorShares { get; set; }
    /// <summary>
    /// ["<c>annual_percentage_yield</c>"] Annual percentage yield
    /// </summary>
    [JsonPropertyName("annual_percentage_yield")]
    public decimal AnnualPercentageYield { get; set; }
    /// <summary>
    /// ["<c>daily_returns</c>"] Daily returns
    /// </summary>
    [JsonPropertyName("daily_returns")]
    public LighterAccountsPoolReturns[] DailyReturns { get; set; } = [];
    /// <summary>
    /// ["<c>share_prices</c>"] Share prices
    /// </summary>
    [JsonPropertyName("share_prices")]
    public LighterAccountsPoolSharePrice[] SharePrices { get; set; } = [];
    /// <summary>
    /// ["<c>sharpe_ratio</c>"] Sharpe ratio
    /// </summary>
    [JsonPropertyName("sharpe_ratio")]
    public decimal SharpeRatio { get; set; }
    /// <summary>
    /// ["<c>strategies</c>"] Strategies
    /// </summary>
    [JsonPropertyName("strategies")]
    public LighterAccountsPoolStrategy[] Strategies { get; set; } = [];
}

/// <summary>
/// Return
/// </summary>
public record LighterAccountsPoolReturns
{
    /// <summary>
    /// ["<c>timestamp</c>"] Timestamp
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }
    /// <summary>
    /// ["<c>daily_return</c>"] Daily return
    /// </summary>
    [JsonPropertyName("daily_return")]
    public decimal DailyReturn { get; set; }
}

/// <summary>
/// Share price
/// </summary>
public record LighterAccountsPoolSharePrice
{
    /// <summary>
    /// ["<c>timestamp</c>"] Timestamp
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }
    /// <summary>
    /// ["<c>share_price</c>"] Share price
    /// </summary>
    [JsonPropertyName("share_price")]
    public decimal SharePrice { get; set; }
}

/// <summary>
/// Pool strategy
/// </summary>
public record LighterAccountsPoolStrategy
{
    /// <summary>
    /// ["<c>collateral</c>"] Collateral
    /// </summary>
    [JsonPropertyName("collateral")]
    public string Collateral { get; set; } = string.Empty;
}

/// <summary>
/// Share
/// </summary>
public record LighterAccountsShare
{
    /// <summary>
    /// ["<c>public_pool_index</c>"] Public pool index
    /// </summary>
    [JsonPropertyName("public_pool_index")]
    public long PublicPoolIndex { get; set; }
    /// <summary>
    /// ["<c>shares_amount</c>"] Shares quantity
    /// </summary>
    [JsonPropertyName("shares_amount")]
    public decimal SharesQuantity { get; set; }
    /// <summary>
    /// ["<c>entry_usdc</c>"] Entry usdc
    /// </summary>
    [JsonPropertyName("entry_usdc")]
    public decimal EntryUsdc { get; set; }
    /// <summary>
    /// ["<c>entry_timestamp</c>"] Entry timestamp
    /// </summary>
    [JsonPropertyName("entry_timestamp")]
    public DateTime EntryTimestamp { get; set; }
    /// <summary>
    /// ["<c>principal_amount</c>"] Principal quantity
    /// </summary>
    [JsonPropertyName("principal_amount")]
    public decimal PrincipalQuantity { get; set; }
}

/// <summary>
/// Pending unlock
/// </summary>
public record LighterAccountPendingUnlock
{
    /// <summary>
    /// ["<c>unlock_timestamp</c>"] Unlock timestamp
    /// </summary>
    [JsonPropertyName("unlock_timestamp")]
    public DateTime UnlockTimestamp { get; set; }
    /// <summary>
    /// ["<c>asset_index</c>"] Asset index
    /// </summary>
    [JsonPropertyName("asset_index")]
    public long AssetIndex { get; set; }
    /// <summary>
    /// ["<c>amount</c>"] Quantity
    /// </summary>
    [JsonPropertyName("amount")]
    public decimal Quantity { get; set; }
}

/// <summary>
/// Integrator info
/// </summary>
public record LighterAccountsIntegrator
{
    /// <summary>
    /// ["<c>account_index</c>"] Account index
    /// </summary>
    [JsonPropertyName("account_index")]
    public long AccountIndex { get; set; }
    /// <summary>
    /// ["<c>name</c>"] Name
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// ["<c>max_perps_taker_fee</c>"] Max perps taker fee
    /// </summary>
    [JsonPropertyName("max_perps_taker_fee")]
    public decimal MaxPerpsTakerFee { get; set; }
    /// <summary>
    /// ["<c>max_perps_maker_fee</c>"] Max perps maker fee
    /// </summary>
    [JsonPropertyName("max_perps_maker_fee")]
    public decimal MaxPerpsMakerFee { get; set; }
    /// <summary>
    /// ["<c>max_spot_taker_fee</c>"] Max spot taker fee
    /// </summary>
    [JsonPropertyName("max_spot_taker_fee")]
    public decimal MaxSpotTakerFee { get; set; }
    /// <summary>
    /// ["<c>max_spot_maker_fee</c>"] Max spot maker fee
    /// </summary>
    [JsonPropertyName("max_spot_maker_fee")]
    public decimal MaxSpotMakerFee { get; set; }
    /// <summary>
    /// ["<c>approval_expiry</c>"] Approval expiry
    /// </summary>
    [JsonPropertyName("approval_expiry")]
    public DateTime ApprovalExpiry { get; set; }
}
