using _2D_RPG.Models;

namespace _2D_RPG.Services;

public sealed class ProjectValidationService
{
    private readonly AssetCatalogService assetCatalog = new();

    public IReadOnlyList<string> Validate(ProjectDefinition project)
    {
        var errors = new List<string>();
        var mapIds = project.Maps.Select(map => map.Id).ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var map in project.Maps)
        {
            ValidateMap(map, mapIds, errors);
        }

        errors.AddRange(assetCatalog.Validate(project).Select(diagnostic => diagnostic.Message));
        return errors;
    }

    private static void ValidateMap(MapDefinition map, ISet<string> mapIds, List<string> errors)
    {
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
}
