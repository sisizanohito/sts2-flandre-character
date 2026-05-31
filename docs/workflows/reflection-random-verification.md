# Reflection And Random Verification

## Purpose

Use this workflow when a task touches `RandomReflectionCard`, generated card
choices, Flandre card-pool display, or `Madness` target routing for random
attacks.

This workflow extends [install-gate-checklist.md](./install-gate-checklist.md).
It does not replace the build and install gate when code, localization, assets,
or packed files changed.

## Confirmed Surface

- `RandomReflectionCard` is defined in `Code/Cards/RandomReflectionCard.cs`.
- It is registered with `[Pool(typeof(FlandreCharacterCardPool))]`.
- It is currently a common, 1-cost attack with `TargetType.RandomEnemy`.
- It deals 3 damage 3 times.
- While `MadnessPower.IsActiveFor(...)` is true, the card targets all enemies
  for each hit instead of random enemies.
- `MadnessTargetTypePatch` also changes attack target display and
  `AttackCommand` targeting while `Madness` is active.

## Static Localization Gate

Before runtime testing, confirm the localization files parse and contain the
keys that make the card, keyword hover tip, and pool name readable.

Required files:

- `flandremod/localization/eng/cards.json`
- `flandremod/localization/jpn/cards.json`
- `flandremod/localization/eng/powers.json`
- `flandremod/localization/jpn/powers.json`
- `flandremod/localization/eng/characters.json`
- `flandremod/localization/jpn/characters.json`

Required card keys in both `cards.json` files:

- `RANDOM_REFLECTION_CARD.title`
- `RANDOM_REFLECTION_CARD.description`
- `RANDOM_REFLECTION_CARD.upgrade.description`
- `FLANDREMOD-RANDOM_REFLECTION_CARD.title`
- `FLANDREMOD-RANDOM_REFLECTION_CARD.description`
- `FLANDREMOD-RANDOM_REFLECTION_CARD.upgrade.description`

Required power keys in both `powers.json` files:

- `MADNESS_POWER.title`
- `MADNESS_POWER.description`
- `FLANDREMOD-MADNESS_POWER.title`
- `FLANDREMOD-MADNESS_POWER.description`

Required pool key in both `characters.json` files:

- `CARD_POOL.FLANDREMOD-FLANDRE_CHARACTER_CARD_POOL`

Reusable PowerShell check:

```powershell
$checks = @(
  @{
    Path = 'flandremod/localization/eng/cards.json'
    Keys = @(
      'RANDOM_REFLECTION_CARD.title',
      'RANDOM_REFLECTION_CARD.description',
      'RANDOM_REFLECTION_CARD.upgrade.description',
      'FLANDREMOD-RANDOM_REFLECTION_CARD.title',
      'FLANDREMOD-RANDOM_REFLECTION_CARD.description',
      'FLANDREMOD-RANDOM_REFLECTION_CARD.upgrade.description'
    )
  },
  @{
    Path = 'flandremod/localization/jpn/cards.json'
    Keys = @(
      'RANDOM_REFLECTION_CARD.title',
      'RANDOM_REFLECTION_CARD.description',
      'RANDOM_REFLECTION_CARD.upgrade.description',
      'FLANDREMOD-RANDOM_REFLECTION_CARD.title',
      'FLANDREMOD-RANDOM_REFLECTION_CARD.description',
      'FLANDREMOD-RANDOM_REFLECTION_CARD.upgrade.description'
    )
  },
  @{
    Path = 'flandremod/localization/eng/powers.json'
    Keys = @(
      'MADNESS_POWER.title',
      'MADNESS_POWER.description',
      'FLANDREMOD-MADNESS_POWER.title',
      'FLANDREMOD-MADNESS_POWER.description'
    )
  },
  @{
    Path = 'flandremod/localization/jpn/powers.json'
    Keys = @(
      'MADNESS_POWER.title',
      'MADNESS_POWER.description',
      'FLANDREMOD-MADNESS_POWER.title',
      'FLANDREMOD-MADNESS_POWER.description'
    )
  },
  @{
    Path = 'flandremod/localization/eng/characters.json'
    Keys = @('CARD_POOL.FLANDREMOD-FLANDRE_CHARACTER_CARD_POOL')
  },
  @{
    Path = 'flandremod/localization/jpn/characters.json'
    Keys = @('CARD_POOL.FLANDREMOD-FLANDRE_CHARACTER_CARD_POOL')
  }
)

foreach ($check in $checks) {
  $json = Get-Content -Raw -LiteralPath $check.Path | ConvertFrom-Json -AsHashtable
  foreach ($key in $check.Keys) {
    if (-not $json.ContainsKey($key)) {
      throw "Missing key '$key' in '$($check.Path)'"
    }
  }
}
```

## Packed Localization Gate

If code or localization changed, run the install gate, then confirm the installed
PCK contains these paths before debugging card logic:

- `flandremod/localization/eng/cards.json`
- `flandremod/localization/jpn/cards.json`
- `flandremod/localization/eng/powers.json`
- `flandremod/localization/jpn/powers.json`
- `flandremod/localization/eng/characters.json`
- `flandremod/localization/jpn/characters.json`

When using game logs, record the matching `Found loc table from mod:` lines for
the same files. If a card title, generated choice, or pool label renders as a
raw key, classify it as localization or install packaging first.

## Runtime Bridge Path

Use `sts2_moddding` bridge tools for strict runtime checks. Record each tool
name, success or failure, raw returned values or exact error, and conclusion.

1. Confirm setup and reach combat.
   - `mcp__sts2_moddding__get_setup_status`
   - `mcp__sts2_moddding__bridge_ping`
   - `mcp__sts2_moddding__bridge_get_full_state`
   - Start or resume a Flandre run and confirm `character: FlandreCharacter`.
   - Use `bridge_navigate_to_combat` if the run is not already in combat.
2. Create the no-`Madness` baseline.
   - Add `FLANDREMOD-RANDOM_REFLECTION_CARD` to hand with
     `bridge_manipulate_state`.
   - Call `bridge_get_combat_state`.
   - Confirm the hand entry is readable and behaves as `RandomEnemy`.
   - Play it with `bridge_act_and_wait` or `bridge_play_card`.
   - Confirm it resolves as 3 random hits and no raw card or pool keys appear.
3. Create the `Madness` comparison.
   - Add `FLANDREMOD-DOKAAN_CARD` and
     `FLANDREMOD-RANDOM_REFLECTION_CARD` to hand with
     `bridge_manipulate_state`.
   - Add `FLANDREMOD-MADNESS_POWER` with 2 stacks to the player.
   - Call `bridge_get_combat_state`.
   - Confirm `DokaanCard` and `RandomReflectionCard` display as `AllEnemies`.
   - Play `DokaanCard`, then `RandomReflectionCard`.
   - Confirm both can be played without manual target selection.
   - Confirm `MadnessPower` is consumed in order: `2 -> 1 -> removed`.
4. Stop on generated choice or reward screens when relevant.
   - Use `bridge_auto_proceed` with card and reward skipping disabled when a
     task specifically touches generated card choices or pool display.
   - Confirm the displayed card title, card description, and pool label are
     readable display text.
   - If any displayed value is a raw key, return to the static and packed
     localization gates before changing card behavior.

## Result Template

```text
Reflection/random verification:
- code/localization changed:
- static localization gate:
- install gate:
- packed localization files:
- setup/bridge tools:
- no-Madness RandomReflection result:
- Madness Dokaan -> RandomReflection result:
- generated choice / pool label result:
- residual risk:
```
