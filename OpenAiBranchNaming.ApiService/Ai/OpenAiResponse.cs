// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace OpenAiBranchNaming.ApiService.Ai;

public record OpenAiResponse
{
    public Choice[]? Choices { get; set; }

    public record Choice
    {
        public string? Text { get; set; }
    }
}

public record OpenAIChatCompletionResponse
{
    public string Id { get; init; }
    public string Object { get; init; }
    public long Created { get; init; }
    public string Model { get; init; }
    public string SystemFingerprint { get; init; }
    public Choice[] Choices { get; init; }
    public UsageData Usage { get; init; }
}

public record Choice
{
    public int Index { get; init; }
    public ChatMessage Message { get; init; }
    // ReSharper disable once IdentifierTypo
    public object Logprobs { get; init; } // Can be changed to a specific type if the structure is known
    public string FinishReason { get; init; }
}

public record ChatMessage
{
    public string Role { get; init; } // "assistant", "user", etc.
    public string Content { get; init; }
}

public record UsageData
{
    public int PromptTokens { get; init; }
    public int CompletionTokens { get; init; }
    public int TotalTokens { get; init; }
}