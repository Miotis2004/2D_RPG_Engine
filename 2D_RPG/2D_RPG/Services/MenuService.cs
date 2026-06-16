using _2D_RPG.Models;

namespace _2D_RPG.Services;

public sealed class MenuService
{
    public RuntimeSaveData SaveData { get; private set; } = CreateDefaultSave();

    public void LoadSave(RuntimeSaveData saveData) => SaveData = saveData;

    public int AddItem(string itemId, int quantity, IReadOnlyCollection<ItemDefinition> catalog)
    {
        if (quantity <= 0) return 0;
        var item = catalog.FirstOrDefault(candidate => candidate.Id == itemId) ?? throw new ArgumentException($"Unknown item '{itemId}'.", nameof(itemId));
        var maxStack = Math.Max(1, item.MaxStack);
        var remaining = quantity;
        foreach (var stack in SaveData.Inventory.Where(stack => stack.ItemId == itemId && stack.Quantity < maxStack))
        {
            var add = Math.Min(maxStack - stack.Quantity, remaining);
            stack.Quantity += add;
            remaining -= add;
            if (remaining == 0) return 0;
        }

        while (remaining > 0)
        {
            var add = Math.Min(maxStack, remaining);
            SaveData.Inventory.Add(new InventoryStack { ItemId = itemId, Quantity = add });
            remaining -= add;
        }

        return remaining;
    }

    public bool EquipItem(string memberId, string itemId, IReadOnlyCollection<ItemDefinition> catalog)
    {
        var item = catalog.FirstOrDefault(candidate => candidate.Id == itemId);
        var member = SaveData.Party.FirstOrDefault(candidate => candidate.Id == memberId);
        if (item?.EquipmentSlot is null || member is null || !SaveData.Inventory.Any(stack => stack.ItemId == itemId && stack.Quantity > 0)) return false;
        member.Equipment[item.EquipmentSlot.Value] = itemId;
        return true;
    }

    public bool CanCastSpell(string memberId, string spellId, IReadOnlyCollection<SpellDefinition> spells)
    {
        var member = SaveData.Party.FirstOrDefault(candidate => candidate.Id == memberId);
        var spell = spells.FirstOrDefault(candidate => candidate.Id == spellId);
        return member is not null && spell is not null && member.KnownSpellIds.Contains(spellId) && member.Mp >= spell.MpCost;
    }

    public QuestStatus GetQuestStatus(QuestDefinition quest)
    {
        if (!string.IsNullOrWhiteSpace(quest.CompletionFlag) && SaveData.Flags.GetValueOrDefault(quest.CompletionFlag)) return QuestStatus.Completed;
        if (string.IsNullOrWhiteSpace(quest.RequiredFlag) || SaveData.Flags.GetValueOrDefault(quest.RequiredFlag)) return QuestStatus.Active;
        return QuestStatus.NotStarted;
    }

    public static RuntimeSaveData CreateDefaultSave() => new()
    {
        SaveName = "Oakvale Autosave",
        CurrentMapId = "map-oakvale-village",
        PlayerX = 2,
        PlayerY = 2,
        Party = [new() { Id = "party-hero", Name = "Hero", Level = 3, Hp = 112, MaxHp = 120, Mp = 18, MaxMp = 24, KnownSpellIds = ["spark"] }],
        Inventory = [new() { ItemId = "item-potion", Quantity = 3 }],
        Flags = new() { ["intro_seen"] = true, ["elder_helped"] = false }
    };
}
