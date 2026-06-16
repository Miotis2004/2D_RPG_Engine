using _2D_RPG.Components.Editor;
using _2D_RPG.Services;
using Bunit;
using Microsoft.Extensions.DependencyInjection;

namespace _2D_RPG.Tests.Components;

public sealed class EditorShellTests : TestContext
{
    [Fact]
    public void RendersProjectMapAndCleanDirtyStatus()
    {
        var editorState = new EditorStateService();
        Services.AddSingleton(editorState);
        Services.AddSingleton<TileMapService>();
        Services.AddSingleton<MapObjectService>();
        Services.AddSingleton<ProjectValidationService>();

        var shell = RenderComponent<EditorShell>();

        Assert.Contains("Sample RPG Project", shell.Markup);
        Assert.Contains("Oakvale Village", shell.Markup);
        Assert.Contains("Clean", shell.Markup);

        shell.FindAll("button").Single(button => button.TextContent.Contains("New map")).Click();

        Assert.True(editorState.IsDirty);
        Assert.Contains("Unsaved changes", shell.Markup);
    }
}
