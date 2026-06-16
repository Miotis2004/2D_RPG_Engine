using System.Text.Json;
using _2D_RPG.Models;

namespace _2D_RPG.Services;

public sealed class ProjectPersistenceService
{
    public const string CurrentSchemaVersion = "1.1.0";
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web) { WriteIndented = true };

    public string SaveProject(ProjectDefinition project)
    {
        project.SchemaVersion = CurrentSchemaVersion;
        return JsonSerializer.Serialize(project, Options);
    }

    public ProjectDefinition LoadProject(string json)
    {
        var project = JsonSerializer.Deserialize<ProjectDefinition>(json, Options) ?? throw new InvalidDataException("Project file was empty or invalid.");
        return Migrate(project);
    }

    public string SaveRuntime(RuntimeSaveData saveData)
    {
        saveData.SchemaVersion = CurrentSchemaVersion;
        saveData.SavedAt = DateTimeOffset.UtcNow;
        return JsonSerializer.Serialize(saveData, Options);
    }

    public RuntimeSaveData LoadRuntime(string json)
    {
        var saveData = JsonSerializer.Deserialize<RuntimeSaveData>(json, Options) ?? throw new InvalidDataException("Runtime save was empty or invalid.");
        if (string.IsNullOrWhiteSpace(saveData.SchemaVersion)) saveData.SchemaVersion = "1.0.0";
        saveData.SchemaVersion = CurrentSchemaVersion;
        saveData.Inventory.RemoveAll(stack => stack.Quantity <= 0);
        return saveData;
    }

    private static ProjectDefinition Migrate(ProjectDefinition project)
    {
        if (string.IsNullOrWhiteSpace(project.SchemaVersion)) project.SchemaVersion = "1.0.0";
        if (project.Items.Count == 0) project.Items.AddRange(EditorStateService.CreateSampleMenuCatalog().Items);
        if (project.Spells.Count == 0) project.Spells.AddRange(EditorStateService.CreateSampleMenuCatalog().Spells);
        if (project.Quests.Count == 0) project.Quests.AddRange(EditorStateService.CreateSampleMenuCatalog().Quests);
        project.SchemaVersion = CurrentSchemaVersion;
        return project;
    }
}
