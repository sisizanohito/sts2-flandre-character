# Card Pool Package Map

## Purpose

`docs/design/card-pool-draft.md` still contains mojibake in the package-note sections, so it is not a reliable handoff document when choosing the next narrow implementation slice.

This file rewrites only the currently implemented package structure in a readable form. It does not propose new cards or change the roadmap order.

Confirmed sources:
- `Code/Cards/*.cs`
- `Code/Characters/FlandreCharacter.cs`
- `flandremod/localization/eng/cards.json`

## Starter Identity

- Starter deck: `FlandreStrikeCard` x4, `FlandreDefendCard` x4, `EchoLinkCard` x2
- Starter relic: `DestructionEyeRelic`
- Immediate gameplay identity:
  - establish `Destruction Eye` early through `EchoLinkCard`
  - convert that setup into efficient common-card payoffs
  - keep `Madness` as a secondary branch that changes targeting and defensive payoff timing

## Common Package

The common pool is already split into five readable lanes.

### 1. Eye setup

- `MadGazeCard`
  - adds `Madness`
  - creates one `Destruction Eye` on a random enemy
  - upgrade adds draw
- `CruelBlinkCard`
  - single-target attack
  - creates `Destruction Eye` on the target after dealing damage
- role:
  - starts the eye engine without spending the starter `EchoLinkCard`
  - gives the common pool one direct "hit first, then mark" setup card
  - keeps `MadGazeCard` as the bridge between the eye lane and the madness lane

### 2. Eye payoff

- `SparkScatterCard`
  - cheap single-target hit
  - deals extra damage to the linked eye on that target
- `RendingClawCard`
  - stronger single-target hit
  - also cashes in damage against the linked eye
- `LaughingEyeCard`
  - gets bonus damage when any active eye exists
- `CrackedSmileCard`
  - gets bonus block when any active eye exists
- role:
  - rewards the player for keeping at least one eye active
  - mixes direct target payoff with broader "any eye exists" payoff so the deck can pivot between focused damage and stability

### 3. Madness support

- `RandomReflectionCard`
  - random-hit attack that spreads across all enemies when `Madness` is active
- `ClosedWingsCard`
  - defensive payoff that gets better while `Madness` is active
- `TwistedPlayCard`
  - applies `Madness`
  - turns repeated madness application into card draw
- role:
  - keeps the madness package self-contained at common
  - lets the deck profit from `Madness` before committing to rarer finishers

### 4. Generic pressure

- `SweepingBurstCard`
  - unconditional all-enemy attack
- role:
  - gives the pool one simple baseline attack that does not require eye or madness setup
  - keeps early reward choices from being over-constrained by synergy requirements

### 5. Bloodshed support

- `BloodScentCard`
  - gains block
  - draws when current `Bloodshed` reaches its threshold
- `CrimsonAdvanceCard`
  - loses HP
  - draws cards
  - gains energy when current `Bloodshed` reaches its threshold
- role:
  - opens the vampire build axis with both passive field-damage payoff and active HP-payment setup
  - rewards damage that happens anywhere on the field, including damage involving `Destruction Eye`
  - starts with non-consuming threshold payoffs so the resource can be verified before spenders are added

## Uncommon Package

- `EchoLinkCard`
  - targeted eye application
  - upgrade increases eye count instead of changing cost
- `ProliferatingGazeCard`
  - applies eyes to all enemies
  - upgrade reduces cost
- `BloodMakeupCard`
  - loses HP
  - gains efficient block
  - heals back a small amount when current `Bloodshed` reaches its threshold
- `BloodPactCard`
  - consumes current `Bloodshed`
  - gains Energy and draws cards if the consume succeeds

Uncommon cards are the package expanders.

- `EchoLinkCard` is the precise setup card and already anchors the starter deck
- `ProliferatingGazeCard` is the wide-board escalation card
- `BloodMakeupCard` lets the bloodshed lane use HP payment as defensive tempo instead of only raw acceleration
- `BloodPactCard` is the first explicit `Bloodshed` spender and turns the resource into burst tempo
- together they push the deck from "single eye payoff" into "board-wide eye state" or into HP-payment stabilization

## Rare Package

- `DokaanCard`
  - heavy damage finisher
  - becomes an all-enemy hit under `Madness`
- `VampiricImpulseCard`
  - heavy single-target attack
  - heals for half of unblocked damage dealt when current `Bloodshed` reaches its threshold

The current rare package is intentionally narrow but now has one finisher for each of the two newer branches.

- `DokaanCard` is the only shipped rare and acts as the existing madness payoff finisher
- `VampiricImpulseCard` is the first vampire-style closer that turns bloodshed setup into HP recovery
- this leaves room for later rares to specialize into:
  - eye-state burst conversion
  - high-cost board control
  - setup compression that combines eye creation with payoff
  - explicit `Bloodshed` consumption

## Current Reading

From the implemented cards alone, the current Flandre package reads as:

1. start with targeted `Destruction Eye` setup from the starter deck
2. draft commons that either cash in active eyes or stabilize the deck while building toward them
3. use `Madness` as the alternative branch that changes how attacks fan out
4. use `Bloodshed` as a field-damage resource that can reward either eye turns, ordinary combat damage, or deliberate HP payment
5. let uncommons widen or deepen the eye board state
6. let `DokaanCard` serve as the current rare closer

## Next Safe Slice

This document only replaces the unreadable package notes from the draft.

Completed follow-up note:
- every currently shipped card whose localization text includes `[Destruction Eye]` now exposes `DestructionEye.CustomType` through `CanonicalKeywords`
- `BloodshedPower`, `BloodScentCard`, `CrimsonAdvanceCard`, `BloodMakeupCard`, `VampiricImpulseCard`, and `BloodPactCard` add the first vampire axis slice.

The next non-asset slice should therefore be one of:

1. add one new card that cleanly extends the existing eye package without a new subsystem
2. add one new card that cleanly extends the existing madness package without a new subsystem
3. add one follow-up `Bloodshed` card that scales by every N `Bloodshed`, since the first fixed-cost spender now exists
