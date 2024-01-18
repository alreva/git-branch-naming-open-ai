namespace OpenAiBranchNaming.Web;

public class BranchNamingApiClient(HttpClient httpClient)
{
    public async Task<string> GetBranchName(string ticketName)
    {
        return await httpClient.GetStringAsync($"/branch/gen/{ticketName}") ?? "";
    }
}
