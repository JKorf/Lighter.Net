using System.Text.Json.Serialization;

namespace Lighter.Net.Objects.Models;

/// <summary>
/// System config
/// </summary>
public record LighterSystemConfig : LighterResponse
{
    /// <summary>
    /// ["<c>liquidity_pool_index</c>"] Liquidity pool index
    /// </summary>
    [JsonPropertyName("liquidity_pool_index")]
    public long LiquidityPoolIndex { get; set; }
    /// <summary>
    /// ["<c>staking_pool_index</c>"] Staking pool index
    /// </summary>
    [JsonPropertyName("staking_pool_index")]
    public long StakingPoolIndex { get; set; }
    /// <summary>
    /// ["<c>funding_fee_rebate_account_index</c>"] Funding fee rebate account index
    /// </summary>
    [JsonPropertyName("funding_fee_rebate_account_index")]
    public long FundingFeeRebateAccountIndex { get; set; }
    /// <summary>
    /// ["<c>liquidity_pool_cooldown_period</c>"] Liquidity pool cooldown period
    /// </summary>
    [JsonPropertyName("liquidity_pool_cooldown_period")]
    public int LiquidityPoolCooldownPeriod { get; set; }
    /// <summary>
    /// ["<c>staking_pool_lockup_period</c>"] Staking pool lockup period
    /// </summary>
    [JsonPropertyName("staking_pool_lockup_period")]
    public int StakingPoolLockupPeriod { get; set; }
    /// <summary>
    /// ["<c>max_integrator_perps_maker_fee</c>"] Max integrator perps maker fee
    /// </summary>
    [JsonPropertyName("max_integrator_perps_maker_fee")]
    public int MaxIntegratorPerpsMakerFee { get; set; }
    /// <summary>
    /// ["<c>max_integrator_perps_taker_fee</c>"] Max integrator perps taker fee
    /// </summary>
    [JsonPropertyName("max_integrator_perps_taker_fee")]
    public int MaxIntegratorPerpsTakerFee { get; set; }
    /// <summary>
    /// ["<c>max_integrator_spot_maker_fee</c>"] Max integrator spot maker fee
    /// </summary>
    [JsonPropertyName("max_integrator_spot_maker_fee")]
    public int MaxIntegratorSpotMakerFee { get; set; }
    /// <summary>
    /// ["<c>max_integrator_spot_taker_fee</c>"] Max integrator spot taker fee
    /// </summary>
    [JsonPropertyName("max_integrator_spot_taker_fee")]
    public int MaxIntegratorSpotTakerFee { get; set; }
    /// <summary>
    /// ["<c>market_maker_incentive_account_index</c>"] Market market incentive account index
    /// </summary>
    [JsonPropertyName("market_maker_incentive_account_index")]
    public int MarketMakerIncentiveAccountIndex { get; set; }
}

