using CryptoExchange.Net.Objects.Options;
using CryptoExchange.Net.SharedApis;

namespace Lighter.Net.Objects.Options
{
    /// <summary>
    /// Lighter options
    /// </summary>
    public class LighterOptions : LibraryOptions<LighterRestOptions, LighterSocketOptions, LighterCredentials, LighterEnvironment>
    {
        /// <summary>
        /// Options for Shared API usage
        /// </summary>
        public SharedApiOptions SharedApi { get; set; } = new();
    }
}
