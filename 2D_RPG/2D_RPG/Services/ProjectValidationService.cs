using _2D_RPG.Models;

namespace _2D_RPG.Services;

public sealed class ProjectValidationService
{
    private readonly AssetCatalogService assetCatalog = new();

    public IReadOnlyList<string> Validate(ProjectDefinition project)
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(project.Id)) errors.Add("Project is missing a stable id.");
        if (string.IsNullOrWhiteSpace(project.Name)) errors.Add("Project is missing a display name.");
        if (string.IsNullOrWhiteSpace(project.SchemaVersion)) errors.Add("Project is missing a schema version.");

        var mapIds = project.Maps.Select(map => map.Id).ToList();
        AddDuplicates(mapIds, "map id", errors);
        var mapIdSet = mapIds.ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (project.Maps.Count == 0) errors.Add("Project must include at least one map before release.");

        foreach (var tileSet in project.TileSets)
        {
            ValidateTileSet(tileSet, errors);
        }

        foreach (var map in project.Maps)
        {
            ValidateMap(map, mapIdSet, project.TileSets, errors);
        }

        errors.AddRange(assetCatalog.Validate(project).Select(diagnostic => diagnostic.Message));
        return errors;
    }

    private static void ValidateMap(MapDefinition map, ISet<string> mapIds, IReadOnlyCollection<TileSetDefinition> tileSets, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(map.Id)) errors.Add($"Map '{map.Name}' is missing a stable id.");
        if (string.IsNullOrWhiteSpace(map.Name)) errors.Add($"Map '{map.Id}' is missing a display name.");
        if (map.Width <= 0 || map.Height <= 0) errors.Add($"Map '{map.Id}' must be at least 1 tile wide and 1 tile tall.");
        if (map.TileWidth <= 0 || map.TileHeight <= 0) errors.Add($"Map '{map.Id}' has invalid tile dimensions.");
        if (!tileSets.Any(tileSet => string.Equals(tileSet.Id, map.TileSetId, StringComparison.OrdinalIgnoreCase))) errors.Add($"Map '{map.Id}' references missing tileset '{map.TileSetId}'.");
        AddDuplicates(map.Layers.Select(layer => layer.Id), $"layer id on map '{map.Id}'", errors);
        AddDuplicates(map.Objects.Select(obj => obj.Id), $"object id on map '{map.Id}'", errors);
        AddDuplicates(map.Events.Select(evt => evt.Id), $"event id on map '{map.Id}'", errors);

        foreach (var layer in map.Layers)
        {
            foreach (var tile in layer.Tiles)
            {
                if (!TileMapService.IsInBounds(map, tile.X, tile.Y)) errors.Add($"Layer '{layer.Id}' on map '{map.Id}' has an out-of-bounds tile at {tile.X},{tile.Y}.");
                if (tile.TileId < 0) errors.Add($"Layer '{layer.Id}' on map '{map.Id}' has a negative tile id.");
            }
        }

        foreach (var obj in map.Objects)
        {
            if (!TileMapService.IsInBounds(map, obj.X, obj.Y)) errors.Add($"Object '{obj.Id}' is outside map '{map.Id}'.");
            if (!string.IsNullOrWhiteSpace(obj.EventId) && map.Events.All(evt => evt.Id != obj.EventId)) errors.Add($"Object '{obj.Id}' references missing event '{obj.EventId}'.");
        }

        foreach (var evt in map.Events)
        {
            if (evt.Trigger != EventTriggerKind.EnterMap && (evt.X is null || evt.Y is null || !TileMapService.IsInBounds(map, evt.X.Value, evt.Y.Value))) errors.Add($"Event '{evt.Id}' has an invalid trigger location.");
            foreach (var command in evt.Commands)
            {
                if (command.Kind == EventCommandKind.ItemReward && (string.IsNullOrWhiteSpace(command.ItemId) || command.Quantity <= 0)) errors.Add($"Event '{evt.Id}' has an invalid item reward command.");
                if (command.Kind == EventCommandKind.TransferMap && (!mapIds.Contains(command.TargetMapId) || command.TargetX < 0 || command.TargetY < 0)) errors.Add($"Event '{evt.Id}' has an invalid transfer command.");
                if (command.Kind == EventCommandKind.Dialogue && string.IsNullOrWhiteSpace(command.Text)) errors.Add($"Event '{evt.Id}' has an empty dialogue command.");
            }
        }
    }

    private static void ValidateTileSet(TileSetDefinition tileSet, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(tileSet.Id)) errors.Add("A tileset is missing a stable id.");
        if (string.IsNullOrWhiteSpace(tileSet.Name)) errors.Add($"Tileset '{tileSet.Id}' is missing a display name.");
        if (tileSet.TileWidth <= 0 || tileSet.TileHeight <= 0) errors.Add($"Tileset '{tileSet.Id}' has invalid tile dimensions.");
        AddDuplicates(tileSet.Tiles.Select(tile => tile.Id), $"tile id in tileset '{tileSet.Id}'", errors);
    }

    private static void AddDuplicates(IEnumerable<string> ids, string label, List<string> errors)
    {
        foreach (var duplicate in ids.Where(id => !string.IsNullOrWhiteSpace(id)).GroupBy(id => id, StringComparer.OrdinalIgnoreCase).Where(group => group.Count() > 1).Select(group => group.Key))
        {
            errors.Add($"Duplicate {label} '{duplicate}'.");
        }
    }
}
