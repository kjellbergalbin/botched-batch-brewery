# Claude Code project context

Read `AGENTS.md` first; it is the canonical repository policy.

## Start here

- Product and setup: `README.md`
- Architecture: `docs/architecture.md`
- Agent workflow and tool rationale: `docs/ai-workflow.md`
- Main scene: `game/world/world.tscn`
- Full verification: `./scripts/verify.sh`

## Claude-specific guidance

- The project-scoped `.mcp.json` starts the Godot MCP 4.1.11 client after normal client approval, but the Godot addon is disabled by default. The user must explicitly enable it in the editor for a trusted local session.
- Keep write permissions gated. Read/inspect first, then make a bounded change.
- Prefer MCP read tools and structured runtime state before screenshots.
- Only one MCP client can own the live Godot editor connection at a time.
- Use a worktree for parallel implementation; do not let subagents share this checkout.
- Compilation is not visual verification. Run/import the project and inspect user-visible changes before declaring completion.
