using System;
using System.Text.Json.Serialization;

namespace Lighter.Net.Objects.Models;

/// <summary>
/// Funding rate history
/// </summary>
public record LighterFundingRateHistory : LighterResponse
{
    /// <summary>
    /// ["<c>resolution</c>"] Resolution
    /// </summary>
    [JsonPropertyName("resolution")]
    public string Resolution { get; set; } = string.Empty;
    /// <summary>
    /// ["<c>fundings</c>"] Fundings
    /// </summary>
    [JsonPropertyName("fundings")]
    public LighterFundingRate[] Fundings { get; set; } = [];
}

/// <summary>
/// Funding rate
/// </summary>
public record LighterFundingRate
{
    /// <summary>
    /// ["<c>timestamp</c>"] Timestamp
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }
    /// <summary>
    /// ["<c>value</c>"] Value
    /// </summary>
    [JsonPropertyName("value")]
    public decimal Value { get; set; }
    /// <summary>
    /// ["<c>rate</c>"] Rate
    /// </summary>
    [JsonPropertyName("rate")]
    public decimal Rate { get; set; }
    /// <summary>
    /// ["<c>direction</c>"] Direction
    /// </summary>
    [JsonPropertyName("direction")]
    public string Direction { get; set; } = string.Empty;
}

