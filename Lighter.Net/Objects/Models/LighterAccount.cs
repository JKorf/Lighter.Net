using System;
using System.Text.Json.Serialization;
using Lighter.Net.Enums;

namespace Lighter.Net.Objects.Models;

/// <summary>
/// Account
/// </summary>
public record LighterAccount : LighterResponse
{
    /// <summary>
    /// ["<c>l1_address</c>"] L1 address
    /// </summary>
    [JsonPropertyName("l1_address")]
    public string L1Address { get; set; } = string.Empty;
    /// <summary>
    /// ["<c>sub_accounts</c>"] Sub accounts
    /// </summary>
    [JsonPropertyName("sub_accounts")]
    public LighterSubAccount[] SubAccounts { get; set; } = [];
}

/// <summary>
/// Sub account
/// </summary>
public record LighterSubAccount
{
    /// <summary>
    /// ["<c>code</c>"] Code
    /// </summary>
    [JsonPropertyName("code")]
    public int Code { get; set; }
    /// <summary>
    /// ["<c>account_type</c>"] Account type
    /// </summary>
    [JsonPropertyName("account_type")]
    public int AccountType { get; set; }
    /// <summary>
    /// ["<c>index</c>"] Index
    /// </summary>
    [JsonPropertyName("index")]
    public long Index { get; set; }
    /// <summary>
    /// ["<c>l1_address</c>"] L1 address
    /// </summary>
    [JsonPropertyName("l1_address")]
    public string L1Address { get; set; } = string.Empty;
    /// <summary>
    /// ["<c>cancel_all_time</c>"] Cancel all time
    /// </summary>
    [JsonPropertyName("cancel_all_time")]
    public DateTime? CancelAllTime { get; set; }
    /// <summary>
    /// ["<c>total_order_count</c>"] Total order count
    /// </summary>
    [JsonPropertyName("total_order_count")]
    public int TotalOrderCount { get; set; }
    /// <summary>
    /// ["<c>total_isolated_order_count</c>"] Total isolated order count
    /// </summary>
    [JsonPropertyName("total_isolated_order_count")]
    public int TotalIsolatedOrderCount { get; set; }
    /// <summary>
    /// ["<c>pending_order_count</c>"] Pending order count
    /// </summary>
    [JsonPropertyName("pending_order_count")]
    public int PendingOrderCount { get; set; }
    /// <summary>
    /// ["<c>available_balance</c>"] Available balance
    /// </summary>
    [JsonPropertyName("available_balance")]
    public decimal? AvailableBalance { get; set; }
    /// <summary>
    /// ["<c>status</c>"] Status
    /// </summary>
    [JsonPropertyName("status")]
    public AccountStatus Status { get; set; }
    /// <summary>
    /// ["<c>collateral</c>"] Collateral
    /// </summary>
    [JsonPropertyName("collateral")]
    public decimal Collateral { get; set; }
    /// <summary>
    /// ["<c>transaction_time</c>"] Transaction time
    /// </summary>
    [JsonPropertyName("transaction_time")]
    public DateTime? TransactionTime { get; set; }
    /// <summary>
    /// ["<c>account_trading_mode</c>"] Account trading mode
    /// </summary>
    [JsonPropertyName("account_trading_mode")]
    public AccountTradeMode AccountTradingMode { get; set; }
}

