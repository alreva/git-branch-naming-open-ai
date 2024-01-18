namespace OpenAiBranchNaming.Web;

public class BranchNamingApiClient(HttpClient httpClient)
{
    public async Task<string> GetBranchName(string ticketName)
    {
        var r = await httpClient.PostAsJsonAsync($"/branch/gen", new {ticketName});
        return await r.Content.ReadAsStringAsync();
    }
}
