namespace Polarion.Api.Requests;

public class SearchRequirementsRequest
{
    public string? Query { get; set; }
    public int MaxResults { get; set; } = 50;
}
