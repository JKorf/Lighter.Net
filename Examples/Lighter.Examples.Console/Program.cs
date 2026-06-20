using Lighter.Net.Clients;

// REST
var restClient = new LighterRestClient();
var ticker = await restClient.ExchangeApi.ExchangeData.GetSymbolDetailsAsync("ETH/USDC");
if (!ticker.Success)
{
    Console.WriteLine($"Failed to get ticker: {ticker.Error}");
    return;
}

Console.WriteLine($"Rest client ticker price for ETH/USDC: {ticker.Data.SpotSymbols[1].LastPrice}");

Console.WriteLine();
Console.WriteLine("Press enter to start websocket subscription");
Console.ReadLine();

// Websocket
var socketClient = new LighterSocketClient();
var subscription = await socketClient.ExchangeApi.ExchangeData.SubscribeToSpotTickerUpdatesAsync("ETH/USDC", update =>
{
    Console.WriteLine($"Websocket client ticker price for ETH/USDC: {update.Data.Ticker.LastPrice}");
});

if (!subscription.Success)
{
    Console.WriteLine($"Failed to subscribe to ticker updates: {subscription.Error}");
    return;
}

Console.ReadLine();
