using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace Lighter.Net.Enums
{
    /// <summary>
    /// Transfer filter type
    /// </summary>
    [JsonConverter(typeof(EnumConverter<TransferFilterType>))]
    public enum TransferFilterType
    {
        /// <summary>
        /// All
        /// </summary>
        [Map("all")]
        All,
        /// <summary>
        /// L2 transfer
        /// </summary>
        [Map("L2Transfer")]
        L2Transfer,
        /// <summary>
        /// L2 mint shares
        /// </summary>
        [Map("L2MintShares")]
        L2MintShares,
        /// <summary>
        /// L2 burn shares
        /// </summary>
        [Map("L2BurnShares")]
        L2BurnShares,
        /// <summary>
        /// L2 stake assets
        /// </summary>
        [Map("L2StakeAssets")]
        L2StakeAssets,
        /// <summary>
        /// L2 unstake assets
        /// </summary>
        [Map("L2UnstakeAssets")]
        L2UnstakeAssets,
        /// <summary>
        /// L2 create public pool
        /// </summary>
        [Map("L2CreatePublicPool")]
        L2CreatePublicPool,
        /// <summary>
        /// L2 create staking pool
        /// </summary>
        [Map("L2CreateStakingPool")]
        L2CreateStakingPool,
        /// <summary>
        /// L1 burn shares
        /// </summary>
        [Map("L1BurnShares")]
        L1BurnShares,
        /// <summary>
        /// L1 unstake assets
        /// </summary>
        [Map("L1UnstakeAssets")]
        L1UnstakeAssets,
    }
}