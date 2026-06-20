using CryptoExchange.Net.Objects.Options;

namespace Lighter.Net.Objects.Options
{
    /// <summary>
    /// Options for the LighterRestClient
    /// </summary>
    public class LighterRestOptions : RestExchangeOptions<LighterEnvironment, LighterCredentials>
    {
        /// <summary>
        /// Default options for new clients
        /// </summary>
        internal static LighterRestOptions Default { get; set; } = new LighterRestOptions()
        {
            Environment = LighterEnvironment.Live,
            AutoTimestamp = true
        };

        /// <summary>
        /// ctor
        /// </summary>
        public LighterRestOptions()
        {
            Default?.Set(this);
        }

        /// <summary>
        /// Exchange API options
        /// </summary>
        public RestApiOptions ExchangeOptions { get; private set; } = new RestApiOptions();

        /// <summary>
        /// The integrator fee percentage to apply to orders. This refers to a fee percentage being paid to the developer to support development. Defaults to 1bps/0.01%, but can be set to 0/null. Value can be between 0.001% and 0.1%.
        /// </summary>
        public decimal? IntegratorFeePercentage { get; set; } = 0.01m;
        /// <summary>
        /// Index of the integrator 
        /// </summary>
        public long? IntegratorAccountIndex { get; set; } = 722486;
        /// <summary>
        /// Library path
        /// </summary>
        public string? LibraryPath { get; set; }

        internal LighterRestOptions Set(LighterRestOptions targetOptions)
        {
            targetOptions = base.Set<LighterRestOptions>(targetOptions);
            targetOptions.IntegratorFeePercentage = IntegratorFeePercentage;
            targetOptions.IntegratorAccountIndex = IntegratorAccountIndex;
            targetOptions.LibraryPath = LibraryPath;
            targetOptions.ExchangeOptions = ExchangeOptions.Set(targetOptions.ExchangeOptions);

            return targetOptions;
        }
    }
}
