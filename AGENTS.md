# Flandre Mod Agent Notes

Read this file first whenever a new Codex thread starts in this repository.

## First Reading Order

1. `docs/roadmap.md`
2. `docs/agent-knowledge.md`
3. If the task touches build, install, localization, assets, or packed files: `docs/workflows/build-install-workflow.md`
4. If the task needs runtime display or install verification: `docs/workflows/install-gate-checklist.md`

## Project Context

- This repository is the Flandre mod for Slay the Spire 2.
- Treat paths in docs as repository-relative unless a task explicitly requires an external location.
- The `sts2_moddding` MCP server is configured for this workspace.
- Do not treat repository edits alone as complete when the change affects assets, localization, PCK contents, install state, or runtime display.
- For those changes, think through install and live verification before calling the task done.

## Shell Standard

- Use PowerShell 7 (`pwsh`) as the default shell for this workspace.
- If a Codex shell tool starts in Windows PowerShell, run substantive commands through `pwsh -NoLogo -NoProfile -Command ...`.
- Use Windows PowerShell 5.1 only when a task specifically requires it.

## MCP Usage

Prefer MCP or developer tools over ad-hoc shell workarounds when checking game code, game state, or live behavior.

Common `sts2_moddding` uses:

- `search_game_code`: search game source.
- `get_entity_source`: inspect card, power, relic, or other entity source.
- `install_mod`: install the local mod build.
- `launch_game`: launch the game.
- `bridge_*`: inspect or manipulate the running game state.

Do not use `hot_reload_project` in this workspace. It currently fails with an MCP-side
`UnboundLocalError: cannot access local variable 'asyncio' where it is not associated
with a value`. Use `build_mod` with `build_pck_artifact: true`, then `install_mod`,
then restart the game when a fresh DLL/PCK load is needed.

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
- Agent Teams の task subject / description / acceptance criteria / notes / comments など、タスク本文はユーザーから別指定がない限り日本語で書く。

## Useful Docs

- `docs/roadmap.md`: current status, near-term priorities, and next entry points.
- `docs/agent-knowledge.md`: MCP operations, verification rules, and lessons from previous debugging.
- `docs/README.md`: documentation index by topic.
- `docs/workflows/build-install-workflow.md`: build, PCK rebuild, and install workflow.
- `docs/workflows/install-gate-checklist.md`: short install verification gate.
- `docs/status/card-pool-package-map.md`: current card pool package map.
- `docs/design/bloodshed-design-note.md`: Bloodshed design notes.
- `docs/status/combat-animation-progress.md`: combat animation progress.
- `docs/visuals/flandre-spine-visual-entry.md`: Spine migration entry point.

## Team Task Management

The durable task tracker for this repository is the Agent Teams `koumakan` board.

### Workflow

- For assigned work, read the full board task context first with `task_get`.
- Start only when ready to actively work, using `task_start`.
- Keep blockers, progress, verification notes, and handoff context in `task_add_comment`.
- When work is complete, add a final results comment before `task_complete`.
- For visible replies to leads, teammates, or users during Agent Teams handoffs, use the exposed Agent Teams `message_send` MCP tool as requested by the handoff.
- Do not use markdown TODO files as the shared task tracker.

### Beads Migration Boundary

- Do not use `bd` or Beads for new task tracking.
- Beads data may be read only for migration archive or historical lookup when explicitly needed.
- Do not write, close, delete, or sync Beads data unless a board task or lead/user instruction explicitly authorizes it.
- Do not delete `.beads/**`, `.agents/skills/beads/**`, or remote `refs/dolt/data` as part of ordinary docs or implementation work.

### Session Completion

- Run quality gates appropriate to the files changed.
- Commit only files that belong to the current board task.
- Push completed work to the remote when the task requires repository changes.
- Record gates, commit/push status, residual risk, and follow-up work in the board task before closing it.
