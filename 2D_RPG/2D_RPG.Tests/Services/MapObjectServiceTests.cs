using _2D_RPG.Models;
using _2D_RPG.Services;

namespace _2D_RPG.Tests.Services;

public sealed class MapObjectServiceTests
{
    [Fact]
    public void PlacesMovesAndDeletesObjectsAndEvents()
    {
        var project = EditorStateService.CreateSampleProject();
        var map = project.ActiveMap!;
        var service = new MapObjectService();

        var obj = service.PlaceObject(map, MapObjectKind.Chest, 2, 3);
        Assert.NotNull(obj);
        Assert.Equal((2, 3), (obj.X, obj.Y));
        Assert.True(obj.IsBlocking);

        Assert.True(service.MoveObject(map, obj.Id, 4, 5));
        Assert.Equal((4, 5), (obj.X, obj.Y));

        var evt = service.PlaceEvent(map, EventTriggerKind.Touch, 4, 5);
        Assert.NotNull(evt);
        Assert.Equal(EventCommandKind.Dialogue, evt.Commands.Single().Kind);

        Assert.True(service.DeleteObject(map, obj.Id));
        Assert.True(service.DeleteEvent(map, evt.Id));
    }

    [Fact]
    public void ProjectValidationReportsBrokenReferencesAndCommandData()
    {
        var project = EditorStateService.CreateSampleProject();
        var map = project.ActiveMap!;
        map.Objects.Add(new() { Id = "obj-broken", Name = "Broken", X = map.Width + 1, Y = 0, EventId = "missing-event" });
        map.Events.Add(new() { Id = "event-bad-reward", Name = "Bad Reward", Trigger = EventTriggerKind.Interact, X = 1, Y = 1, Commands = [new() { Kind = EventCommandKind.ItemReward, Quantity = 0 }] });

        var errors = new ProjectValidationService().Validate(project);

        Assert.Contains(errors, error => error.Contains("outside map"));
        Assert.Contains(errors, error => error.Contains("missing event"));
        Assert.Contains(errors, error => error.Contains("invalid item reward"));
    }
}
