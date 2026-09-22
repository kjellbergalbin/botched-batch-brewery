# Botched Batch Brewery

A 3D brewery-management and night-defense game: brew unusual beer by day, then defend the abandoned island brewery from sewer creatures attracted to failed batches.

> **Status:** early playable prototype. Movement, camera, blockout geometry, lighting, and an AI-editor bridge are in place. Brewing, customers, building, combat, and progression are not implemented yet.

## Toolchain

- Godot **4.7.2 .NET**
- .NET SDK **8.0** (`global.json` allows the latest 8.0 feature band)
- C# with nullable reference types and warnings-as-errors
- Godot MCP **4.1.11**, pinned for Claude Code, OpenCode, and VS Code

Use the .NET build of Godot, not the standard build.

## Quick start

```bash
git clone https://github.com/kjellbergalbin/botched-batch-brewery.git
cd botched-batch-brewery
export GODOT4=/absolute/path/to/Godot_v4.7.2-stable_mono_linux.x86_64
dotnet restore --locked-mode
dotnet build --no-restore
$GODOT4 --editor --path .
```

On Windows PowerShell:

```powershell
$env:GODOT4 = "C:\path\to\Godot_v4.7.2-stable_mono_win64.exe"
dotnet restore --locked-mode
dotnet build --no-restore
& $env:GODOT4 --editor --path .
```

## Verification

Linux/macOS:

```bash
./scripts/verify.sh
```

Windows:

```powershell
./scripts/verify.ps1
```

The verification pipeline restores the locked NuGet graph, checks formatting, compiles C#, imports every Godot resource, and boots the main scene headlessly.

## Project map

```text
game/
  player/          Player scene, controller, and camera
  world/           Playable world and world-local props
addons/
  godot_mcp/       Vendored, pinned editor bridge
docs/
  architecture.md  Project boundaries and growth path
  ai-workflow.md   Agent workflow and research rationale
scripts/           Reproducible local verification
```

New gameplay should be organized by feature (`game/brewing/`, `game/building/`, `game/combat/`) rather than split globally into `Scripts/`, `Scenes/`, and `Resources/`.

## AI-assisted development

Read [`AGENTS.md`](AGENTS.md) before editing. Claude Code also loads [`CLAUDE.md`](CLAUDE.md) and the project-scoped [`.mcp.json`](.mcp.json).

The agent should use source files for normal code changes and Godot MCP for editor state, scene inspection, deterministic playtesting, runtime state, screenshots, and visual verification. MCP listens on `127.0.0.1:6550`; do not expose it to an untrusted network.

## Current controls

| Action | Keyboard | Gamepad |
|---|---|---|
| Move | WASD / arrow keys | Left stick |
| Jump | Space | South/A button |

## License

No project license has been selected yet. The vendored Godot MCP addon is MIT-licensed; see [`addons/godot_mcp/UPSTREAM.md`](addons/godot_mcp/UPSTREAM.md).
