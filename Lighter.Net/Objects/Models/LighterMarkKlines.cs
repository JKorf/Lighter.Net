using System;
using System.Text.Json.Serialization;
using Lighter.Net.Enums;

namespace Lighter.Net.Objects.Models;

/// <summary>
/// Mark klines
/// </summary>
public record LighterMarkKlines : LighterResponse
{
    /// <summary>
    /// ["<c>r</c>"] Interval
    /// </summary>
    [JsonPropertyName("r")]
    public KlineInterval Interval { get; set; }
    /// <summary>
    /// ["<c>c</c>"] Klines
    /// </summary>
    [JsonPropertyName("c")]
    public LighterMarkKline[] Klines { get; set; } = [];
}

/// <summary>
/// Kline/candlestick data
/// </summary>
public record LighterMarkKline
{
    /// <summary>
    /// ["<c>t</c>"] Open time
    /// </summary>
    [JsonPropertyName("t")]
    public DateTime Timestamp { get; set; }
    /// <summary>
    /// ["<c>o</c>"] Open price
    /// </summary>
    [JsonPropertyName("o")]
    public decimal OpenPrice { get; set; }
    /// <summary>
    /// ["<c>h</c>"] High price
    /// </summary>
    [JsonPropertyName("h")]
    public decimal HighPrice { get; set; }
    /// <summary>
    /// ["<c>l</c>"] Low price
    /// </summary>
    [JsonPropertyName("l")]
    public decimal LowPrice { get; set; }
    /// <summary>
    /// ["<c>c</c>"] Close
    /// </summary>
    [JsonPropertyName("c")]
    public decimal ClosePrice { get; set; }
    /// <summary>
    /// ["<c>sc</c>"] Sample count
    /// </summary>
    [JsonPropertyName("sc")]
    public long SampleCount { get; set; }
}

