# 2D RPG Engine

A web-based, Blazor-powered 2D RPG game engine and editor prototype.

## Vision

- Tile-based top-down world and map editing for exploration.
- Side-view combat scene preview for party and enemy encounters.
- Spritesheet-driven animation metadata for environments, characters, NPCs, and mobs.
- RPG menu foundations for settings, character management, inventory, spells, and quests.

## Project Structure

- `src/RpgEngine.Client` - Blazor WebAssembly client and editor UI.
- `Components/TileMapEditor.razor` - interactive tile map editor surface.
- `Components/CombatPreview.razor` - side-view battle layout preview.
- `Services/GameState.cs` - starter in-memory engine/editor state.
- `Services/EngineCatalog.cs` - sample tile, actor, animation, and menu catalog data.

## Getting Started

```bash
dotnet restore
dotnet run --project src/RpgEngine.Client
```

Open the displayed local URL to use the editor prototype.
