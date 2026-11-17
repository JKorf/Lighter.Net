using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace Lighter.Net.Enums;

/// <summary>
/// Self trade behavior
/// </summary>
[JsonConverter(typeof(EnumConverter<SelfTradeBehavior>))]
public enum SelfTradeBehavior
{
    /// <summary>
    /// ["<c>0</c>"] Expire maker
    /// </summary>
    [Map("0")]
    ExpireMaker,
    /// <summary>
    /// ["<c>1</c>"] Expire taker
    /// </summary>
    [Map("1")]
    ExpireTaker,
    /// <summary>
    /// ["<c>2</c>"] Expire both
    /// </summary>
    [Map("2")]
    ExpireBoth,
    /// <summary>
    /// ["<c>3</c>"] Reduce
    /// </summary>
    [Map("3")]
    Reduce,
}
