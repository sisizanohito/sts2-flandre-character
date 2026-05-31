# Neow Startup Debugging

## 2026-05-31 Flandre Blessing Regression

Confirmed repro:

- `mcp__sts2_moddding__bridge_start_run` with `character: FlandreCharacter` starts a Flandre run at Neow.
- `mcp__sts2_moddding__bridge_get_full_state` reports `screen: EVENT`, `character: FlandreCharacter`, expected starter deck, and `DestructionEyeRelic`, but no Neow blessing options are available.

Confirmed runtime blocker:

- The game log showed `MegaCrit.Sts2.Core.Assets.AssetLoadException` while `Neow.GenerateInitialOptions()` evaluated `PositiveOptions`.
- The missing asset was `res://images/atlases/ui_atlas.sprites/card/energy_flandrecharacter.tres`.
- The bad install was built from only the `flandremod/` resource root, so the root `images/` folder was absent from `flandremod.pck`.
- Rebuilding `flandremod.pck` from `out/pck_stage` with both root folders, `flandremod/` and `images/`, restored Neow options.

Related custom-character guard:

- `NEventRoom.SetupLayout()` selects an ancient dialogue before calling `SetOptions(_event)`.
- `Neow.DefineDialogues()` only registers built-in character dialogue keys.
- Flandre now registers a Neow dialogue entry keyed by the exact model id entry (`FLANDREMOD-FLANDRE_CHARACTER`) with matching `ancients.json` localization.
- This guard is scoped to `Neow.DefineDialogues()` and does not change Neow option generation.

Verification:

- After correct PCK rebuild/install/restart, Flandre on seed `FLANDRENEOW03` exposed three Neow `event_option` actions through `bridge_get_full_state`.
- Ironclad on seed `IRONCLADNEOW03` also exposed three Neow `event_option` actions.
- `bridge_get_exceptions` returned `count: 0` after both checks.
