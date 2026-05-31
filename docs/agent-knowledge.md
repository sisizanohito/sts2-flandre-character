# Agent Knowledge

## Purpose

This file captures the current working rules for the Flandre mod project so agents can start from the same assumptions every time.

Read this before starting a new task, after workflow changes, and after major debugging sessions.

## Task Management

- The durable task tracker is the Agent Teams `koumakan` board.
- For assigned work, check the full task context first, start the task only when actively beginning, and use task comments for progress, blockers, verification notes, residual risk, and handoff context.
- Agent Teams task subject / description / acceptance criteria / notes / comments should be written in Japanese unless the user requests otherwise.
- When a handoff or user message names a specific Agent Teams MCP tool, call that tool first. Do not replace it with shell, HTTP, or hand-rolled JSON-RPC unless explicitly allowed.
- Do not use Beads for new task tracking. Treat Beads as read-only migration archive or historical lookup unless a board task or lead/user instruction explicitly authorizes Beads writes, closes, deletes, or sync operations.
- When using old Beads data as historical context, record both the old Beads ID and the current Agent Teams task ID in the relevant task comment, and separate confirmed facts from inference.

## Work Areas

- Project ownership:
  - final decisions, priority, and start/stop calls
- Runtime verification / game operation:
  - use `sts2_moddding` for game state checks, combat verification, and reproduction checks
- Implementation:
  - code changes, feature additions, and concrete diffs
- API / internal design review:
  - inspect recovered source, BaseLib, Harmony, and STS2 API risks before implementation
- Docs / knowledge maintenance:
  - document findings, operating rules, verification steps, and recurring pitfalls
- Git / release hygiene:
  - keep commit scope clear, run pre-PR checks, and verify game assets before publishing

## Sub-Agent Model Default

- Sub-agents should use `gpt-5.4` by default in this workspace.
- Do not use `gpt-5.4-mini` for strict MCP execution tasks.
- This especially applies to any sub-agent expected to call `sts2_moddding` without fallback behavior.
- Reason:
  - `gpt-5.4` proved more reliable for strict MCP execution and reduced ambiguous fallback behavior in sub-agent checks.

## Operating Rules

- Before substantial work, state the concrete work areas involved when it affects verification or handoff.
- Use PowerShell 7 (`pwsh`) as the default shell. If Codex starts in Windows PowerShell, wrap substantive commands with `pwsh -NoLogo -NoProfile -Command ...`.
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
- New spawn-created sub-agents may still fail to execute `sts2_moddding` tools even when older existing agents can execute them.
- A Codex restart by itself did not restore `sts2_moddding` execution for newly spawned agents in this workspace.
- In this workspace, `gpt-5.4-mini` proved unreliable for strict MCP execution tasks. Prefer `gpt-5.4` for sub-agents that need to call `sts2_moddding` or follow strict no-fallback rules.
- After standardizing sub-agent model selection on `gpt-5.4`, all checked sub-agents successfully executed `sts2_moddding` tools.
- When checking sub-agent capability, distinguish between:
  - tool visibility
  - actual tool execution
  - shell or network fallbacks that are not the requested MCP path
- For strict MCP checks, judge success from `tool + raw result`.
- For STS2 work, verify prerequisites in this order:
  1. `mcp__sts2_moddding__get_setup_status`
  2. `mcp__sts2_moddding__bridge_ping`
  3. task-specific state calls such as `bridge_get_full_state`
- As of 2026-05-31, GDRE-backed resource tools can fail before execution with
  `UnboundLocalError: cannot access local variable 'asyncio' where it is not associated with a value`.
  Confirmed affected tools in this session: `recover_game_project` and `list_game_assets`.
  The current workaround was to call the same `sts2mcp.gdre_tools.recover_game_project`
  backend directly and recover assets to `C:/mcp/sts2-modding-mcp/recovered`.

## Runtime Verification Rule

- Runtime verification agents may use `sts2_moddding` only when explicitly instructed to do so.
- When testing tool access, do not accept a generic `connected` answer without the tool name and result payload.
- If an agent is told to use `sts2_moddding`, explicitly forbid PowerShell or other substitute paths in the instruction.
- During any `sts2_moddding` verification, shell substitution is prohibited.
- Judge success or failure from the named MCP tool and its raw returned payload, not from summaries.
- Ask the agent to check visible tools first when capability is unclear, but confirm execution through an actual `sts2_moddding` call.
- Runtime verification agents must not use shell, HTTP, or manual JSON-RPC as a substitute for named MCP tools.
- Required response pattern for tool checks:
  1. tool name
  2. success or failure
  3. returned values or exact error
  4. conclusion

## Oversight Notes

- 判断や引き継ぎが必要な場面では、短い checkpoint note を残す
- 形式は `owner / action / reason / next_step`
- 文面は短く、判断と次の作業だけが分かる運用メモにする
- hook ベースの監視運用は現行方針では採用しない
- 監督と越権確認は heartbeat ベースのチェックで扱う
- 旧来の hook 中心のメモやテンプレートは参考資料としてのみ扱い、現行ルールとしては使わない

## Sub-Agent Tooling Lessons

Observed sequence:

1. Older long-lived agents such as `Gibbs` and `Dirac` could execute `mcp__sts2_moddding__bridge_ping`
2. Multiple newly spawned agents could see or describe the tool but failed to execute it
3. One reset verification sub-agent answered only `connected`, but later confirmed that was from `Test-NetConnection`, not `sts2_moddding`
4. A reset verification sub-agent succeeded only after being told to use `sts2_moddding` and to report both the tool name and the raw result
5. After standardizing current sub-agents on `gpt-5.4`, all checked sub-agents were able to execute `sts2_moddding`

Working guidance:

- For sub-agent MCP checks, require:
  - the exact tool name
  - the raw returned payload
  - no substitute commands
- Treat `visible but not executable` as a distinct failure mode
- If a sub-agent starts giving vague answers, reset it and retest with a minimal instruction
- Do not assume a new spawned agent inherits the same MCP execution surface as an older surviving one
- If a sub-agent must execute MCP tools reliably, use `gpt-5.4` instead of `gpt-5.4-mini`

## Tooltip Debugging Lessons

Problem sequence:

1. `EchoLinkCard` tooltip for `[Destruction Eye]` did not appear
2. Tooltip appeared but rendered raw localization keys

Confirmed fixes:

- `EchoLinkCard` now exposes the custom keyword through `CanonicalKeywords`
- `CardHoverTipDuringTargetingPatch` keeps hover tips visible during targeting card flows
- `DestructionEye` custom keyword exists in code and in localization
- every currently shipped card that refers to Destruction Eye now exposes `DestructionEye.CustomType` through `CanonicalKeywords`
  - confirmed mainline cards: `EchoLinkCard`, `SparkScatterCard`, `MadGazeCard`, `CrackedSmileCard`, `RendingClawCard`, `ProliferatingGazeCard`, `CruelBlinkCard`

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

Current debugging default:

- for already shipped Destruction Eye cards on `main`, treat missing tooltip text as an install / localization packaging suspicion first, not as a new `CanonicalKeywords` omission
- do not put the two-word keyword in card body text as `[Destruction Eye]`; the card description rich-text pass can parse it as an unclosed `Destruction` BBCode tag
- only reopen the card-level keyword path when a newly added card introduces Destruction Eye text without joining the shared exposure pattern

## Testplay Stop Lessons

- Do not use `hot_reload_project` for this workspace; it currently fails inside the MCP server and can leave the live game in a poisoned type/cache state. Rebuild, install, and restart instead.
- For live bridge testing, MCPTest may need local updates after a STS2 game update. On 2026-05-31 the bridge needed:
  - `BeforePlayPhaseStart` hook remapped to an existing play-phase hook
  - `CombatManager.IsPlayPhase` replaced with `ActionQueueSynchronizer.CombatState`
  - `PowerCmd.Apply` calls updated for the required `PlayerChoiceContext`
  - `MerchantRoom.Inventory` updated for the current inventory API
- Flandre's BaseLib dependency must be installed as its own mod and declared in `mod_manifest.json`; a copied `BaseLib.dll` beside `FlandreMod.dll` is not enough for reliable game loading.
- Keep `GodotSharp` package references aligned with the current game/BaseLib runtime. As of the current STS2 install and BaseLib 3.1.0, use `GodotSharp` 4.5.1, not 4.4.0.
- BaseLib 3.1.0 currently logs `Applied 150 patches successfully, 3 failed` on STS2 startup. The failed patches are
  `ExhaustivePatch`, `PersistPatch`, and `PurgePatch`, all targeting the old
  `CardModel.GetResultPileType` method. The current installed `sts2.dll` uses
  `GetResultPileTypeForCardPlay` instead. Flandre's current cards do not use
  `Exhaustive`, `Persist`, `Purge`, `BaseLibKeywords.Purge`, or `CardKeyword.Exhaust`, so this is not an active Flandre card behavior bug as of task `24bf4d4a`. If a future Flandre card needs these BaseLib mechanics, update BaseLib or add a local result-pile implementation against the current `Hook.ModifyCardPlayResultPileTypeAndPosition` / `GetResultPileTypeForCardPlay` path before relying on them.
- If testplay stops on Flandre startup, validate localization JSON first. A malformed locale file can surface as an in-game generic popup rather than a clear mod-load error.
- Flandre raw PNG assets packed in the PCK need raw-buffer loading before `ResourceLoader.Load`; otherwise Godot logs `No loader found for resource` even when the PNG exists.
- Flandre mod-local Power icons should use `res://flandremod/images/powers/...`, matching the actual PCK layout.
- Avoid `vfx/vfx_explosion` until a valid asset path is confirmed. `vfx/vfx_bloody_impact` is confirmed to resolve in live combat.

## Random Reflection Lessons

- Implementation:
  - `RandomReflectionCard` was added as a random-generation card that creates or routes into Reflection-related outcomes during live play verification.
- Duplicate ID warning:
  - The startup warning `Two AbstractModels FlandreMod.Cards.RandomReflectionCard and FlandreMod.Cards.RandomReflectionCard from mod flandremod share an ID` came from the game's `ModelIdSerializationCache` sort comparer, not from a second card source file or duplicate pool registration.
  - `RandomReflectionCard` had one source definition, one compiled `AbstractModel` type, and one installed mod manifest; the warning disappeared after guarding the comparer path so the same `Type` instance is not treated as a duplicate model.
- Verification blocker:
  - Missing or broken pool-name localization blocked the intended verification path because the generated choice flow could not be read cleanly in-game.
- Localization repair:
  - `jpn` localization needed repair so the card and its related pool naming would render correctly during verification.
- Live verification result:
  - After the localization repair, the `RandomReflectionCard` flow was confirmed in the running game and the verification path became readable again.

## Build And Install Rules

- Build managed code first
- Rebuild the PCK from a staging directory that contains both `flandremod/` and root `images/` folders, with no extra base prefix from the staging root.
- Use [Install-FlandreMod.ps1](../tools/Install-FlandreMod.ps1) to copy the exact local PCK into the Steam mod folder
- Treat [install-gate-checklist.md](./workflows/install-gate-checklist.md) as the short completion gate before deeper runtime debugging
- Use [build-install-workflow.md](./workflows/build-install-workflow.md) when the stale PCK suspicion needs the longer explanation or extra path/log checks
- If a build tool reports `source_dir` as only the repo `flandremod/` folder, do not treat that PCK as sufficient for runtime checks involving Flandre energy icons; it will omit `res://images/atlases/ui_atlas.sprites/card/energy_flandrecharacter.tres`.
- Startup log error `Detected old-style dependencies without min version specified` means `mod_manifest.json` still uses string dependencies. Use object dependencies with `id` and `min_version`.
- If the startup log says `Tried to load mod with id BaseLib, but a mod is already loaded with that name`, check for duplicate BaseLib folders in the game `mods` directory. The Flandre installer should sync BaseLib into the canonical `BaseLib` folder and remove old `BaseLib.*` installs that declare `id: BaseLib`.

Do not trust a generic install path when localization or packed assets are involved.

## Character Identity Debugging

- If selecting Flandre appears to start Ironclad, verify the live run identity through `bridge_get_full_state` before assuming it is only a UI texture issue.
- A clean Flandre run should report `character: FlandreCharacter`, `hp: 75`, starter cards `FlandreStrikeCard` / `FlandreDefendCard` / `EchoLinkCard`, and starter relic `DestructionEyeRelic`.
- If the current run reports `character: Ironclad` with `StrikeIronclad`, `DefendIronclad`, `Bash`, and `BurningBlood`, the run is really Ironclad.
- A failed hot reload while the game is running can leave the session in a poisoned state even when installed files are correct. In that case, stop the game, rebuild, reinstall, relaunch, and retest with `run_test_scenario` or `bridge_get_full_state`.

## Energy Icon Debugging

- Flandre's energy prefix must be lower-case `flandrecharacter`; `EnergyIconsFormatter` does not lower-case the prefix when building `res://images/packed/sprite_fonts/{prefix}_energy_icon.png`.
- Godot rich text `[img]` did not render a mod-local raw PNG path from `flandremod/Characters/FlandreCharacter/ui/`, even though the string resolved to that path.
- For Flandre text energy icons, the card energy `.tres` path can render through the atlas loader patch, but it must be emitted as `[img=24x24]...[/img]`; plain `[img]...[/img]` renders the 64px card icon too large in body text.
- Visual check: start a Flandre run with seed `FLANDREENERGY02`; Neow's Booming Conch option should show an energy icon after `gain`.

## Neow Startup Debugging

- If Flandre starts at Neow but blessing choices are missing, check the game log for asset-load exceptions from `Neow.GenerateInitialOptions()` before changing Neow option generation.
- A missing `res://images/atlases/ui_atlas.sprites/card/energy_flandrecharacter.tres` means the installed PCK omitted the root `images/` folder; rebuild from staged `flandremod/` plus `images/`.
- Flandre also needs a Neow dialogue entry keyed by the exact model id entry `FLANDREMOD-FLANDRE_CHARACTER`; see [neow-startup-debugging.md](./status/neow-startup-debugging.md).

## Commit Hygiene

- Commit only the files that belong to the current fix
- Keep unrelated local edits unstaged
- If a debugging helper was only needed during investigation, remove it before commit unless it remains useful in normal operation

## When To Update This File

Update this document after:

- workflow or responsibility changes
- major debugging discoveries
- workflow changes
- new recurring pitfalls
