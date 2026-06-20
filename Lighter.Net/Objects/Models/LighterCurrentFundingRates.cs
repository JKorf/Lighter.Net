using System.Text.Json.Serialization;

namespace Lighter.Net.Objects.Models;

/// <summary>
/// Funding rates
/// </summary>
public record LighterCurrentFundingRates : LighterResponse
{
    /// <summary>
    /// ["<c>funding_rates</c>"] Funding rates
    /// </summary>
    [JsonPropertyName("funding_rates")]
    public LighterCurrentFundingRate[] FundingRates { get; set; } = [];
}

/// <summary>
/// Funding rate
/// </summary>
public record LighterCurrentFundingRate
{
    /// <summary>
    /// ["<c>market_id</c>"] Market id
    /// </summary>
    [JsonPropertyName("market_id")]
    public long MarketId { get; set; }
    /// <summary>
    /// ["<c>exchange</c>"] Exchange
    /// </summary>
    [JsonPropertyName("exchange")]
    public string Exchange { get; set; } = string.Empty;
    /// <summary>
    /// ["<c>symbol</c>"] Symbol
    /// </summary>
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;
    /// <summary>
    /// ["<c>rate</c>"] Rate
    /// </summary>
    [JsonPropertyName("rate")]
    public decimal Rate { get; set; }
}

