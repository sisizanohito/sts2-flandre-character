# Card Pool Implementation Status

## Purpose

`docs/card-pool-draft.md` still contains mojibake, so it is not a reliable source for the current implementation state.

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

### Uncommon

- `EchoLinkCard`
  - cost `2`
  - applies `Destruction Eye` to the chosen target
  - upgrade changes the application count from `1` to `2`
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

### Rare

- `DokaanCard`
  - cost `3`
  - high single-target damage
  - madness-specific all-enemy behavior is handled by the shared patch
- `VampiricImpulseCard`
  - cost `2`
  - single-target attack
  - heals for half of unblocked damage dealt when current `Bloodshed` reaches its threshold

## Stable Shared Base

- `DestructionEyeCardHelper` already owns eye creation, eye reinforcement, and initial eye HP setup
- madness target-shift behavior has already been confirmed for `DokaanCard` and `RandomReflectionCard`
- every currently shipped card whose localization text includes `[Destruction Eye]` now exposes `DestructionEye.CustomType` through `CanonicalKeywords`
- the current codebase now ships 18 non-basic cards across common, uncommon, and rare

## Follow-Up Status

- completed: reclassified the current implemented cards into [card-pool-package-map.md](./card-pool-package-map.md), a readable package doc that does not depend on the mojibake draft
- completed: closed the remaining `Destruction Eye` tooltip-follow-up cards on `main` by landing keyword exposure for `MadGazeCard`, `CrackedSmileCard`, `RendingClawCard`, `ProliferatingGazeCard`, and the new `CruelBlinkCard`

## Safe Next Slice

The next non-asset task should stay narrow:

1. Add one new card that closes over the existing `DestructionEyeCardHelper` behavior without introducing a new shared subsystem
2. Add one new card that closes over the existing `MadnessPower` behavior without introducing a new shared subsystem
3. Add one `Bloodshed` follow-up that scales by every N `Bloodshed`, since the first fixed-cost spender now exists

This document still does not decide which card should go next. It only records the verified inventory and the completed follow-up doc slice.
