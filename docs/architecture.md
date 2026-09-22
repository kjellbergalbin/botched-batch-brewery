# Architecture

## Current shape

```text
project.godot
  -> game/world/world.tscn
       -> game/world/island.tscn
       -> game/world/props/box.tscn
       -> game/player/player.tscn
            -> PlayerController.cs
            -> FollowCamera.cs
```

This is intentionally a small, feature-first structure. The previous type-first `Scenes/`, `Prefabs/`, and `Scripts/` split forced every feature change to span unrelated top-level directories. Keeping scenes, scripts, resources, and UI close to the owning gameplay feature reduces search cost and makes an AI agent's edit scope easier to constrain.

## Boundaries

### Godot-facing layer

Node scripts own engine integration:

- input and physics callbacks
- scene-tree references
- animation, audio, particles, and rendering
- serialization to Godot resources

Keep these scripts thin. They should translate Godot state into calls on game rules and apply the result back to nodes.

### Game-rule layer

Brewing recipes, inventory transactions, customer demand, pricing, wave scheduling, and progression should be plain C# wherever possible. Plain C# is fast to test, does not require a running editor, and gives coding agents deterministic feedback.

Create this layer only as features arrive. A likely future layout is:

```text
game/brewing/
  BrewingBatch.cs          # plain C# domain rule
  BrewingStation.cs        # Godot adapter
  brewing_station.tscn
  resources/
    BeerRecipe.cs
```

### Shared code

Do not add a generic `Common`, `Managers`, or `Utils` dumping ground. Promote code to `game/shared/` only after at least two real features need the same abstraction.

## Scene ownership

- `world.tscn` composes the playable prototype. It should not implement feature logic.
- `player.tscn` owns the player collision body, presentation mesh, and follow camera.
- Reusable props stay with the world feature until another feature clearly owns them.
- A scene should have one clear root responsibility and be runnable in isolation where practical.

## State and communication

Use direct child references inside a scene. Across feature boundaries, prefer typed C# events or Godot signals. Autoloads are reserved for truly global lifecycle concerns such as save-slot coordination or scene transitions—not ordinary gameplay services.

## Testing strategy

1. **Plain .NET tests:** engine-independent domain rules.
2. **Godot integration tests:** scenes, signals, input, and node lifecycle; adopt GdUnit4Net when the first such behavior needs automation.
3. **Headless smoke:** resource import and main-scene boot on every change.
4. **MCP playtest:** deterministic input/runtime inspection plus targeted screenshots for visual work.
5. **Human playtest:** game feel, pacing, readability, and fun.

A coverage percentage is useful for the rule layer, but it is not a substitute for playtesting. Do not chase coverage for declarative scenes or visual assets.

## Near-term vertical slices

1. Grid placement: preview, validate, place, pay, remove, save/load.
2. Brewing batch: choose ingredients, advance stages, produce quality and failure outcomes.
3. Customer sale: request, fulfillment, payment, and preference feedback.
4. Night defense: one spawn route, one enemy, one weapon station, win/lose state.
5. Day/night loop: connect the first four slices into one repeatable run.
