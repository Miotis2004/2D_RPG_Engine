namespace _2D_RPG.Models;

public enum ItemKind { Consumable, Weapon, Armor, KeyItem }
public enum EquipmentSlotKind { Weapon, Armor, Accessory }
public enum QuestStatus { NotStarted, Active, Completed }

public sealed class ItemDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ItemKind Kind { get; set; } = ItemKind.Consumable;
    public int MaxStack { get; set; } = 99;
    public int HpRestore { get; set; }
    public int MpRestore { get; set; }
    public EquipmentSlotKind? EquipmentSlot { get; set; }
    public string Description { get; set; } = string.Empty;
}

public sealed class SpellDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int MpCost { get; set; }
    public string Description { get; set; } = string.Empty;
}

public sealed class QuestDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string RequiredFlag { get; set; } = string.Empty;
    public string CompletionFlag { get; set; } = string.Empty;
}

public sealed class PartyMemberState
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Level { get; set; } = 1;
    public int Hp { get; set; } = 100;
    public int MaxHp { get; set; } = 100;
    public int Mp { get; set; } = 20;
    public int MaxMp { get; set; } = 20;
    public List<string> KnownSpellIds { get; set; } = [];
    public Dictionary<EquipmentSlotKind, string> Equipment { get; set; } = [];
}

public sealed class InventoryStack
{
    public string ItemId { get; set; } = string.Empty;
    public int Quantity { get; set; }
}

public sealed class RuntimeSaveData
{
    public string SchemaVersion { get; set; } = "1.1.0";
    public string SaveName { get; set; } = "Autosave";
    public DateTimeOffset SavedAt { get; set; } = DateTimeOffset.UtcNow;
    public string CurrentMapId { get; set; } = string.Empty;
    public int PlayerX { get; set; }
    public int PlayerY { get; set; }
    public List<PartyMemberState> Party { get; set; } = [];
    public List<InventoryStack> Inventory { get; set; } = [];
    public Dictionary<string, bool> Flags { get; set; } = [];
}
