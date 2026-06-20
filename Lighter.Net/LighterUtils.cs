using CryptoExchange.Net;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Errors;
using Lighter.Net.Clients;
using Lighter.Net.Objects.Models;
using Lighter.Net.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lighter.Net
{
    internal static class LighterUtils
    {
        class CachedAuthToken
        {
            public string Token { get; set; } = string.Empty;
            public DateTime Expiry { get; set; }
        }

        class IntegratorStatus
        {
            public SemaphoreSlim Semaphore { get; set; } = default!;
            public bool Usable { get; set; }
            public bool Done { get; set; }
        }

        private static ConcurrentDictionary<string, long> _nonceDictionary = new ConcurrentDictionary<string, long>();
        private static ConcurrentDictionary<string, CachedAuthToken> _authTokenDictionary = new ConcurrentDictionary<string, CachedAuthToken>();
        private static ConcurrentDictionary<string, LighterSigner> _clientDictionary = new ConcurrentDictionary<string, LighterSigner>();
        private static ConcurrentDictionary<string, LighterSymbol[]> _symbolDictionary = new ConcurrentDictionary<string, LighterSymbol[]>();

        private static ConcurrentDictionary<string, IntegratorStatus> _integratorStatus = new ConcurrentDictionary<string, IntegratorStatus>();

        internal static int UsdcMultiplier => 1000000;

        public static bool NonceSet(LighterCredentials credentials)
        {
            var key = $"{credentials.Credential.AccountIndex}:{credentials.Credential.ApiKeyIndex}";
            return _nonceDictionary.ContainsKey(key);
        }

        public static long GetNonce(LighterCredentials credentials, int count = 1)
        {
            var key = $"{credentials.Credential.AccountIndex}:{credentials.Credential.ApiKeyIndex}";
            if (!_nonceDictionary.TryGetValue(key, out long nonce))
                _nonceDictionary[key] = 0;

            var newNonce = nonce + count;
            _nonceDictionary[key] = newNonce;
            return nonce;
        }

        public static void SetNonce(LighterCredentials credentials, long nonce) 
        {
            var key = $"{credentials.Credential.AccountIndex}:{credentials.Credential.ApiKeyIndex}";
            _nonceDictionary[key] = nonce;
        }

        public static LighterSigner GetSigner(string? libPath, string url, LighterCredentials credentials)
        {
            var key = $"{credentials.Credential.AccountIndex}:{credentials.Credential.ApiKeyIndex}";
            if (!_clientDictionary.TryGetValue(key, out LighterSigner? signer))
            {
                signer = new LighterSigner(libPath);
                signer.CreateClient(
                    url,
                    credentials.Credential.PrivateApiKey,
                    304,
                    credentials.Credential.ApiKeyIndex,
                    credentials.Credential.AccountIndex
                );
                _clientDictionary.TryAdd(key, signer);
            }

            return signer;
        }

        public static string GetAuthToken(string? libraryPath, string url, LighterCredentials credentials)
        {
            var key = $"{credentials.Credential.AccountIndex}:{credentials.Credential.ApiKeyIndex}";
            if (!_authTokenDictionary.TryGetValue(key, out CachedAuthToken? authToken))
            {
                var expireTime = DateTime.UtcNow.AddHours(1);
                var signClient = GetSigner(libraryPath, url, credentials);
                var token = signClient.CreateAuthToken(
                    DateTimeConverter.ConvertToSeconds(expireTime).Value,
                    credentials.Credential.ApiKeyIndex,
                    credentials.Credential.AccountIndex);
                authToken = new CachedAuthToken
                {
                    Token = token,
                    Expiry = expireTime.AddSeconds(-5)
                };

                _authTokenDictionary[key] = authToken;
            }

            return authToken.Token;
        }

        internal static void SetSymbolsCache(string environmentName, LighterSymbols data)
        {
            _symbolDictionary[environmentName] = data.OrderBooks;
        }

        internal static int? GetSymbolId(string environmentName, string name)
        {
            if (!_symbolDictionary.TryGetValue(environmentName, out var symbols))
                return null;

            var symbol = symbols.FirstOrDefault(s => s.Symbol.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            return symbol?.MarketId;
        }

        internal static string? GetSymbolName(string environmentName, int marketIndex)
        {
            if (!_symbolDictionary.TryGetValue(environmentName, out var symbols))
                return null;

            var symbol = symbols.FirstOrDefault(s => s.MarketId == marketIndex);
            return symbol?.Symbol;
        }

        internal static LighterSymbol? GetSymbolInfo(string environmentName, string name)
        {
            if (!_symbolDictionary.TryGetValue(environmentName, out var symbols))
                return null;

            var symbol = symbols.FirstOrDefault(s => s.Symbol.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            return symbol;
        }

        internal static bool IntegratorFeeUsable(LighterCredentials credentials)
        {
            var key = $"{credentials.Credential.AccountIndex}:{credentials.Credential.ApiKeyIndex}";
            if (!_integratorStatus.TryGetValue(key, out var status))
                return false;

            return status.Usable;
        }

        internal static async Task<ICallResult> CheckBuilderFeeAsync(LighterRestClient client)
        {
            if (!client.ExchangeApi.Authenticated)
                // No credentials provided, no need to check builder fee
                return CallResult.Ok();

            var envName = client.ClientOptions.Environment.Name;
            if (envName.Equals("UnitTest", StringComparison.Ordinal))
                return CallResult.Ok();

            if (client.ClientOptions.IntegratorFeePercentage == null
                || client.ClientOptions.IntegratorFeePercentage == 0
                || client.ClientOptions.IntegratorAccountIndex == null)
            {
                // No builder fee, no need to check
                return CallResult.Ok();
            }

            var accountIndex = client.ExchangeApi.ApiCredentials!.Credential!.AccountIndex;
            var apiKeyIndex = client.ExchangeApi.ApiCredentials!.Credential!.ApiKeyIndex;
            var key = $"{accountIndex}:{apiKeyIndex}";
            if (!_integratorStatus.TryGetValue(key, out var status))
            {
                status = new IntegratorStatus { Semaphore = new SemaphoreSlim(1, 1) };
                _integratorStatus[key] = status;
            }

            if (status.Done)
                return CallResult.Ok();

            await status.Semaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                // Set to true even if the check fails to avoid continuously trying to check and approve the builder fee if there's an issue
                status.Done = true;

                var approvedResult = await client.ExchangeApi.Account.GetAccountsAsync().ConfigureAwait(false);
                if (!approvedResult.Success)
                    return approvedResult;

                var account = approvedResult.Data.Accounts.SingleOrDefault(x => x.AccountIndex == client.ExchangeApi.ApiCredentials.Credential.AccountIndex);
                if (account == null)
                {
                    // Current account not found?
                    return CallResult.Fail(new ServerError(ErrorType.Unknown, "Current account not found"));
                }

                var accountApproved = account.ApprovedIntegrators.SingleOrDefault(x => x.AccountIndex == client.ClientOptions.IntegratorAccountIndex);
                var approvedTakerRate = accountApproved?.MaxSpotTakerFee;
                var approvedMakerRate = accountApproved?.MaxSpotMakerFee;

                // 0.01% = 1bps = 100
                var targetBps = (int)(client.ClientOptions.IntegratorFeePercentage.Value * 10000);
                if (approvedTakerRate >= targetBps && approvedMakerRate >= targetBps)
                {
                    // Integrator fee is approved, we're good
                    status.Usable = true;
                    return CallResult.Ok();
                }

                var approveResult = await client.ExchangeApi.Account.ApproveIntegratorAsync(
                    client.ClientOptions.IntegratorAccountIndex.Value,
                    targetBps,
                    targetBps,
                    DateTime.UtcNow.AddYears(1)
                    ).ConfigureAwait(false);
                if (approveResult.Success)
                    status.Usable = true;
                else
                    LibraryHelpers.StaticLogger?.LogDebug("Builder fee approval failed: {Error}", approveResult.Error);

                return approveResult;
            }
            finally
            {
                status.Semaphore.Release();
            }
        }

    }
}
