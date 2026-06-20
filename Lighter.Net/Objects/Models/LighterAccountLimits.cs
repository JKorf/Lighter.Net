using System;
using System.Text.Json.Serialization;

namespace Lighter.Net.Objects.Models;

/// <summary>
/// Account limits
/// </summary>
public record LighterAccountLimits : LighterResponse
{
    /// <summary>
    /// ["<c>max_llp_percentage</c>"] Max llp percentage
    /// </summary>
    [JsonPropertyName("max_llp_percentage")]
    public decimal MaxLlpPercentage { get; set; }
    /// <summary>
    /// ["<c>user_tier</c>"] User tier
    /// </summary>
    [JsonPropertyName("user_tier")]
    public string UserTier { get; set; } = string.Empty;
    /// <summary>
    /// ["<c>can_create_public_pool</c>"] Can create public pool
    /// </summary>
    [JsonPropertyName("can_create_public_pool")]
    public bool CanCreatePublicPool { get; set; }
    /// <summary>
    /// ["<c>max_llp_amount</c>"] Max llp quantity
    /// </summary>
    [JsonPropertyName("max_llp_amount")]
    public decimal MaxLlpQuantity { get; set; }
    /// <summary>
    /// ["<c>current_maker_fee_tick</c>"] Current maker fee tick
    /// </summary>
    [JsonPropertyName("current_maker_fee_tick")]
    public decimal CurrentMakerFeeTick { get; set; }
    /// <summary>
    /// ["<c>current_taker_fee_tick</c>"] Current taker fee tick
    /// </summary>
    [JsonPropertyName("current_taker_fee_tick")]
    public decimal CurrentTakerFeeTick { get; set; }
    /// <summary>
    /// ["<c>effective_lit_stakes</c>"] Effective lit stakes
    /// </summary>
    [JsonPropertyName("effective_lit_stakes")]
    public string EffectiveLitStakes { get; set; } = string.Empty;
    /// <summary>
    /// ["<c>leased_lit</c>"] Leased lit
    /// </summary>
    [JsonPropertyName("leased_lit")]
    public string LeasedLit { get; set; } = string.Empty;
    /// <summary>
    /// ["<c>user_tier_name</c>"] User tier name
    /// </summary>
    [JsonPropertyName("user_tier_name")]
    public string UserTierName { get; set; } = string.Empty;
    /// <summary>
    /// ["<c>user_tier_last_update</c>"] User tier last update
    /// </summary>
    [JsonPropertyName("user_tier_last_update")]
    public DateTime? UserTierLastUpdate { get; set; }
}

