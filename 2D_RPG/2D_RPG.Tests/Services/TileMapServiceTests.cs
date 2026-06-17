using Xunit;
using _2D_RPG.Services;

namespace _2D_RPG.Tests.Services;

public sealed class TileMapServiceTests
{
    [Fact]
    public void PaintTileChecksBoundsAndSupportsUndoRedo()
    {
        var project = EditorStateService.CreateSampleProject();
        var map = project.ActiveMap!;
        var service = new TileMapService();

        Assert.False(service.PaintTile(map, "layer-ground", -1, 0, 2));
        Assert.True(service.PaintTile(map, "layer-ground", 0, 0, 2));
        Assert.Equal(2, map.GetLayer("layer-ground")!.Tiles.Single(tile => tile.X == 0 && tile.Y == 0).TileId);

        Assert.True(service.Undo(project));
        Assert.Equal(4, map.GetLayer("layer-ground")!.Tiles.Single(tile => tile.X == 0 && tile.Y == 0).TileId);

        Assert.True(service.Redo(project));
        Assert.Equal(2, map.GetLayer("layer-ground")!.Tiles.Single(tile => tile.X == 0 && tile.Y == 0).TileId);
    }

    [Fact]
    public void CollisionLayerBlocksOccupiedTiles()
    {
        var project = EditorStateService.CreateSampleProject();
        var map = project.ActiveMap!;
        var service = new TileMapService();

        Assert.False(TileMapService.IsBlocked(map, 2, 2));
        Assert.True(service.PaintTile(map, "layer-collision", 2, 2, 1));

        Assert.True(TileMapService.IsBlocked(map, 2, 2));
        Assert.True(TileMapService.IsBlocked(map, map.Width, map.Height));
    }
}
