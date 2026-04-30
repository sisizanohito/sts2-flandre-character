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

## First Payoff Card

- `BloodScentCard`
  - common skill
  - gains Block
  - draws 1 card if current `Bloodshed` is at least 20

This first card only reads the resource. It does not consume `Bloodshed`.
That keeps the first slice easy to verify before adding spenders or scaling finishers.

## Extension Notes

Good next cards should test one new behavior at a time:

1. a damage payoff that checks a threshold after its first hit
2. a defensive card that scales by every N `Bloodshed`
3. a rare card that consumes current `Bloodshed`

Avoid adding consumption and multiple threshold tiers in the same slice.
