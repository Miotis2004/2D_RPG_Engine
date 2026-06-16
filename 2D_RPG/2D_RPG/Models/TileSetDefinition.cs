namespace _2D_RPG.Models;

public sealed class TileSetDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string SourcePath { get; set; } = string.Empty;
    public int TileWidth { get; set; } = 32;
    public int TileHeight { get; set; } = 32;
    public List<TileDefinition> Tiles { get; set; } = [];
}

public sealed class TileDefinition
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CssClass { get; set; } = "grass";
    public bool IsWalkable { get; set; } = true;
    public List<string> Tags { get; set; } = [];
}
