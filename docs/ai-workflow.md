# AI-assisted Godot workflow

## Research conclusion

Godot is unusually suitable for agent-assisted development because project settings, scenes, and resources are primarily text and are designed to work with version control.[1][3]

The engine still has editor-owned and runtime-only state, so the strongest workflow combines normal source editing with headless validation and a live editor bridge rather than asking an agent to edit every `.tscn` blindly.[4][8]

The project targets Godot 4.7.2, a stable maintenance release. Godot says maintenance releases are expected to be safe upgrades while recommending backups or version control before upgrading.[7] The C# project and solution are committed because Godot's C# documentation identifies them as important version-controlled project files; only generated `.godot/mono` state belongs in the ignore list.[5]

## Recommended loop

```text
brief -> inspect -> acceptance criteria -> tests -> bounded edit
      -> format/build -> import -> headless smoke
      -> MCP runtime observation -> visual review -> human playtest
```

### 1. Start from a vertical slice

Ask for one player-visible outcome that crosses only the layers it needs. “Place one valid shelf, charge its cost, and restore it after load” is better than “build the construction system.” A small slice limits the agent's context, makes rollback easy, and produces a playable checkpoint.

### 2. Separate rules from engine adapters

Put deterministic rules in plain C# and keep Nodes focused on lifecycle, input, physics, and presentation. Godot officially supports modern .NET but requires the .NET editor build and SDK.[5] This split lets `dotnet test` give fast feedback while scene-level behavior remains testable in Godot.

### 3. Keep source agent-readable

Use text `.tscn`/`.tres` resources, feature-local directories, stable names, and small scenes with one clear responsibility.[2] Godot's project-organization guidance recommends grouping assets near the scenes that use them and using snake_case for files and folders.[1] C# files retain PascalCase to match their classes and Godot's C# conventions.[6]

### 4. Validate at increasing cost

Run cheap checks first:

1. `dotnet format --verify-no-changes`
2. `dotnet build`
3. Godot `--headless --editor --import --quit`
4. Godot headless main-scene boot
5. MCP runtime state and deterministic input
6. screenshots only where visual judgment is required
7. manual playtest for feel and pacing

Godot documents `--headless`, `--import`, `--build-solutions`, and command-line export as supported automation paths.[4]

### 5. Use MCP as eyes and hands, not as authority

The pinned Godot MCP bridge can inspect editor state, run and freeze the game, inject input, collect structured runtime state, profile, and capture screenshots.[8] It is best used for information that source files cannot prove. Normal C# and small text-resource edits should remain visible in Git and pass the same local/CI gates.

The bridge is pinned to 4.1.11 and configured per project for Claude Code, OpenCode, and VS Code; its server requires Node.js 20 or later.[8][9] The Godot addon and the OpenCode entry are disabled by default.

Version 4.1.11 has no client authentication. Its unencrypted WebSocket accepts commands that can modify editor state and execute guarded—but not sandboxed—GDScript in the running game. Binding to localhost reduces network exposure but does not authorize clients, so enable the addon only for a trusted local session, never expose port 6550 to an untrusted interface, and disable it afterward.[8][9]

## Agent roles

Use roles sequentially, not as a crowd editing one checkout:

| Role | Output | Write access |
|---|---|---|
| Researcher | docs/API evidence and constraints | none |
| Planner/architect | acceptance criteria and affected boundaries | docs only |
| Implementer | one vertical slice in a worktree | bounded files |
| Test/review agent | failing scenarios and diff findings | none by default |
| Human designer | feel, art direction, pacing, scope decisions | final authority |

Parallelism is useful for independent research and review. Parallel writers touching the same scenes create merge noise and inconsistent design decisions.

## Testing choice

Do not install a large test framework before there is logic to test. Start with plain .NET tests for domain rules. When the project needs automated Nodes, scenes, signals, or input, GdUnit4 and GdUnit4Net are maintained options with Godot 4/C# and command-line integration.[10][11]

The practical target is high confidence in rule code plus automated critical journeys—not an arbitrary coverage percentage over scenes and assets.

## What not to automate

- Do not accept a screenshot as proof of correct rules or persistence.
- Do not accept a successful build as proof of correct visuals or game feel.
- Do not let an agent change engine version, addons, input maps, or autoloads without reviewing the full diff.
- Do not generate broad manager/service frameworks before a concrete feature requires them.
- Do not give several agents write access to the same Godot checkout or live MCP editor.

## Sources

[1] https://docs.godotengine.org/en/4.7/tutorials/best_practices/project_organization.html
[2] https://docs.godotengine.org/en/4.7/tutorials/best_practices/scene_organization.html
[3] https://docs.godotengine.org/en/4.7/tutorials/best_practices/version_control_systems.html
[4] https://docs.godotengine.org/en/4.7/tutorials/editor/command_line_tutorial.html
[5] https://docs.godotengine.org/en/4.7/tutorials/scripting/c_sharp/c_sharp_basics.html
[6] https://docs.godotengine.org/en/4.7/tutorials/scripting/c_sharp/c_sharp_style_guide.html
[7] https://godotengine.org/article/maintenance-release-godot-4-7-2
[8] https://github.com/satelliteoflove/godot-mcp/tree/godot-mcp-v4.1.11
[9] https://github.com/satelliteoflove/godot-mcp/blob/godot-mcp-v4.1.11/INSTALL.md
[10] https://github.com/godot-gdunit-labs/gdUnit4
[11] https://github.com/godot-gdunit-labs/gdUnit4Net
