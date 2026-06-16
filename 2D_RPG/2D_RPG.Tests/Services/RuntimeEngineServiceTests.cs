using _2D_RPG.Services;

namespace _2D_RPG.Tests.Services;

public sealed class RuntimeEngineServiceTests
{
    [Fact]
    public void StartSessionClonesMapWithoutMutatingAuthoringState()
    {
        var project = EditorStateService.CreateSampleProject();
        var runtime = new RuntimeEngineService();

        var state = runtime.StartSession(project, project.ActiveMapId, 1, 1);
        Assert.NotNull(state.Map);

        state.Map!.Name = "Runtime Only";
        state.Map.Objects.Clear();

        Assert.NotEqual("Runtime Only", project.ActiveMap!.Name);
        Assert.NotEmpty(project.ActiveMap.Objects);
    }

    [Fact]
    public void MovePlayerRejectsCollisionsAndUpdatesFacing()
    {
        var project = EditorStateService.CreateSampleProject();
        var runtime = new RuntimeEngineService();
        runtime.StartSession(project, project.ActiveMapId, 9, 3);

        Assert.False(runtime.MovePlayer(1, 0));
        Assert.Equal(9, runtime.State.Player.X);
        Assert.Equal(3, runtime.State.Player.Y);
        Assert.Equal("Blocked at 10, 3.", runtime.State.LastInteraction);

        Assert.True(runtime.MovePlayer(-1, 0));
        Assert.Equal(8, runtime.State.Player.X);
        Assert.Equal(3, runtime.State.Player.Y);
    }

    [Fact]
    public void InteractExecutesEventCommandsInFacingTile()
    {
        var project = EditorStateService.CreateSampleProject();
        var runtime = new RuntimeEngineService();
        runtime.StartSession(project, project.ActiveMapId, 5, 9);

        Assert.True(runtime.MovePlayer(0, -1) is false); // face the blocking elder above
        Assert.True(runtime.Interact());

        Assert.Contains(runtime.State.Messages, message => message.Text == "Welcome to Oakvale.");
    }

    [Fact]
    public void ConditionsEvaluateBooleanFlags()
    {
        var runtime = new RuntimeEngineService();
        runtime.StartSession(EditorStateService.CreateSampleProject(), "map-oakvale-village");

        Assert.True(runtime.EvaluateCondition("flag.intro_seen == false"));
        runtime.State.Flags["intro_seen"] = true;
        Assert.True(runtime.EvaluateCondition("flag.intro_seen == true"));
        Assert.False(runtime.EvaluateCondition("flag.intro_seen == false"));
    }
}
