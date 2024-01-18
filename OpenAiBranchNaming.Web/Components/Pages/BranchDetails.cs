using System.Text;

namespace OpenAiBranchNaming.Web.Components.Pages;

public class BranchDetails
{
    public string WorkItemDetails { get; set; } = "";
    public string BranchName { get; set; } = "";
    public Status Status { get; set; }
}

public enum Status
{
    Initial, Loading, Idle
}