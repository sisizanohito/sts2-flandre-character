# Bloodshed Design Note

## Purpose

`Bloodshed` is the first draft of Flandre's vampire-style build axis.
It is intentionally separate from direct self-damage: the resource tracks HP damage that happens anywhere on the combat field during the current turn.

## Current Rule

- `Bloodshed` is Flandre-specific combat state, not a displayed player-side Power.
- `BloodshedPower` remains only as a compatibility facade and hover-tip localization source for cards that mention `[Bloodshed]`.
- It counts HP loss from any creature in the same combat.
- Healing and non-HP changes do not increase it.
- It resets at the start of Flandre's turn.
- The live count is displayed under/near Flandre's energy counter, following the same combat UI area that Regent uses for the built-in Star counter.
- If Stars become visible for Flandre through a nonstandard effect, the Bloodshed label shifts lower to avoid directly covering the Star counter.
- A primitive blood clot visual is attached above Flandre's combat bounds whenever current `Bloodshed` is greater than 0.
- The clot grows from `Bloodshed` 1 through 60, caps after that to avoid covering intent UI, and hides again when `Bloodshed` resets or is consumed to 0.
- This visual is code-drawn with Godot primitives, so it does not add new asset or localization requirements.

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
- `ScarletAppetiteCard`
  - uncommon power
  - whenever Flandre loses HP, adds extra `Bloodshed`
  - tests the "reward self HP loss" power slice without adding a new cost model
- `SanguineGuardCard`
  - uncommon power
  - at the end of Flandre's turn, gains Block for every 10 current `Bloodshed`
  - tests the first "N per Bloodshed" scaling payoff without consuming the resource

The first four cards only read or build the resource. `BloodPactCard` is the first fixed-cost spender and intentionally avoids scaling by every N `Bloodshed`. `SanguineGuardCard` is the first non-consuming scaling payoff, while `ScarletAppetiteCard` keeps the HP-loss reward behavior isolated in a power.

## Extension Notes

Good next cards should test one new behavior at a time:

1. a rare card that consumes current `Bloodshed`
2. a second scaling payoff that uses attack or draw instead of block
3. a stricter self-HP-loss payoff that only checks HP paid by cards, if the STS2 API exposes a reliable source hook

Avoid adding consumption and multiple threshold tiers in the same slice.
