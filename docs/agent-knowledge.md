# Agent Knowledge

## Purpose

This file captures the current working rules for the Flandre mod project so the team can start from the same assumptions every time.

Read this before starting a new task, after role changes, and after major debugging sessions.

## Team Roles

- `レミリア`
  - 総括オーナー
  - 最終判断、優先順位の確定、作業の開始・停止判断を行う
- `フランドール`
  - 実機検証 / ゲーム操作
  - `sts2_moddding` を使ったゲーム状態確認、戦闘検証、再現確認を担当する
- `咲夜`
  - 実装担当
  - コード修正、機能追加、差分の具体化を担当する
- `パチュリー`
  - API / 内部設計レビュー
  - recovered source、BaseLib、Harmony、STS2 API の観点から設計上の危険筋を先に洗い出す
- `小悪魔`
  - docs / ナレッジ整理
  - 調査結果、運用ルール、検証手順、再発防止事項を文書化する
- `美鈴`
  - Git / 公開導線管理
  - commit 範囲の整理、PR 前確認、ゲーム用 assets を含む公開前チェックを担当する

## Active Seat Default

- Sub-agents should use `gpt-5.4` by default in this workspace.
- Do not use `gpt-5.4-mini` for strict MCP execution tasks.
- This especially applies to any sub-agent expected to call `sts2_moddding` without fallback behavior.
- Reason:
  - `gpt-5.4` proved more reliable for strict MCP execution and reduced ambiguous fallback behavior in sub-agent checks.

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
- New spawn-created sub-agents may still fail to execute `sts2_moddding` tools even when older existing agents can execute them.
- A Codex restart by itself did not restore `sts2_moddding` execution for newly spawned agents in this workspace.
- In this workspace, `gpt-5.4-mini` proved unreliable for strict MCP execution tasks. Prefer `gpt-5.4` for sub-agents that need to call `sts2_moddding` or follow strict no-fallback rules.
- In the current seat lineup standardized on `gpt-5.4`, all active members successfully executed `sts2_moddding` tools.
- When checking a sub-agent seat, distinguish between:
  - tool visibility
  - actual tool execution
  - shell or network fallbacks that are not the requested MCP path
- For strict MCP checks, judge success from `tool + raw result`.
- For STS2 work, verify prerequisites in this order:
  1. `mcp__sts2_moddding__get_setup_status`
  2. `mcp__sts2_moddding__bridge_ping`
  3. task-specific state calls such as `bridge_get_full_state`

## Flandre Test Rule

- Flandre is the intended game-operation and test specialist when the seat can execute `sts2_moddding`.
- Flandre may use `sts2_moddding` only when explicitly instructed to do so.
- When testing tool access, do not accept a generic `connected` answer without the tool name and result payload.
- If Flandre is told to use `sts2_moddding`, explicitly forbid PowerShell or other substitute paths in the instruction.
- During any `sts2_moddding` verification, shell substitution is prohibited.
- Judge success or failure from the named MCP tool and its raw returned payload, not from summaries.
- Ask Flandre to check visible tools first when the seat behavior is unclear, but confirm execution through an actual `sts2_moddding` call.
- Flandre must not use shell, HTTP, or manual JSON-RPC as a substitute for named MCP tools.
- Required response pattern for tool checks:
  1. tool name
  2. success or failure
  3. returned values or exact error
  4. conclusion

## Oversight Check Pattern

- 作業開始前、実装前、docs 更新前、検証前、commit 前に `レミリア check` を入れる
- 形式は固定で `owner / remilia_action / reason / next_actor`
- 文面は短く、判断と次の担当だけが分かる運用メモにする
- hook ベースの監視運用は現行方針では採用しない
- 監督と越権確認は heartbeat ベースのチェックで扱う
- 旧来の hook 中心のメモやテンプレートは参考資料としてのみ扱い、現行ルールとしては使わない

## Sub-Agent Tooling Lessons

Observed sequence:

1. Older long-lived agents such as `Gibbs` and `Dirac` could execute `mcp__sts2_moddding__bridge_ping`
2. Multiple newly spawned agents could see or describe the tool but failed to execute it
3. One reset Flandre seat answered only `connected`, but later confirmed that was from `Test-NetConnection`, not `sts2_moddding`
4. A reset Flandre seat succeeded only after being told to use `sts2_moddding` and to report both the tool name and the raw result
5. After standardizing current seats on `gpt-5.4`, all active members were able to execute `sts2_moddding`

Working guidance:

- For sub-agent MCP checks, require:
  - the exact tool name
  - the raw returned payload
  - no substitute commands
- Treat `visible but not executable` as a distinct failure mode
- If a seat starts giving vague answers, reset the seat and retest with a minimal instruction
- Do not assume a new spawned agent inherits the same MCP execution surface as an older surviving one
- If a seat must execute MCP tools reliably, use a `gpt-5.4` sub-agent instead of `gpt-5.4-mini`

## Tooltip Debugging Lessons

Problem sequence:

1. `EchoLinkCard` tooltip for `[Destruction Eye]` did not appear
2. Tooltip appeared but rendered raw localization keys

Confirmed fixes:

- `EchoLinkCard` now exposes the custom keyword through `CanonicalKeywords`
- `CardHoverTipDuringTargetingPatch` keeps hover tips visible during targeting card flows
- `DestructionEye` custom keyword exists in code and in localization
- every currently shipped card whose localization text includes `[Destruction Eye]` now exposes `DestructionEye.CustomType` through `CanonicalKeywords`
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

- for already shipped `[Destruction Eye]` cards on `main`, treat missing tooltip text as an install / localization packaging suspicion first, not as a new `CanonicalKeywords` omission
- only reopen the card-level keyword path when a newly added card introduces `[Destruction Eye]` text without joining the shared exposure pattern

## Random Reflection Lessons

- Implementation:
  - `RandomReflectionCard` was added as a random-generation card that creates or routes into Reflection-related outcomes during live play verification.
- Verification blocker:
  - Missing or broken pool-name localization blocked the intended verification path because the generated choice flow could not be read cleanly in-game.
- Localization repair:
  - `jpn` localization needed repair so the card and its related pool naming would render correctly during verification.
- Live verification result:
  - After the localization repair, the `RandomReflectionCard` flow was confirmed in the running game and the verification path became readable again.

## Build And Install Rules

- Build managed code first
- Rebuild the PCK with `base_prefix = flandremod/`
- Use [Install-FlandreMod.ps1](../tools/Install-FlandreMod.ps1) to copy the exact local PCK into the Steam mod folder
- Treat [install-gate-checklist.md](./install-gate-checklist.md) as the short completion gate before deeper runtime debugging
- Use [build-install-workflow.md](./build-install-workflow.md) when the stale PCK suspicion needs the longer explanation or extra path/log checks

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
