namespace OpenAiBranchNaming.ApiService.Ai.Branching;

public interface IBranchNamingService
{
    Task<string> GetBranchName(string ticketName);
}