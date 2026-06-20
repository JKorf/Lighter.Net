using System;
using System.Text.Json.Serialization;
using Lighter.Net.Enums;

namespace Lighter.Net.Objects.Models;

/// <summary>
/// Liquidations
/// </summary>
public record LighterLiquidations : LighterResponse
{
    /// <summary>
    /// ["<c>liquidations</c>"] Liquidations
    /// </summary>
    [JsonPropertyName("liquidations")]
    public LighterLiquidation[] Liquidations { get; set; } = [];
    /// <summary>
    /// ["<c>next_cursor</c>"] Next cursor
    /// </summary>
    [JsonPropertyName("next_cursor")]
    public string NextCursor { get; set; } = string.Empty;
}

/// <summary>
/// Liquidation info
/// </summary>
public record LighterLiquidation
{
    /// <summary>
    /// ["<c>id</c>"] Id
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; set; }
    /// <summary>
    /// ["<c>market_id</c>"] Market id
    /// </summary>
    [JsonPropertyName("market_id")]
    public long MarketId { get; set; }
    /// <summary>
    /// ["<c>type</c>"] Type
    /// </summary>
    [JsonPropertyName("type")]
    public LiquidationType Type { get; set; }
    /// <summary>
    /// ["<c>trade</c>"] Trade
    /// </summary>
    [JsonPropertyName("trade")]
    public LighterLiquidationTrade Trade { get; set; } = null!;
    /// <summary>
    /// ["<c>info</c>"] Info
    /// </summary>
    [JsonPropertyName("info")]
    public LighterLiquidationInfo Info { get; set; } = null!;
    /// <summary>
    /// ["<c>executed_at</c>"] ExecuteTime
    /// </summary>
    [JsonPropertyName("executed_at")]
    public DateTime ExecuteTime { get; set; }
}

/// <summary>
/// Trade info
/// </summary>
public record LighterLiquidationTrade
{
    /// <summary>
    /// ["<c>price</c>"] Price
    /// </summary>
    [JsonPropertyName("price")]
    public decimal Price { get; set; }
    /// <summary>
    /// ["<c>size</c>"] Quantity
    /// </summary>
    [JsonPropertyName("size")]
    public decimal Quantity { get; set; }
    /// <summary>
    /// ["<c>taker_fee</c>"] Taker fee
    /// </summary>
    [JsonPropertyName("taker_fee")]
    public decimal TakerFee { get; set; }
    /// <summary>
    /// ["<c>maker_fee</c>"] Maker fee
    /// </summary>
    [JsonPropertyName("maker_fee")]
    public decimal MakerFee { get; set; }
    /// <summary>
    /// ["<c>transaction_time</c>"] Transaction time
    /// </summary>
    [JsonPropertyName("transaction_time")]
    public DateTime TransactionTime { get; set; }
}

/// <summary>
/// Liquidation info
/// </summary>
public record LighterLiquidationInfo
{
    /// <summary>
    /// ["<c>positions</c>"] Positions
    /// </summary>
    [JsonPropertyName("positions")]
    public LighterPosition[] Positions { get; set; } = [];
    /// <summary>
    /// ["<c>risk_info_before</c>"] Risk info before
    /// </summary>
    [JsonPropertyName("risk_info_before")]
    public LighterLiquidationRiskInfo RiskInfoBefore { get; set; } = null!;
    /// <summary>
    /// ["<c>risk_info_after</c>"] Risk info after
    /// </summary>
    [JsonPropertyName("risk_info_after")]
    public LighterLiquidationRiskInfo RiskInfoAfter { get; set; } = null!;
    /// <summary>
    /// ["<c>assets</c>"] Assets
    /// </summary>
    [JsonPropertyName("assets")]
    public LighterAccountBalance[] Balances { get; set; } = [];
}

/// <summary>
/// Risk info
/// </summary>
public record LighterLiquidationRiskInfo
{
    /// <summary>
    /// ["<c>cross_risk_parameters</c>"] Cross risk parameters
    /// </summary>
    [JsonPropertyName("cross_risk_parameters")]
    public LighterLiquidationRiskParameters CrossRiskParameters { get; set; } = null!;
    /// <summary>
    /// ["<c>isolated_risk_parameters</c>"] Isolated risk parameters
    /// </summary>
    [JsonPropertyName("isolated_risk_parameters")]
    public LighterLiquidationRiskParameters[] IsolatedRiskParameters { get; set; } = [];
}

/// <summary>
/// Risk parameters
/// </summary>
public record LighterLiquidationRiskParameters
{
    /// <summary>
    /// ["<c>market_id</c>"] Market id
    /// </summary>
    [JsonPropertyName("market_id")]
    public long MarketId { get; set; }
    /// <summary>
    /// ["<c>collateral</c>"] Collateral
    /// </summary>
    [JsonPropertyName("collateral")]
    public decimal Collateral { get; set; }
    /// <summary>
    /// ["<c>total_account_value</c>"] Total account value
    /// </summary>
    [JsonPropertyName("total_account_value")]
    public decimal TotalAccountValue { get; set; }
    /// <summary>
    /// ["<c>initial_margin_req</c>"] Initial margin req
    /// </summary>
    [JsonPropertyName("initial_margin_req")]
    public decimal InitialMarginReq { get; set; }
    /// <summary>
    /// ["<c>maintenance_margin_req</c>"] Maintenance margin req
    /// </summary>
    [JsonPropertyName("maintenance_margin_req")]
    public decimal MaintenanceMarginReq { get; set; }
    /// <summary>
    /// ["<c>close_out_margin_req</c>"] Close out margin req
    /// </summary>
    [JsonPropertyName("close_out_margin_req")]
    public decimal CloseOutMarginReq { get; set; }
    /// <summary>
    /// ["<c>total_account_liquidation_threshold</c>"] Total account liquidation threshold
    /// </summary>
    [JsonPropertyName("total_account_liquidation_threshold")]
    public decimal TotalAccountLiquidationThreshold { get; set; }
    /// <summary>
    /// ["<c>usdc_collateral_with_funding</c>"] Usdc collateral with funding
    /// </summary>
    [JsonPropertyName("usdc_collateral_with_funding")]
    public decimal UsdcCollateralWithFunding { get; set; }
    /// <summary>
    /// ["<c>usdc_portfolio_value</c>"] Usdc portfolio value
    /// </summary>
    [JsonPropertyName("usdc_portfolio_value")]
    public decimal UsdcPortfolioValue { get; set; }
}