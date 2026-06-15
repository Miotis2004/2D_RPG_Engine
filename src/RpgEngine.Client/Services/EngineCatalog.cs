using RpgEngine.Client.Models;

namespace RpgEngine.Client.Services;

public sealed class EngineCatalog
{
    public IReadOnlyList<TileDefinition> Tiles { get; } =
    [
        new(1, "Grass", "terrain.png", 0, 0),
        new(2, "Stone Path", "terrain.png", 1, 0),
        new(3, "Water", "terrain.png", 2, 0, BlocksMovement: true),
        new(4, "Town Wall", "terrain.png", 3, 0, BlocksMovement: true),
        new(5, "Treasure", "objects.png", 0, 0)
    ];

    public IReadOnlyList<ActorDefinition> Actors { get; } =
    [
        new("hero", "Hero", "Player", new("Walk Down", "hero.png", 32, 32, 4, 120)),
        new("slime", "Slime", "Mob", new("Bounce", "slime.png", 32, 32, 4, 160)),
        new("merchant", "Merchant", "NPC", new("Idle", "merchant.png", 32, 32, 2, 400))
    ];

    public IReadOnlyList<MenuItem> ManagementMenus { get; } =
    [
        new("Settings", "Configure audio, input, display, and save slots."),
        new("Character", "Inspect stats, equipment, experience, and party order."),
        new("Items", "Browse inventory, consumables, key items, and crafting materials."),
        new("Spells", "Assign combat and exploration magic from learned spell lists."),
        new("Quest Log", "Track objectives and dialogue flags.")
    ];
}
