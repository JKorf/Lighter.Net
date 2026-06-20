namespace Lighter.Net.Objects
{
    /// <summary>
    /// Api addresses
    /// </summary>
    public class LighterApiAddresses
    {
        /// <summary>
        /// The address used by the LighterRestClient for the API
        /// </summary>
        public string RestClientAddress { get; set; } = "";
        /// <summary>
        /// The address used by the LighterSocketClient for the websocket API
        /// </summary>
        public string SocketClientAddress { get; set; } = "";

        /// <summary>
        /// The default addresses to connect to the Lighter API
        /// </summary>
        public static LighterApiAddresses Default = new LighterApiAddresses
        {
            RestClientAddress = "https://mainnet.zklighter.elliot.ai",
            SocketClientAddress = "wss://mainnet.zklighter.elliot.ai"
        };
    }
}
