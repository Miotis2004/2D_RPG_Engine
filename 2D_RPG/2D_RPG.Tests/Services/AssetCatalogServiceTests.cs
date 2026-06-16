using _2D_RPG.Models;
using _2D_RPG.Services;

namespace _2D_RPG.Tests.Services;

public sealed class AssetCatalogServiceTests
{
    [Fact]
    public void ImportResolvesStandardFolderAndUniqueId()
    {
        var project = new ProjectDefinition();
        var service = new AssetCatalogService();

        var first = service.Import(project, "Hero", AssetKind.SpriteSheet, "hero.png");
        var second = service.Import(project, "Hero Duplicate", AssetKind.SpriteSheet, "hero.png");

        Assert.Equal("assets/spritesheets/hero.png", first.SourcePath);
        Assert.Equal("asset-hero", first.Id);
        Assert.Equal("asset-hero-2", second.Id);
    }

    [Fact]
    public void ValidateReportsMissingFilesDuplicateIdsAndBadDimensions()
    {
        var project = new ProjectDefinition
        {
            Assets =
            [
                new() { Id = "asset-a", Name = "A", Kind = AssetKind.TileSet, SourcePath = "assets/tilesets/a.png", Width = 31, Height = 32, FrameWidth = 16, FrameHeight = 16 },
                new() { Id = "asset-a", Name = "A Copy", Kind = AssetKind.Audio, SourcePath = "assets/audio/theme.txt" }
            ]
        };
        var service = new AssetCatalogService();

        var diagnostics = service.Validate(project, path => path.EndsWith("theme.txt", StringComparison.OrdinalIgnoreCase));

        Assert.Contains(diagnostics, diagnostic => diagnostic.Message.Contains("Duplicate asset id"));
        Assert.Contains(diagnostics, diagnostic => diagnostic.Message.Contains("missing"));
        Assert.Contains(diagnostics, diagnostic => diagnostic.Message.Contains("unsupported file extension"));
        Assert.Contains(diagnostics, diagnostic => diagnostic.Message.Contains("width must be divisible"));
    }
}
