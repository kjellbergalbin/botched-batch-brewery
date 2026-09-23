# Botched Batch Brewery

A 3D brewery-management and night-defense game: brew unusual beer by day, then defend the abandoned island brewery from sewer creatures attracted to failed batches.

> **Status:** early playable prototype. Movement, camera, blockout geometry, lighting, and an optional AI-editor bridge are in place. Brewing, customers, building, combat, and progression are not implemented yet.

## Toolchain

- Godot **4.7.2 .NET**
- .NET SDK **8.0.425** (pinned to the installed .NET 8 LTS feature band; later patches in this band are accepted)
- C# with nullable reference types and warnings-as-errors
- Optional Godot MCP **4.1.11**, with OpenCode and VS Code client configurations
- Node.js **20 or later** (only required for Godot MCP)

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

Read [`AGENTS.md`](AGENTS.md) before editing. It is the canonical policy for Codex and other coding agents.

The agent should use source files for normal code changes and, when explicitly enabled, Godot MCP for editor state, scene inspection, deterministic playtesting, runtime state, screenshots, and visual verification.

The addon is **disabled by default** because version 4.1.11 accepts unauthenticated WebSocket commands, including runtime script execution. Loopback binding limits network exposure but is not authorization: software running on the same machine and potentially browser-originated WebSocket clients can still reach it.

Enable `Godot MCP` in **Project > Project Settings > Plugins** only for a trusted local development session, keep its bind mode on `127.0.0.1`, and disable the plugin when finished. Enabling the plugin adds `MCPGameBridge` as an autoload and changes `project.godot`. To restore the checked-in disabled state, disable the plugin and remove the `MCPGameBridge` autoload—or revert those changes to `project.godot`—before committing. OpenCode's checked-in MCP entry is also disabled by default and must be enabled deliberately.

## Current controls

| Action | Keyboard | Gamepad |
|---|---|---|
| Move | WASD / arrow keys | Left stick |
| Jump | Space | South/A button |

## License

No project license has been selected yet. The vendored Godot MCP addon is MIT-licensed; see [`addons/godot_mcp/UPSTREAM.md`](addons/godot_mcp/UPSTREAM.md).
