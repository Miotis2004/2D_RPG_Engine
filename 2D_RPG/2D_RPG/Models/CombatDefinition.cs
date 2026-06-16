namespace _2D_RPG.Models;

public enum CombatTeam { Party, Enemy }
public enum CombatCommandKind { Attack, Skill, Item, Defend }
public enum TargetingRule { SingleEnemy, SingleAlly, Self, AllEnemies, AllAllies }
public enum CombatStatusEffectKind { Poison, Stun, Guard }
public enum CombatPhase { NotStarted, AwaitingCommand, Resolving, Victory, Defeat }

public sealed class CombatStatsDefinition
{
    public int MaxHp { get; set; } = 100;
    public int MaxMp { get; set; } = 20;
    public int Attack { get; set; } = 12;
    public int Defense { get; set; } = 6;
    public int Speed { get; set; } = 10;
}

public sealed class CombatActorDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public CombatTeam Team { get; set; }
    public int Lane { get; set; }
    public CombatStatsDefinition Stats { get; set; } = new();
    public string AnimationId { get; set; } = string.Empty;
}

public sealed class CombatCommandDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public CombatCommandKind Kind { get; set; } = CombatCommandKind.Attack;
    public TargetingRule Targeting { get; set; } = TargetingRule.SingleEnemy;
    public int MpCost { get; set; }
    public double PowerMultiplier { get; set; } = 1;
    public CombatStatusEffectKind? AppliesStatus { get; set; }
    public int StatusDurationTurns { get; set; }
    public AnimationClipKind AnimationKind { get; set; } = AnimationClipKind.Attack;
}

public sealed class EncounterDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string BackgroundCssClass { get; set; } = "forest-battle";
    public List<CombatActorDefinition> Party { get; set; } = [];
    public List<CombatActorDefinition> Enemies { get; set; } = [];
    public List<CombatCommandDefinition> Commands { get; set; } = [];
    public int RewardExperience { get; set; }
    public int RewardGold { get; set; }
}

public sealed class CombatStatusEffectState
{
    public CombatStatusEffectKind Kind { get; set; }
    public int RemainingTurns { get; set; }
}

public sealed class CombatActorState
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public CombatTeam Team { get; set; }
    public int Lane { get; set; }
    public CombatStatsDefinition Stats { get; set; } = new();
    public int Hp { get; set; }
    public int Mp { get; set; }
    public bool IsDefending { get; set; }
    public AnimationPlaybackState Animation { get; set; } = new();
    public List<CombatStatusEffectState> StatusEffects { get; set; } = [];
    public bool IsAlive => Hp > 0;
}

public sealed class CombatLogEntry
{
    public string Text { get; set; } = string.Empty;
}

public sealed class CombatBattleState
{
    public EncounterDefinition? Encounter { get; set; }
    public CombatPhase Phase { get; set; } = CombatPhase.NotStarted;
    public List<CombatActorState> Actors { get; set; } = [];
    public Queue<string> TurnQueue { get; set; } = new();
    public string? ActiveActorId { get; set; }
    public List<CombatLogEntry> Log { get; set; } = [];
    public int ExperienceAwarded { get; set; }
    public int GoldAwarded { get; set; }
}
