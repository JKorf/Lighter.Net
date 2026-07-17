using System;
using System.Text.Json.Serialization;
using Lighter.Net.Enums;

namespace Lighter.Net.Objects.Models;

/// <summary>
/// Tokens
/// </summary>
public record LighterTokens : LighterResponse
{
    /// <summary>
    /// ["<c>order_books</c>"] Order books
    /// </summary>
    [JsonPropertyName("tokens")]
    public LighterToken[] Tokens { get; set; } = [];
}

/// <summary>
/// Token info
/// </summary>
public record LighterToken
{
    /// <summary>
    /// ["<c>symbol</c>"] Symbol
    /// </summary>
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;
    /// <summary>
    /// ["<c>name</c>"] Name
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// ["<c>logo</c>"] Logo
    /// </summary>
    [JsonPropertyName("logo")]
    public string Logo { get; set; } = string.Empty;
    /// <summary>
    /// ["<c>logo_extension</c>"] Logo extension
    /// </summary>
    [JsonPropertyName("logo_extension")]
    public string LogoExtension { get; set; } = string.Empty;
    /// <summary>
    /// ["<c>description_key</c>"] Description key
    /// </summary>
    [JsonPropertyName("description_key")]
    public string DescriptionKey { get; set; } = string.Empty;
    /// <summary>
    /// ["<c>gecko_id</c>"] Gecko ID
    /// </summary>
    [JsonPropertyName("gecko_id")]
    public string GeckoId { get; set; } = string.Empty;
    /// <summary>
    /// ["<c>paprika_id</c>"] Paprika ID
    /// </summary>
    [JsonPropertyName("paprika_id")]
    public string PaprikaId { get; set; } = string.Empty;
    /// <summary>
    /// ["<c>market</c>"] Market
    /// </summary>
    [JsonPropertyName("market")]
    public MarketType MarketType { get; set; }
    /// <summary>
    /// ["<c>asset_type</c>"] Asset type
    /// </summary>
    [JsonPropertyName("asset_type")]
    public AssetType AssetType { get; set; }
    /// <summary>
    /// ["<c>categories</c>"] Categories
    /// </summary>
    [JsonPropertyName("categories")]
    public string[] Categories { get; set; } = [];
    /// <summary>
    /// ["<c>is_allowed_mainnet</c>"] Is allowed on mainnet
    /// </summary>
    [JsonPropertyName("is_allowed_mainnet")]
    public bool IsAllowedMainnet { get; set; }
    /// <summary>
    /// ["<c>is_asset_allowed_mainnet</c>"] Is asset allowed on mainnet
    /// </summary>
    [JsonPropertyName("is_asset_allowed_mainnet")]
    public bool IsAssetAllowedMainnet { get; set; }

}

