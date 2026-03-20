namespace Confluence.Api.Requests;

public class SearchRequest
{
    public required string Cql { get; set; }
    public int MaxResults { get; set; } = 25;
}
