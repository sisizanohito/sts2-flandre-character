# Flandre Mod Agent Notes

Read this file first whenever a new Codex thread starts in this repository.

## First Reading Order

1. `docs/roadmap.md`
2. `docs/agent-knowledge.md`
3. If the task touches build, install, localization, assets, or packed files: `docs/build-install-workflow.md`
4. If the task needs runtime display or install verification: `docs/install-gate-checklist.md`

## Project Context

- This repository is the Flandre mod for Slay the Spire 2.
- Treat paths in docs as repository-relative unless a task explicitly requires an external location.
- The `sts2_moddding` MCP server is configured for this workspace.
- Do not treat repository edits alone as complete when the change affects assets, localization, PCK contents, install state, or runtime display.
- For those changes, think through install and live verification before calling the task done.

## MCP Usage

Prefer MCP or developer tools over ad-hoc shell workarounds when checking game code, game state, or live behavior.

Common `sts2_moddding` uses:

- `search_game_code`: search game source.
- `get_entity_source`: inspect card, power, relic, or other entity source.
- `install_mod`: install the local mod build.
- `launch_game`: launch the game.
- `bridge_*`: inspect or manipulate the running game state.

For strict MCP verification, report:

1. the exact tool name
2. success or failure
3. returned values or exact error
4. conclusion

Do not accept a generic `connected` answer as proof of MCP success.

## Working Rules

- Read the current roadmap and relevant docs before substantial work.
- Separate confirmed facts from inference.
- Keep each implementation unit small enough to verify.
- Do not revert unrelated local changes.
- If committing, stage only files that belong to the current issue.
- After useful discoveries, update the relevant docs briefly.

## Useful Docs

- `docs/roadmap.md`: current status, near-term priorities, and next entry points.
- `docs/agent-knowledge.md`: MCP operations, verification rules, and lessons from previous debugging.
- `docs/build-install-workflow.md`: build, PCK rebuild, and install workflow.
- `docs/install-gate-checklist.md`: short install verification gate.
- `docs/card-pool-package-map.md`: current card pool package map.
- `docs/bloodshed-design-note.md`: Bloodshed design notes.
- `docs/combat-animation-progress.md`: combat animation progress.
- `docs/flandre-spine-visual-entry.md`: Spine migration entry point.
