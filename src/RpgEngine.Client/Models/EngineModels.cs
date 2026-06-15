namespace RpgEngine.Client.Models;

public enum TileLayer
{
    Ground,
    Decoration,
    Collision,
    Interaction
}

public enum EditorTool
{
    Paint,
    Erase,
    Select,
    Collision,
    Interaction
}

public sealed record TileDefinition(int Id, string Name, string SpriteSheet, int SpriteX, int SpriteY, bool BlocksMovement = false);

public sealed record TileCell(int X, int Y, int TileId, TileLayer Layer);

public sealed record SpriteAnimation(string Name, string SpriteSheet, int FrameWidth, int FrameHeight, int FrameCount, int FrameDurationMs);

public sealed record ActorDefinition(string Id, string Name, string Role, SpriteAnimation Animation);

public sealed record Combatant(string Name, int HitPoints, int MaxHitPoints, int MagicPoints, int MaxMagicPoints, SpriteAnimation IdleAnimation);

public sealed record MenuItem(string Label, string Description);
