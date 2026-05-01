# Bloodshed Design Note

## Purpose

`Bloodshed` is the first draft of Flandre's vampire-style build axis.
It is intentionally separate from direct self-damage: the resource tracks HP damage that happens anywhere on the combat field during the current turn.

## Initial Rule

- `Bloodshed` is a player-side Power.
- It is applied at combat start by `DestructionEyeRelic`.
- It counts HP loss from any creature in the same combat.
- Healing and non-HP changes do not increase it.
- It resets at the start of Flandre's turn.
- Current implementation keeps an internal baseline amount of `1` so the Power can exist while displaying `0`.

## First Payoff Cards

- `BloodScentCard`
  - common skill
  - gains Block
  - draws 1 card if current `Bloodshed` is at least 20
- `CrimsonAdvanceCard`
  - common skill
  - loses HP to actively seed `Bloodshed`
  - draws cards, then gains Energy if current `Bloodshed` is at least 10
- `BloodMakeupCard`
  - uncommon skill
  - loses HP for efficient Block
  - heals a small amount if current `Bloodshed` is at least 15
- `VampiricImpulseCard`
  - rare attack
  - heals for half of unblocked damage dealt if current `Bloodshed` is at least 20
- `BloodPactCard`
  - uncommon skill
  - consumes 15 current `Bloodshed`
  - gains Energy and draws cards only if the consume succeeds

The first four cards only read or build the resource. `BloodPactCard` is the first fixed-cost spender and intentionally avoids scaling by every N `Bloodshed`.

## Extension Notes

Good next cards should test one new behavior at a time:

1. a defensive card that scales by every N `Bloodshed`
2. a rare card that consumes current `Bloodshed`
3. a power that changes the reward for self HP loss

Avoid adding consumption and multiple threshold tiers in the same slice.
