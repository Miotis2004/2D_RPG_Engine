#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
SLN="$ROOT/2D_RPG/2D_RPG.slnx"
APP="$ROOT/2D_RPG/2D_RPG/2D_RPG.csproj"
PUBLISH_DIR="$ROOT/artifacts/publish"

DOTNET_CLI_TELEMETRY_OPTOUT=1 dotnet restore "$SLN"
DOTNET_CLI_TELEMETRY_OPTOUT=1 dotnet format "$SLN" --verify-no-changes --no-restore
DOTNET_CLI_TELEMETRY_OPTOUT=1 dotnet build "$SLN" --configuration Release --no-restore
DOTNET_CLI_TELEMETRY_OPTOUT=1 dotnet test "$SLN" --configuration Release --no-build
DOTNET_CLI_TELEMETRY_OPTOUT=1 dotnet publish "$APP" --configuration Release --output "$PUBLISH_DIR" --no-build
