# Prototype Visual Setup

## Goal

Make the first playable island feel like the beginning of a warm, stylized brewery world rather than an overexposed blockout.

## Chosen baseline

- `WorldEnvironment` owns the scene-wide visual language.
- A `ProceduralSkyMaterial` supplies a dark blue upper sky and warm copper horizon.
- The first directional light is the warm key light; its lower energy prevents the nearly white result produced by the earlier 3.0-energy key, 1.2 ambient energy, 1.2 exposure, and 0.8 fill-light combination.
- The sky supplies low-intensity ambient light, with a weak cool fill preserving shape separation.
- A small warm `OmniLight3D` creates a prototype focal area around the starting brewery space.
- Ground and blockout props use rough, darker material colors. These are deliberate low-poly placeholders, not final texture assets.

## Why no downloaded textures yet

The current prototype has no licensed texture pack or final asset style. Introducing arbitrary image textures now would create provenance and art-direction debt. The material palette proves readability and lighting first; later texture assets must be licensed, recorded, and placed only in the Godot project when they are production-ready.

## Tuning order

When the scene changes, tune in this order:

1. Key-light energy and color.
2. Environment exposure and sky ambient energy.
3. Material albedo value and roughness.
4. Fill-light energy.
5. Glow, SSAO, and fog last.

Do not compensate for an overbright key light by only lowering exposure: it flattens the entire scene instead of restoring light contrast.

## References

- [Godot: Environment and post-processing](https://docs.godotengine.org/en/stable/tutorials/3d/environment_and_post_processing.html)
- [Godot: ProceduralSkyMaterial](https://docs.godotengine.org/en/stable/classes/class_proceduralskymaterial.html)
- [Third-person controller scene example](https://github.com/selgesel/godot4-third-person-controller/blob/main/Main.tscn)
