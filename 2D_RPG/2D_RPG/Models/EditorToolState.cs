namespace _2D_RPG.Models;

public enum EditorToolKind
{
    Paint,
    Erase,
    Fill,
    Select
}

public sealed class EditorToolState
{
    public EditorToolKind ActiveTool { get; set; } = EditorToolKind.Paint;
    public int SelectedTileId { get; set; } = 1;
    public string ActiveLayerId { get; set; } = "layer-ground";
    public double Zoom { get; set; } = 1;
}
