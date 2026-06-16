namespace _2D_RPG.Models;

public enum AssetKind
{
    TileSet,
    SpriteSheet,
    Audio,
    Portrait,
    UserInterface,
    Other
}

public sealed class AssetDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public AssetKind Kind { get; set; } = AssetKind.Other;
    public string SourcePath { get; set; } = string.Empty;
    public int? Width { get; set; }
    public int? Height { get; set; }
    public int? FrameWidth { get; set; }
    public int? FrameHeight { get; set; }
    public List<string> Tags { get; set; } = [];
    public DateTimeOffset ImportedAt { get; set; } = DateTimeOffset.UtcNow;
}

public sealed record AssetValidationDiagnostic(string AssetId, string Message);
