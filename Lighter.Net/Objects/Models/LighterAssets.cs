using System.Text.Json.Serialization;
using Lighter.Net.Enums;

namespace Lighter.Net.Objects.Models;

/// <summary>
/// Assets
/// </summary>
public record LighterAssets : LighterResponse
{
    /// <summary>
    /// ["<c>asset_details</c>"] Asset details
    /// </summary>
    [JsonPropertyName("asset_details")]
    public LighterAsset[] AssetDetails { get; set; } = [];
}

/// <summary>
/// Asset
/// </summary>
public record LighterAsset
{
    /// <summary>
    /// ["<c>asset_id</c>"] Asset id
    /// </summary>
    [JsonPropertyName("asset_id")]
    public long AssetId { get; set; }
    /// <summary>
    /// ["<c>symbol</c>"] Symbol
    /// </summary>
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;
    /// <summary>
    /// ["<c>l1_decimals</c>"] L1 decimals
    /// </summary>
    [JsonPropertyName("l1_decimals")]
    public int L1Decimals { get; set; }
    /// <summary>
    /// ["<c>decimals</c>"] Decimals
    /// </summary>
    [JsonPropertyName("decimals")]
    public int Decimals { get; set; }
    /// <summary>
    /// ["<c>min_transfer_amount</c>"] Min transfer quantity
    /// </summary>
    [JsonPropertyName("min_transfer_amount")]
    public decimal MinTransferQuantity { get; set; }
    /// <summary>
    /// ["<c>min_withdrawal_amount</c>"] Min withdrawal quantity
    /// </summary>
    [JsonPropertyName("min_withdrawal_amount")]
    public decimal MinWithdrawalQuantity { get; set; }
    /// <summary>
    /// ["<c>margin_mode</c>"] Margin mode
    /// </summary>
    [JsonPropertyName("margin_mode")]
    public EnabledStatus MarginMode { get; set; }
    /// <summary>
    /// ["<c>index_price</c>"] Index price
    /// </summary>
    [JsonPropertyName("index_price")]
    public decimal IndexPrice { get; set; }
    /// <summary>
    /// ["<c>l1_address</c>"] L1 address
    /// </summary>
    [JsonPropertyName("l1_address")]
    public string L1Address { get; set; } = string.Empty;
    /// <summary>
    /// ["<c>global_supply_cap</c>"] Global supply cap
    /// </summary>
    [JsonPropertyName("global_supply_cap")]
    public decimal GlobalSupplyCap { get; set; }
    /// <summary>
    /// ["<c>liquidation_fee</c>"] Liquidation fee
    /// </summary>
    [JsonPropertyName("liquidation_fee")]
    public decimal LiquidationFee { get; set; }
    /// <summary>
    /// ["<c>liquidation_threshold</c>"] Liquidation threshold
    /// </summary>
    [JsonPropertyName("liquidation_threshold")]
    public decimal LiquidationThreshold { get; set; }
    /// <summary>
    /// ["<c>liquidation_factor</c>"] Liquidation factor
    /// </summary>
    [JsonPropertyName("liquidation_factor")]
    public decimal LiquidationFactor { get; set; }
    /// <summary>
    /// ["<c>loan_to_value</c>"] Loan to value
    /// </summary>
    [JsonPropertyName("loan_to_value")]
    public decimal LoanToValue { get; set; }
    /// <summary>
    /// ["<c>price_decimals</c>"] Price decimals
    /// </summary>
    [JsonPropertyName("price_decimals")]
    public int PriceDecimals { get; set; }
    /// <summary>
    /// ["<c>total_supplied</c>"] Total supplied
    /// </summary>
    [JsonPropertyName("total_supplied")]
    public decimal TotalSupplied { get; set; }
    /// <summary>
    /// ["<c>user_supply_cap</c>"] User supply cap
    /// </summary>
    [JsonPropertyName("user_supply_cap")]
    public decimal UserSupplyCap { get; set; }
}

