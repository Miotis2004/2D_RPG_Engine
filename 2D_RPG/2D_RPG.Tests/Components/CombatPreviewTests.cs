using _2D_RPG.Components.Runtime;
using _2D_RPG.Services;
using Bunit;
using Microsoft.Extensions.DependencyInjection;

namespace _2D_RPG.Tests.Components;

public sealed class CombatPreviewTests : TestContext
{
    [Fact]
    public void CommandMenuBlocksInvalidTargetsAndUpdatesBattleLog()
    {
        var combat = new CombatService();
        Services.AddSingleton(new EditorStateService());
        Services.AddSingleton(combat);

        var preview = RenderComponent<CombatPreview>();

        Assert.Contains("Turn: Hero", preview.Markup);
        Assert.Contains("Slime", preview.Markup);
        Assert.True(preview.FindAll("button").Single(button => button.TextContent.Contains("HeroHP")).HasAttribute("disabled"));

        preview.FindAll("button").Single(button => button.TextContent.Contains("SlimeHP")).Click();

        Assert.Contains("Hero used Attack on Slime", preview.Markup);
        Assert.Equal("enemy-goblin", combat.State.ActiveActorId);
    }
}
