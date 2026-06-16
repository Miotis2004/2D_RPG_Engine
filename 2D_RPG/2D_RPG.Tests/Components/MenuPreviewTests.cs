using _2D_RPG.Components.Runtime;
using _2D_RPG.Services;
using Bunit;
using Microsoft.Extensions.DependencyInjection;

namespace _2D_RPG.Tests.Components;

public sealed class MenuPreviewTests : TestContext
{
    [Fact]
    public void MenuPreviewDisplaysInventoryAndCanRoundTripSave()
    {
        Services.AddSingleton(new EditorStateService());
        Services.AddSingleton(new MenuService());
        Services.AddSingleton(new ProjectPersistenceService());

        var menu = RenderComponent<MenuPreview>();

        Assert.Contains("Potion × 3", menu.Markup);
        menu.FindAll("button").Single(button => button.TextContent.Contains("Save / load")).Click();
        Assert.Contains("Saved and loaded Oakvale Autosave", menu.Markup);
    }
}
