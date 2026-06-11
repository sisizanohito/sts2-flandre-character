param(
    [Parameter(Mandatory = $true)]
    [string]$SpineAssetDir,
    [string]$ProjectRoot = (Get-Location).Path
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Resolve-AbsolutePath {
    param([string]$Path)
    if ([System.IO.Path]::IsPathRooted($Path)) {
        return [System.IO.Path]::GetFullPath($Path)
    }
    return [System.IO.Path]::GetFullPath((Join-Path $ProjectRoot $Path))
}

$projectAbs = Resolve-AbsolutePath -Path $ProjectRoot
$spineDirAbs = Resolve-AbsolutePath -Path $SpineAssetDir

if (-not (Test-Path -LiteralPath $spineDirAbs -PathType Container)) {
    throw "Spine asset directory not found: $spineDirAbs"
}

$required = @("flandre.atlas", "flandre.skel", "flandre.png")
foreach ($name in $required) {
    $candidate = Join-Path $spineDirAbs $name
    if (-not (Test-Path -LiteralPath $candidate -PathType Leaf)) {
        throw "Missing required Spine asset: $candidate"
    }
}

$destDir = Join-Path $projectAbs "flandremod\\animations\\characters\\flandre"
New-Item -ItemType Directory -Force -Path $destDir | Out-Null

foreach ($name in $required) {
    Copy-Item -LiteralPath (Join-Path $spineDirAbs $name) -Destination (Join-Path $destDir $name) -Force
}

$skelDataPath = Join-Path $destDir "flandre_skel_data.tres"
$skelData = @'
[gd_resource type="SpineSkeletonDataResource" load_steps=3 format=3]

[ext_resource type="SpineAtlasResource" path="res://flandremod/animations/characters/flandre/flandre.atlas" id="1_atlas"]
[ext_resource type="SpineSkeletonFileResource" path="res://flandremod/animations/characters/flandre/flandre.skel" id="2_skel"]

[resource]
atlas_res = ExtResource("1_atlas")
skeleton_file_res = ExtResource("2_skel")
default_mix = 0.05
'@
Set-Content -LiteralPath $skelDataPath -Value $skelData -Encoding UTF8

$scenePath = Join-Path $projectAbs "flandremod\\Characters\\FlandreCharacter\\flandre_character.tscn"
if (-not (Test-Path -LiteralPath $scenePath -PathType Leaf)) {
    throw "Scene file not found: $scenePath"
}

. "$projectAbs\\tools\\Backup-ImageAsset.ps1" -SourcePath $scenePath -ProjectRoot $projectAbs -BackupRoot "assets_tmp/backups/scenes"

function Invoke-ScenePatch {
    param([string]$Content, [string]$Pattern, [string]$Replacement, [string]$Description)
    $patched = $Content -replace $Pattern, $Replacement
    if ($patched -eq $Content) {
        throw "Scene patch step matched nothing: $Description. The scene layout has drifted; update tools/Enable-FlandreSpineVisuals.ps1 before rerunning."
    }
    return $patched
}

$scene = Get-Content -LiteralPath $scenePath -Raw
if ($scene -match '\[node name="Visuals" type="SpineSprite" parent="\."\]') {
    Write-Output "Scene already uses SpineSprite visuals; skipping scene patch: $scenePath"
}
else {
    $scene = Invoke-ScenePatch -Content $scene `
        -Pattern '\[gd_scene load_steps=2 format=3\]' `
        -Replacement '[gd_scene load_steps=3 format=3]' `
        -Description 'bump load_steps for the added SpineSkeletonDataResource ext_resource'
    $scene = Invoke-ScenePatch -Content $scene `
        -Pattern '(\[ext_resource type="Script"[^\r\n]*\]\r?\n)' `
        -Replacement ('$1[ext_resource type="SpineSkeletonDataResource" path="res://flandremod/animations/characters/flandre/flandre_skel_data.tres" id="2_skeleton"]' + [Environment]::NewLine) `
        -Description 'insert SpineSkeletonDataResource ext_resource after the Script ext_resource'
    $scene = Invoke-ScenePatch -Content $scene `
        -Pattern '\[node name="Visuals" type="Sprite2D" parent="\."\]' `
        -Replacement ('[node name="Visuals" type="SpineSprite" parent="."]' + [Environment]::NewLine + 'skeleton_data_res = ExtResource("2_skeleton")' + [Environment]::NewLine + 'preview_skin = "Default"' + [Environment]::NewLine + 'preview_animation = "idle_loop"' + [Environment]::NewLine + 'preview_frame = false' + [Environment]::NewLine + 'preview_time = 0.0') `
        -Description 'replace the Sprite2D Visuals node with a SpineSprite node bound to the skeleton data'
    Set-Content -LiteralPath $scenePath -Value $scene -Encoding UTF8
    Write-Output "Scene patched: $scenePath"
}

Write-Output "Enabled Spine visuals for Flandre."
Write-Output "Assets copied to: $destDir"
