using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Interfaces.Clients;
using CryptoExchange.Net.Testing;
using Lighter.Net.Clients;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net.Http;

namespace Lighter.Net.UnitTests
{
    [TestFixture()]
    public class LighterRestClientTests
    {
        [Test]
        public void CheckInterfaces()
        {
            CryptoExchange.Net.Testing.TestHelpers.CheckForMissingRestInterfaces<LighterRestClient>();
            CryptoExchange.Net.Testing.TestHelpers.CheckForMissingSocketInterfaces<LighterSocketClient>();
        }

        [Test]
        public void TestRestSharedApiDiscoveryMatchesAggregate()
        {
            var (missingOptions, missingInterfaces) = CryptoExchange.Net.Testing.TestHelpers.ValidateSharedApi(new LighterRestClient().ExchangeApi.SharedApi);

            Assert.That(missingOptions, Is.Empty);
            Assert.That(missingInterfaces, Is.Empty);
        }

        [Test]
        public void TestSocketSharedApiDiscoveryMatchesAggregate()
        {
            var (missingOptions, missingInterfaces) = CryptoExchange.Net.Testing.TestHelpers.ValidateSharedApi(new LighterSocketClient().ExchangeApi.SharedApi);

            Assert.That(missingOptions, Is.Empty);
            Assert.That(missingInterfaces, Is.Empty);
        }

        [Test]
        public void TestRestSharedApiDoesntHaveUnsupportedCapabilities()
        {
            var unsupported = TestHelpers.ValidateUnsupportedCapabilities(new LighterRestClient().ExchangeApi.SharedApi);

            Assert.That(unsupported, Is.Empty);
        }

        [Test]
        public void TestSocketSharedApiDoesntHaveUnsupportedCapabilities()
        {
            var unsupported =  TestHelpers.ValidateUnsupportedCapabilities(new LighterSocketClient().ExchangeApi.SharedApi);

            Assert.That(unsupported, Is.Empty);
        }

    }
}
