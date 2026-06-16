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
    public MapObjectKind ObjectBrush { get; set; } = MapObjectKind.Prop;
    public EventTriggerKind EventBrush { get; set; } = EventTriggerKind.Interact;
    public string? SelectedObjectId { get; set; }
    public string? SelectedEventId { get; set; }
    public double Zoom { get; set; } = 1;
}
