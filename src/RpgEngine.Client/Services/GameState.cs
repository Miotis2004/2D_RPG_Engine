using RpgEngine.Client.Models;

namespace RpgEngine.Client.Services;

public sealed class GameState
{
    public const int MapWidth = 14;
    public const int MapHeight = 10;

    public EditorTool ActiveTool { get; set; } = EditorTool.Paint;
    public int SelectedTileId { get; set; } = 1;
    public TileLayer ActiveLayer { get; set; } = TileLayer.Ground;

    public List<TileCell> Cells { get; } = BuildStarterMap();

    public IReadOnlyList<Combatant> Party { get; } =
    [
        new("Ari", 96, 120, 28, 36, new("Battle Idle", "hero-battle.png", 48, 48, 4, 140)),
        new("Mira", 72, 80, 54, 60, new("Cast Ready", "mage-battle.png", 48, 48, 4, 130))
    ];

    public IReadOnlyList<Combatant> Enemies { get; } =
    [
        new("Forest Slime", 34, 34, 0, 0, new("Squish", "slime-battle.png", 48, 48, 4, 180)),
        new("Goblin Scout", 58, 58, 8, 8, new("Guard", "goblin-battle.png", 48, 48, 4, 150))
    ];

    public void Paint(int x, int y)
    {
        Cells.RemoveAll(cell => cell.X == x && cell.Y == y && cell.Layer == ActiveLayer);
        if (ActiveTool is not EditorTool.Erase)
        {
            Cells.Add(new TileCell(x, y, SelectedTileId, ActiveLayer));
        }
    }

    private static List<TileCell> BuildStarterMap()
    {
        var cells = new List<TileCell>();
        for (var y = 0; y < MapHeight; y++)
        {
            for (var x = 0; x < MapWidth; x++)
            {
                var tile = x is 0 || y is 0 || x == MapWidth - 1 || y == MapHeight - 1 ? 4 : 1;
                if (x is >= 4 and <= 9 && y == 5)
                {
                    tile = 2;
                }
                if (x is >= 10 && y is >= 2 and <= 4)
                {
                    tile = 3;
                }
                cells.Add(new TileCell(x, y, tile, TileLayer.Ground));
            }
        }
        cells.Add(new TileCell(7, 4, 5, TileLayer.Interaction));
        return cells;
    }
}
