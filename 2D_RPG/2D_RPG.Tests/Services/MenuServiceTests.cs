using Xunit;
using _2D_RPG.Models;
using _2D_RPG.Services;

namespace _2D_RPG.Tests.Services;

public sealed class MenuServiceTests
{
    [Fact]
    public void AddItemStacksUpToCatalogLimit()
    {
        var service = new MenuService();
        service.LoadSave(new RuntimeSaveData());
        var catalog = new[] { new ItemDefinition { Id = "potion", Name = "Potion", MaxStack = 10 } };

        service.AddItem("potion", 23, catalog);

        Assert.Equal([10, 10, 3], service.SaveData.Inventory.Select(stack => stack.Quantity));
    }

    [Fact]
    public void EquipItemRequiresMatchingEquipmentSlotAndInventory()
    {
        var service = new MenuService();
        service.LoadSave(new RuntimeSaveData
        {
            Party = [new() { Id = "hero", Name = "Hero" }],
            Inventory = [new() { ItemId = "sword", Quantity = 1 }]
        });
        var catalog = new[] { new ItemDefinition { Id = "sword", Name = "Sword", Kind = ItemKind.Weapon, MaxStack = 1, EquipmentSlot = EquipmentSlotKind.Weapon } };

        Assert.True(service.EquipItem("hero", "sword", catalog));
        Assert.Equal("sword", service.SaveData.Party.Single().Equipment[EquipmentSlotKind.Weapon]);
    }

    [Fact]
    public void QuestStatusUsesRequiredAndCompletionFlags()
    {
        var service = new MenuService();
        service.LoadSave(new RuntimeSaveData { Flags = new() { ["started"] = true, ["done"] = false } });
        var quest = new QuestDefinition { RequiredFlag = "started", CompletionFlag = "done" };

        Assert.Equal(QuestStatus.Active, service.GetQuestStatus(quest));

        service.SaveData.Flags["done"] = true;
        Assert.Equal(QuestStatus.Completed, service.GetQuestStatus(quest));
    }
}
