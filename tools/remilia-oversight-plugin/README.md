# Remilia Oversight Hook Template

This directory is a local plugin-style template for Codex hook experiments.

## Files

- `.codex-plugin/plugin.json`
- `hooks.json`
- `scripts/check_remilia_oversight.ps1`

## Suggested placement

Copy this directory to a local Codex plugin path such as:

- `%CODEX_HOME%\\plugins\\local\\remilia-oversight\\`

`CODEX_HOME` is the local Codex home directory on the current machine.

Keep the relative layout intact so `hooks.json` can call `.\scripts\check_remilia_oversight.ps1`.

## Hook intent

The included `hooks.json` uses this structure:

- root `hooks`
- inside that, `PostToolUse`
- each rule has `matcher`
- each rule has `hooks`
- each hook entry is `{ "type": "command", "command": ... }`

It watches these risky tool categories with `PostToolUse` rules:

- `shell_command`
- `apply_patch`
- GitHub write actions via `mcp__codex_apps__github_*`

The PowerShell checker emits a 4-line oversight result in this format:

- `owner: ...`
- `remilia_action: allowed / not_allowed`
- `reason: ...`
- `next_actor: ...`

Example output:

- `owner: Remilia`
- `remilia_action: not_allowed`
- `reason: Shell execution can bypass seat boundaries. Confirm remit before continuing. :: shell_command`
- `next_actor: Gibbs`
