using _2D_RPG.Models;

namespace _2D_RPG.Services;

public sealed class AnimationService
{
    public AnimationPlaybackState Start(AnimationDefinition animation, AnimationClipKind kind, RuntimeDirection direction)
    {
        var clip = FindClip(animation, kind, direction) ?? throw new InvalidOperationException($"Animation '{animation.Id}' has no {kind} clip.");
        var state = new AnimationPlaybackState
        {
            AnimationId = animation.Id,
            ClipId = clip.Id,
            Kind = kind,
            Direction = direction,
            FrameIndex = 0,
            IsComplete = clip.Frames.Count == 0
        };
        FireCues(state, clip, 0);
        return state;
    }

    public AnimationFrameDefinition? GetCurrentFrame(AnimationDefinition? animation, AnimationPlaybackState? state)
    {
        var clip = GetClip(animation, state);
        return clip is null || state is null || clip.Frames.Count == 0 ? null : clip.Frames[Math.Clamp(state.FrameIndex, 0, clip.Frames.Count - 1)];
    }

    public IReadOnlyList<AnimationCueEvent> Advance(AnimationDefinition animation, AnimationPlaybackState state, TimeSpan elapsed)
    {
        var firedAtStart = state.FiredCues.Count;
        var clip = GetClip(animation, state);
        if (clip is null || clip.Frames.Count == 0 || state.IsComplete || elapsed <= TimeSpan.Zero)
        {
            return [];
        }

        state.ElapsedInFrameMilliseconds += elapsed.TotalMilliseconds;
        while (!state.IsComplete)
        {
            var frame = clip.Frames[state.FrameIndex];
            var duration = Math.Max(1, frame.DurationMilliseconds);
            if (state.ElapsedInFrameMilliseconds < duration)
            {
                break;
            }

            state.ElapsedInFrameMilliseconds -= duration;
            var nextFrame = state.FrameIndex + 1;
            if (nextFrame >= clip.Frames.Count)
            {
                if (clip.IsLooping)
                {
                    nextFrame = 0;
                }
                else
                {
                    state.FrameIndex = clip.Frames.Count - 1;
                    state.ElapsedInFrameMilliseconds = 0;
                    state.IsComplete = true;
                    break;
                }
            }

            state.FrameIndex = nextFrame;
            FireCues(state, clip, state.FrameIndex);
        }

        return state.FiredCues.Skip(firedAtStart).ToList();
    }

    public AnimationClipDefinition? FindClip(AnimationDefinition animation, AnimationClipKind kind, RuntimeDirection direction) =>
        animation.Clips.FirstOrDefault(clip => clip.Kind == kind && clip.Direction == direction)
        ?? animation.Clips.FirstOrDefault(clip => clip.Kind == kind && clip.Direction is null)
        ?? animation.Clips.FirstOrDefault(clip => clip.Kind == AnimationClipKind.Idle && clip.Direction == direction)
        ?? animation.Clips.FirstOrDefault(clip => clip.Kind == AnimationClipKind.Idle && clip.Direction is null);

    private static AnimationClipDefinition? GetClip(AnimationDefinition? animation, AnimationPlaybackState? state) =>
        animation?.Clips.FirstOrDefault(clip => clip.Id == state?.ClipId);

    private static void FireCues(AnimationPlaybackState state, AnimationClipDefinition clip, int frameIndex)
    {
        foreach (var cue in clip.Cues.Where(cue => cue.FrameIndex == frameIndex))
        {
            state.FiredCues.Add(new AnimationCueEvent { ClipId = clip.Id, Cue = cue });
        }
    }
}
