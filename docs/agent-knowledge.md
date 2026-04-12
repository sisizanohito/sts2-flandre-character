# Agent Knowledge

## Purpose

This file captures the current working rules for the Flandre mod project so the team can start from the same assumptions every time.

Read this before starting a new task, after role changes, and after major debugging sessions.

## Team Roles

- `レミリア / Halley`
  - Coordination support and bug triage
  - Break down requests, assign work, and separate observed facts from guesses
- `咲夜 / Gibbs`
  - Implementation
  - Own code change proposals and implementation-focused edits
- `パチュリー / Hegel`
  - API and internal design review
  - Inspect recovered source, BaseLib, Harmony, and STS2 structure
- `フランドール / Faraday`
  - Test design
  - Define verification points and only run `sts2_moddding` calls when explicitly asked
- `小悪魔 / Sagan`
  - Docs and reusable knowledge
  - Write down workflows, pitfalls, known bugs, and verification steps
- `美鈴 / Dirac`
  - Git and release hygiene
  - Separate commit scope, inspect secrets, generated files, and game-origin assets

## Operating Rules

- Always announce the active members for a task before substantial work starts.
- Prefer direct MCP / developer tools over ad-hoc workarounds.
- When a specific MCP tool is requested, call that tool first.
- Do not fall back to shell, HTTP, or JSON-RPC hand-rolled requests unless explicitly allowed.
- Separate confirmed facts, inferred causes, and proposed fixes.
- Before committing, separate:
  - changes that belong to the current issue
  - unrelated local changes that must stay out of the commit

## MCP Rules

- `sts2_moddding` is usable from the main thread and from most sub-agents.
- If a sub-agent is suspected to be failing because of context pollution, re-create the agent instead of arguing with the old thread.
- For STS2 work, verify prerequisites in this order:
  1. `mcp__sts2_moddding__get_setup_status`
  2. `mcp__sts2_moddding__bridge_ping`
  3. task-specific state calls such as `bridge_get_full_state`

## Flandre Test Rule

- Flandre is a test-design specialist, not the default game operator.
- Flandre may use `sts2_moddding` only when explicitly instructed to do so.
- Flandre must not use shell, HTTP, or manual JSON-RPC as a substitute for named MCP tools.
- Required response pattern for tool checks:
  1. tool name
  2. success or failure
  3. returned values or exact error
  4. conclusion

## Tooltip Debugging Lessons

Problem sequence:

1. `EchoLinkCard` tooltip for `[Destruction Eye]` did not appear
2. Tooltip appeared but rendered raw localization keys

Confirmed fixes:

- `EchoLinkCard` now exposes the custom keyword through `CanonicalKeywords`
- `CardHoverTipDuringTargetingPatch` keeps hover tips visible during targeting card flows
- `DestructionEye` custom keyword exists in code and in localization

Root cause of raw-key display:

- The installed `flandremod.pck` was stale and did not include:
  - `flandremod/localization/eng/card_keywords.json`
  - `flandremod/localization/jpn/card_keywords.json`

Verification pattern:

- Check installed PCK contents
- Check game log for:
  - `Found loc table from mod: eng card_keywords.json`
  - `Found loc table from mod: jpn card_keywords.json`
- Check rendered hover-tip title and description in the live scene

## Build And Install Rules

- Build managed code first
- Rebuild the PCK with `base_prefix = flandremod/`
- Use [Install-FlandreMod.ps1](C:/Users/isiis/Documents/flandre-mod/tools/Install-FlandreMod.ps1) to copy the exact local PCK into the Steam mod folder

Do not trust a generic install path when localization or packed assets are involved.

## Commit Hygiene

- Commit only the files that belong to the current fix
- Keep unrelated local edits unstaged
- If a debugging helper was only needed during investigation, remove it before commit unless it remains useful in normal operation

## When To Update This File

Update this document after:

- role or authority changes
- major debugging discoveries
- workflow changes
- new recurring pitfalls
