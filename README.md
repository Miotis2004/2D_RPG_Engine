# 2D RPG Engine

A web-based, Blazor-powered 2D RPG game engine and editor prototype.

## Vision

- Tile-based top-down world and map editing for exploration.
- Side-view combat scene preview for party and enemy encounters.
- Spritesheet-driven animation metadata for environments, characters, NPCs, and mobs.
- RPG menu foundations for settings, character management, inventory, spells, and quests.

## Project Structure

- `2D_RPG/2D_RPG.slnx` - Visual Studio solution for the editor and engine web app.
- `2D_RPG/2D_RPG/2D_RPG.csproj` - ASP.NET Core Blazor project targeting .NET 10.
- `2D_RPG/2D_RPG/Program.cs` - server startup and Blazor component registration.
- `2D_RPG/2D_RPG/Components` - Razor components for the editor, engine UI, pages, and layouts.
- `2D_RPG/2D_RPG/wwwroot` - static web assets such as CSS, images, and client libraries.

## Prerequisites

Install the following tools before building the editor and engine:

1. [.NET 10 SDK](https://dotnet.microsoft.com/download) or newer compatible SDK.
2. A terminal with Git available on the path.
3. Optional: Visual Studio 2026, Visual Studio Code, or another C# editor with Razor support.

Verify the SDK installation:

```bash
dotnet --info
```

## Build Steps

Run these commands from the repository root.

### 1. Restore dependencies

```bash
dotnet restore 2D_RPG/2D_RPG.slnx
```

### 2. Build the editor and engine

```bash
dotnet build 2D_RPG/2D_RPG.slnx --configuration Release --no-restore
```

### 3. Run the development server

```bash
dotnet run --project 2D_RPG/2D_RPG/2D_RPG.csproj --configuration Debug
```

Open the local URL displayed by `dotnet run` to use the editor and engine in a browser. The default ASP.NET Core launch profile usually exposes both HTTP and HTTPS endpoints.

### 4. Optional: publish a production build

```bash
dotnet publish 2D_RPG/2D_RPG/2D_RPG.csproj --configuration Release --output artifacts/publish
```

The publish output in `artifacts/publish` can be deployed to an ASP.NET Core-compatible host.

## Development Workflow

Use this checklist when moving from mockup work into editor and engine implementation:

1. Start the app with `dotnet run --project 2D_RPG/2D_RPG/2D_RPG.csproj`.
2. Implement editor features in `2D_RPG/2D_RPG/Components` and shared engine services/models as they are added.
3. Keep static art, spritesheet samples, and CSS in `2D_RPG/2D_RPG/wwwroot`.
4. Run `dotnet build 2D_RPG/2D_RPG.slnx --configuration Release` before committing changes.
5. Add automated tests once engine logic is separated into testable services or libraries.

## Troubleshooting

- If `dotnet` is not found, install the .NET 10 SDK and restart the terminal so the SDK is available on `PATH`.
- If restore fails, confirm network access to NuGet and retry `dotnet restore 2D_RPG/2D_RPG.slnx`.
- If HTTPS startup fails locally, trust the development certificate with `dotnet dev-certs https --trust` and rerun the project.
