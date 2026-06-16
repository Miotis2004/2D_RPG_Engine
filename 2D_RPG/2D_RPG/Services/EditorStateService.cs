using _2D_RPG.Models;

namespace _2D_RPG.Services;

public sealed class EditorStateService
{
    public ProjectDefinition CurrentProject { get; private set; } = CreateSampleProject();
    public bool IsDirty { get; private set; }
    public EditorToolState ToolState { get; } = new();
    public string ActiveMapId
    {
        get => CurrentProject.ActiveMapId;
        set
        {
            if (CurrentProject.Maps.Any(map => map.Id == value) && CurrentProject.ActiveMapId != value)
            {
                CurrentProject.ActiveMapId = value;
                MarkDirty();
            }
        }
    }

    public MapDefinition? ActiveMap => CurrentProject.ActiveMap;

    public void SetActiveTool(EditorToolKind tool) => ToolState.ActiveTool = tool;

    public void SelectTile(int tileId)
    {
        ToolState.SelectedTileId = tileId;
        ToolState.ActiveTool = EditorToolKind.Paint;
    }

    public void SetActiveLayer(string layerId) => ToolState.ActiveLayerId = layerId;

    public void MarkDirty() => IsDirty = true;

    public void MarkClean() => IsDirty = false;

    public static ProjectDefinition CreateSampleProject()
    {
        var tileSet = new TileSetDefinition
        {
            Id = "tileset-oakvale-fields",
            Name = "Oakvale Fields",
            SourcePath = "assets/tilesets/oakvale-fields.png",
            Tiles =
            [
                new() { Id = 1, Name = "Grass", CssClass = "grass", Tags = ["terrain", "field"] },
                new() { Id = 2, Name = "Path", CssClass = "path", Tags = ["terrain", "road"] },
                new() { Id = 3, Name = "Water", CssClass = "water", IsWalkable = false, Tags = ["terrain", "water"] },
                new() { Id = 4, Name = "Forest", CssClass = "forest", IsWalkable = false, Tags = ["terrain", "blocked"] },
                new() { Id = 5, Name = "Stone", CssClass = "stone", Tags = ["terrain", "town"] },
                new() { Id = 6, Name = "Roof", CssClass = "roof", IsWalkable = false, Tags = ["building"] }
            ]
        };

        return new ProjectDefinition
        {
            TileSets = [tileSet],
            Assets =
            [
                new() { Id = "asset-oakvale-fields", Name = "Oakvale Fields Tileset", Kind = AssetKind.TileSet, SourcePath = tileSet.SourcePath, Width = 192, Height = 32, FrameWidth = 32, FrameHeight = 32, Tags = ["terrain", "field"] },
                new() { Id = "asset-village-npcs", Name = "Village NPCs", Kind = AssetKind.SpriteSheet, SourcePath = "assets/spritesheets/village-npcs.png", Width = 128, Height = 192, FrameWidth = 32, FrameHeight = 48, Tags = ["npc", "village"] },
                new() { Id = "asset-oakvale-theme", Name = "Oakvale Theme", Kind = AssetKind.Audio, SourcePath = "assets/audio/oakvale-theme.ogg", Tags = ["music", "village"] }
            ],
            Animations = [CreateHeroAnimation()],
            Encounters = [CreateSampleEncounter()],
            Items = CreateSampleMenuCatalog().Items,
            Spells = CreateSampleMenuCatalog().Spells,
            Quests = CreateSampleMenuCatalog().Quests,
            Maps = [CreateSampleMap(tileSet.Id)]
        };
    }
    public static (List<ItemDefinition> Items, List<SpellDefinition> Spells, List<QuestDefinition> Quests) CreateSampleMenuCatalog() =>
    (
        [
            new() { Id = "item-potion", Name = "Potion", Kind = ItemKind.Consumable, MaxStack = 10, HpRestore = 50, Description = "Restores 50 HP to one ally." },
            new() { Id = "item-ether", Name = "Ether", Kind = ItemKind.Consumable, MaxStack = 5, MpRestore = 20, Description = "Restores 20 MP to one ally." },
            new() { Id = "item-bronze-sword", Name = "Bronze Sword", Kind = ItemKind.Weapon, MaxStack = 1, EquipmentSlot = EquipmentSlotKind.Weapon, Description = "A reliable starter blade." },
            new() { Id = "item-travel-cloak", Name = "Travel Cloak", Kind = ItemKind.Armor, MaxStack = 1, EquipmentSlot = EquipmentSlotKind.Armor, Description = "Light armor for long roads." }
        ],
        [
            new() { Id = "spark", Name = "Spark", MpCost = 4, Description = "A quick bolt of elemental damage." },
            new() { Id = "mend", Name = "Mend", MpCost = 6, Description = "Restores a small amount of HP." }
        ],
        [
            new() { Id = "quest-oakvale-help", Name = "Help the Elder", Description = "Speak with the elder and recover supplies from the village chest.", RequiredFlag = "intro_seen", CompletionFlag = "elder_helped" },
            new() { Id = "quest-north-road", Name = "Open the North Road", Description = "Find a way through the blocked north gate.", RequiredFlag = "elder_helped", CompletionFlag = "north_gate_open" }
        ]
    );

    private static AnimationDefinition CreateHeroAnimation()
    {
        static AnimationClipDefinition Clip(AnimationClipKind kind, RuntimeDirection direction, int row, bool loop = true) => new()
        {
            Id = $"hero-{kind.ToString().ToLowerInvariant()}-{direction.ToString().ToLowerInvariant()}",
            Kind = kind,
            Direction = direction,
            IsLooping = loop,
            Frames =
            [
                new() { X = 0, Y = row * 48, Width = 32, Height = 48, DurationMilliseconds = 140 },
                new() { X = 32, Y = row * 48, Width = 32, Height = 48, DurationMilliseconds = 140 },
                new() { X = 64, Y = row * 48, Width = 32, Height = 48, DurationMilliseconds = 140 }
            ],
            Cues = kind == AnimationClipKind.Walk ? [new() { Id = $"step-{direction}", Kind = "sound", FrameIndex = 1, Payload = "footstep-soft" }] : []
        };

        return new AnimationDefinition
        {
            Id = "anim-hero-explorer",
            Name = "Hero Explorer",
            SpriteSheetAssetId = "asset-village-npcs",
            Clips =
            [
                Clip(AnimationClipKind.Idle, RuntimeDirection.Down, 0),
                Clip(AnimationClipKind.Idle, RuntimeDirection.Left, 1),
                Clip(AnimationClipKind.Idle, RuntimeDirection.Right, 2),
                Clip(AnimationClipKind.Idle, RuntimeDirection.Up, 3),
                Clip(AnimationClipKind.Walk, RuntimeDirection.Down, 0),
                Clip(AnimationClipKind.Walk, RuntimeDirection.Left, 1),
                Clip(AnimationClipKind.Walk, RuntimeDirection.Right, 2),
                Clip(AnimationClipKind.Walk, RuntimeDirection.Up, 3),
                Clip(AnimationClipKind.Attack, RuntimeDirection.Right, 4, loop: false)
            ]
        };
    }


    private static EncounterDefinition CreateSampleEncounter() => new()
    {
        Id = "encounter-oakvale-road",
        Name = "Oakvale Road Ambush",
        BackgroundCssClass = "forest-battle",
        RewardExperience = 18,
        RewardGold = 12,
        Party =
        [
            new() { Id = "party-hero", Name = "Hero", Team = CombatTeam.Party, Lane = 0, AnimationId = "anim-hero-explorer", Stats = new() { MaxHp = 120, MaxMp = 24, Attack = 18, Defense = 8, Speed = 14 } },
            new() { Id = "party-mage", Name = "Mage", Team = CombatTeam.Party, Lane = 1, AnimationId = "anim-hero-explorer", Stats = new() { MaxHp = 82, MaxMp = 42, Attack = 13, Defense = 5, Speed = 11 } }
        ],
        Enemies =
        [
            new() { Id = "enemy-slime", Name = "Slime", Team = CombatTeam.Enemy, Lane = 0, Stats = new() { MaxHp = 46, MaxMp = 0, Attack = 9, Defense = 3, Speed = 7 } },
            new() { Id = "enemy-goblin", Name = "Goblin", Team = CombatTeam.Enemy, Lane = 1, Stats = new() { MaxHp = 64, MaxMp = 8, Attack = 14, Defense = 5, Speed = 12 } }
        ],
        Commands =
        [
            new() { Id = "attack", Name = "Attack", Kind = CombatCommandKind.Attack, Targeting = TargetingRule.SingleEnemy, PowerMultiplier = 1, AnimationKind = AnimationClipKind.Attack },
            new() { Id = "firebolt", Name = "Firebolt", Kind = CombatCommandKind.Skill, Targeting = TargetingRule.SingleEnemy, MpCost = 5, PowerMultiplier = 1.6, AppliesStatus = CombatStatusEffectKind.Poison, StatusDurationTurns = 2, AnimationKind = AnimationClipKind.Cast },
            new() { Id = "guard", Name = "Guard", Kind = CombatCommandKind.Defend, Targeting = TargetingRule.Self }
        ]
    };

    private static MapDefinition CreateSampleMap(string tileSetId)
    {
        var groundRows = new[]
        {
            "44444111111111333333",
            "41111111111111333333",
            "11112222211111133333",
            "11112111211551111111",
            "11112166111551111111",
            "22222166111111114444",
            "11111111222222114444",
            "11511111111112111111",
            "11511155551112111111",
            "11111155551112111551",
            "33333111111112111551",
            "33333111111112222222"
        };

        return new MapDefinition
        {
            Id = "map-oakvale-village",
            Name = "Oakvale Village",
            Width = 20,
            Height = 12,
            TileSetId = tileSetId,
            MusicCue = "music/oakvale-theme.ogg",
            Layers =
            [
                new()
                {
                    Id = "layer-ground",
                    Name = "Ground",
                    Kind = MapLayerKind.Ground,
                    Tiles = ToPlacements(groundRows)
                },
                new() { Id = "layer-decoration", Name = "Decoration", Kind = MapLayerKind.Decoration },
                new() { Id = "layer-collision", Name = "Collision", Kind = MapLayerKind.Collision },
                new() { Id = "layer-objects", Name = "Objects", Kind = MapLayerKind.Objects },
                new() { Id = "layer-events", Name = "Events", Kind = MapLayerKind.Events }
            ],
            Objects =
            [
                new() { Id = "obj-town-gate", Name = "North Gate", Kind = MapObjectKind.Door, X = 10, Y = 3, IsBlocking = true, SpriteKey = "door", EventId = "event-gate-transfer" },
                new() { Id = "obj-elder", Name = "Village Elder", Kind = MapObjectKind.Npc, X = 5, Y = 8, IsBlocking = true, SpriteKey = "npc", EventId = "event-elder-dialogue" },
                new() { Id = "obj-supply-chest", Name = "Supply Chest", Kind = MapObjectKind.Chest, X = 14, Y = 9, IsBlocking = true, SpriteKey = "chest", EventId = "event-supply-chest" }
            ],
            Events =
            [
                new() { Id = "event-elder-dialogue", Name = "Elder Greeting", Trigger = EventTriggerKind.Interact, X = 5, Y = 8, Commands = [new() { Kind = EventCommandKind.Dialogue, Text = "Welcome to Oakvale." }] },
                new() { Id = "event-supply-chest", Name = "Supply Reward", Trigger = EventTriggerKind.Interact, X = 14, Y = 9, Commands = [new() { Kind = EventCommandKind.ItemReward, ItemId = "item-potion", Quantity = 2 }] },
                new() { Id = "event-gate-transfer", Name = "Gate Transfer", Trigger = EventTriggerKind.Touch, X = 10, Y = 3, Commands = [new() { Kind = EventCommandKind.TransferMap, TargetMapId = "map-oakvale-village", TargetX = 1, TargetY = 1 }] },
                new() { Id = "event-oakvale-enter", Name = "Oakvale Arrival", Trigger = EventTriggerKind.EnterMap, ConditionExpression = "flag.intro_seen == false", Commands = [new() { Kind = EventCommandKind.Dialogue, Text = "You arrive at Oakvale Village." }] }
            ]
        };
    }

    private static List<TilePlacement> ToPlacements(IReadOnlyList<string> rows)
    {
        var placements = new List<TilePlacement>();
        for (var y = 0; y < rows.Count; y++)
        {
            for (var x = 0; x < rows[y].Length; x++)
            {
                placements.Add(new TilePlacement { X = x, Y = y, TileId = rows[y][x] - '0' });
            }
        }

        return placements;
    }
}
