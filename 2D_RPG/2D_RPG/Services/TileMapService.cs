using _2D_RPG.Models;

namespace _2D_RPG.Services;

public sealed class TileMapService
{
    private readonly Stack<MapEditOperation> undoStack = [];
    private readonly Stack<MapEditOperation> redoStack = [];

    public bool CanUndo => undoStack.Count > 0;
    public bool CanRedo => redoStack.Count > 0;

    public bool PaintTile(MapDefinition map, string layerId, int x, int y, int tileId)
    {
        if (!IsInBounds(map, x, y) || tileId < 0)
        {
            return false;
        }

        var layer = map.GetLayer(layerId);
        if (layer is null || layer.IsLocked)
        {
            return false;
        }

        var previous = GetTile(layer, x, y)?.TileId;
        if (previous == tileId)
        {
            return false;
        }

        SetTile(layer, x, y, tileId);
        PushUndo(new MapEditOperation(map.Id, layerId, x, y, previous, tileId));
        return true;
    }

    public bool EraseTile(MapDefinition map, string layerId, int x, int y)
    {
        if (!IsInBounds(map, x, y))
        {
            return false;
        }

        var layer = map.GetLayer(layerId);
        if (layer is null || layer.IsLocked)
        {
            return false;
        }

        var existing = GetTile(layer, x, y);
        if (existing is null)
        {
            return false;
        }

        layer.Tiles.Remove(existing);
        PushUndo(new MapEditOperation(map.Id, layerId, x, y, existing.TileId, null));
        return true;
    }

    public bool FillLayer(MapDefinition map, string layerId, int tileId)
    {
        var layer = map.GetLayer(layerId);
        if (layer is null || layer.IsLocked || tileId < 0)
        {
            return false;
        }

        var before = layer.Tiles.Select(tile => new TilePlacement { X = tile.X, Y = tile.Y, TileId = tile.TileId }).ToList();
        layer.Tiles.Clear();
        for (var y = 0; y < map.Height; y++)
        {
            for (var x = 0; x < map.Width; x++)
            {
                layer.Tiles.Add(new TilePlacement { X = x, Y = y, TileId = tileId });
            }
        }

        PushUndo(new MapEditOperation(map.Id, layerId, before, layer.Tiles.Select(tile => new TilePlacement { X = tile.X, Y = tile.Y, TileId = tile.TileId }).ToList()));
        return true;
    }

    public bool Undo(ProjectDefinition project) => MoveEdit(project, undoStack, redoStack, useBefore: true);

    public bool Redo(ProjectDefinition project) => MoveEdit(project, redoStack, undoStack, useBefore: false);

    public static bool IsInBounds(MapDefinition map, int x, int y) => x >= 0 && y >= 0 && x < map.Width && y < map.Height;

    public static bool IsBlocked(MapDefinition map, int x, int y)
    {
        if (!IsInBounds(map, x, y))
        {
            return true;
        }

        return map.Layers.Any(layer => layer.Kind == MapLayerKind.Collision && GetTile(layer, x, y) is not null)
            || map.Objects.Any(obj => obj.IsBlocking && obj.X == x && obj.Y == y);
    }

    private static TilePlacement? GetTile(MapLayerDefinition layer, int x, int y) => layer.Tiles.FirstOrDefault(tile => tile.X == x && tile.Y == y);

    private static void SetTile(MapLayerDefinition layer, int x, int y, int tileId)
    {
        var tile = GetTile(layer, x, y);
        if (tile is null)
        {
            layer.Tiles.Add(new TilePlacement { X = x, Y = y, TileId = tileId });
        }
        else
        {
            tile.TileId = tileId;
        }
    }

    private void PushUndo(MapEditOperation operation)
    {
        undoStack.Push(operation);
        redoStack.Clear();
    }

    private static bool MoveEdit(ProjectDefinition project, Stack<MapEditOperation> source, Stack<MapEditOperation> destination, bool useBefore)
    {
        if (!source.TryPop(out var operation))
        {
            return false;
        }

        var map = project.Maps.FirstOrDefault(candidate => candidate.Id == operation.MapId);
        var layer = map?.GetLayer(operation.LayerId);
        if (layer is null)
        {
            return false;
        }

        if (operation.BeforeTiles is not null && operation.AfterTiles is not null)
        {
            layer.Tiles = (useBefore ? operation.BeforeTiles : operation.AfterTiles)
                .Select(tile => new TilePlacement { X = tile.X, Y = tile.Y, TileId = tile.TileId }).ToList();
        }
        else if ((useBefore ? operation.BeforeTileId : operation.AfterTileId) is { } tileId)
        {
            SetTile(layer, operation.X, operation.Y, tileId);
        }
        else
        {
            var tile = GetTile(layer, operation.X, operation.Y);
            if (tile is not null)
            {
                layer.Tiles.Remove(tile);
            }
        }

        destination.Push(operation);
        return true;
    }

    private sealed record MapEditOperation(string MapId, string LayerId, int X, int Y, int? BeforeTileId, int? AfterTileId)
    {
        public MapEditOperation(string mapId, string layerId, List<TilePlacement> beforeTiles, List<TilePlacement> afterTiles)
            : this(mapId, layerId, 0, 0, null, null)
        {
            BeforeTiles = beforeTiles;
            AfterTiles = afterTiles;
        }

        public List<TilePlacement>? BeforeTiles { get; }
        public List<TilePlacement>? AfterTiles { get; }
    }
}
