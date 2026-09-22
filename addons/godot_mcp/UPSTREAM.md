# Vendored Godot MCP addon

- Upstream: <https://github.com/satelliteoflove/godot-mcp>
- Version: `4.1.11` (`godot-mcp-v4.1.11`)
- License: MIT
- Distribution: `@satelliteoflove/godot-mcp@4.1.11`
- npm tarball SHA-512 integrity: `sha512-XYAPURo4Yyw7eOijZ15fyrf21hbFi+Dw+lO2xELvPQ1F2QBo6ZiZKzQQw9yF5jruiIJ4fnP5sPdt0lpKHQQedw==`

The addon source under this directory is copied from the pinned npm package. The matching MCP server version is pinned in `.mcp.json`, `.vscode/mcp.json`, and `opencode.json`. The recorded SHA-512 documents the reviewed npm tarball; `npx` does not enforce this value as a local lockfile would.

The addon is disabled in `project.godot` by default. Upstream 4.1.11 has no client authentication, and its runtime execution guard explicitly is not a security sandbox. Enable it only for a trusted local development session, keep it bound to loopback, and disable it afterward.

Do not edit vendored files directly. Update the pinned package version, review the upstream changelog/diff, replace the whole addon directory, then run the complete verification pipeline.
