using System;
using System.Text.Json.Serialization;
using Lighter.Net.Enums;

namespace Lighter.Net.Objects.Models;

/// <summary>
/// Deposit history
/// </summary>
public record LighterDepositHistory : LighterResponse
{
    /// <summary>
    /// ["<c>deposits</c>"] Deposits
    /// </summary>
    [JsonPropertyName("deposits")]
    public LighterDeposit[] Deposits { get; set; } = [];
    /// <summary>
    /// ["<c>cursor</c>"] Cursor
    /// </summary>
    [JsonPropertyName("cursor")]
    public string Cursor { get; set; } = string.Empty;
}

/// <summary>
/// Deposit info
/// </summary>
public record LighterDeposit
{
    /// <summary>
    /// ["<c>id</c>"] Id
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; set; }
    /// <summary>
    /// ["<c>amount</c>"] Quantity
    /// </summary>
    [JsonPropertyName("amount")]
    public decimal Quantity { get; set; }
    /// <summary>
    /// ["<c>timestamp</c>"] Timestamp
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }
    /// <summary>
    /// ["<c>status</c>"] Status
    /// </summary>
    [JsonPropertyName("status")]
    public DepositStatus Status { get; set; }
    /// <summary>
    /// ["<c>l1_tx_hash</c>"] L1 transaction hash
    /// </summary>
    [JsonPropertyName("l1_tx_hash")]
    public string L1TransactionHash { get; set; } = string.Empty;
    /// <summary>
    /// ["<c>asset_id</c>"] Asset id
    /// </summary>
    [JsonPropertyName("asset_id")]
    public long AssetId { get; set; }
}

