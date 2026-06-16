using System.Text.Json.Serialization;

namespace _2D_RPG.Models;

public sealed class ProjectDefinition
{
    public string Id { get; set; } = "project-sample-rpg";
    public string SchemaVersion { get; set; } = "1.0.0";
    public string Name { get; set; } = "Sample RPG Project";
    public string Description { get; set; } = "A starter project used to bootstrap the RPG editor.";
    public DateTimeOffset CreatedAt { get; set; } = new(2026, 6, 16, 0, 0, 0, TimeSpan.Zero);
    public string ActiveMapId { get; set; } = "map-oakvale-village";
    public List<TileSetDefinition> TileSets { get; set; } = [];
    public List<AssetDefinition> Assets { get; set; } = [];
    public List<AnimationDefinition> Animations { get; set; } = [];
    public List<MapDefinition> Maps { get; set; } = [];

    [JsonIgnore]
    public MapDefinition? ActiveMap => Maps.FirstOrDefault(map => map.Id == ActiveMapId);
}
