using CryptoExchange.Net;
using Lighter.Net.Converters;
using System.Text.Json.Serialization;

namespace Lighter.Net.Objects.Internal
{
    internal record LighterRequestWrapper<T> where T: LighterQueryRequest
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;
        [JsonPropertyName("data")]
        public T Data { get; set; } = default!;
    }

    internal record LighterQueryRequest
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = ExchangeHelpers.NextId().ToString();
    }

    internal record LighterTxRequest : LighterQueryRequest
    {
        [JsonPropertyName("tx_type")]
        public int TransactionType { get; set; }
        [JsonPropertyName("tx_info"), JsonConverter(typeof(JsonRawStringConverter))]
        public string TransactionInfo { get; set; } = string.Empty;
    }

    internal record LighterBatchTxRequest : LighterQueryRequest
    {
        [JsonPropertyName("tx_types")]
        public string TransactionTypes { get; set; } = string.Empty;
        [JsonPropertyName("tx_infos")]
        public string TransactionInfos { get; set; } = string.Empty;
    }

    /// <summary>
    /// Query response
    /// </summary>
    public record LighterQueryResponse
    {
        [JsonInclude, JsonPropertyName("id")]
        internal string Id { get; set; } = string.Empty;

        [JsonInclude, JsonPropertyName("error")]
        internal LighterSocketErrorInfo Error { get; set; } = default!;
    }
}
