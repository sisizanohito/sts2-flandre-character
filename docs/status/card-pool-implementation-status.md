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
- `UnsteadyStrikeCard`
  - cost `1`
  - single-target attack
  - draws 1 card only when `Madness` is active during play resolution
- `ShatteredGlintCard`
  - cost `1`
  - single-target attack
  - simple baseline damage
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
- `RuddyStepCard`
  - cost `1`
  - gain block
  - uses the mod-local defend portrait
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
- `EchoingRuptureCard`
  - cost `1`
  - single-target attack
  - contributes to `Bloodshed` through normal HP damage tracking
- `FrenziedFlightCard`
  - cost `1`
  - applies `MadnessPower` 2
  - draws cards
  - upgrade increases draw by 1
- `UnhingedShelterCard`
  - cost `1`
  - gains block
  - applies `MadnessPower` 1
  - upgrade increases block by 3
- `ScarletAppetiteCard`
  - cost `1`
  - power
  - whenever Flandre loses HP, gains additional `Bloodshed`
- `SanguineGuardCard`
  - cost `1`
  - power
  - at end of Flandre's turn, gains Block for every 10 current `Bloodshed`

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
- `CrimsonFeastCard`
  - cost `2`
  - single-target attack
  - consumes 25 current `Bloodshed`
  - deals damage and heals only when the consume succeeds

## Stable Shared Base

- `DestructionEyeCardHelper` already owns eye creation, eye reinforcement, and initial eye HP setup
- madness target-shift behavior has already been confirmed for `DokaanCard` and `RandomReflectionCard`
- every currently shipped card whose localization text includes `[Destruction Eye]` now exposes `DestructionEye.CustomType` through `CanonicalKeywords`
- the current codebase now ships 23 non-basic cards across common, uncommon, and rare

## Follow-Up Status

- completed: reclassified the current implemented cards into [card-pool-package-map.md](./card-pool-package-map.md), a readable package doc that no longer depends on the historically mojibake draft notes
- completed: closed the remaining `Destruction Eye` tooltip-follow-up cards on `main` by landing keyword exposure for `MadGazeCard`, `CrackedSmileCard`, `RendingClawCard`, `ProliferatingGazeCard`, and the new `CruelBlinkCard`
- completed: added `FrenziedFlightCard` as a single uncommon madness extender; localization changed, so build plus install verification is required before runtime display can be called verified
- completed: added `UnsteadyStrikeCard` as a single common madness payoff; localization changed, so build plus install verification is required before runtime display can be called verified
- completed: added `RuddyStepCard` as a single common defensive skill; localization changed, so build plus install verification is required before runtime display can be called verified
- completed: added `ShatteredGlintCard` as a simple common attack; localization changed, so build plus install verification is required before runtime display can be called verified
- completed: added `EchoingRuptureCard` as an uncommon Bloodshed attack; localization changed, so build plus install verification is required before runtime display can be called verified
- completed: added `CrimsonFeastCard` as the first rare fixed-cost `Bloodshed` spender that converts a successful consume into damage plus healing

## Safe Next Slice

The next non-asset task should stay narrow:

1. Add one new card that closes over the existing `DestructionEyeCardHelper` behavior without introducing a new shared subsystem
2. Add one new card that closes over the existing `MadnessPower` behavior without introducing a new shared subsystem
3. Add a second narrow `Bloodshed` payoff only after `CrimsonFeastCard` runtime behavior remains stable

This document still does not decide which card should go next. It only records the verified inventory and the completed follow-up doc slice.
