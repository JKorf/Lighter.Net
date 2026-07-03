using CryptoExchange.Net;
using CryptoExchange.Net.Authentication.Signing;
using Secp256k1Net;
using System;

namespace Lighter.Net
{
    // Move to a shared library location; for now Secp256k1Net dependency prevents moving it to CryptoExchange.Net

    /// <summary>
    /// Eth credentials/wallet info for signing requests
    /// </summary>
    public record EthKey
    {
        /// <summary>
        /// Provided private key
        /// </summary>
        public string? PrivateKey { get; set; }
        /// <summary>
        /// Provided or derived public key
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        /// New instance, should not be used directly. Use FromPublicKey or FromPrivateKey static methods instead.
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public EthKey() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        private EthKey(string? privateKey, string? publicKey)
        {
            if (privateKey == null && publicKey == null)
                throw new ArgumentNullException("Both private pey and public key not provided");

            PrivateKey = privateKey;
            PublicKey = publicKey ?? GetPublicAddress(privateKey!);
        }

        /// <summary>
        /// Create new Eth credentials with only a public key. This might not not be sufficient depending on the exchange and operation as it will not allow layer 1 signing.
        /// </summary>
        /// <param name="publicAddress">The public wallet address</param>
        public static EthKey FromPublicKey(string publicAddress)
        {
            return new EthKey(null, publicAddress);
        }

        /// <summary>
        /// Create new Eth credentials from a private key. The public key will be derived from the private key. Allows signing of layer 1 requests.
        /// </summary>
        /// <param name="privateKey">The private key for the wallet</param>
        public static EthKey FromPrivateKey(string privateKey)
        {
            return new EthKey(privateKey, null);
        }

        private static string GetPublicAddress(string privateKey)
        {
            var publicKeyBytes = Secp256k1.CreatePublicKey(ExchangeHelpers.HexToBytesString(privateKey), false);

            var withoutPrefix = new byte[64];
            Array.Copy(publicKeyBytes, 1, withoutPrefix, 0, 64);

            var hash = CeSha3Keccack.CalculateHash(withoutPrefix);
            var pubAddress = new byte[20];
            Array.Copy(hash, hash.Length - 20, pubAddress, 0, 20);

            return "0x" + ExchangeHelpers.BytesToHexString(pubAddress).ToLower();
        }
    }
}
