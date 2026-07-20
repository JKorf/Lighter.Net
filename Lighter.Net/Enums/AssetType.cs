using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace Lighter.Net.Enums;

/// <summary>
/// Asset type
/// </summary>
[JsonConverter(typeof(EnumConverter<AssetType>))]
public enum AssetType
{
    /// <summary>
    /// ["<c>CRYPTO</c>"] Crypto
    /// </summary>
    [Map("CRYPTO")]
    Crypto,
    /// <summary>
    /// ["<c>RWA</c>"] RWA
    /// </summary>
    [Map("RWA")]
    Rwa
}
