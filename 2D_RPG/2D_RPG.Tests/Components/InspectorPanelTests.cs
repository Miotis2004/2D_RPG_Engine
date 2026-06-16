using _2D_RPG.Components.Editor;
using _2D_RPG.Services;
using Bunit;
using Microsoft.Extensions.DependencyInjection;

namespace _2D_RPG.Tests.Components;

public sealed class InspectorPanelTests : TestContext
{
    [Fact]
    public void UpdatesSelectedObjectAndEventThroughInputs()
    {
        var editorState = new EditorStateService();
        var project = editorState.CurrentProject;
        var map = project.ActiveMap!;
        editorState.ToolState.SelectedObjectId = "obj-elder";
        editorState.ToolState.SelectedEventId = "event-elder-dialogue";
        Services.AddSingleton(editorState);
        Services.AddSingleton<MapObjectService>();

        var panel = RenderComponent<InspectorPanel>(parameters => parameters
            .Add(component => component.Project, project)
            .Add(component => component.Map, map));

        panel.Find("input[value='Village Elder']").Change("Mayor Rowan");
        panel.Find("input[value='Welcome to Oakvale.']").Change("Mind the old road.");

        Assert.Equal("Mayor Rowan", map.Objects.Single(obj => obj.Id == "obj-elder").Name);
        Assert.Equal("Mind the old road.", map.Events.Single(evt => evt.Id == "event-elder-dialogue").Commands.Single().Text);
        Assert.True(editorState.IsDirty);
    }
}
