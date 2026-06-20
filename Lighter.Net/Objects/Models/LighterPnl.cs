using System;
using System.Text.Json.Serialization;
using Lighter.Net.Enums;

namespace Lighter.Net.Objects.Models;

/// <summary>
/// Profit and loss info
/// </summary>
public record LighterPnl : LighterResponse
{
    /// <summary>
    /// ["<c>resolution</c>"] Resolution
    /// </summary>
    [JsonPropertyName("resolution")]
    public Resolution Resolution { get; set; }
    /// <summary>
    /// ["<c>pnl</c>"] Pnl
    /// </summary>
    [JsonPropertyName("pnl")]
    public LighterPnlInfo[] Pnl { get; set; } = [];
}

/// <summary>
/// PNL info
/// </summary>
public record LighterPnlInfo
{
    /// <summary>
    /// ["<c>timestamp</c>"] Timestamp
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }
    /// <summary>
    /// ["<c>trade_pnl</c>"] Trade pnl
    /// </summary>
    [JsonPropertyName("trade_pnl")]
    public decimal TradePnl { get; set; }
    /// <summary>
    /// ["<c>inflow</c>"] Inflow
    /// </summary>
    [JsonPropertyName("inflow")]
    public decimal Inflow { get; set; }
    /// <summary>
    /// ["<c>outflow</c>"] Outflow
    /// </summary>
    [JsonPropertyName("outflow")]
    public decimal Outflow { get; set; }
    /// <summary>
    /// ["<c>pool_pnl</c>"] Pool pnl
    /// </summary>
    [JsonPropertyName("pool_pnl")]
    public decimal PoolPnl { get; set; }
    /// <summary>
    /// ["<c>pool_inflow</c>"] Pool inflow
    /// </summary>
    [JsonPropertyName("pool_inflow")]
    public decimal PoolInflow { get; set; }
    /// <summary>
    /// ["<c>pool_outflow</c>"] Pool outflow
    /// </summary>
    [JsonPropertyName("pool_outflow")]
    public decimal PoolOutflow { get; set; }
    /// <summary>
    /// ["<c>pool_total_shares</c>"] Pool total shares
    /// </summary>
    [JsonPropertyName("pool_total_shares")]
    public decimal PoolTotalShares { get; set; }
    /// <summary>
    /// ["<c>spot_inflow</c>"] Spot inflow
    /// </summary>
    [JsonPropertyName("spot_inflow")]
    public decimal SpotInflow { get; set; }
    /// <summary>
    /// ["<c>spot_outflow</c>"] Spot outflow
    /// </summary>
    [JsonPropertyName("spot_outflow")]
    public decimal SpotOutflow { get; set; }
    /// <summary>
    /// ["<c>staked_lit</c>"] Staked lit
    /// </summary>
    [JsonPropertyName("staked_lit")]
    public decimal StakedLit { get; set; }
    /// <summary>
    /// ["<c>staking_inflow</c>"] Staking inflow
    /// </summary>
    [JsonPropertyName("staking_inflow")]
    public decimal StakingInflow { get; set; }
    /// <summary>
    /// ["<c>staking_outflow</c>"] Staking outflow
    /// </summary>
    [JsonPropertyName("staking_outflow")]
    public decimal StakingOutflow { get; set; }
    /// <summary>
    /// ["<c>staking_pnl</c>"] Staking pnl
    /// </summary>
    [JsonPropertyName("staking_pnl")]
    public decimal StakingPnl { get; set; }
    /// <summary>
    /// ["<c>trade_spot_pnl</c>"] Trade spot pnl
    /// </summary>
    [JsonPropertyName("trade_spot_pnl")]
    public decimal TradeSpotPnl { get; set; }
    /// <summary>
    /// ["<c>volume</c>"] Volume
    /// </summary>
    [JsonPropertyName("volume")]
    public decimal Volume { get; set; }
}

