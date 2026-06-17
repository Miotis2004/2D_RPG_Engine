using Xunit;
using _2D_RPG.Models;
using _2D_RPG.Services;

namespace _2D_RPG.Tests.Services;

public sealed class CombatServiceTests
{
    [Fact]
    public void StartEncounterOrdersTurnsBySpeedThenLane()
    {
        var service = new CombatService();
        service.StartEncounter(EditorStateService.CreateSampleProject().Encounters.Single());

        Assert.Equal("party-hero", service.State.ActiveActorId);
        Assert.Equal(["enemy-goblin", "party-mage", "enemy-slime"], service.State.TurnQueue.ToArray());
    }

    [Fact]
    public void DamageCalculationUsesAttackMultiplierAndDefense()
    {
        var service = new CombatService();
        service.StartEncounter(EditorStateService.CreateSampleProject().Encounters.Single());
        var hero = service.State.Actors.Single(actor => actor.Id == "party-hero");
        var slime = service.State.Actors.Single(actor => actor.Id == "enemy-slime");
        var firebolt = service.State.Encounter!.Commands.Single(command => command.Id == "firebolt");

        Assert.Equal(27, service.CalculateDamage(hero, slime, firebolt));
    }

    [Fact]
    public void TargetFiltersSeparateEnemyAllyAndSelfRules()
    {
        var service = new CombatService();
        service.StartEncounter(EditorStateService.CreateSampleProject().Encounters.Single());
        var hero = service.State.Actors.Single(actor => actor.Id == "party-hero");
        var attack = service.State.Encounter!.Commands.Single(command => command.Id == "attack");
        var guard = service.State.Encounter!.Commands.Single(command => command.Id == "guard");

        Assert.Equal(["enemy-slime", "enemy-goblin"], service.GetValidTargets(attack, hero).Select(actor => actor.Id));
        Assert.Equal(["party-hero"], service.GetValidTargets(guard, hero).Select(actor => actor.Id));
    }

    [Fact]
    public void ExecuteCommandAppliesCostsDamageStatusAndVictory()
    {
        var service = new CombatService();
        service.StartEncounter(EditorStateService.CreateSampleProject().Encounters.Single());

        Assert.True(service.ExecuteCommand("firebolt", "enemy-slime"));
        var hero = service.State.Actors.Single(actor => actor.Id == "party-hero");
        var slime = service.State.Actors.Single(actor => actor.Id == "enemy-slime");

        Assert.Equal(19, hero.Mp);
        Assert.Equal(19, slime.Hp);
        Assert.Contains(slime.StatusEffects, status => status.Kind == CombatStatusEffectKind.Poison && status.RemainingTurns == 2);

        service.State.Actors.Where(actor => actor.Team == CombatTeam.Enemy).ToList().ForEach(actor => actor.Hp = 1);
        service.ExecuteCommand("attack", "enemy-goblin");
        if (service.State.Phase != CombatPhase.Victory)
        {
            while (service.State.Phase == CombatPhase.AwaitingCommand)
            {
                var active = service.State.Actors.Single(actor => actor.Id == service.State.ActiveActorId);
                var target = service.State.Actors.FirstOrDefault(actor => actor.Team != active.Team && actor.IsAlive)?.Id;
                if (target is null) break;
                service.ExecuteCommand("attack", target);
            }
        }

        Assert.Equal(CombatPhase.Victory, service.State.Phase);
        Assert.Equal(18, service.State.ExperienceAwarded);
    }
}
