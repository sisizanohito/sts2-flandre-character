# Card Pool Implementation Status

## Purpose

`docs/design/card-pool-draft.md` previously contained mojibake, so this file remains the confirmed implementation inventory for handoff work. As of the 2026-05-31 docs cleanup, no active mojibake or unrecoverable `UNKNOWN` section was found in `docs/**`.

This document records only confirmed facts from the codebase so the next non-asset task can start from a clean inventory.

Sources:
- `Code/Cards/*.cs`
- `Code/Characters/FlandreCharacter.cs`
- `flandremod/localization/eng/cards.json`

Related readable package summary:
- [card-pool-package-map.md](./card-pool-package-map.md)

## Confirmed Starter Deck

- `FlandreStrikeCard` x4
- `FlandreDefendCard` x4
- `EchoLinkCard` x2
- starter relic: `DestructionEyeRelic`

## Confirmed Non-Basic Card Inventory

### Common

- `MadGazeCard`
  - cost `1`
  - applies `MadnessPower` 1
  - creates one `Destruction Eye` on a random enemy
  - upgrade adds draw 1
- `SweepingBurstCard`
  - cost `1`
  - attack all enemies
- `SparkScatterCard`
  - cost `0`
  - single-target attack
  - deals extra damage to the linked eye if the target has one
- `RendingClawCard`
  - cost `1`
  - single-target attack
  - deals extra damage to the linked eye if the target has one
- `RandomReflectionCard`
  - cost `1`
  - three random hits
  - madness-specific all-enemy behavior is handled by `MadnessTargetTypePatch` plus card logic
- `LaughingEyeCard`
  - cost `1`
  - single-target attack
  - adds bonus damage when any active eye exists
- `CrackedSmileCard`
  - cost `1`
  - gain block
  - adds bonus block when any active eye exists
- `CruelBlinkCard`
  - cost `1`
  - single-target attack
  - creates `Destruction Eye` on the target after dealing damage
- `ClosedWingsCard`
  - cost `1`
  - gain block
  - adds bonus block when `Madness` is active
- `TwistedPlayCard`
  - cost `0`
  - applies `MadnessPower` 1
  - draws if `Madness` was already active
- `BloodScentCard`
  - cost `1`
  - gains block
  - draws when current `Bloodshed` reaches its threshold
- `CrimsonAdvanceCard`
  - cost `0`
  - loses HP
  - draws cards
  - gains Energy when current `Bloodshed` reaches its threshold
- `BloodyCrescentCard`
  - cost `1`
  - single-target attack
  - gains Block when current `Bloodshed` reaches its threshold
- `RedMistCard`
  - cost `1`
  - loses HP
  - gains Block
  - attacks all enemies when current `Bloodshed` reaches its threshold
- `ShatteredGlintCard`
  - cost `1`
  - single-target attack
  - deals extra damage to the linked eye if the target has one
- `UnsteadyStrikeCard`
  - cost `1`
  - single-target attack
  - draws when `Madness` is active
- `RuddyStepCard`
  - cost `1`
  - gains Block
  - draws when current `Bloodshed` reaches its low threshold
- `EchoLinkCard`
  - cost `2`
  - applies `Destruction Eye` to the chosen target
  - upgrade changes the application count from `1` to `2`
  - also anchors the starter deck (x2)

### Uncommon

- `ProliferatingGazeCard`
  - cost `2`
  - applies `Destruction Eye` to all enemies
  - upgrade changes cost from `2` to `1`
- `BloodMakeupCard`
  - cost `1`
  - loses HP
  - gains efficient block
  - heals back a small amount when current `Bloodshed` reaches its threshold
- `BloodPactCard`
  - cost `1`
  - consumes 15 current `Bloodshed`
  - gains Energy and draws cards only when the consume succeeds
- `FrenziedFlightCard`
  - cost `1`
  - applies `MadnessPower` 2
  - draws cards
  - upgrade increases draw by 1
- `ScarletAppetiteCard`
  - cost `1`
  - power
  - whenever Flandre loses HP, gains additional `Bloodshed`
- `SanguineGuardCard`
  - cost `1`
  - power
  - at end of Flandre's turn, gains Block for every 10 current `Bloodshed`
- `SanguineOverflowCard`
  - cost `1`
  - single-target attack
  - adds damage for every N current `Bloodshed` without consuming it
- `EchoingRuptureCard`
  - cost `1`
  - single-target attack
  - creates `Destruction Eye` on a random enemy when an active eye already exists
- `UnhingedShelterCard`
  - cost `1`
  - gains Block
  - applies `MadnessPower` 1

### Rare

- `DokaanCard`
  - cost `3`
  - high single-target damage
  - madness-specific all-enemy behavior is handled by the shared patch
- `VampiricImpulseCard`
  - cost `2`
  - single-target attack
  - heals for half of unblocked damage dealt when current `Bloodshed` reaches its threshold
- `CataclysmicGazeCard`
  - cost `2`
  - damages each active `Destruction Eye`
  - then attacks all enemies
- `ScarletDelugeCard`
  - cost `2`
  - consumes all current `Bloodshed`
  - attacks all enemies with bonus damage for every N `Bloodshed` consumed
- `CrimsonBanquetCard`
  - cost `2`
  - consumes 25 current `Bloodshed`
  - single-target attack plus HP recovery only when the consume succeeds

## Stable Shared Base

- `DestructionEyeCardHelper` already owns eye creation, eye reinforcement, and initial eye HP setup
- madness target-shift behavior has already been confirmed for `DokaanCard` and `RandomReflectionCard`
- every currently shipped card whose localization text includes `[Destruction Eye]` now exposes `DestructionEye.CustomType` through `CanonicalKeywords`
- the current codebase now ships 32 non-basic cards across common, uncommon, and rare

## Follow-Up Status

- completed: reclassified the current implemented cards into [card-pool-package-map.md](./card-pool-package-map.md), a readable package doc that no longer depends on the historically mojibake draft notes
- completed: closed the remaining `Destruction Eye` tooltip-follow-up cards on `main` by landing keyword exposure for `MadGazeCard`, `CrackedSmileCard`, `RendingClawCard`, `ProliferatingGazeCard`, and the new `CruelBlinkCard`
- completed: added `FrenziedFlightCard` as a single uncommon madness extender; localization changed, so build plus install verification is required before runtime display can be called verified
- completed: added `ShatteredGlintCard`, `UnsteadyStrikeCard`, `RuddyStepCard`, `EchoingRuptureCard`, `UnhingedShelterCard`, and `CrimsonBanquetCard` as the next six-card gameplay slice

## Safe Next Slice

The next non-asset task should stay narrow:

1. Build and install the current card-pool changes with the documented PCK workflow
2. Verify runtime card text, hover tips, and conditional combat behavior for the newly added cards
3. Rebalance only after the cards have been seen in a live Flandre combat

Localization and packed card text changed, so repository edits alone are not enough to call this slice runtime-verified.
