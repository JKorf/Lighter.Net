using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Objects;
using Lighter.Net.Clients.ExchangeApi;
using Lighter.Net.Objects;

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
            requestConfig.QueryParameters.Add("auth", token);
        }
    }
}
