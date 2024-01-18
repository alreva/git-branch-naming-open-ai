using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using OpenAiBranchNaming.ApiService.Ai;
using OpenAiBranchNaming.ApiService.Ai.Branching;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();
builder.AddRedisOutputCache("cache");

// Add services to the container.
builder.Services.AddProblemDetails();

var apiKey = builder.Configuration["OpenAI:ApiKey"]!;
builder.Services.AddHttpClient<OpenAiClient>(client =>
{
    client.BaseAddress = new Uri("https://api.openai.com");
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
});

builder.Services.AddSingleton<IBranchNamingService, OpenAiBranchNamingService>();

const string slidingPolicy = "sliding";

builder.Services.AddRateLimiter(limiterOptions =>
{
    limiterOptions
        .AddSlidingWindowLimiter(policyName: slidingPolicy, options =>
        {
            options.PermitLimit = 60;
            options.Window = TimeSpan.FromMinutes(60);
            options.SegmentsPerWindow = 6;
            options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            options.QueueLimit = 20;
        });
    
    static string GetUserEndPoint(HttpContext context) =>
        $"User {context.User.Identity?.Name ?? "Anonymous"} " +
        $"endpoint:{context.Request.Path} {context.Connection.RemoteIpAddress}";

    limiterOptions.OnRejected = (context, cancellationToken) =>
    {
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            context.HttpContext.Response.Headers.RetryAfter = ((int) retryAfter.TotalSeconds).ToString();
        }

        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.RequestServices.GetService<ILoggerFactory>()?
            .CreateLogger("Microsoft.AspNetCore.RateLimitingMiddleware")
            .LogWarning("OnRejected: {GetUserEndPoint}", GetUserEndPoint(context.HttpContext));

        return new ValueTask();
    };
});

var app = builder.Build();

app.UseRateLimiter();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.UseOutputCache();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app
    .MapPost(
        "/branch/gen",
        async (
                [FromServices] IBranchNamingService svc,
                BranchRequest request)
            => await svc.GetBranchName(request.TicketName))
    .CacheOutput(options => options.Expire(TimeSpan.FromHours(2)))
    .RequireRateLimiting(slidingPolicy);

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

record BranchRequest(string TicketName);

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
