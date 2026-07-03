using CryptoExchange.Net.Authentication;
using Lighter.Net.Objects;
using System;

namespace Lighter.Net
{
    /// <summary>
    /// Lighter API credentials
    /// </summary>
    public class LighterCredentials : ApiCredentials
    {
        /// <summary>
        /// API credentials
        /// </summary>
        public LighterCredential Credential { get; set; }

        /// <summary>
        /// Create new credentials
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public LighterCredentials() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        /// <summary>
        /// Create new credentials
        /// </summary>
        /// <param name="ethKey">Ethereum key/wallet info</param>
        /// <param name="accountIndex">Account index</param>
        /// <param name="apiKeyIndex">API key index</param>
        /// <param name="apiSecret">API secret</param>
        public LighterCredentials(EthKey ethKey, long accountIndex, int apiKeyIndex, string apiSecret)
        {
            Credential = new LighterCredential(ethKey, accountIndex, apiKeyIndex, apiSecret);
        }

        /// <summary>
        /// Create new credentials
        /// </summary>
        /// <param name="credential">Credential details</param>
        public LighterCredentials(LighterCredential credential)
        {
            Credential = credential;
        }

        /// <summary>
        /// Specify the credentials
        /// </summary>
        /// <param name="ethKey">Ethereum key/wallet info</param>
        /// <param name="accountIndex">Account index</param>
        /// <param name="apiKeyIndex">API key index</param>
        /// <param name="apiSecret">API secret</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public LighterCredentials With(EthKey ethKey, long accountIndex, int apiKeyIndex, string apiSecret)
        {
            if (Credential != null) throw new InvalidOperationException("Credentials already set");

            Credential = new LighterCredential(ethKey, accountIndex, apiKeyIndex, apiSecret);
            return this;
        }

        /// <inheritdoc />
        public override ApiCredentials Copy()
        {
            return new LighterCredentials
            {
                Credential = Credential
            };
        }

        /// <inheritdoc />
        public override void Validate()
        {
            if (Credential == null)
                throw new ArgumentException("Credential not set");

            Credential.Validate();
        }
    }
}
