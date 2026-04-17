# Card Pool Package Map

## Purpose

`docs/card-pool-draft.md` still contains mojibake in the package-note sections, so it is not a reliable handoff document when choosing the next narrow implementation slice.

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

The common pool is already split into three readable lanes.

### 1. Eye setup

- `MadGazeCard`
  - adds `Madness`
  - creates one `Destruction Eye` on a random enemy
  - upgrade adds draw
- role:
  - starts the eye engine without spending the starter `EchoLinkCard`
  - bridges the eye lane and the madness lane in one card

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

## Uncommon Package

- `EchoLinkCard`
  - targeted eye application
  - upgrade increases eye count instead of changing cost
- `ProliferatingGazeCard`
  - applies eyes to all enemies
  - upgrade reduces cost

Uncommon cards are the package expanders.

- `EchoLinkCard` is the precise setup card and already anchors the starter deck
- `ProliferatingGazeCard` is the wide-board escalation card
- together they push the deck from "single eye payoff" into "board-wide eye state"

## Rare Package

- `DokaanCard`
  - heavy damage finisher
  - becomes an all-enemy hit under `Madness`

The current rare package is intentionally narrow.

- `DokaanCard` is the only shipped rare and acts as the existing madness payoff finisher
- this leaves room for later rares to specialize into:
  - eye-state burst conversion
  - high-cost board control
  - setup compression that combines eye creation with payoff

## Current Reading

From the implemented cards alone, the current Flandre package reads as:

1. start with targeted `Destruction Eye` setup from the starter deck
2. draft commons that either cash in active eyes or stabilize the deck while building toward them
3. use `Madness` as the alternative branch that changes how attacks fan out
4. let uncommons widen or deepen the eye board state
5. let `DokaanCard` serve as the current rare closer

## Next Safe Slice

This document only replaces the unreadable package notes from the draft.

The next non-asset slice should therefore be one of:

1. add one new card that cleanly extends the existing eye package without a new subsystem
2. add one new card that cleanly extends the existing madness package without a new subsystem
