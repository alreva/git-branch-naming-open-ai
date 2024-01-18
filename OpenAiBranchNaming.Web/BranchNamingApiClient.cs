using Microsoft.AspNetCore.WebUtilities;

namespace OpenAiBranchNaming.Web;

public class BranchNamingApiClient(HttpClient httpClient)
{
    public async Task<string> GetBranchName(string ticketName)
    {
        var uri = QueryHelpers.AddQueryString(
            $"/branch/gen",
            new[] { new KeyValuePair<string, string?>("ticketName", ticketName) });
        return await httpClient.GetStringAsync(uri);
    }
}
