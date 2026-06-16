# Development Guide

This guide describes the complete development workflow for turning the current Blazor mockup into a working 2D RPG editor and engine. Follow the steps in order when setting up a workstation, adding features, validating changes, and preparing a release build.

## 1. Project Goals

The final working product should provide a browser-based RPG editor backed by reusable engine systems:

- A top-down tile map editor with layered painting, collision editing, object placement, event triggers, and scene inspection.
- A runtime exploration engine that can load authored maps, move actors, resolve collisions, and transition between maps.
- A side-view combat engine that can load party and enemy encounters, play sprite-sheet animations, process turns, and award results.
- RPG menu systems for settings, character sheets, inventory, equipment, spells, quests, saves, and party management.
- Structured project data that can be saved, loaded, validated, versioned, exported, and eventually packaged with game assets.

## 2. Repository Layout

Current important paths:

```text
README.md                         Build and quick-start instructions.
DEVELOPMENT.md                    Full development workflow and product checklist.
2D_RPG/2D_RPG.slnx                Solution file.
2D_RPG/2D_RPG/2D_RPG.csproj       Blazor web project targeting .NET 10.
2D_RPG/2D_RPG/Program.cs          ASP.NET Core startup and Blazor registration.
2D_RPG/2D_RPG/Components          Razor components, pages, layouts, and UI composition.
2D_RPG/2D_RPG/wwwroot             Static CSS, images, sprites, and browser assets.
2D_RPG/2D_RPG/Properties          Launch profiles for local HTTP and HTTPS development.
```

As the engine grows, add these folders to keep editor UI, runtime logic, and data models separated:

```text
2D_RPG/2D_RPG/Components/Editor       Editor-specific Razor components.
2D_RPG/2D_RPG/Components/Runtime      Runtime preview and playtest components.
2D_RPG/2D_RPG/Models                  Serializable game data models.
2D_RPG/2D_RPG/Services                Editor state, engine systems, persistence, and validation.
2D_RPG/2D_RPG/wwwroot/assets          Tilesets, spritesheets, icons, and sample game art.
2D_RPG/2D_RPG.Tests                   Future automated tests for engine and editor services.
```

## 3. Workstation Setup

### 3.1 Install required tools

1. Install the .NET 10 SDK or newer compatible SDK.
2. Install Git.
3. Install an editor with C# and Razor support, such as Visual Studio, Visual Studio Code with the C# Dev Kit extension, or JetBrains Rider.
4. Install a modern browser for testing the Blazor app.

### 3.2 Verify the toolchain

Run these commands from a terminal:

```bash
dotnet --info
git --version
```

Confirm that `dotnet --info` lists a .NET SDK compatible with the `net10.0` target in `2D_RPG/2D_RPG/2D_RPG.csproj`.

### 3.3 Clone and enter the repository

```bash
git clone <repository-url> 2D_RPG_Engine
cd 2D_RPG_Engine
```

If the repository is already cloned, update your local branch before starting new work:

```bash
git fetch --all --prune
git pull --ff-only
```

## 4. First Local Build

Run all commands from the repository root.

### 4.1 Restore NuGet packages

```bash
dotnet restore 2D_RPG/2D_RPG.slnx
```

### 4.2 Build the solution

```bash
dotnet build 2D_RPG/2D_RPG.slnx --configuration Debug --no-restore
```

### 4.3 Run the web app

```bash
dotnet run --project 2D_RPG/2D_RPG/2D_RPG.csproj --configuration Debug
```

Open the URL printed by the command. The current launch profiles use:

- HTTP: `http://localhost:5283`
- HTTPS: `https://localhost:7003`

If HTTPS fails because the development certificate is not trusted, run:

```bash
dotnet dev-certs https --trust
```

Then restart the app.

## 5. Daily Development Loop

Use this loop for every feature or bug fix:

1. Create or update an issue describing the feature, bug, acceptance criteria, and affected systems.
2. Create a focused branch:

   ```bash
   git checkout -b feature/<short-feature-name>
   ```

3. Restore and build before editing if dependencies changed:

   ```bash
   dotnet restore 2D_RPG/2D_RPG.slnx
   dotnet build 2D_RPG/2D_RPG.slnx --configuration Debug --no-restore
   ```

4. Run the app and keep it open while editing:

   ```bash
   dotnet watch --project 2D_RPG/2D_RPG/2D_RPG.csproj run
   ```

5. Implement the smallest complete vertical slice of the feature.
6. Add or update data models, services, UI components, CSS, and tests together so the feature is usable end to end.
7. Validate in the browser with the sample map, combat preview, or menu flow affected by the change.
8. Run formatting, build, and tests before committing.
9. Commit with a clear message:

   ```bash
   git status
   git add <changed-files>
   git commit -m "Add <feature summary>"
   ```

10. Open a pull request that includes summary, screenshots for visible UI work, and test results.

## 6. Architecture Plan

### 6.1 Data models

Create serializable models before building complex UI. Models should represent game data without depending on Razor components.

Recommended model groups:

- `ProjectDefinition`: project metadata, version, asset folders, and global settings.
- `MapDefinition`: map size, tile dimensions, layers, objects, encounters, and transitions.
- `TileSetDefinition`: source image, tile size, tile IDs, tags, animation frames, and collision defaults.
- `ActorDefinition`: player characters, NPCs, mobs, stats, animations, inventory, and behavior tags.
- `AnimationDefinition`: spritesheet source, frame rectangles, frame duration, looping, and event markers.
- `EncounterDefinition`: battle background, party spawn slots, enemy spawn slots, rewards, and scripts.
- `ItemDefinition`: item type, stack rules, effects, equipment slots, prices, and descriptions.
- `QuestDefinition`: quest states, objectives, requirements, rewards, and dialogue hooks.
- `EventDefinition`: trigger type, conditions, commands, and target references.

All definitions should use stable IDs instead of display names for references. Display names can change; IDs should remain stable for save compatibility.

### 6.2 Services

Add services under `2D_RPG/2D_RPG/Services` and register them in `Program.cs`.

Recommended services:

- `EditorStateService`: current project, selected tool, selected asset, active scene, undo/redo stack, and dirty state.
- `ProjectPersistenceService`: load, save, import, export, and schema migration.
- `ProjectValidationService`: validate references, dimensions, missing assets, duplicate IDs, and unsupported data.
- `TileMapService`: tile placement, layer edits, brush operations, flood fill, collision updates, and map resizing.
- `RuntimeEngineService`: playtest state, scene loading, update ticks, actor movement, collision checks, and events.
- `CombatService`: battle state, turn order, command resolution, targeting, effects, victory, and defeat.
- `AnimationService`: animation clip lookup, frame progression, and spritesheet metadata queries.
- `AssetCatalogService`: available tilesets, spritesheets, sounds, icons, and validation of asset references.

Keep services small and testable. UI components should call services rather than duplicating engine rules in Razor markup.

### 6.3 Components

Break the current page mockup into focused components as the implementation grows:

- `EditorShell`: main layout for toolbar, panels, viewport, and status bar.
- `TilePalette`: tileset selection, brush size, tile tags, and active tile preview.
- `MapCanvas`: interactive tile rendering, selection, painting, dragging, zooming, and panning.
- `LayerPanel`: layer visibility, lock state, ordering, and active layer selection.
- `InspectorPanel`: selected object, tile, event, actor, or map properties.
- `EventScriptEditor`: event conditions, commands, validation, and preview.
- `CombatPreview`: battle stage layout and encounter test controls.
- `AssetBrowser`: browse, import, rename, tag, and delete project assets.
- `RuntimePreview`: playtest mode that runs authored maps using runtime systems.

Each component should receive data through parameters and raise callbacks for user actions when possible. Store cross-component state in services.

## 7. Editor Feature Build Order

Build the editor in vertical slices so each stage produces a usable result.

### 7.1 Project bootstrap

1. Create initial model classes for project metadata, maps, tilesets, layers, and tiles.
2. Add an in-memory sample project loaded by `EditorStateService`.
3. Register the editor services in `Program.cs`.
4. Replace hardcoded page arrays with model-backed data.
5. Show project name, active map, and dirty state in the UI.

### 7.2 Tile painting

1. Create a tile palette from the active tileset definition.
2. Add active tool state for paint, erase, fill, select, collision, object placement, and event placement.
3. Implement tile click and drag painting on the map canvas.
4. Support multiple layers such as ground, decoration, collision, objects, and events.
5. Add layer visibility and lock controls.
6. Add undo and redo for every map edit.
7. Validate painting against map bounds and locked layers.

### 7.3 Map editing

1. Add map creation, duplication, rename, resize, and delete operations.
2. Add map properties for dimensions, tile size, background color, music, and encounter table.
3. Add map transitions that point to another map ID and spawn location.
4. Add zoom and pan controls for the canvas.
5. Add grid visibility and collision overlay toggles.
6. Add save and load support for map data.

### 7.4 Object and event placement

1. Add object definitions for props, doors, chests, NPCs, and triggers.
2. Support object placement, selection, movement, deletion, and property editing.
3. Add event trigger types such as interact, touch, enter map, leave map, and battle start.
4. Add event commands for dialogue, quest updates, item rewards, map transfer, battle start, sound, and variable changes.
5. Validate event references and show warnings in the inspector.

### 7.5 Asset workflow

1. Create `wwwroot/assets` subfolders for tilesets, spritesheets, portraits, icons, audio, and sample data.
2. Add asset metadata models that reference source paths and dimensions.
3. Build an asset browser for importing and selecting assets.
4. Validate missing files, unsupported dimensions, and duplicate asset IDs.
5. Add sample assets with clear licenses or placeholders.

## 8. Runtime Engine Build Order

### 8.1 Exploration runtime

1. Load a `MapDefinition` into runtime state.
2. Render layers in deterministic order.
3. Spawn the player actor at a map-defined spawn point.
4. Handle keyboard/gamepad input for movement.
5. Resolve tile and object collision before movement is committed.
6. Trigger events when the player interacts or enters trigger zones.
7. Support map transitions and scene reloads.
8. Keep runtime state separate from editor state so playtesting does not accidentally mutate authoring data.

### 8.2 Animation runtime

1. Define spritesheet clips for idle, walk, attack, cast, hit, and death.
2. Add animation state per actor.
3. Advance frames using elapsed time rather than fixed frame counts.
4. Support directional animations for exploration actors.
5. Support one-shot and looping clips.
6. Trigger animation events for effects, hit timing, and sound cues.

### 8.3 Combat runtime

1. Define battle actors, stats, commands, target rules, and effects.
2. Load an encounter into a battle scene.
3. Place party and enemies into configured lanes and slots.
4. Calculate turn order.
5. Present command menus for attack, item, spell, defend, and flee.
6. Resolve commands and update HP, MP, status effects, rewards, and battle log entries.
7. Play matching animations and effects.
8. Return to exploration after victory, defeat, or escape.

### 8.4 Menu runtime

1. Add menu state for settings, character, inventory, equipment, spells, quests, save, and load screens.
2. Connect menus to the same data models used by exploration and combat.
3. Add inventory stack handling and equipment slot validation.
4. Add spell availability checks and MP costs.
5. Add quest journal filtering by active, completed, and failed states.
6. Add save/load UI once persistence is implemented.

## 9. Persistence and Save Data

### 9.1 Project files

1. Serialize project definitions to JSON during early development.
2. Include a schema version on every saved project.
3. Add migration logic when the schema changes.
4. Validate project files after load and before save.
5. Keep authored project data separate from runtime save data.

### 9.2 Runtime saves

1. Store current map ID, actor position, party state, inventory, quest states, variables, and flags.
2. Store references by stable ID.
3. Validate saves against the current project schema.
4. Show user-friendly errors for incompatible saves.

## 10. Styling and UX Standards

- Prefer reusable CSS classes and component-specific CSS files for larger components.
- Keep keyboard accessibility in mind for tools, panels, menus, and modal dialogs.
- Use semantic HTML where possible.
- Provide labels, titles, or ARIA descriptions for interactive editor surfaces.
- Keep high-contrast colors for map overlays, selected objects, validation errors, and combat targeting.
- For visible UI changes, capture screenshots before opening a pull request.

## 11. Testing Strategy

### 11.1 Manual checks

Run these checks whenever UI or runtime behavior changes:

1. Start the app.
2. Open the home/editor page.
3. Confirm the editor shell renders without console errors.
4. Test each edited tool or panel.
5. Resize the browser to desktop, tablet, and narrow widths.
6. Test both HTTP and HTTPS launch profiles when changing startup or static assets.

### 11.2 Automated checks

Add automated tests as services and models are introduced.

Recommended test coverage:

- Model serialization and migration.
- Map bounds checks and tile editing operations.
- Collision rules.
- Event condition evaluation.
- Combat turn order and command resolution.
- Inventory stack and equipment validation.
- Project validation diagnostics.
- Save/load compatibility.

Suggested commands once a test project exists:

```bash
dotnet test 2D_RPG/2D_RPG.slnx --configuration Debug --no-restore
dotnet test 2D_RPG/2D_RPG.slnx --configuration Release --no-restore
```

### 11.3 Required pre-commit validation

Before every commit, run:

```bash
dotnet format 2D_RPG/2D_RPG.slnx --verify-no-changes
dotnet build 2D_RPG/2D_RPG.slnx --configuration Release
dotnet test 2D_RPG/2D_RPG.slnx --configuration Release --no-build
```

If the solution does not yet contain tests, document that `dotnet test` had no test projects or skip it until a test project is added.

## 12. Release and Publish Workflow

### 12.1 Create a release build

```bash
dotnet restore 2D_RPG/2D_RPG.slnx
dotnet build 2D_RPG/2D_RPG.slnx --configuration Release --no-restore
dotnet publish 2D_RPG/2D_RPG/2D_RPG.csproj --configuration Release --output artifacts/publish --no-build
```

### 12.2 Verify publish output

1. Confirm `artifacts/publish` exists.
2. Confirm static assets were copied.
3. Run the published app locally if the target host supports it.
4. Open the app and smoke-test editor load, map rendering, navigation, and static asset loading.

### 12.3 Deployment readiness checklist

Do not call the product complete until all items below are true:

- Project load and save work reliably.
- The editor can create, edit, validate, and persist maps.
- Runtime playtest can load an authored map and move a player with collision.
- Event triggers can run at least dialogue, map transfer, item reward, and battle start commands.
- Combat can complete a full encounter loop from start to victory or defeat.
- Menus can inspect party, inventory, spells, equipment, quests, and settings.
- Assets are referenced through project metadata and validated before runtime use.
- Automated tests cover core model, editor service, runtime, combat, persistence, and validation rules.
- Release build and publish commands complete successfully.
- Documentation describes setup, development, build, test, publish, and troubleshooting steps.

## 13. Troubleshooting

### `dotnet` command not found

Install the .NET 10 SDK and restart the terminal. Re-run:

```bash
dotnet --info
```

### Restore fails

1. Confirm internet access.
2. Confirm NuGet configuration is valid.
3. Clear local package caches if needed:

   ```bash
   dotnet nuget locals all --clear
   dotnet restore 2D_RPG/2D_RPG.slnx
   ```

### HTTPS certificate warnings

Run:

```bash
dotnet dev-certs https --trust
```

Then restart the development server.

### Static assets do not load

1. Confirm files are under `2D_RPG/2D_RPG/wwwroot`.
2. Confirm asset paths are relative to the web root.
3. Restart `dotnet watch` if new folders were added.
4. Hard-refresh the browser to clear cached CSS or images.

### Razor changes do not update

1. Stop `dotnet watch`.
2. Clean and rebuild:

   ```bash
   dotnet clean 2D_RPG/2D_RPG.slnx
   dotnet build 2D_RPG/2D_RPG.slnx --configuration Debug
   ```

3. Restart the development server.
