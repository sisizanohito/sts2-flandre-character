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

2. Rebuild the PCK from `flandremod/` with the correct prefix:

```text
source_dir = %REPO_ROOT%\flandremod
output_path = %REPO_ROOT%\flandremod.pck
base_prefix = flandremod/
convert_pngs = true
```

3. Sync the exact built artifacts into the Steam mods folder:

```powershell
.\tools\Install-FlandreMod.ps1 -ProjectRoot . -Configuration Debug -StopGame
```

This copies the local `flandremod.pck` directly. Use this instead of relying on a generic install step when localization or PCK contents matter.

## PCK verification

If hover text looks wrong, inspect the installed PCK and confirm these paths exist:

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
