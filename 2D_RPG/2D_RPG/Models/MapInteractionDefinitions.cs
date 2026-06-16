namespace _2D_RPG.Models;

public enum MapObjectKind
{
    Prop,
    Door,
    Chest,
    Npc,
    Trigger
}

public enum EventTriggerKind
{
    Interact,
    Touch,
    EnterMap
}

public enum EventCommandKind
{
    Dialogue,
    ItemReward,
    TransferMap
}

public sealed class MapObjectDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public MapObjectKind Kind { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public bool IsBlocking { get; set; }
    public string SpriteKey { get; set; } = string.Empty;
    public string EventId { get; set; } = string.Empty;
    public Dictionary<string, string> Properties { get; set; } = [];
}

public sealed class MapEventDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public EventTriggerKind Trigger { get; set; }
    public int? X { get; set; }
    public int? Y { get; set; }
    public string ConditionExpression { get; set; } = string.Empty;
    public List<EventCommandDefinition> Commands { get; set; } = [];
}

public sealed class EventCommandDefinition
{
    public EventCommandKind Kind { get; set; }
    public string Text { get; set; } = string.Empty;
    public string ItemId { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public string TargetMapId { get; set; } = string.Empty;
    public int TargetX { get; set; }
    public int TargetY { get; set; }
}
