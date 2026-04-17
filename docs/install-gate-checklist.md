# Install Gate Checklist

## Purpose

This checklist is the operational gate for treating a local build/install cycle as complete without reintroducing the stale PCK problem.

Use [build-install-workflow.md](./build-install-workflow.md) for the longer explanation. Use this file when you need a short repeatable check before moving on to runtime debugging.

## Scope

This gate is limited to:

- managed build
- local `flandremod.pck` rebuild
- install into the Steam mods folder
- install-complete judgment

This gate does not cover:

- animation or graphics work
- live combat verification
- general gameplay validation

## Completion Conditions

Treat the install as complete only when all of the following are true:

- the managed build succeeded
- the local `flandremod.pck` was rebuilt with `base_prefix = flandremod/`
- `.\tools\Install-FlandreMod.ps1 -ProjectRoot . -Configuration Debug -StopGame` completed
- the install script self-check summary confirms the local and installed `flandremod.pck` matched

If any item is missing, stop and treat the install as incomplete.

## Standard Procedure

1. Run the managed build.

```powershell
dotnet build .\FlandreMod.csproj -c Debug
```

2. Rebuild the local PCK from `flandremod/` with the correct prefix.

```text
source_dir = %REPO_ROOT%\flandremod
output_path = %REPO_ROOT%\flandremod.pck
base_prefix = flandremod/
convert_pngs = true
```

3. Install the exact local artifacts into the Steam mods folder.

```powershell
.\tools\Install-FlandreMod.ps1 -ProjectRoot . -Configuration Debug -StopGame
```

4. Read the install script self-check summary and confirm:

- it inspected the local `flandremod.pck`
- it copied that exact file into the target mod folder
- the local and installed hashes matched

## Escalate To Extra Checks Only When Needed

Run the extra checks below only when one of these is true:

- hover tips appear but text renders as raw keys
- localization looks broken while code behavior seems otherwise unchanged
- post-install behavior does not match the repo diff you expect

## Minimal Extra Checks

If extra verification is needed, confirm these PCK paths exist:

- `flandremod/localization/eng/card_keywords.json`
- `flandremod/localization/jpn/card_keywords.json`

Then confirm the game log contains:

- `Found loc table from mod: eng card_keywords.json`
- `Found loc table from mod: jpn card_keywords.json`

If either path or either log line is missing, treat the problem as an install/localization packaging issue before debugging card code.

## Post-Install Display Check

After the install gate passes, use this short runtime check before deeper debugging:

1. Hover tip
   - open a card or state that should show a hover tip
   - confirm the title and description render as real text, not raw localization keys
2. Keyword
   - confirm the expected keyword label appears in card text or tooltip
   - confirm the keyword explanation opens with readable text
3. Pool name
   - confirm any pool or generated-choice name renders as a normal display string
   - if a pool-related name renders as a key, treat it as localization loading failure first
4. Combat display
   - confirm combat-facing labels or counts that depend on the current change render without missing text
   - when checking `Madness`-related behavior, use `DokaanCard` and `RandomReflectionCard` as the short verification pair
   - without `Madness`, confirm `DokaanCard` stays `AnyEnemy` and `RandomReflectionCard` stays `RandomEnemy`
   - with `Madness`, confirm both display as `AllEnemies`, can be played without manual target selection, and consume `MadnessPower` in order

If any item fails, classify it in this order:

- raw keys or missing names -> localization / install issue first
- correct text but wrong target count or wrong combat-facing behavior -> card logic or targeting issue

## Prohibitions

- Do not build the PCK with `base_prefix = flandremod`
- Do not treat a generic install step as sufficient evidence
- Do not assume localization failures are card-code bugs before checking the install gate

## Run Summary Template

Record a short summary after each install-sensitive task:

- owner:
- date:
- managed build:
- install self-check:
- extra checks:
- remaining risk:
