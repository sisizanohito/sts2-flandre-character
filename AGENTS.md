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
- Beads の issue title / description / acceptance criteria / notes など、タスク本文はユーザーから別指定がない限り日本語で書く。

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

<!-- BEGIN BEADS INTEGRATION v:1 profile:minimal hash:7510c1e2 -->
## Beads Issue Tracker

This project uses **bd (beads)** for issue tracking. Run `bd prime` to see full workflow context and commands.

### Quick Reference

```bash
bd ready              # Find available work
bd show <id>          # View issue details
bd update <id> --claim  # Claim work
bd close <id>         # Complete work
```

### Rules

- Use `bd` for ALL task tracking — do NOT use TodoWrite, TaskCreate, or markdown TODO lists
- Run `bd prime` for detailed command reference and session close protocol
- Use `bd remember` for persistent knowledge — do NOT use MEMORY.md files

**Architecture in one line:** issues live in a local Dolt DB; sync uses `refs/dolt/data` on your git remote; `.beads/issues.jsonl` is a passive export. See https://github.com/gastownhall/beads/blob/main/docs/SYNC_CONCEPTS.md for details and anti-patterns.

## Session Completion

**When ending a work session**, you MUST complete ALL steps below. Work is NOT complete until `git push` succeeds.

**MANDATORY WORKFLOW:**

1. **File issues for remaining work** - Create issues for anything that needs follow-up
2. **Run quality gates** (if code changed) - Tests, linters, builds
3. **Update issue status** - Close finished work, update in-progress items
4. **PUSH TO REMOTE** - This is MANDATORY:
   ```bash
   git pull --rebase
   git push
   git status  # MUST show "up to date with origin"
   ```
5. **Clean up** - Clear stashes, prune remote branches
6. **Verify** - All changes committed AND pushed
7. **Hand off** - Provide context for next session

**CRITICAL RULES:**
- Work is NOT complete until `git push` succeeds
- NEVER stop before pushing - that leaves work stranded locally
- NEVER say "ready to push when you are" - YOU must push
- If push fails, resolve and retry until it succeeds
<!-- END BEADS INTEGRATION -->

<!-- BEGIN BEADS CODEX SETUP: generated by bd setup codex -->
## Beads Issue Tracker

Use Beads (`bd`) for durable task tracking in repositories that include it. Use the `beads` skill at `.agents/skills/beads/SKILL.md` (project install) or `~/.agents/skills/beads/SKILL.md` (global install) for Beads workflow guidance, then use the `bd` CLI for issue operations.

### Quick Reference

```bash
bd ready                # Find available work
bd show <id>            # View issue details
bd update <id> --claim  # Claim work
bd close <id>           # Complete work
bd prime                # Refresh Beads context
```

### Rules

- Use `bd` for all task tracking; do not create markdown TODO lists.
- Run `bd prime` when Beads context is missing or stale.
- Keep persistent project memory in Beads via `bd remember`; do not create ad hoc memory files.
<!-- END BEADS CODEX SETUP -->
