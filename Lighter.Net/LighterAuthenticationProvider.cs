using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Authentication.Signing;
using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Objects;
using Lighter.Net.Clients.ExchangeApi;
using Lighter.Net.Objects;
using Secp256k1Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lighter.Net
{
    internal class LighterAuthenticationProvider : AuthenticationProvider<LighterCredentials, LighterCredential>
    {
        public LighterAuthenticationProvider(LighterCredentials credentials) : base(credentials, credentials.Credential)
        {
        }


        public override void ProcessRequest(RestApiClient apiClient, RestRequestConfiguration requestConfig)
        {
            if (!requestConfig.RequestDefinition.Authenticated)
                return;

            requestConfig.QueryParameters ??= new Parameters(LighterExchange._parameterSerializationSettings);
            if (apiClient.EnvironmentName == "UnitTest")
            {
                requestConfig.QueryParameters.Add("auth", "123");
                return;
            }

            var token = LighterUtils.GetAuthToken(((LighterRestClientExchangeApi)apiClient).ClientOptions.LibraryPath, apiClient.BaseAddress, ApiCredentials);
            requestConfig.QueryParameters["auth"] = token;
        }

        public string CreateL1Signature(string msg)
        {
            var total = "Ethereum Signed Message:\n" + msg.Length + msg;
            var bytes = new byte[] { 0x19 }.Concat(Encoding.UTF8.GetBytes(total)).ToArray();
            var keccak = CeSha3Keccack.CalculateHash(bytes);
            var signed = SignRequest(keccak, Credential.EthKey.PrivateKey!);
            return $"0x{((string)signed["r"]).Substring(2)}{((string)signed["s"]).Substring(2)}{(int)signed["v"]:x2}";
        }

        public static Dictionary<string, object> SignRequest(byte[] request, string secret)
        {
            (var signature, var recover) = Secp256k1.SignRecoverable(request, HexToBytesString(secret));
            var hexCompactR = BytesToHexString(new ArraySegment<byte>(signature, 0, 32));
            var hexCompactS = BytesToHexString(new ArraySegment<byte>(signature, 32, 32));
            var hexCompactV = recover + 27;

            return new Dictionary<string, object>
            {
                { "r", "0x" + hexCompactR },
                { "s", "0x" + hexCompactS },
                { "v", hexCompactV },
            };
        }
    }
}
