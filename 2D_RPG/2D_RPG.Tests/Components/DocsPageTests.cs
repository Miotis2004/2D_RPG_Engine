using Xunit;
using _2D_RPG.Components.Pages;
using Bunit;

namespace _2D_RPG.Tests.Components;

public sealed class DocsPageTests : BunitContext
{
    [Fact]
    public void RendersBuildTestAndPlayInstructions()
    {
        var docs = Render<Docs>();

        Assert.Contains("Build the application", docs.Markup);
        Assert.Contains("Test project changes", docs.Markup);
        Assert.Contains("Build a level in the Editor", docs.Markup);
        Assert.Contains("Play and verify levels", docs.Markup);
        Assert.Contains("dotnet test 2D_RPG/2D_RPG.slnx", docs.Markup);
    }
}
