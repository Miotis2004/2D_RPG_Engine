namespace _2D_RPG.Models;

public sealed class MapDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Width { get; set; }
    public int Height { get; set; }
    public int TileWidth { get; set; } = 32;
    public int TileHeight { get; set; } = 32;
    public string TileSetId { get; set; } = string.Empty;
    public string MusicCue { get; set; } = string.Empty;
    public List<MapLayerDefinition> Layers { get; set; } = [];

    public MapLayerDefinition? GetLayer(string id) => Layers.FirstOrDefault(layer => layer.Id == id);
}

public sealed class MapLayerDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public MapLayerKind Kind { get; set; }
    public bool IsVisible { get; set; } = true;
    public bool IsLocked { get; set; }
    public List<TilePlacement> Tiles { get; set; } = [];
}

public enum MapLayerKind
{
    Ground,
    Decoration,
    Collision,
    Objects,
    Events
}

public sealed class TilePlacement
{
    public int X { get; set; }
    public int Y { get; set; }
    public int TileId { get; set; }
}
