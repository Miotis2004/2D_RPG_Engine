using _2D_RPG.Models;

namespace _2D_RPG.Services;

public sealed class AssetCatalogService
{
    private static readonly HashSet<string> SupportedImageExtensions = new(StringComparer.OrdinalIgnoreCase) { ".png", ".jpg", ".jpeg", ".gif", ".webp" };
    private static readonly HashSet<string> SupportedAudioExtensions = new(StringComparer.OrdinalIgnoreCase) { ".ogg", ".mp3", ".wav" };

    public static IReadOnlyDictionary<AssetKind, string> StandardFolders { get; } = new Dictionary<AssetKind, string>
    {
        [AssetKind.TileSet] = "assets/tilesets",
        [AssetKind.SpriteSheet] = "assets/spritesheets",
        [AssetKind.Audio] = "assets/audio",
        [AssetKind.Portrait] = "assets/portraits",
        [AssetKind.UserInterface] = "assets/ui",
        [AssetKind.Other] = "assets/misc"
    };

    public AssetDefinition Import(ProjectDefinition project, string name, AssetKind kind, string fileName, int? width = null, int? height = null, IEnumerable<string>? tags = null)
    {
        var baseId = Slugify(Path.GetFileNameWithoutExtension(fileName));
        var asset = new AssetDefinition
        {
            Id = NextUniqueId(project, $"asset-{baseId}"),
            Name = string.IsNullOrWhiteSpace(name) ? Path.GetFileNameWithoutExtension(fileName) : name.Trim(),
            Kind = kind,
            SourcePath = ResolveStandardPath(kind, fileName),
            Width = width,
            Height = height,
            Tags = tags?.Where(tag => !string.IsNullOrWhiteSpace(tag)).Select(tag => tag.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToList() ?? []
        };

        project.Assets.Add(asset);
        return asset;
    }

    public void Rename(AssetDefinition asset, string name)
    {
        if (!string.IsNullOrWhiteSpace(name)) asset.Name = name.Trim();
    }

    public void SetTags(AssetDefinition asset, string commaSeparatedTags) => asset.Tags = commaSeparatedTags
        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .ToList();

    public bool Delete(ProjectDefinition project, string assetId)
    {
        var asset = project.Assets.FirstOrDefault(item => string.Equals(item.Id, assetId, StringComparison.OrdinalIgnoreCase));
        return asset is not null && project.Assets.Remove(asset);
    }

    public string ResolveWebPath(AssetDefinition asset) => asset.SourcePath.StartsWith("/", StringComparison.Ordinal) ? asset.SourcePath : $"/{asset.SourcePath}";

    public IReadOnlyList<AssetValidationDiagnostic> Validate(ProjectDefinition project, Func<string, bool>? fileExists = null)
    {
        fileExists ??= _ => true;
        var diagnostics = new List<AssetValidationDiagnostic>();
        foreach (var duplicate in project.Assets.GroupBy(asset => asset.Id, StringComparer.OrdinalIgnoreCase).Where(group => group.Count() > 1))
        {
            diagnostics.Add(new(duplicate.Key, $"Duplicate asset id '{duplicate.Key}'."));
        }

        foreach (var asset in project.Assets)
        {
            if (string.IsNullOrWhiteSpace(asset.Id)) diagnostics.Add(new(asset.Id, "Asset id is required."));
            if (string.IsNullOrWhiteSpace(asset.SourcePath)) diagnostics.Add(new(asset.Id, $"Asset '{asset.Id}' is missing a source path."));
            else
            {
                if (!IsSupportedExtension(asset)) diagnostics.Add(new(asset.Id, $"Asset '{asset.Id}' has an unsupported file extension."));
                if (!fileExists(asset.SourcePath)) diagnostics.Add(new(asset.Id, $"Asset file '{asset.SourcePath}' is missing."));
            }

            if (asset.Kind is AssetKind.TileSet or AssetKind.SpriteSheet)
            {
                if (asset.Width is <= 0 || asset.Height is <= 0) diagnostics.Add(new(asset.Id, $"Asset '{asset.Id}' has unsupported dimensions."));
                if (asset.FrameWidth is > 0 && asset.Width is > 0 && asset.Width % asset.FrameWidth != 0) diagnostics.Add(new(asset.Id, $"Asset '{asset.Id}' width must be divisible by frame width."));
                if (asset.FrameHeight is > 0 && asset.Height is > 0 && asset.Height % asset.FrameHeight != 0) diagnostics.Add(new(asset.Id, $"Asset '{asset.Id}' height must be divisible by frame height."));
            }
        }

        return diagnostics;
    }

    public static string ResolveStandardPath(AssetKind kind, string fileName) => $"{StandardFolders.GetValueOrDefault(kind, StandardFolders[AssetKind.Other])}/{Path.GetFileName(fileName)}";

    private static bool IsSupportedExtension(AssetDefinition asset)
    {
        var extension = Path.GetExtension(asset.SourcePath);
        return asset.Kind == AssetKind.Audio ? SupportedAudioExtensions.Contains(extension) : SupportedImageExtensions.Contains(extension);
    }

    private static string NextUniqueId(ProjectDefinition project, string baseId)
    {
        var candidate = baseId;
        var suffix = 2;
        while (project.Assets.Any(asset => string.Equals(asset.Id, candidate, StringComparison.OrdinalIgnoreCase))) candidate = $"{baseId}-{suffix++}";
        return candidate;
    }

    private static string Slugify(string value) => string.Join("-", value.ToLowerInvariant().Split(Path.GetInvalidFileNameChars().Concat(new[] { ' ', '_' }).ToArray(), StringSplitOptions.RemoveEmptyEntries));
}
