using System;
using System.Text.Json.Serialization;
using Lighter.Net.Enums;

namespace Lighter.Net.Objects.Models;

/// <summary>
/// Funding history
/// </summary>
public record LighterFundingHistory : LighterResponse
{
    /// <summary>
    /// ["<c>position_fundings</c>"] Position fundings
    /// </summary>
    [JsonPropertyName("position_fundings")]
    public LighterFundingHistoryItem[] PositionFundings { get; set; } = [];
    /// <summary>
    /// ["<c>next_cursor</c>"] Next cursor
    /// </summary>
    [JsonPropertyName("next_cursor")]
    public string NextCursor { get; set; } = string.Empty;
}

/// <summary>
/// Funding item
/// </summary>
public record LighterFundingHistoryItem
{
    /// <summary>
    /// ["<c>timestamp</c>"] Timestamp
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }
    /// <summary>
    /// ["<c>market_id</c>"] Market id
    /// </summary>
    [JsonPropertyName("market_id")]
    public long MarketId { get; set; }
    /// <summary>
    /// ["<c>funding_id</c>"] Funding id
    /// </summary>
    [JsonPropertyName("funding_id")]
    public long FundingId { get; set; }
    /// <summary>
    /// ["<c>change</c>"] Change
    /// </summary>
    [JsonPropertyName("change")]
    public decimal Change { get; set; }
    /// <summary>
    /// ["<c>discount</c>"] Discount
    /// </summary>
    [JsonPropertyName("discount")]
    public decimal? Discount { get; set; }
    /// <summary>
    /// ["<c>rate</c>"] Rate
    /// </summary>
    [JsonPropertyName("rate")]
    public decimal Rate { get; set; }
    /// <summary>
    /// ["<c>position_size</c>"] Position quantity
    /// </summary>
    [JsonPropertyName("position_size")]
    public decimal PositionQuantity { get; set; }
    /// <summary>
    /// ["<c>position_side</c>"] Position side
    /// </summary>
    [JsonPropertyName("position_side")]
    public PositionSide PositionSide { get; set; }
}

