using System.Text.Json.Serialization;

namespace Lighter.Net.Objects.Models
{
    /// <summary>
    /// L1 info
    /// </summary>
    public record LighterL1Info : LighterResponse
    {
        /// <summary>
        /// ["<c>l1_providers</c>"] L1 providers
        /// </summary>
        [JsonPropertyName("l1_providers")]
        public LighterL1Provider[] L1Providers { get; set; } = [];
        /// <summary>
        /// ["<c>l1_providers_health</c>"] L1 providers health
        /// </summary>
        [JsonPropertyName("l1_providers_health")]
        public bool L1ProvidersHealth { get; set; }
        /// <summary>
        /// ["<c>validator_info</c>"] Validator info
        /// </summary>
        [JsonPropertyName("validator_info")]
        public LighterL1Validator[] ValidatorInfo { get; set; } = [];
        /// <summary>
        /// ["<c>contract_addresses</c>"] Contract addresses
        /// </summary>
        [JsonPropertyName("contract_addresses")]
        public LighterL1Address[] ContractAddresses { get; set; } = [];
        /// <summary>
        /// ["<c>latest_l1_generic_block</c>"] Latest l1 generic block
        /// </summary>
        [JsonPropertyName("latest_l1_generic_block")]
        public long LatestL1GenericBlock { get; set; }
        /// <summary>
        /// ["<c>latest_l1_governance_block</c>"] Latest l1 governance block
        /// </summary>
        [JsonPropertyName("latest_l1_governance_block")]
        public long LatestL1GovernanceBlock { get; set; }
        /// <summary>
        /// ["<c>latest_l1_desert_block</c>"] Latest l1 desert block
        /// </summary>
        [JsonPropertyName("latest_l1_desert_block")]
        public long LatestL1DesertBlock { get; set; }
    }

    /// <summary>
    /// Provider info
    /// </summary>
    public record LighterL1Provider
    {
        /// <summary>
        /// ["<c>chainId</c>"] Chain id
        /// </summary>
        [JsonPropertyName("chainId")]
        public int ChainId { get; set; }
        /// <summary>
        /// ["<c>networkId</c>"] Network id
        /// </summary>
        [JsonPropertyName("networkId")]
        public int NetworkId { get; set; }
        /// <summary>
        /// ["<c>latestBlockNumber</c>"] Latest block number
        /// </summary>
        [JsonPropertyName("latestBlockNumber")]
        public long LatestBlockNumber { get; set; }
    }

    /// <summary>
    /// Validator info
    /// </summary>
    public record LighterL1Validator
    {
        /// <summary>
        /// ["<c>address</c>"] Address
        /// </summary>
        [JsonPropertyName("address")]
        public string Address { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>is_active</c>"] Is active
        /// </summary>
        [JsonPropertyName("is_active")]
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Address
    /// </summary>
    public record LighterL1Address
    {
        /// <summary>
        /// ["<c>name</c>"] Name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>address</c>"] Address
        /// </summary>
        [JsonPropertyName("address")]
        public string Address { get; set; } = string.Empty;
    }
}
