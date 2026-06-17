using Xunit;
using _2D_RPG.Models;
using _2D_RPG.Services;

namespace _2D_RPG.Tests.Services;

public sealed class ProjectPersistenceServiceTests
{
    [Fact]
    public void LoadProjectMigratesMissingMenuCatalogsAndSchemaVersion()
    {
        var service = new ProjectPersistenceService();
        var json = "{\"id\":\"legacy\",\"name\":\"Legacy\",\"schemaVersion\":\"1.0.0\",\"maps\":[]}";

        var project = service.LoadProject(json);

        Assert.Equal(ProjectPersistenceService.CurrentSchemaVersion, project.SchemaVersion);
        Assert.NotEmpty(project.Items);
        Assert.NotEmpty(project.Spells);
        Assert.NotEmpty(project.Quests);
    }

    [Fact]
    public void RuntimeSaveRoundTripsCurrentMapPartyInventoryAndFlags()
    {
        var service = new ProjectPersistenceService();
        var save = MenuService.CreateDefaultSave();

        var loaded = service.LoadRuntime(service.SaveRuntime(save));

        Assert.Equal(ProjectPersistenceService.CurrentSchemaVersion, loaded.SchemaVersion);
        Assert.Equal("map-oakvale-village", loaded.CurrentMapId);
        Assert.Contains(loaded.Party, member => member.Id == "party-hero");
        Assert.Contains(loaded.Inventory, stack => stack.ItemId == "item-potion");
        Assert.True(loaded.Flags["intro_seen"]);
    }
}
