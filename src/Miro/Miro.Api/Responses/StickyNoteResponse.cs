namespace Miro.Api.Responses;

public class StickyNoteResponse
{
    public required string Id { get; set; }
    public string? Content { get; set; }
    public string? Shape { get; set; }
    public string? FillColor { get; set; }
    public double? PositionX { get; set; }
    public double? PositionY { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
