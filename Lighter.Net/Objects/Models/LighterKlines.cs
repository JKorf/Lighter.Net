using System;
using System.Text.Json.Serialization;
using Lighter.Net.Enums;

namespace Lighter.Net.Objects.Models;

/// <summary>
/// Klines
/// </summary>
public record LighterKlines : LighterResponse
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
    public LighterKline[] Klines { get; set; } = [];
}

/// <summary>
/// Kline/candlestick data
/// </summary>
public record LighterKline
{
    /// <summary>
    /// ["<c>t</c>"] Open time
    /// </summary>
    [JsonPropertyName("t")]
    public DateTime OpenTime { get; set; }
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
    /// ["<c>v</c>"] Volume in base asset
    /// </summary>
    [JsonPropertyName("v")]
    public decimal Volume { get; set; }
    /// <summary>
    /// ["<c>V</c>"] Volume in quote asset
    /// </summary>
    [JsonPropertyName("V")]
    public decimal QuoteVolume { get; set; }
    /// <summary>
    /// ["<c>i</c>"] Last trade id
    /// </summary>
    [JsonPropertyName("i")]
    public long LastTradeId { get; set; }
}

