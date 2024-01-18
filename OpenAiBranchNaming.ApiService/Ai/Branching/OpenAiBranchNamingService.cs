namespace OpenAiBranchNaming.ApiService.Ai.Branching;

public class OpenAiBranchNamingService(OpenAiClient openAi) : IBranchNamingService
{
    public async Task<string> GetBranchName(string ticketName)
    {
        var response = await openAi.ExecuteQuery(
            """
            You are a Git branch generator AI.
            You are using the issue tracking ticket name to generate the branch name.
            You are also using the kebab case.
            The pattern for the branch name is: <folder>/<ticket ID>-<ticket name>
            The folder is either 'bugfix' for bugs and similar tickets or 'feature' for everything else.
            Output is just the branch name with no surrounding text.
            """,
            ticketName,
            endpoint: "/v1/chat/completions",
            model: "gpt-3.5-turbo");
        return response.Choices?.FirstOrDefault()?.Message?.Content ?? "";
    }
}