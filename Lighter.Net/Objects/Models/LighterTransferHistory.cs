using Lighter.Net.Enums;
using System;
using System.Text.Json.Serialization;

namespace Lighter.Net.Objects.Models;

/// <summary>
/// Transfer history
/// </summary>
public record LighterTransferHistory : LighterResponse
{
    /// <summary>
    /// ["<c>transfers</c>"] Transfers
    /// </summary>
    [JsonPropertyName("transfers")]
    public LighterTransfer[] Transfers { get; set; } = [];
    /// <summary>
    /// ["<c>cursor</c>"] Cursor
    /// </summary>
    [JsonPropertyName("cursor")]
    public string Cursor { get; set; } = string.Empty;
}

/// <summary>
/// Transfer info
/// </summary>
public record LighterTransfer
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
    /// ["<c>type</c>"] Type
    /// </summary>
    [JsonPropertyName("type")]
    public TransferType Type { get; set; }
    /// <summary>
    /// ["<c>from_l1_address</c>"] From l1 address
    /// </summary>
    [JsonPropertyName("from_l1_address")]
    public string FromL1Address { get; set; } = string.Empty;
    /// <summary>
    /// ["<c>to_l1_address</c>"] To l1 address
    /// </summary>
    [JsonPropertyName("to_l1_address")]
    public string ToL1Address { get; set; } = string.Empty;
    /// <summary>
    /// ["<c>from_account_index</c>"] From account index
    /// </summary>
    [JsonPropertyName("from_account_index")]
    public long FromAccountIndex { get; set; }
    /// <summary>
    /// ["<c>to_account_index</c>"] To account index
    /// </summary>
    [JsonPropertyName("to_account_index")]
    public long ToAccountIndex { get; set; }
    /// <summary>
    /// ["<c>tx_hash</c>"] Transaction hash
    /// </summary>
    [JsonPropertyName("tx_hash")]
    public string TransactionHash { get; set; } = string.Empty;
    /// <summary>
    /// ["<c>asset_id</c>"] Asset id
    /// </summary>
    [JsonPropertyName("asset_id")]
    public long AssetId { get; set; }
    /// <summary>
    /// ["<c>fee</c>"] Fee
    /// </summary>
    [JsonPropertyName("fee")]
    public string Fee { get; set; } = string.Empty;
    /// <summary>
    /// ["<c>from_route</c>"] From route
    /// </summary>
    [JsonPropertyName("from_route")]
    public string FromRoute { get; set; } = string.Empty;
    /// <summary>
    /// ["<c>to_route</c>"] To route
    /// </summary>
    [JsonPropertyName("to_route")]
    public string ToRoute { get; set; } = string.Empty;
}

