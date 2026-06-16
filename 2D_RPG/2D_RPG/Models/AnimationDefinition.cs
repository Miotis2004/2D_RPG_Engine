namespace _2D_RPG.Models;

public enum AnimationClipKind
{
    Idle,
    Walk,
    Attack,
    Cast,
    Hit,
    Death
}

public sealed class AnimationCueDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Kind { get; set; } = "effect";
    public int FrameIndex { get; set; }
    public string Payload { get; set; } = string.Empty;
}

public sealed class AnimationFrameDefinition
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; } = 32;
    public int Height { get; set; } = 32;
    public int DurationMilliseconds { get; set; } = 120;
}

public sealed class AnimationClipDefinition
{
    public string Id { get; set; } = string.Empty;
    public AnimationClipKind Kind { get; set; } = AnimationClipKind.Idle;
    public RuntimeDirection? Direction { get; set; }
    public bool IsLooping { get; set; } = true;
    public List<AnimationFrameDefinition> Frames { get; set; } = [];
    public List<AnimationCueDefinition> Cues { get; set; } = [];
}

public sealed class AnimationDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string SpriteSheetAssetId { get; set; } = string.Empty;
    public List<AnimationClipDefinition> Clips { get; set; } = [];
}

public sealed class AnimationCueEvent
{
    public string ClipId { get; set; } = string.Empty;
    public AnimationCueDefinition Cue { get; set; } = new();
}

public sealed class AnimationPlaybackState
{
    public string AnimationId { get; set; } = string.Empty;
    public string ClipId { get; set; } = string.Empty;
    public AnimationClipKind Kind { get; set; } = AnimationClipKind.Idle;
    public RuntimeDirection Direction { get; set; } = RuntimeDirection.Down;
    public int FrameIndex { get; set; }
    public double ElapsedInFrameMilliseconds { get; set; }
    public bool IsComplete { get; set; }
    public List<AnimationCueEvent> FiredCues { get; set; } = [];
}
