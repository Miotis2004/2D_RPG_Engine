using _2D_RPG.Models;
using _2D_RPG.Services;

namespace _2D_RPG.Tests.Services;

public sealed class AnimationServiceTests
{
    [Fact]
    public void AdvanceUsesElapsedTimeAndCarriesRemainder()
    {
        var service = new AnimationService();
        var animation = TestAnimation(looping: true);
        var state = service.Start(animation, AnimationClipKind.Walk, RuntimeDirection.Right);

        service.Advance(animation, state, TimeSpan.FromMilliseconds(150));

        Assert.Equal(1, state.FrameIndex);
        Assert.Equal(50, state.ElapsedInFrameMilliseconds);
    }

    [Fact]
    public void LoopingClipRecyclesFrames()
    {
        var service = new AnimationService();
        var animation = TestAnimation(looping: true);
        var state = service.Start(animation, AnimationClipKind.Walk, RuntimeDirection.Right);

        service.Advance(animation, state, TimeSpan.FromMilliseconds(350));

        Assert.Equal(0, state.FrameIndex);
        Assert.False(state.IsComplete);
    }

    [Fact]
    public void OneShotClipStopsOnLastFrame()
    {
        var service = new AnimationService();
        var animation = TestAnimation(looping: false);
        var state = service.Start(animation, AnimationClipKind.Walk, RuntimeDirection.Right);

        service.Advance(animation, state, TimeSpan.FromMilliseconds(500));

        Assert.Equal(2, state.FrameIndex);
        Assert.True(state.IsComplete);
    }

    [Fact]
    public void DirectionalLookupFallsBackToDirectionlessIdle()
    {
        var service = new AnimationService();
        var animation = new AnimationDefinition
        {
            Id = "anim",
            Clips = [new() { Id = "idle-any", Kind = AnimationClipKind.Idle, Frames = [new()] }]
        };

        var clip = service.FindClip(animation, AnimationClipKind.Attack, RuntimeDirection.Up);

        Assert.NotNull(clip);
        Assert.Equal("idle-any", clip!.Id);
    }

    [Fact]
    public void CuesFireWhenFrameIsReached()
    {
        var service = new AnimationService();
        var animation = TestAnimation(looping: true);
        var state = service.Start(animation, AnimationClipKind.Walk, RuntimeDirection.Right);

        var cues = service.Advance(animation, state, TimeSpan.FromMilliseconds(100));

        Assert.Single(cues);
        Assert.Equal("footstep", cues[0].Cue.Payload);
    }

    private static AnimationDefinition TestAnimation(bool looping) => new()
    {
        Id = "anim",
        Clips =
        [
            new()
            {
                Id = "walk-right",
                Kind = AnimationClipKind.Walk,
                Direction = RuntimeDirection.Right,
                IsLooping = looping,
                Frames =
                [
                    new() { X = 0, DurationMilliseconds = 100 },
                    new() { X = 32, DurationMilliseconds = 100 },
                    new() { X = 64, DurationMilliseconds = 100 }
                ],
                Cues = [new() { Id = "step", Kind = "sound", FrameIndex = 1, Payload = "footstep" }]
            }
        ]
    };
}
