using System.Text.Json.Serialization;

namespace Lighter.Net.Objects.Models;

/// <summary>
/// Nonce
/// </summary>
public record LighterNonce : LighterResponse
{
    /// <summary>
    /// ["<c>nonce</c>"] Nonce value
    /// </summary>
    [JsonPropertyName("nonce")]
    public long Nonce { get; set; }
}

