using Microsoft.AspNetCore.Mvc;
using OpenAiBranchNaming.ApiService.Ai;
using OpenAiBranchNaming.ApiService.Ai.Branching;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

var apiKey = builder.Configuration["OpenAI:ApiKey"]!;
builder.Services.AddHttpClient<OpenAiClient>(client =>
{
    client.BaseAddress = new Uri("https://api.openai.com");
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
});

builder.Services.AddSingleton<IBranchNamingService, OpenAiBranchNamingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app
    .MapGet(
        "/branch/gen/{ticketName}",
        async (
                [FromServices] IBranchNamingService svc,
                string ticketName)
            => await svc.GetBranchName(ticketName))
    .CacheOutput(options => options.Expire(TimeSpan.FromHours(2)));

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
});

app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
