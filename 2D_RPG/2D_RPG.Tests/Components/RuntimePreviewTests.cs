using _2D_RPG.Components.Runtime;
using _2D_RPG.Services;
using Bunit;
using Microsoft.Extensions.DependencyInjection;

namespace _2D_RPG.Tests.Components;

public sealed class RuntimePreviewTests : TestContext
{
    [Fact]
    public void RendersAnimationFrameCoordinatesAndAdvancesFrame()
    {
        var animationService = new AnimationService();
        Services.AddSingleton(new EditorStateService());
        Services.AddSingleton(animationService);
        Services.AddSingleton(new RuntimeEngineService(animationService));

        var preview = RenderComponent<RuntimePreview>();

        Assert.Contains("--sprite-x:0px", preview.Markup);
        preview.FindAll("button").Single(button => button.TextContent.Contains("Advance 140ms")).Click();

        Assert.Contains("frame 1", preview.Markup);
        Assert.Contains("--sprite-x:32px", preview.Markup);
    }
}
