# Flandre Spine Visual Entry

## Purpose

This note fixes the entry conditions for enabling Spine-based combat visuals for Flandre without requiring guesswork from older animation notes.

Use this document before running `tools/Enable-FlandreSpineVisuals.ps1`.

## Scope

This document covers only:

- the required Spine export files
- where those files must be placed inside the mod
- which scene file the script rewrites
- what the script changes in that scene

This document does not cover:

- how to create the Spine animation itself
- image generation or Meshy workflow details
- runtime combat verification after install

## Required Input Files

Prepare a local export folder that contains all of the following files:

- `flandre.atlas`
- `flandre.skel`
- `flandre.png`

The script treats these three filenames as mandatory and fails immediately if any one is missing.

## Destination Inside The Mod

`tools/Enable-FlandreSpineVisuals.ps1` copies the required files into:

- `flandremod/animations/characters/flandre/flandre.atlas`
- `flandremod/animations/characters/flandre/flandre.skel`
- `flandremod/animations/characters/flandre/flandre.png`

It also writes this generated Godot resource file:

- `flandremod/animations/characters/flandre/flandre_skel_data.tres`

That `.tres` file points at the copied `.atlas` and `.skel` files through:

- `res://flandremod/animations/characters/flandre/flandre.atlas`
- `res://flandremod/animations/characters/flandre/flandre.skel`

## Scene Patch Target

The script rewrites exactly this combat scene:

- `flandremod/Characters/FlandreCharacter/flandre_character.tscn`

Before patching, it creates a backup under:

- `assets_tmp/backups/scenes/`

The merchant and rest-site scenes are not touched by this script.

## Scene Changes Applied By The Script

The current base scene contains a `Visuals` node with:

- node type: `Sprite2D`
- texture source: `res://flandremod/Characters/FlandreCharacter/flandre_character.png`

The script changes that scene in the following way:

1. Replaces the texture ext-resource with a Spine skeleton-data ext-resource:
   - `res://flandremod/animations/characters/flandre/flandre_skel_data.tres`
2. Replaces the `Visuals` node type:
   - `Sprite2D` -> `SpineSprite`
3. Replaces the node property block:
   - removes `texture = ExtResource("2_texture")`
   - adds `skeleton_data_res = ExtResource("2_skeleton")`
   - adds `preview_skin = "Default"`
   - adds `preview_animation = "idle_loop"`
   - adds `preview_frame = false`
   - adds `preview_time = 0.0`

Positioning and marker nodes such as `Bounds`, `CenterPos`, `IntentPos`, and `OrbPos` remain in the same scene file.

## Expected Spine Naming Assumption

The generated scene preview assumes these names already exist in the Spine export:

- skin: `Default`
- animation: `idle_loop`

If either name does not exist in the exported data, the script still patches the scene, but the preview assumptions in the scene will no longer match the export.

## Standard Invocation

Run from the repository root:

```powershell
.\tools\Enable-FlandreSpineVisuals.ps1 -SpineAssetDir <your_spine_export_folder>
```

`-SpineAssetDir` may be absolute or relative. Relative paths are resolved from `-ProjectRoot`, which defaults to the current working directory.

## Completion Check For This Step

Treat the Spine entry-preparation step as complete only when all of the following are true:

- the three required export files are identified and named exactly as expected
- their in-repo destination is fixed to `flandremod/animations/characters/flandre/`
- the generated resource target `flandre_skel_data.tres` is documented
- the patched scene target is fixed to `flandremod/Characters/FlandreCharacter/flandre_character.tscn`
- the `Visuals` node replacement from `Sprite2D` to `SpineSprite` is documented
