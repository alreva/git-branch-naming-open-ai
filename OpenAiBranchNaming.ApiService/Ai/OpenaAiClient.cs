using System.Text.Json;

namespace OpenAiBranchNaming.ApiService.Ai;

public class OpenAiClient(HttpClient httpClient, ILogger<OpenAiClient> logger)
{
    public async Task<OpenAIChatCompletionResponse> ExecuteQuery(
        string system,
        string prompt,
        string? endpoint = "/v1/chat/completions",
        string? model = "gpt-3.5-turbo")
    {
        logger.LogInformation("Base address: {BaseAddress}", httpClient.BaseAddress);
        logger.LogInformation("Authorization: {Authorization}", httpClient.DefaultRequestHeaders.Authorization);
        var requestData = new
        {
            model = model ?? "gpt-3.5-turbo",
            messages = new[] {
                new { role = "system", content = system },
                new { role = "user", content = prompt }
            }
        };
        logger.LogInformation("Request data: {@Request}", JsonSerializer.Serialize(requestData));
        using var httpResponse = await httpClient.PostAsJsonAsync(endpoint ?? "/v1/chat/completions", requestData);
        httpResponse.EnsureSuccessStatusCode();
        var response = (await httpResponse.Content.ReadFromJsonAsync<OpenAIChatCompletionResponse>())!;
        logger.LogInformation(
            "Response data: {@Response}",
            JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
        return response;
    }
}