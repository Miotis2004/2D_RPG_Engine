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
            Assets =
            [
                new() { Id = "asset-oakvale-fields", Name = "Oakvale Fields Tileset", Kind = AssetKind.TileSet, SourcePath = tileSet.SourcePath, Width = 192, Height = 32, FrameWidth = 32, FrameHeight = 32, Tags = ["terrain", "field"] },
                new() { Id = "asset-village-npcs", Name = "Village NPCs", Kind = AssetKind.SpriteSheet, SourcePath = "assets/spritesheets/village-npcs.png", Width = 128, Height = 192, FrameWidth = 32, FrameHeight = 48, Tags = ["npc", "village"] },
                new() { Id = "asset-oakvale-theme", Name = "Oakvale Theme", Kind = AssetKind.Audio, SourcePath = "assets/audio/oakvale-theme.ogg", Tags = ["music", "village"] }
            ],
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
            ],
            Objects =
            [
                new() { Id = "obj-town-gate", Name = "North Gate", Kind = MapObjectKind.Door, X = 10, Y = 3, IsBlocking = true, SpriteKey = "door", EventId = "event-gate-transfer" },
                new() { Id = "obj-elder", Name = "Village Elder", Kind = MapObjectKind.Npc, X = 5, Y = 8, IsBlocking = true, SpriteKey = "npc", EventId = "event-elder-dialogue" },
                new() { Id = "obj-supply-chest", Name = "Supply Chest", Kind = MapObjectKind.Chest, X = 14, Y = 9, IsBlocking = true, SpriteKey = "chest", EventId = "event-supply-chest" }
            ],
            Events =
            [
                new() { Id = "event-elder-dialogue", Name = "Elder Greeting", Trigger = EventTriggerKind.Interact, X = 5, Y = 8, Commands = [new() { Kind = EventCommandKind.Dialogue, Text = "Welcome to Oakvale." }] },
                new() { Id = "event-supply-chest", Name = "Supply Reward", Trigger = EventTriggerKind.Interact, X = 14, Y = 9, Commands = [new() { Kind = EventCommandKind.ItemReward, ItemId = "item-potion", Quantity = 2 }] },
                new() { Id = "event-gate-transfer", Name = "Gate Transfer", Trigger = EventTriggerKind.Touch, X = 10, Y = 3, Commands = [new() { Kind = EventCommandKind.TransferMap, TargetMapId = "map-oakvale-village", TargetX = 1, TargetY = 1 }] },
                new() { Id = "event-oakvale-enter", Name = "Oakvale Arrival", Trigger = EventTriggerKind.EnterMap, ConditionExpression = "flag.intro_seen == false", Commands = [new() { Kind = EventCommandKind.Dialogue, Text = "You arrive at Oakvale Village." }] }
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
