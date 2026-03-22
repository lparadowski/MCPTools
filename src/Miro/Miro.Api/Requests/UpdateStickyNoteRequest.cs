namespace Miro.Api.Requests;

public class UpdateStickyNoteRequest
{
    public string? Content { get; set; }
    public string? FillColor { get; set; }
    public double? PositionX { get; set; }
    public double? PositionY { get; set; }
}
