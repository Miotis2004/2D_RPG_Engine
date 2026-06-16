using System.Text.Json;
using _2D_RPG.Models;

namespace _2D_RPG.Services;

public sealed class CombatService
{
    private static readonly JsonSerializerOptions CloneOptions = new(JsonSerializerDefaults.Web);
    public CombatService()
    {
    }

    public CombatBattleState State { get; private set; } = new();

    public CombatBattleState StartEncounter(EncounterDefinition encounter)
    {
        var runtimeEncounter = Clone(encounter);
        State = new CombatBattleState { Encounter = runtimeEncounter, Phase = CombatPhase.AwaitingCommand };
        State.Actors = runtimeEncounter.Party.Concat(runtimeEncounter.Enemies)
            .OrderBy(actor => actor.Team).ThenBy(actor => actor.Lane)
            .Select(ToState).ToList();
        RebuildTurnQueue();
        AdvanceToNextActor();
        State.Log.Add(new() { Text = $"Encounter started: {runtimeEncounter.Name}." });
        return State;
    }

    public IReadOnlyList<CombatActorState> GetValidTargets(CombatCommandDefinition command, CombatActorState actor)
    {
        var enemy = actor.Team == CombatTeam.Party ? CombatTeam.Enemy : CombatTeam.Party;
        return command.Targeting switch
        {
            TargetingRule.Self => [actor],
            TargetingRule.SingleAlly or TargetingRule.AllAllies => State.Actors.Where(candidate => candidate.Team == actor.Team && candidate.IsAlive).ToList(),
            TargetingRule.SingleEnemy or TargetingRule.AllEnemies => State.Actors.Where(candidate => candidate.Team == enemy && candidate.IsAlive).ToList(),
            _ => []
        };
    }

    public bool ExecuteCommand(string commandId, string? targetActorId = null)
    {
        var actor = ActiveActor;
        var command = State.Encounter?.Commands.FirstOrDefault(c => c.Id == commandId);
        if (actor is null || command is null || !actor.IsAlive || actor.Mp < command.MpCost || State.Phase is CombatPhase.Victory or CombatPhase.Defeat)
        {
            return false;
        }

        var targets = SelectTargets(command, actor, targetActorId);
        if (targets.Count == 0) return false;

        actor.Mp -= command.MpCost;
        actor.IsDefending = command.Kind == CombatCommandKind.Defend;
        if (command.Kind == CombatCommandKind.Defend)
        {
            ApplyStatus(actor, CombatStatusEffectKind.Guard, 1);
            State.Log.Add(new() { Text = $"{actor.Name} braces for impact." });
        }
        else
        {
            foreach (var target in targets) ResolveAgainstTarget(actor, target, command);
        }

        EndActorTurn(actor);
        UpdateOutcome();
        if (State.Phase is not (CombatPhase.Victory or CombatPhase.Defeat)) AdvanceToNextActor();
        return true;
    }

    public int CalculateDamage(CombatActorState actor, CombatActorState target, CombatCommandDefinition command)
    {
        if (command.Kind is CombatCommandKind.Item) return 0;
        var defense = target.IsDefending || target.StatusEffects.Any(s => s.Kind == CombatStatusEffectKind.Guard) ? target.Stats.Defense * 2 : target.Stats.Defense;
        return Math.Max(1, (int)Math.Round((actor.Stats.Attack * command.PowerMultiplier) - (defense * 0.5)));
    }

    private void ResolveAgainstTarget(CombatActorState actor, CombatActorState target, CombatCommandDefinition command)
    {
        var damage = CalculateDamage(actor, target, command);
        target.Hp = Math.Max(0, target.Hp - damage);
        State.Log.Add(new() { Text = $"{actor.Name} used {command.Name} on {target.Name} for {damage} damage." });
        if (command.AppliesStatus is { } status && target.IsAlive) ApplyStatus(target, status, Math.Max(1, command.StatusDurationTurns));
    }

    private void ApplyStatus(CombatActorState target, CombatStatusEffectKind status, int turns)
    {
        var existing = target.StatusEffects.FirstOrDefault(s => s.Kind == status);
        if (existing is null) target.StatusEffects.Add(new() { Kind = status, RemainingTurns = turns });
        else existing.RemainingTurns = Math.Max(existing.RemainingTurns, turns);
        State.Log.Add(new() { Text = $"{target.Name} gained {status} ({turns} turn{(turns == 1 ? string.Empty : "s")})." });
    }

    private void EndActorTurn(CombatActorState actor)
    {
        foreach (var status in actor.StatusEffects.ToList())
        {
            status.RemainingTurns--;
            if (status.Kind == CombatStatusEffectKind.Poison && actor.IsAlive)
            {
                var poison = Math.Max(1, actor.Stats.MaxHp / 10);
                actor.Hp = Math.Max(0, actor.Hp - poison);
                State.Log.Add(new() { Text = $"{actor.Name} suffers {poison} poison damage." });
            }
            if (status.RemainingTurns <= 0) actor.StatusEffects.Remove(status);
        }
        actor.IsDefending = false;
    }

    private List<CombatActorState> SelectTargets(CombatCommandDefinition command, CombatActorState actor, string? targetActorId)
    {
        var valid = GetValidTargets(command, actor);
        return command.Targeting is TargetingRule.AllAllies or TargetingRule.AllEnemies
            ? valid.ToList()
            : valid.Where(target => target.Id == targetActorId || targetActorId is null).Take(1).ToList();
    }

    private CombatActorState? ActiveActor => State.Actors.FirstOrDefault(actor => actor.Id == State.ActiveActorId);
    private void RebuildTurnQueue() => State.TurnQueue = new Queue<string>(State.Actors.Where(a => a.IsAlive).OrderByDescending(a => a.Stats.Speed).ThenBy(a => a.Lane).Select(a => a.Id));
    private void AdvanceToNextActor() { if (State.TurnQueue.Count == 0) RebuildTurnQueue(); State.ActiveActorId = State.TurnQueue.Count == 0 ? null : State.TurnQueue.Dequeue(); State.Phase = CombatPhase.AwaitingCommand; }
    private void UpdateOutcome()
    {
        if (State.Actors.Where(a => a.Team == CombatTeam.Enemy).All(a => !a.IsAlive)) { State.Phase = CombatPhase.Victory; State.ExperienceAwarded = State.Encounter?.RewardExperience ?? 0; State.GoldAwarded = State.Encounter?.RewardGold ?? 0; State.Log.Add(new() { Text = "Victory!" }); }
        else if (State.Actors.Where(a => a.Team == CombatTeam.Party).All(a => !a.IsAlive)) { State.Phase = CombatPhase.Defeat; State.Log.Add(new() { Text = "Defeat..." }); }
    }
    private CombatActorState ToState(CombatActorDefinition actor) => new() { Id = actor.Id, Name = actor.Name, Team = actor.Team, Lane = actor.Lane, Stats = actor.Stats, Hp = actor.Stats.MaxHp, Mp = actor.Stats.MaxMp, Animation = new AnimationPlaybackState { AnimationId = actor.AnimationId, Kind = AnimationClipKind.Idle } };
    private static EncounterDefinition Clone(EncounterDefinition encounter) => JsonSerializer.Deserialize<EncounterDefinition>(JsonSerializer.Serialize(encounter, CloneOptions), CloneOptions) ?? new();
}
