using Xunit;
using _2D_RPG.Models;
using _2D_RPG.Services;

namespace _2D_RPG.Tests.Services;

public sealed class ProjectValidationServiceTests
{
    [Fact]
    public void ValidateReportsReleaseBlockingAuthoringDiagnostics()
    {
        var project = new ProjectDefinition
        {
            Id = string.Empty,
            SchemaVersion = string.Empty,
            Maps =
            [
                new()
                {
                    Id = "map-a",
                    Name = "Broken Map",
                    Width = 2,
                    Height = 2,
                    TileSetId = "missing-tileset",
                    Layers =
                    [
                        new()
                        {
                            Id = "ground",
                            Name = "Ground",
                            Kind = MapLayerKind.Ground,
                            Tiles = [new() { X = 4, Y = 4, TileId = 0 }]
                        },
                        new() { Id = "ground", Name = "Ground Copy", Kind = MapLayerKind.Decoration }
                    ],
                    Objects = [new() { Id = "npc", Name = "NPC", X = 3, Y = 0, EventId = "missing-event" }],
                    Events = [new() { Id = "event", Name = "Event", Trigger = EventTriggerKind.Touch, X = 0, Y = 0, Commands = [new() { Kind = EventCommandKind.Dialogue }] }]
                },
                new() { Id = "map-a", Name = "Duplicate Map", Width = 1, Height = 1, TileSetId = "missing-tileset" }
            ]
        };

        var diagnostics = new ProjectValidationService().Validate(project);

        Assert.Contains("Project is missing a stable id.", diagnostics);
        Assert.Contains("Project is missing a schema version.", diagnostics);
        Assert.Contains("Duplicate map id 'map-a'.", diagnostics);
        Assert.Contains("Map 'map-a' references missing tileset 'missing-tileset'.", diagnostics);
        Assert.Contains("Duplicate layer id on map 'map-a' 'ground'.", diagnostics);
        Assert.Contains("Layer 'ground' on map 'map-a' has an out-of-bounds tile at 4,4.", diagnostics);
        Assert.Contains("Object 'npc' is outside map 'map-a'.", diagnostics);
        Assert.Contains("Object 'npc' references missing event 'missing-event'.", diagnostics);
        Assert.Contains("Event 'event' has an empty dialogue command.", diagnostics);
    }
}
