# Card Pool Package Map

## Purpose

`docs/design/card-pool-draft.md` previously contained mojibake in the package-note sections. As of the 2026-05-31 docs cleanup, the draft and this package map are readable.

This file records only the currently implemented package structure in a readable form. It does not propose new cards or change the roadmap order.

Recovery note:
- no unrecoverable package-note text remained in this pass, so no `UNKNOWN` placeholders were added

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

- `EchoLinkCard`
  - targeted eye application
  - upgrade increases eye count instead of changing cost
  - also anchors the starter deck (x2); rarity is `Common` in code
- `MadGazeCard`
  - adds `Madness`
  - creates one `Destruction Eye` on a random enemy
  - upgrade adds draw
- `CruelBlinkCard`
  - single-target attack
  - creates `Destruction Eye` on the target after dealing damage
- role:
  - starts the eye engine without spending the starter `EchoLinkCard` copies
  - gives the common pool one direct "hit first, then mark" setup card
  - keeps `MadGazeCard` as the bridge between the eye lane and the madness lane

### 2. Eye payoff

- `SparkScatterCard`
  - cheap single-target hit
  - deals extra damage to the linked eye on that target
- `ShatteredGlintCard`
  - low-pressure single-target hit
  - deals smaller extra damage to the linked eye on that target
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
- `UnsteadyStrikeCard`
  - attack that draws when `Madness` is active
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
- `RuddyStepCard`
  - gains block
  - draws at a lower `Bloodshed` threshold
- `CrimsonAdvanceCard`
  - loses HP
  - draws cards
  - gains energy when current `Bloodshed` reaches its threshold
- `BloodyCrescentCard`
  - attacks first
  - gains block when current `Bloodshed` reaches its threshold
- `RedMistCard`
  - pays HP for block
  - turns a low `Bloodshed` threshold into a small all-enemy hit
- role:
  - opens the vampire build axis with both passive field-damage payoff and active HP-payment setup
  - rewards damage that happens anywhere on the field, including damage involving `Destruction Eye`
  - starts with non-consuming threshold payoffs so the resource can be verified before spenders are added

## Uncommon Package

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
- `FrenziedFlightCard`
  - applies `Madness`
  - draws cards
- `UnhingedShelterCard`
  - gains Block
  - applies one `Madness`
- `ScarletAppetiteCard`
  - power that adds extra `Bloodshed` whenever Flandre loses HP
- `SanguineGuardCard`
  - power that turns every 10 current `Bloodshed` into end-of-turn Block
- `SanguineOverflowCard`
  - single-target attack that scales with every N current `Bloodshed`
- `EchoingRuptureCard`
  - attacks and chains another random eye setup only when an active eye already exists

Uncommon cards are the package expanders.

- `ProliferatingGazeCard` is the wide-board escalation card
- `BloodMakeupCard` lets the bloodshed lane use HP payment as defensive tempo instead of only raw acceleration
- `BloodPactCard` is the first explicit `Bloodshed` spender and turns the resource into burst tempo
- `FrenziedFlightCard` is the first uncommon madness extender, giving the lane a compact way to queue multiple all-enemy attacks without adding a new targeting rule
- `UnhingedShelterCard` gives the madness lane a defensive setup card with only one fixed `Madness` stack
- `ScarletAppetiteCard` makes self HP loss accelerate the Bloodshed count
- `SanguineGuardCard` is the first non-consuming "per N Bloodshed" scaling payoff
- `SanguineOverflowCard` applies that same non-consuming scaling shape to active attack output
- `EchoingRuptureCard` is the first uncommon eye chain card that requires an existing active eye
- together they push the deck from "single eye payoff" into "board-wide eye state", madness burst setup, or HP-payment stabilization

## Rare Package

- `DokaanCard`
  - heavy damage finisher
  - becomes an all-enemy hit under `Madness`
- `VampiricImpulseCard`
  - heavy single-target attack
  - heals for half of unblocked damage dealt when current `Bloodshed` reaches its threshold
- `CataclysmicGazeCard`
  - damages each active `Destruction Eye`
  - then attacks all enemies
- `ScarletDelugeCard`
  - consumes all current `Bloodshed`
  - converts the consumed amount into all-enemy bonus damage
- `CrimsonBanquetCard`
  - consumes a fixed 25 `Bloodshed`
  - deals heavy single-target damage and heals only when the consume succeeds

The current rare package is intentionally narrow but now has one finisher for each of the two newer branches.

- `DokaanCard` is the rare madness payoff finisher
- `VampiricImpulseCard` is the first vampire-style closer that turns bloodshed setup into HP recovery
- `CataclysmicGazeCard` is the first rare eye-state burst card, converting placed eyes into immediate relay pressure before a board hit
- `ScarletDelugeCard` is the all-in `Bloodshed` spender
- `CrimsonBanquetCard` is the fixed-cost `Bloodshed` spender for a single-target payoff
- this leaves room for later rares to specialize into:
  - high-cost board control
  - setup compression that combines eye creation with payoff
  - non-consuming late-game Bloodshed payoff

## Current Reading

From the implemented cards alone, the current Flandre package reads as:

1. start with targeted `Destruction Eye` setup from the starter deck
2. draft commons that either cash in active eyes or stabilize the deck while building toward them
3. use `Madness` as the alternative branch that changes how attacks fan out
4. use `Bloodshed` as a field-damage resource that can reward either eye turns, ordinary combat damage, or deliberate HP payment
5. let uncommons widen or deepen the eye board state
6. let `DokaanCard` serve as the current rare closer

## Next Safe Slice

This document originated as the readable replacement for the draft package notes and now remains the current handoff for implemented package structure.

Completed follow-up note:
- every currently shipped card whose localization text includes `[Destruction Eye]` now exposes `DestructionEye.CustomType` through `CanonicalKeywords`
- `BloodshedPower`, `BloodScentCard`, `CrimsonAdvanceCard`, `BloodMakeupCard`, `VampiricImpulseCard`, `BloodPactCard`, `ScarletAppetiteCard`, and `SanguineGuardCard` add the first vampire axis slice.
- `FrenziedFlightCard` adds one narrow uncommon madness slice. Because it changes localization and packed card text, build plus install verification is required before treating runtime display as verified.
- `ShatteredGlintCard`, `UnsteadyStrikeCard`, `RuddyStepCard`, `EchoingRuptureCard`, `UnhingedShelterCard`, and `CrimsonBanquetCard` extend the eye, madness, and Bloodshed lanes without introducing another shared subsystem.

The next non-asset slice should be install and runtime verification for the current card-pool changes before adding more cards.
