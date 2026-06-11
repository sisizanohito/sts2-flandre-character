# Power Icon Notes

## Summary

This project uses custom power icons for:

- `MadnessPower`
- `LinkPower`
- `BloodshedPower` (currently reuses the `LinkPower` icon as a placeholder)

The final working approach is:

1. Make the powers inherit from `BaseLib.Abstracts.CustomPowerModel`
2. Provide custom icon paths on the model
3. Patch the `PowerModel.Icon` / `PowerModel.BigIcon` getters plus `NPower._Ready` and `NPower.Reload`
4. If `ResourceLoader.Load<Texture2D>(res://...)` fails, load the PNG from disk and build an `ImageTexture`
5. Assign both the small icon and the flash texture directly on the node

## What Did Not Work Reliably

- Relying on `PowerModel.Icon` alone
- Relying on `PowerModel.BigIcon` alone
- Expecting `res://images/powers/*.png` to load as `Texture2D` through `ResourceLoader.Load<Texture2D>`
- Assuming atlas fallback would automatically pick up modded PNG files

## Important Finding

At startup, `ResourceLoader.Exists("res://images/powers/<name>.png")` returned `true`, but:

- `ResourceLoader.Load<Texture2D>(...)` returned `null`

Because of that, the icon patch needs a fallback path that reads the installed PNG from disk and converts it with:

- `Image.Load(...)`
- `ImageTexture.CreateFromImage(...)`

## Working Files

- `Code/Powers/MadnessPower.cs`
- `Code/Powers/LinkPower.cs`
- `Code/Powers/CustomPowerIconPatch.cs`

## Node-Level Fix

The small combat icon is controlled by `MegaCrit.Sts2.Core.Nodes.Combat.NPower`.

Important fields:

- `Icon` -> small power icon
- `PowerFlash` -> large flash texture

The reliable fix is to override both of these in:

- `NPower._Ready`
- `NPower.Reload`

## IDs Used By The Patch

`CustomPowerIconPatch.GetCustomIconPath` switches on the unprefixed `Id.Entry` values:

- `MADNESS_POWER`
- `LINK_POWER`
- `BLOODSHED_POWER`

The patch also keeps type checks as a fallback for:

- `MadnessPower`
- `LinkPower`
- `BloodshedPower`

The type-check fallback matters because it keeps the patch working whether or not the runtime `Id.Entry` carries the `FLANDREMOD-` prefix (console commands and localization keys use the prefixed form, e.g. `FLANDREMOD-MADNESS_POWER`).

## Verification Checklist

- Summon a `LinkPower` source by using `EchoLinkCard`
- Apply `FLANDREMOD-MADNESS_POWER` through the dev console
- Confirm both icons appear in combat
- Confirm the small icon is no longer an `AtlasTexture` missing icon
- Confirm `PowerFlash` also uses the custom texture

## If This Breaks Again

Check these first:

1. The powers still inherit from `CustomPowerModel`
2. The runtime `Id.Entry` still includes the `FLANDREMOD-` prefix
3. The installed PNG files exist under the mod folder
4. `NPower` patches are still applied
5. The fallback disk loader still resolves the installed file path
