# Release Readiness Checklist

Use this checklist before tagging a release or deploying a published build.

## Automated gate

Run the full local gate from the repository root:

```bash
./scripts/validate.sh
```

The script restores packages, verifies formatting, builds in Release mode, runs the test suite, and publishes the Blazor app to `artifacts/publish`.

## Manual smoke test

1. Start the app with `dotnet run --project 2D_RPG/2D_RPG/2D_RPG.csproj --configuration Release`.
2. Open the editor route and confirm the project, active map, toolbar, map canvas, asset browser, and inspector render.
3. Paint and erase a tile, place an object, place an event, and confirm validation diagnostics remain understandable.
4. Start a runtime playtest session and move the player through at least one passable tile and one blocked tile.
5. Open combat and menu previews and confirm controls are keyboard reachable.
6. Resize the browser to desktop, tablet, and narrow widths and confirm panels stack without clipping critical controls.
7. Verify `artifacts/publish` contains the app binaries and `wwwroot` static assets after publishing.

## Deployment criteria

- Project validation reports blocking authoring issues before save or release.
- Keyboard focus is visible on editor controls, map tiles, menu controls, and runtime controls.
- Semantic landmarks and ARIA labels identify major panels and interactive canvases.
- Release build, tests, and publish complete successfully in CI.
