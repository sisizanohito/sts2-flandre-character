# Build And Install Workflow

## Why this exists

`card_keywords.json` once failed to appear in game even though it existed in the repo. The root cause was not the card code. The installed `flandremod.pck` was stale and did not contain:

- `flandremod/localization/eng/card_keywords.json`
- `flandremod/localization/jpn/card_keywords.json`

When that happens, hover tips can still appear, but the title and description show raw localization keys instead of real text.

## Safe workflow

1. Build the managed mod:

```powershell
dotnet build .\FlandreMod.csproj -c Debug
```

2. Rebuild the PCK from a staging directory that contains both root asset folders:

```text
source_dir = %REPO_ROOT%\out\pck_stage
output_path = %REPO_ROOT%\flandremod.pck
base_prefix =
convert_pngs = true
```

The staging directory must contain:

- `%REPO_ROOT%\out\pck_stage\flandremod\...`
- `%REPO_ROOT%\out\pck_stage\images\...`

The root `images` folder is required for custom atlas sprite paths such as `res://images/atlases/ui_atlas.sprites/card/energy_flandrecharacter.tres`.

3. Sync the exact built artifacts into the Steam mods folder:

```powershell
.\tools\Install-FlandreMod.ps1 -ProjectRoot . -Configuration Debug -StopGame
```

This copies the local `flandremod.pck` directly. Use this instead of relying on a generic install step when localization or PCK contents matter.

## Build / install checklist

Before treating a build or install as complete, confirm all of the following:

- the managed build finished successfully
- the rebuilt local `flandremod.pck` has a fresh timestamp
- the install script self-check summary says the exact local `flandremod.pck` was copied into the game mods folder

If any of these checks fail, treat the result as a stale PCK risk and rerun the rebuild and install flow before debugging game behavior.

## What the install summary can confirm

The install script's self-check summary is useful for confirming:

- which local build artifacts were selected for install
- whether the target mod folder and copied files match the expected install destination
- whether the local `flandremod.pck` and installed `flandremod.pck` matched at copy time

The install script's self-check summary cannot confirm by itself:

- that the installed PCK still contains the required localization files
- that the game actually loaded those localization tables at runtime
- that a stale PCK was not reintroduced by a later manual copy or older artifact

Use the install summary as the main gate for ruling out stale PCK suspicion. If behavior still looks wrong, follow with game-log confirmation and optional PCK content checks when you already have a working inspection method.

## Minimal localization path check

If you already have a working way to inspect PCK contents, check only these required paths before and after install:

- `flandremod/localization/eng/card_keywords.json`
- `flandremod/localization/jpn/card_keywords.json`

Minimal procedure:

1. Before install, inspect the rebuilt local `flandremod.pck` and confirm both paths exist.
2. Run `.\tools\Install-FlandreMod.ps1 -ProjectRoot . -Configuration Debug -StopGame` and confirm the self-check summary says the local and installed `flandremod.pck` matched.
3. After install, inspect the installed `flandremod.pck` and confirm the same two paths still exist.

If either path is missing on either side, treat it as a stale PCK issue and rerun rebuild plus install before checking game behavior. If you cannot inspect PCK contents in the current environment, rely on the install summary first and then confirm via game log.

## PCK verification

If hover text looks wrong and you already have a working PCK inspection method, inspect the installed PCK and confirm these paths exist:

- `flandremod/localization/eng/card_keywords.json`
- `flandremod/localization/jpn/card_keywords.json`

Also confirm the game log contains:

- `Found loc table from mod: eng card_keywords.json`
- `Found loc table from mod: jpn card_keywords.json`

If those lines are missing, the game is not reading your keyword localization table.

## Known pitfall

Do not build the PCK with `base_prefix = flandremod`.

That produces broken paths like:

- `flandremodlocalization/eng/card_keywords.json`

The correct prefix is:

- `flandremod/`
