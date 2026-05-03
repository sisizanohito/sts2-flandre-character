# Combat Animation Progress

## Implemented now
- Added an idle animation for Flandre in combat using Harmony on `NCreatureVisuals._Process`.
- The sprite now has a breathing-like bobbing motion (Y offset + slight scale pulse).
- Existing texture fix logic remains intact.

## New tools
- `tools/Enable-FlandreSpineVisuals.ps1`
  - Copies Spine exports (`flandre.atlas`, `flandre.skel`, `flandre.png`) into the mod.
  - Writes `flandre_skel_data.tres`.
  - Patches `flandre_character.tscn` from `Sprite2D` to `SpineSprite`.
  - Creates a scene backup before patching.
  - Fixed entry conditions are documented in [flandre-spine-visual-entry.md](../visuals/flandre-spine-visual-entry.md).

## Meshy output status
- Meshy generation tasks completed successfully:
  - `019d7c77-093e-7568-be80-313a0ac41968`
  - `019d7c77-1f4c-7ad9-a716-a3d9e3205adf`
  - `019d7c77-1fcc-7c9b-9c0d-1adfd5acb353`
- The current tool responses do not include direct image URLs, so exports still need manual retrieval from the Meshy workspace UI.

## Next immediate step
1. Export Spine files from your art pipeline into a local folder with:
   - `flandre.atlas`
   - `flandre.skel`
   - `flandre.png`
2. Run:
   - `.\tools\Enable-FlandreSpineVisuals.ps1 -SpineAssetDir <your_spine_export_folder>`
3. Build and hot-reload.

For the exact destination paths, generated `.tres`, patched scene file, and preview-name assumptions, use [flandre-spine-visual-entry.md](../visuals/flandre-spine-visual-entry.md).

## Direct download fallback (without MCP image URLs)
Use Meshy REST API directly:
1. Set API key:
   - `$env:MESHY_API_KEY = "msy_..."`
2. Download by known task ids:
   - `.\tools\Fetch-MeshyTextToImage.ps1 -TaskId 019d7c77-093e-7568-be80-313a0ac41968,019d7c77-1f4c-7ad9-a716-a3d9e3205adf,019d7c77-1fcc-7c9b-9c0d-1adfd5acb353`
3. Output folder:
   - `assets_tmp/meshy/images/`
