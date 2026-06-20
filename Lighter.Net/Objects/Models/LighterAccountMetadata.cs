using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Lighter.Net.Objects.Models;

/// <summary>
/// Metadata
/// </summary>
public record LighterAccountMetadata : LighterResponse
{
    /// <summary>
    /// ["<c>account_metadatas</c>"] Account metadatas
    /// </summary>
    [JsonPropertyName("account_metadatas")]
    public LighterAccountMetadataInfo[] AccountMetadatas { get; set; } = [];
    /// <summary>
    /// ["<c>next_cursor</c>"] Next cursor
    /// </summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }
}

/// <summary>
/// Metadata info
/// </summary>
public record LighterAccountMetadataInfo
{
    /// <summary>
    /// ["<c>account_index</c>"] Account index
    /// </summary>
    [JsonPropertyName("account_index")]
    public long AccountIndex { get; set; }
    /// <summary>
    /// ["<c>name</c>"] Name
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// ["<c>description</c>"] Description
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
    /// <summary>
    /// ["<c>can_invite</c>"] Can invite
    /// </summary>
    [JsonPropertyName("can_invite")]
    public bool CanInvite { get; set; }
    /// <summary>
    /// ["<c>referral_points_percentage</c>"] Referral points percentage
    /// </summary>
    [JsonPropertyName("referral_points_percentage")]
    public decimal? ReferralPointsPercentage { get; set; }
    /// <summary>
    /// ["<c>created_at</c>"] Created at time
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTime CreateTime { get; set; }
    /// <summary>
    /// ["<c>can_rfq</c>"] Can rfq
    /// </summary>
    [JsonPropertyName("can_rfq")]
    public bool CanRfq { get; set; }
    /// <summary>
    /// ["<c>can_rfq_market_ids</c>"] Can rfq market ids
    /// </summary>
    [JsonPropertyName("can_rfq_market_ids")]
    public string[] CanRfqMarketIds { get; set; } = [];
    /// <summary>
    /// ["<c>metadata</c>"] Metadata
    /// </summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object> Metadata { get; set; } = null!;
}


