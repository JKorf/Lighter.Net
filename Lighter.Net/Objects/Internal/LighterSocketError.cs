using System.Text.Json.Serialization;

namespace Lighter.Net.Objects.Internal
{
    internal record LighterSocketError
    {
        [JsonPropertyName("error")]
        public LighterSocketErrorInfo Error { get; set; } = default!;
    }

    internal record LighterSocketErrorInfo
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }
}
