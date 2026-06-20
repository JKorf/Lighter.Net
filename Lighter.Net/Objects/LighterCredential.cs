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
        /// <param name="publicAddress">Public address</param>
        /// <param name="accountIndex">Account index</param>
        /// <param name="apiKeyIndex">API key index</param>
        /// <param name="apiSecret">API secret</param>
        public LighterCredential(string publicAddress, long accountIndex, int apiKeyIndex, string apiSecret) : base(publicAddress)
        {
            ApiKeyIndex = apiKeyIndex;
            AccountIndex = accountIndex;
            PrivateApiKey = apiSecret;
        }

        /// <inheritdoc />
        public override ApiCredentials Copy() => new LighterCredential(Key, AccountIndex, ApiKeyIndex, PrivateApiKey);

        /// <inheritdoc />
        public override void Validate()
        {
            if (string.IsNullOrEmpty(PrivateApiKey))
                throw new ArgumentException($"PrivateApiKey not set on {GetType().Name}", nameof(PrivateApiKey));

            base.Validate();
        }
    }
}
