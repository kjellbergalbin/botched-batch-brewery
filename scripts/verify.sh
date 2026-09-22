#!/usr/bin/env bash
set -euo pipefail

if [[ -z "${GODOT4:-}" ]]; then
  if command -v godot4 >/dev/null 2>&1; then
    GODOT4="$(command -v godot4)"
  elif command -v godot >/dev/null 2>&1; then
    GODOT4="$(command -v godot)"
  else
    echo "Set GODOT4 to the Godot 4.7.2 .NET executable." >&2
    exit 2
  fi
fi

if [[ ! -x "$GODOT4" ]]; then
  echo "GODOT4 is not executable: $GODOT4" >&2
  exit 2
fi

run_godot_gate() {
  local label="$1"
  shift
  local log
  log="$(mktemp)"

  echo "==> $label"
  if ! "$GODOT4" "$@" 2>&1 | tee "$log"; then
    rm -f "$log"
    return 1
  fi

  if grep -Eq '(^|[[:space:]])SCRIPT ERROR:|^ERROR:' "$log"; then
    echo "$label reported an engine or script error." >&2
    rm -f "$log"
    return 1
  fi

  rm -f "$log"
}

echo "==> Dependency restore"
dotnet restore BotchedBatchBrewery.sln --locked-mode --nologo

echo "==> C# format"
dotnet format BotchedBatchBrewery.sln --verify-no-changes --no-restore

echo "==> C# build"
dotnet build BotchedBatchBrewery.sln --no-restore --nologo

run_godot_gate "Godot import" --headless --editor --path . --import --quit
run_godot_gate "Godot main-scene smoke" --headless --path . --quit-after 5

echo "All verification gates passed."
