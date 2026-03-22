namespace Miro.Api.Requests;

public class CreateStickyNoteRequest
{
    public string? Content { get; set; }
    public string? Shape { get; set; }
    public string? FillColor { get; set; }
    public double? PositionX { get; set; }
    public double? PositionY { get; set; }
}
