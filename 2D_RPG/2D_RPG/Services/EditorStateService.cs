using _2D_RPG.Models;

namespace _2D_RPG.Services;

public sealed class EditorStateService
{
    public ProjectDefinition CurrentProject { get; private set; } = CreateSampleProject();
    public bool IsDirty { get; private set; }
    public EditorToolState ToolState { get; } = new();
    public string ActiveMapId
    {
        get => CurrentProject.ActiveMapId;
        set
        {
            if (CurrentProject.Maps.Any(map => map.Id == value) && CurrentProject.ActiveMapId != value)
            {
                CurrentProject.ActiveMapId = value;
                MarkDirty();
            }
        }
    }

    public MapDefinition? ActiveMap => CurrentProject.ActiveMap;

    public void SetActiveTool(EditorToolKind tool) => ToolState.ActiveTool = tool;

    public void SelectTile(int tileId)
    {
        ToolState.SelectedTileId = tileId;
        ToolState.ActiveTool = EditorToolKind.Paint;
    }

    public void SetActiveLayer(string layerId) => ToolState.ActiveLayerId = layerId;

    public void MarkDirty() => IsDirty = true;

    public void MarkClean() => IsDirty = false;

    public static ProjectDefinition CreateSampleProject()
    {
        var tileSet = new TileSetDefinition
        {
            Id = "tileset-oakvale-fields",
            Name = "Oakvale Fields",
            SourcePath = "assets/tilesets/oakvale-fields.png",
            Tiles =
            [
                new() { Id = 1, Name = "Grass", CssClass = "grass", Tags = ["terrain", "field"] },
                new() { Id = 2, Name = "Path", CssClass = "path", Tags = ["terrain", "road"] },
                new() { Id = 3, Name = "Water", CssClass = "water", IsWalkable = false, Tags = ["terrain", "water"] },
                new() { Id = 4, Name = "Forest", CssClass = "forest", IsWalkable = false, Tags = ["terrain", "blocked"] },
                new() { Id = 5, Name = "Stone", CssClass = "stone", Tags = ["terrain", "town"] },
                new() { Id = 6, Name = "Roof", CssClass = "roof", IsWalkable = false, Tags = ["building"] }
            ]
        };

        return new ProjectDefinition
        {
            TileSets = [tileSet],
            Maps = [CreateSampleMap(tileSet.Id)]
        };
    }

    private static MapDefinition CreateSampleMap(string tileSetId)
    {
        var groundRows = new[]
        {
            "44444111111111333333",
            "41111111111111333333",
            "11112222211111133333",
            "11112111211551111111",
            "11112166111551111111",
            "22222166111111114444",
            "11111111222222114444",
            "11511111111112111111",
            "11511155551112111111",
            "11111155551112111551",
            "33333111111112111551",
            "33333111111112222222"
        };

        return new MapDefinition
        {
            Id = "map-oakvale-village",
            Name = "Oakvale Village",
            Width = 20,
            Height = 12,
            TileSetId = tileSetId,
            MusicCue = "music/oakvale-theme.ogg",
            Layers =
            [
                new()
                {
                    Id = "layer-ground",
                    Name = "Ground",
                    Kind = MapLayerKind.Ground,
                    Tiles = ToPlacements(groundRows)
                },
                new() { Id = "layer-decoration", Name = "Decoration", Kind = MapLayerKind.Decoration },
                new() { Id = "layer-collision", Name = "Collision", Kind = MapLayerKind.Collision },
                new() { Id = "layer-objects", Name = "Objects", Kind = MapLayerKind.Objects },
                new() { Id = "layer-events", Name = "Events", Kind = MapLayerKind.Events }
            ]
        };
    }

    private static List<TilePlacement> ToPlacements(IReadOnlyList<string> rows)
    {
        var placements = new List<TilePlacement>();
        for (var y = 0; y < rows.Count; y++)
        {
            for (var x = 0; x < rows[y].Length; x++)
            {
                placements.Add(new TilePlacement { X = x, Y = y, TileId = rows[y][x] - '0' });
            }
        }

        return placements;
    }
}
