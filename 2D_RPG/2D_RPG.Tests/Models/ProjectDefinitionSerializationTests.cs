using Xunit;
using System.Text.Json;
using _2D_RPG.Models;
using _2D_RPG.Services;

namespace _2D_RPG.Tests.Models;

public sealed class ProjectDefinitionSerializationTests
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    [Fact]
    public void SampleProjectRoundTripsWithStableIdsAndReferences()
    {
        var project = EditorStateService.CreateSampleProject();

        var json = JsonSerializer.Serialize(project, JsonOptions);
        var restored = JsonSerializer.Deserialize<ProjectDefinition>(json, JsonOptions);

        Assert.NotNull(restored);
        Assert.Equal(project.Id, restored.Id);
        Assert.Equal(project.ActiveMapId, restored.ActiveMapId);
        Assert.NotNull(restored.ActiveMap);
        Assert.Equal("map-oakvale-village", restored.ActiveMap.Id);
        Assert.Equal(restored.TileSets.Single().Id, restored.ActiveMap.TileSetId);
        Assert.Equal("layer-ground", restored.ActiveMap.GetLayer("layer-ground")?.Id);
        Assert.Equal(20 * 12, restored.ActiveMap.GetLayer("layer-ground")?.Tiles.Count);
        Assert.Equal("obj-elder", restored.ActiveMap.Objects.Single(obj => obj.Name == "Village Elder").Id);
        Assert.Equal(EventCommandKind.ItemReward, restored.ActiveMap.Events.Single(evt => evt.Id == "event-supply-chest").Commands.Single().Kind);
    }
}
