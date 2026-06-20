using System;
using System.Text.Json.Serialization;

namespace Lighter.Net.Objects.Models;

/// <summary>
/// Announcements
/// </summary>
public record LighterAnnouncements : LighterResponse
{
    /// <summary>
    /// ["<c>announcements</c>"] Announcements
    /// </summary>
    [JsonPropertyName("announcements")]
    public LighterAnnouncement[] Announcements { get; set; } = [];
}

/// <summary>
/// Announcement
/// </summary>
public record LighterAnnouncement
{
    /// <summary>
    /// ["<c>title</c>"] Title
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
    /// <summary>
    /// ["<c>content</c>"] Content
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
    /// <summary>
    /// ["<c>created_at</c>"] Created at
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
    /// <summary>
    /// ["<c>expired_at</c>"] Expired at
    /// </summary>
    [JsonPropertyName("expired_at")]
    public DateTime ExpiredAt { get; set; }
}

