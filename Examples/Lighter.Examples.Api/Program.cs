using Lighter.Net.Interfaces.Clients;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add the Lighter services
builder.Services.AddLighter();

// OR to provide API credentials for accessing private endpoints, or setting other options:
/*
builder.Services.AddLighter(options =>
{
    options.ApiCredentials = new LighterCredentials("<PUBLICADDRESS>", ACCOUNT_INDEX, APIKEY_INDEX, "<API_SECRET>");
    options.Rest.RequestTimeout = TimeSpan.FromSeconds(5);
});
*/

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

// Map the endpoint and inject the rest client
app.MapGet("/{Symbol}", async ([FromServices] ILighterRestClient client, string symbol) =>
{
    var result = await client.ExchangeApi.ExchangeData.GetRecentTradesAsync(symbol);
    return result.Success
        ? Results.Ok(result.Data.OrderBy(x => x.Timestamp).Last().Price)
        : Results.Problem(result.Error?.Message, statusCode: 502);
})
.WithOpenApi();


app.MapGet("/Balances", async ([FromServices] ILighterRestClient client) =>
{
    var result = await client.ExchangeApi.Account.GetAccountsAsync();
    return result.Success
        ? Results.Ok(result.Data)
        : Results.Problem(result.Error?.Message, statusCode: 502);
})
.WithOpenApi();

app.Run();