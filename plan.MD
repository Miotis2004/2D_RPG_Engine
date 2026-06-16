# 2D RPG Engine Development Plan

This document outlines the step-by-step development plan, milestones, and testing requirements for building the Blazor-based 2D RPG editor and engine. It is derived from the architecture and workflow guidelines in `DEVELOPMENT.md`.

## Testing Strategy
Features should be developed in vertical slices and tested incrementally. Given the Blazor architecture, we will use the following testing stack:
- **xUnit** (or **NUnit**): For unit testing data models, engine logic, calculation rules, and services.
- **bUnit**: For unit testing Blazor components, ensuring proper rendering, parameter passing, interactions, and event handling.
- **Playwright** (or **Selenium**): For end-to-end (E2E) integration testing in the final stages.

---

## Milestone 1: Project Bootstrap & Foundation

**Goal:** Establish core data models, serialization, and the editor shell layout.

### Tasks:
1. Create core serializable data models (`ProjectDefinition`, `MapDefinition`, `TileSetDefinition`, etc.).
2. Implement an in-memory sample project loaded by `EditorStateService`.
3. Register editor services in `Program.cs`.
4. Build the `EditorShell` component layout (toolbar, panels, viewport, status bar).
5. Bind the UI to model-backed data, showing project name, active map, and dirty state.

### Testing:
- **Unit Tests (xUnit):** Test JSON serialization/deserialization of core models. Ensure IDs remain stable and references do not break.
- **Component Tests (bUnit):** Verify `EditorShell` renders correctly, accepting its state parameters and reflecting dirty/clean status.

---

## Milestone 2: Map Authoring & Tile Painting

**Goal:** Implement map creation, management, and tile-based painting functionality.

### Tasks:
1. Build the `TilePalette` component for tileset selection and brush configuration.
2. Implement active tool state (paint, erase, fill, select).
3. Build the `MapCanvas` component with interactive click and drag painting.
4. Implement layer support (ground, decoration, collision, objects, events) with visibility and lock controls.
5. Implement map operations: creation, duplication, rename, resize, deletion, and properties editing (dimensions, music, transitions).
6. Implement undo/redo functionality for map edits.

### Testing:
- **Unit Tests (xUnit):** Test bounds checking for tile placement, layer collision logic, and the integrity of the undo/redo stack in `TileMapService`.
- **Component Tests (bUnit):** Test `TilePalette` selection state, `MapCanvas` layer rendering, tool interactions, and zoom/pan functionality.

---

## Milestone 3: Object & Event Placement

**Goal:** Allow authors to populate maps with interactive elements.

### Tasks:
1. Define object models (props, doors, chests, NPCs, triggers) and event models.
2. Support object placement, selection, movement, and deletion on the `MapCanvas`.
3. Build the `InspectorPanel` for editing object properties and event conditions/commands.
4. Implement specific event triggers (interact, touch, enter map) and commands (dialogue, item rewards, transfer map).

### Testing:
- **Unit Tests (xUnit):** Validate event references, command data integrity, and object properties within the `ProjectValidationService`.
- **Component Tests (bUnit):** Verify the `InspectorPanel` updates accurately when objects/events are selected and correctly mutates underlying object state via callbacks.

---

## Milestone 4: Asset Management Workflow

**Goal:** Provide an interface for importing and organizing game assets.

### Tasks:
1. Set up standard `wwwroot/assets` subfolders (tilesets, spritesheets, audio, etc.).
2. Add asset metadata models that link source paths to usage configurations.
3. Build an `AssetBrowser` component for browsing, importing, renaming, tagging, and deleting project assets.
4. Implement validation for missing files, duplicate asset IDs, and unsupported dimensions.

### Testing:
- **Unit Tests (xUnit):** Test `AssetCatalogService` for reference validation, missing asset detection, and path resolving.
- **Component Tests (bUnit):** Test the `AssetBrowser` UI for proper asset list display, error messaging for invalid assets, and basic user interaction flows.

---

## Milestone 5: Exploration Runtime

**Goal:** Enable map playtesting with actor movement, collision, and basic interactions.

### Tasks:
1. Implement `RuntimeEngineService` to load a `MapDefinition` and manage runtime state isolated from authoring state.
2. Spawn the player actor and handle keyboard/gamepad movement input.
3. Implement tile and object collision resolution before movement commits.
4. Trigger interactions and map transitions dynamically.
5. Build the `RuntimePreview` component to host playtest sessions.

### Testing:
- **Unit Tests (xUnit):** Exhaustively test movement logic and collision detection algorithms within `RuntimeEngineService`. Test event trigger evaluation (e.g., condition parsing).
- **Integration Tests:** Ensure `RuntimePreview` completely clones or safely isolates the map data so playtesting does not mutate the `ProjectDefinition`.

---

## Milestone 6: Animation Runtime

**Goal:** Animate sprites in both exploration and combat contexts.

### Tasks:
1. Define spritesheet clip metadata (e.g., idle, walk, attack, cast, hit, death).
2. Implement animation state management within an `AnimationService`.
3. Process time-based frame advancement (rather than fixed-frame ticks).
4. Support directional animations, looping vs. one-shot clips, and specific event cues (sound/effect timing).

### Testing:
- **Unit Tests (xUnit):** Test elapsed-time frame progression math, loop recycling logic, and directional lookup mapping in `AnimationService`.
- **Component Tests (bUnit):** Verify components correctly update their internal CSS/Canvas state to display the proper spritesheet coordinates over time.

---

## Milestone 7: Combat Runtime

**Goal:** Implement a functional side-view, turn-based combat system.

### Tasks:
1. Define combat data models (actor stats, encounter lanes, commands, status effects, targeting rules).
2. Build `CombatService` to manage battle state, calculate turn order, and resolve actions.
3. Build the `CombatPreview` component to visualize the stage, party/enemy layouts, and UI.
4. Implement command execution flows: HP/MP updates, application of status effects, matching animations, and victory/defeat resolution.

### Testing:
- **Unit Tests (xUnit):** Extremely critical testing for `CombatService`: turn speed formulas, damage calculations, targeting array filters, and status effect application/expiry logic.
- **Component Tests (bUnit):** Verify `CombatPreview` command menus function properly, correctly block invalid selections, and update the UI in response to state changes.

---

## Milestone 8: Menu Systems & Persistence

**Goal:** Provide standard RPG menus and reliable save/load mechanisms.

### Tasks:
1. Build runtime menu components: Settings, Character, Inventory, Equipment, Spells, Quests, Save/Load.
2. Implement underlying menu mechanics: inventory stacking, equipment slot rules, spell MP requirements, and quest progression tracking.
3. Implement `ProjectPersistenceService` for serializing project files with schema versioning.
4. Implement runtime save/load logic (storing current map ID, actor pos, party state, flags).

### Testing:
- **Unit Tests (xUnit):**
  - Thorough testing of save data serialization and schema migration logic.
  - Test inventory stacking logic, equipment constraints, and quest conditional logic.
- **Component Tests (bUnit):** Ensure menus correctly display data, handle filtering (e.g., active vs. completed quests), and block invalid item usage.

---

## Milestone 9: Polish & Release Preparation

**Goal:** Stabilize the engine, fix UX issues, and prepare for deployment.

### Tasks:
1. Perform comprehensive UI/UX review: keyboard accessibility, semantic HTML correctness, responsive layout.
2. Ensure user-friendly validation diagnostics exist for all authoring workflows.
3. Automate pre-commit validation and CI build scripts (format, build, test).
4. Execute release builds (`dotnet publish`).
5. Conduct a final holistic review against the deployment readiness checklist in `DEVELOPMENT.md`.

### Testing:
- **Integration/E2E Testing (Playwright):** Write smoke tests for critical E2E flows. Ensure a test browser can launch the app, load a project, edit a map, and start a playtest session without crashes.
- **Manual QA:** Final manual sweep through all tools, checking visual coherence and performance across different browser window sizes.