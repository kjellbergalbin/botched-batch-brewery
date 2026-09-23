# Agent instructions

## Mission

Build **Botched Batch Brewery** as small, playable vertical slices. Preserve the core loop: daytime brewing and sales fund nighttime defense against sewer creatures attracted to failed beer.

## Stack

- Godot 4.7.2 .NET; C# on .NET 8
- Main scene: `game/world/world.tscn`
- Project: `BotchedBatchBrewery.csproj`
- Optional Godot MCP addon/server: 4.1.11, disabled by default

## Structure

Organize production files by feature:

```text
game/<feature>/
  FeatureController.cs
  feature_scene.tscn
  resources/
  ui/
```

Existing features are `game/player/` and `game/world/`. Add future work under `game/brewing/`, `game/building/`, `game/customers/`, `game/combat/`, and `game/progression/` only when those slices exist. Do not create empty architecture.

Keep reusable scenes close to the feature that owns them. Use `.tscn` and `.tres`, never binary `.scn` resources. Commit `.csproj`, `.sln`, and `.uid` files; never commit `.godot/`, `bin/`, or `obj/`.

## C# rules

- Follow the Godot C# style guide and `.editorconfig`.
- Godot node scripts are `partial` and use PascalCase filenames matching the class.
- Use nullable reference types; warnings are build failures.
- Export designer-tunable values instead of burying balancing numbers in code.
- Use gameplay-specific input actions, never built-in `ui_*` actions for player mechanics.
- Keep engine-independent game rules in plain C# classes. Node scripts adapt input, scenes, audio, animation, and physics to those rules.
- Prefer signals/events and explicit exported references over global service locators.
- Add an Autoload only for truly global lifecycle state; do not use Autoloads as a default dependency container.

## Scene rules

- A scene owns one coherent concept and should run in isolation when practical.
- Instance feature scenes instead of duplicating node trees.
- Do not hand-edit opaque or binary-encoded resource data. Use Godot MCP/editor operations for GridMap, TileMap, animation, and visual transforms.
- Text scene edits are acceptable for small, reviewable changes, but must pass import and runtime verification.

## Required workflow

1. Inspect the relevant scene tree, scripts, project settings, and current git diff.
2. For non-trivial work, state acceptance criteria and split the feature into one vertical slice.
3. Write tests first for engine-independent rules. Use GdUnit4Net only when a test needs Nodes, scenes, signals, or runtime input.
4. Implement the smallest change.
5. Run `dotnet format`, `dotnet build`, Godot headless import, and a headless main-scene smoke test.
6. If the user explicitly enabled Godot MCP for the session, use it to run and observe user-visible changes. Capture runtime state before spending tokens on screenshots.
7. Review the final diff. Never claim a visual or gameplay result was verified from compilation alone.

Run all local gates with:

```bash
./scripts/verify.sh
```

## Agent coordination

- One writer per worktree. Parallel agents may research or review, but must not edit the same checkout.
- Use a dedicated branch/worktree for each vertical slice.
- Keep commits small and conventional: `feat:`, `fix:`, `test:`, `docs:`, `refactor:`, `chore:`.
- Do not merge, rewrite history, add dependencies, or update vendored addons without explicit task scope.
- Push a dedicated feature branch when the user asks for a testable Godot build; never push directly to `main` or merge a pull request without explicit approval.

## Security

- Keep Godot MCP disabled except during an explicitly authorized local session. Version 4.1.11 has no client authentication and exposes runtime execution helpers.
- When enabled, keep the bridge on `127.0.0.1`, connect only trusted clients, and disable it afterward. Loopback binding reduces exposure but is not authorization.
- Never commit credentials, `.env` files, personal paths, editor caches, or generated builds.
- Treat downloaded addons and assets as third-party dependencies: pin versions, record provenance, and review updates.

## Product guardrails

Prioritize a fun proof of the day/night loop over broad systems. Avoid procedural-content frameworks, generic service layers, multiplayer, online accounts, and premature save migrations until the core loop is playable.
