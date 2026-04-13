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

function Get-InstallVerificationSummary {
    param(
        [string]$ProjectRoot,
        [string]$SourcePckPath,
        [string]$InstalledPckPath,
        [string[]]$RequiredRepoFiles
    )

    $sourcePck = Get-Item -LiteralPath $SourcePckPath
    $installedPck = Get-Item -LiteralPath $InstalledPckPath
    $sourceHash = (Get-FileHash -LiteralPath $SourcePckPath -Algorithm SHA256).Hash
    $installedHash = (Get-FileHash -LiteralPath $InstalledPckPath -Algorithm SHA256).Hash
    $repoFileChecks = foreach ($relativePath in $RequiredRepoFiles) {
        $absolutePath = Join-Path $ProjectRoot $relativePath
        $exists = Test-Path -LiteralPath $absolutePath -PathType Leaf
        [pscustomobject]@{
            Path = $relativePath
            Exists = $exists
            Length = if ($exists) { (Get-Item -LiteralPath $absolutePath).Length } else { $null }
        }
    }

    return [pscustomobject]@{
        SourcePckPath = $SourcePckPath
        InstalledPckPath = $InstalledPckPath
        SourcePckLength = $sourcePck.Length
        InstalledPckLength = $installedPck.Length
        SourcePckLastWriteTime = $sourcePck.LastWriteTime
        InstalledPckLastWriteTime = $installedPck.LastWriteTime
        PckSha256Matches = ($sourceHash -eq $installedHash)
        SourcePckSha256 = $sourceHash
        InstalledPckSha256 = $installedHash
        RequiredRepoFiles = $repoFileChecks
        FollowUpChecks = @(
            "Verify the rebuilt local flandremod.pck is the one copied into the mods folder.",
            "If keyword text still renders as raw keys, confirm the game log contains 'Found loc table from mod: eng card_keywords.json'.",
            "If keyword text still renders as raw keys, confirm the game log contains 'Found loc table from mod: jpn card_keywords.json'."
        )
    }
}

$projectAbs = Resolve-AbsolutePath -Path $ProjectRoot
$modsAbs = Resolve-AbsolutePath -Path $ModsDir
$modOutputDir = Join-Path $modsAbs "flandremod"
$managedOutputDir = Join-Path $projectAbs "bin\$Configuration\net9.0"
$manifestPath = Join-Path $projectAbs "mod_manifest.json"
$pckPath = Join-Path $projectAbs "flandremod.pck"
$requiredRepoFiles = @(
    "flandremod\localization\eng\card_keywords.json",
    "flandremod\localization\jpn\card_keywords.json"
)

$managedFiles = @(
    "FlandreMod.dll",
    "FlandreMod.pdb",
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

$staleManagedJson = @(
    "FlandreMod.deps.json",
    "FlandreMod.runtimeconfig.json"
)

foreach ($file in $staleManagedJson) {
    $stalePath = Join-Path $modOutputDir $file
    if (Test-Path -LiteralPath $stalePath -PathType Leaf) {
        Remove-Item -LiteralPath $stalePath -Force
    }
}

foreach ($file in $managedFiles) {
    $sourcePath = Join-Path $managedOutputDir $file
    Copy-Item -LiteralPath $sourcePath -Destination (Join-Path $modOutputDir $file) -Force
}

Copy-Item -LiteralPath $manifestPath -Destination (Join-Path $modOutputDir "mod_manifest.json") -Force
Copy-Item -LiteralPath $pckPath -Destination (Join-Path $modOutputDir "flandremod.pck") -Force

$installedPckPath = Join-Path $modOutputDir "flandremod.pck"
$installedPck = Get-Item -LiteralPath $installedPckPath
$sourcePck = Get-Item -LiteralPath $pckPath
$verification = Get-InstallVerificationSummary -ProjectRoot $projectAbs -SourcePckPath $pckPath -InstalledPckPath $installedPckPath -RequiredRepoFiles $requiredRepoFiles

[pscustomobject]@{
    ModDir = $modOutputDir
    Configuration = $Configuration
    SourcePckLength = $sourcePck.Length
    InstalledPckLength = $installedPck.Length
    InstalledPckLastWriteTime = $installedPck.LastWriteTime
    ManagedOutputDir = $managedOutputDir
} | Format-List

""
"Install verification:"
$verification | ConvertTo-Json -Depth 5
