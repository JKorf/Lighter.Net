using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

/// <summary>
/// Transfer type
/// </summary>
[JsonConverter(typeof(EnumConverter<TransferType>))]
public enum TransferType
{
    /// <summary>
    /// L2 transfer inflow
    /// </summary>
    [Map("L2TransferInflow")]
    L2TransferInflow,
    /// <summary>
    /// L2 transfer outflow
    /// </summary>
    [Map("L2TransferOutflow")]
    L2TransferOutflow,
    /// <summary>
    /// L2 burn shares inflow
    /// </summary>
    [Map("L2BurnSharesInflow")]
    L2BurnSharesInflow,
    /// <summary>
    /// L2 burn shares outflow
    /// </summary>
    [Map("L2BurnSharesOutflow")]
    L2BurnSharesOutflow,
    /// <summary>
    /// L2 mint shares inflow
    /// </summary>
    [Map("L2MintSharesInflow")]
    L2MintSharesInflow,
    /// <summary>
    /// L2 mint shares outflow
    /// </summary>
    [Map("L2MintSharesOutflow")]
    L2MintSharesOutflow,
    /// <summary>
    /// L2 self transfer
    /// </summary>
    [Map("L2SelfTransfer")]
    L2SelfTransfer,
    /// <summary>
    /// L2 stake asset inflow
    /// </summary>
    [Map("L2StakeAssetInflow")]
    L2StakeAssetInflow,
    /// <summary>
    /// L2 stake asset outflow
    /// </summary>
    [Map("L2StakeAssetOutflow")]
    L2StakeAssetOutflow,
    /// <summary>
    /// L2 unstake asset inflow
    /// </summary>
    [Map("L2UnstakeAssetInflow")]
    L2UnstakeAssetInflow,
    /// <summary>
    /// L2 unstake asset outflow
    /// </summary>
    [Map("L2UnstakeAssetOutflow")]
    L2UnstakeAssetOutflow,
    /// <summary>
    /// L2 force burn shares inflow
    /// </summary>
    [Map("L2ForceBurnSharesInflow")]
    L2ForceBurnSharesInflow,
    /// <summary>
    /// L2 force burn shares outflow
    /// </summary>
    [Map("L2ForceBurnSharesOutflow")]
    L2ForceBurnSharesOutflow,
    /// <summary>
    /// L1 burn shares inflow
    /// </summary>
    [Map("L1BurnSharesInflow")]
    L1BurnSharesInflow,
    /// <summary>
    /// L1 burn shares outflow
    /// </summary>
    [Map("L1BurnSharesOutflow")]
    L1BurnSharesOutflow,
    /// <summary>
    /// L1 unstake asset inflow
    /// </summary>
    [Map("L1UnstakeAssetInflow")]
    L1UnstakeAssetInflow,
    /// <summary>
    /// L1 unstake asset outflow
    /// </summary>
    [Map("L1UnstakeAssetOutflow")]
    L1UnstakeAssetOutflow,
    /// <summary>
    /// L2 create public pool inflow
    /// </summary>
    [Map("L2CreatePublicPoolInflow")]
    L2CreatePublicPoolInflow,
    /// <summary>
    /// L2 create public pool outflow
    /// </summary>
    [Map("L2CreatePublicPoolOutflow")]
    L2CreatePublicPoolOutflow,
    /// <summary>
    /// L2 create staking pool inflow
    /// </summary>
    [Map("L2CreateStakingPoolInflow")]
    L2CreateStakingPoolInflow,
    /// <summary>
    /// L2 create staking pool outflow
    /// </summary>
    [Map("L2CreateStakingPoolOutflow")]
    L2CreateStakingPoolOutflow,
}
