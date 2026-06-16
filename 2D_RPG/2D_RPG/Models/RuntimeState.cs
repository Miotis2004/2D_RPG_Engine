namespace _2D_RPG.Models;

public enum RuntimeDirection
{
    Down,
    Left,
    Right,
    Up
}

public sealed class RuntimeActorState
{
    public string Id { get; set; } = "player";
    public string Name { get; set; } = "Hero";
    public int X { get; set; }
    public int Y { get; set; }
    public RuntimeDirection Facing { get; set; } = RuntimeDirection.Down;
}

public sealed class RuntimeMessage
{
    public string Text { get; set; } = string.Empty;
    public EventCommandKind SourceCommand { get; set; }
}

public sealed class RuntimeSessionState
{
    public MapDefinition? Map { get; set; }
    public RuntimeActorState Player { get; set; } = new();
    public List<RuntimeMessage> Messages { get; set; } = [];
    public HashSet<string> Inventory { get; set; } = [];
    public Dictionary<string, bool> Flags { get; set; } = [];
    public string LastInteraction { get; set; } = "Ready.";
}
