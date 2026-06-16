using _2D_RPG.Components.Editor;
using _2D_RPG.Services;
using Bunit;
using Microsoft.Extensions.DependencyInjection;

namespace _2D_RPG.Tests.Components;

public sealed class AssetBrowserTests : TestContext
{
    [Fact]
    public void RendersAssetsDiagnosticsAndImportFlow()
    {
        var project = EditorStateService.CreateSampleProject();
        Services.AddSingleton<AssetCatalogService>();
        var changed = false;

        var browser = RenderComponent<AssetBrowser>(parameters => parameters
            .Add(component => component.Project, project)
            .Add(component => component.FileExists, _ => false)
            .Add(component => component.Changed, () => changed = true));

        Assert.Contains("Oakvale Fields Tileset", browser.Markup);
        Assert.Contains("is missing", browser.Markup);

        browser.FindAll("button").Single(button => button.TextContent.Contains("Import sample")).Click();

        Assert.True(changed);
        Assert.Contains(project.Assets, asset => asset.Id == "asset-imported-sprite");
    }
}
