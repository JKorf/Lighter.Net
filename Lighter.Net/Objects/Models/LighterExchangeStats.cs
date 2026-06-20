using System.Text.Json.Serialization;

namespace Lighter.Net.Objects.Models;

/// <summary>
/// Exchange stats
/// </summary>
public record LighterExchangeStats : LighterResponse
{
    /// <summary>
    /// ["<c>total</c>"] Total
    /// </summary>
    [JsonPropertyName("total")]
    public int Total { get; set; }
    /// <summary>
    /// ["<c>order_book_stats</c>"] Symbol statistics
    /// </summary>
    [JsonPropertyName("order_book_stats")]
    public LighterSymbolStats[] SymbolStats { get; set; } = [];
    /// <summary>
    /// ["<c>daily_usd_volume</c>"] Daily usd volume
    /// </summary>
    [JsonPropertyName("daily_usd_volume")]
    public decimal DailyUsdVolume { get; set; }
    /// <summary>
    /// ["<c>daily_trades_count</c>"] Daily trades count
    /// </summary>
    [JsonPropertyName("daily_trades_count")]
    public int DailyTradesCount { get; set; }
}

/// <summary>
/// Lighter symbol stats
/// </summary>
public record LighterSymbolStats
{
    /// <summary>
    /// Symbol
    /// </summary>
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;
    /// <summary>
    /// Last trade price
    /// </summary>
    [JsonPropertyName("last_trade_price")]
    public decimal LastTradePrice { get; set; }
    /// <summary>
    /// 24h trade count
    /// </summary>
    [JsonPropertyName("daily_trades_count")]
    public int TradeCount { get; set; }
    /// <summary>
    /// 24h base asset volume
    /// </summary>
    [JsonPropertyName("daily_base_token_volume")]
    public decimal Volume { get; set; }
    /// <summary>
    /// 24h quote asset volume
    /// </summary>
    [JsonPropertyName("daily_quote_token_volume")]
    public decimal QuoteVolume { get; set; }
    /// <summary>
    /// 24h price change
    /// </summary>
    [JsonPropertyName("daily_price_change")]
    public decimal PriceChange { get; set; }
}

