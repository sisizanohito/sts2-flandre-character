param(
    [string]$ProjectRoot = (Get-Location).Path,
    [string]$Configuration = "Debug",
    [string]$ModsDir = "C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods",
    [switch]$BuildManaged,
    [switch]$StopGame
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Resolve-AbsolutePath {
    param([string]$Path)
    if ([System.IO.Path]::IsPathRooted($Path)) {
        return [System.IO.Path]::GetFullPath($Path)
    }

    return [System.IO.Path]::GetFullPath((Join-Path (Get-Location).Path $Path))
}

$projectAbs = Resolve-AbsolutePath -Path $ProjectRoot
$modsAbs = Resolve-AbsolutePath -Path $ModsDir
$modOutputDir = Join-Path $modsAbs "flandremod"
$managedOutputDir = Join-Path $projectAbs "bin\$Configuration\net9.0"
$manifestPath = Join-Path $projectAbs "mod_manifest.json"
$pckPath = Join-Path $projectAbs "flandremod.pck"

$managedFiles = @(
    "FlandreMod.dll",
    "FlandreMod.pdb",
    "FlandreMod.deps.json",
    "FlandreMod.runtimeconfig.json",
    "BaseLib.dll"
)

if ($BuildManaged) {
    Push-Location $projectAbs
    try {
        dotnet build ".\FlandreMod.csproj" -c $Configuration
    }
    finally {
        Pop-Location
    }
}

if ($StopGame) {
    Get-Process SlayTheSpire2 -ErrorAction SilentlyContinue | Stop-Process -Force
    Start-Sleep -Milliseconds 750
}

if (-not (Test-Path -LiteralPath $manifestPath -PathType Leaf)) {
    throw "Manifest not found: $manifestPath"
}

if (-not (Test-Path -LiteralPath $pckPath -PathType Leaf)) {
    throw "PCK not found: $pckPath`nBuild it first so localization files are packed correctly."
}

foreach ($file in $managedFiles) {
    $sourcePath = Join-Path $managedOutputDir $file
    if (-not (Test-Path -LiteralPath $sourcePath -PathType Leaf)) {
        throw "Managed artifact missing: $sourcePath"
    }
}

New-Item -ItemType Directory -Force -Path $modOutputDir | Out-Null

foreach ($file in $managedFiles) {
    $sourcePath = Join-Path $managedOutputDir $file
    Copy-Item -LiteralPath $sourcePath -Destination (Join-Path $modOutputDir $file) -Force
}

Copy-Item -LiteralPath $manifestPath -Destination (Join-Path $modOutputDir "mod_manifest.json") -Force
Copy-Item -LiteralPath $pckPath -Destination (Join-Path $modOutputDir "flandremod.pck") -Force

$installedPck = Get-Item -LiteralPath (Join-Path $modOutputDir "flandremod.pck")
$sourcePck = Get-Item -LiteralPath $pckPath

[pscustomobject]@{
    ModDir = $modOutputDir
    Configuration = $Configuration
    SourcePckLength = $sourcePck.Length
    InstalledPckLength = $installedPck.Length
    InstalledPckLastWriteTime = $installedPck.LastWriteTime
    ManagedOutputDir = $managedOutputDir
} | Format-List
