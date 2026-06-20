using System;
using System.Text.Json.Serialization;

namespace Lighter.Net.Objects.Models;

/// <summary>
/// API keys
/// </summary>
public record LighterApiKeys : LighterResponse
{
    /// <summary>
    /// ["<c>api_keys</c>"] Api keys
    /// </summary>
    [JsonPropertyName("api_keys")]
    public LighterApiKey[] ApiKeys { get; set; } = [];
}

/// <summary>
/// API key info
/// </summary>
public record LighterApiKey
{
    /// <summary>
    /// ["<c>account_index</c>"] Account index
    /// </summary>
    [JsonPropertyName("account_index")]
    public long AccountIndex { get; set; }
    /// <summary>
    /// ["<c>api_key_index</c>"] Api key index
    /// </summary>
    [JsonPropertyName("api_key_index")]
    public long ApiKeyIndex { get; set; }
    /// <summary>
    /// ["<c>nonce</c>"] Nonce
    /// </summary>
    [JsonPropertyName("nonce")]
    public long Nonce { get; set; }
    /// <summary>
    /// ["<c>public_key</c>"] Public key
    /// </summary>
    [JsonPropertyName("public_key")]
    public string PublicKey { get; set; } = string.Empty;
    /// <summary>
    /// ["<c>transaction_time</c>"] Transaction time
    /// </summary>
    [JsonPropertyName("transaction_time")]
    public DateTime TransactionTime { get; set; }
}

