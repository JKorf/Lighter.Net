using CryptoExchange.Net.Authentication;
using System;

namespace Lighter.Net.Objects
{
    /// <summary>
    /// Lighter credentials
    /// </summary>
    public class LighterCredential : CredentialSet
    {
        /// <summary>
        /// Eth key
        /// </summary>
        public EthKey EthKey { get; set; }
        /// <summary>
        /// Private API key
        /// </summary>
        public string PrivateApiKey { get; set; }
        /// <summary>
        /// API key index
        /// </summary>
        public int ApiKeyIndex { get; set; }
        /// <summary>
        /// Account index
        /// </summary>
        public long AccountIndex { get; set; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="ethKey">Ethereum wallet info</param>
        /// <param name="accountIndex">Account index</param>
        /// <param name="apiKeyIndex">API key index</param>
        /// <param name="privateApiKey">API key secret</param>
        public LighterCredential(EthKey ethKey, long accountIndex, int apiKeyIndex, string privateApiKey) : base(ethKey.PublicKey)
        {
            EthKey = ethKey;
            ApiKeyIndex = apiKeyIndex;
            AccountIndex = accountIndex;
            PrivateApiKey = privateApiKey;
        }

        /// <inheritdoc />
        public override ApiCredentials Copy() => new LighterCredential(EthKey, AccountIndex, ApiKeyIndex, PrivateApiKey);

        /// <inheritdoc />
        public override void Validate()
        {
            if (string.IsNullOrEmpty(PrivateApiKey))
                throw new ArgumentException($"PrivateApiKey not set on {GetType().Name}", nameof(PrivateApiKey));

            base.Validate();
        }
    }
}
