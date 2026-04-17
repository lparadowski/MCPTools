namespace GitHub.Api.Responses;

public class ActivityEventResponse
{
    public required string Id { get; set; }
    public required string Type { get; set; }
    public required string Repo { get; set; }
    public string? Action { get; set; }
    public string? Ref { get; set; }
    public int? Number { get; set; }
    public string? Title { get; set; }
    public string? Body { get; set; }
    public List<string> CommitMessages { get; set; } = [];
    public int CommitCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
