using Xunit;
using _2D_RPG.Components.Runtime;
using _2D_RPG.Services;
using Bunit;
using Microsoft.Extensions.DependencyInjection;

namespace _2D_RPG.Tests.Components;

public sealed class CombatPreviewTests : BunitContext
{
    [Fact]
    public void CommandMenuBlocksInvalidTargetsAndUpdatesBattleLog()
    {
        var combat = new CombatService();
        Services.AddSingleton(new EditorStateService());
        Services.AddSingleton(combat);

        var preview = Render<CombatPreview>();

        Assert.Contains("Turn: Hero", preview.Markup);
        Assert.Contains("Slime", preview.Markup);
        Assert.True(preview.FindAll("button").First(button => button.TextContent.Contains("HP") && button.ClassList.Contains("party-combatant")).HasAttribute("disabled"));

        preview.FindAll("button").First(button => button.TextContent.Contains("HP") && button.ClassList.Contains("enemy-combatant")).Click();

        Assert.Contains("Hero used Attack on Slime", preview.Markup);
        Assert.Equal("enemy-goblin", combat.State.ActiveActorId);
    }
}
