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
        [string]$ManagedOutputDir,
        [string]$ModOutputDir,
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
    $managedArtifactChecks = foreach ($artifact in @(
        @{ Source = "FlandreMod.dll"; Installed = "FlandreMod.dll" },
        @{ Source = "FlandreMod.dll"; Installed = "flandremod.dll" },
        @{ Source = "FlandreMod.pdb"; Installed = "FlandreMod.pdb" },
        @{ Source = "FlandreMod.pdb"; Installed = "flandremod.pdb" },
        @{ Source = "BaseLib.dll"; Installed = "BaseLib.dll" }
    )) {
        $fileName = $artifact.Source
        $installedFileName = $artifact.Installed
        $sourcePath = Join-Path $ManagedOutputDir $fileName
        $installedPath = Join-Path $ModOutputDir $installedFileName
        $sourceExists = Test-Path -LiteralPath $sourcePath -PathType Leaf
        $installedExists = Test-Path -LiteralPath $installedPath -PathType Leaf
        [pscustomobject]@{
            SourceFile = $fileName
            InstalledFile = $installedFileName
            SourceExists = $sourceExists
            InstalledExists = $installedExists
            Sha256Matches = if ($sourceExists -and $installedExists) {
                (Get-FileHash -LiteralPath $sourcePath -Algorithm SHA256).Hash -eq
                (Get-FileHash -LiteralPath $installedPath -Algorithm SHA256).Hash
            } else {
                $false
            }
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
        ManagedArtifacts = $managedArtifactChecks
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
$baseLibOutputDir = Join-Path $modsAbs "BaseLib.0.2.7"
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

$projectXml = [xml](Get-Content -LiteralPath (Join-Path $projectAbs "FlandreMod.csproj") -Raw)
$baseLibPackageRef = $projectXml.SelectNodes("//PackageReference") |
    Where-Object { $_.GetAttribute("Include") -eq "Alchyr.Sts2.BaseLib" } |
    Select-Object -First 1
$baseLibPackageVersion = if ($null -ne $baseLibPackageRef) { $baseLibPackageRef.GetAttribute("Version") } else { $null }
if ($null -eq $baseLibPackageRef -or [string]::IsNullOrWhiteSpace($baseLibPackageVersion)) {
    throw "Could not determine Alchyr.Sts2.BaseLib package version from FlandreMod.csproj"
}

$baseLibPackageDir = Join-Path $env:USERPROFILE ".nuget\packages\alchyr.sts2.baselib\$baseLibPackageVersion"
$baseLibPackageFiles = @(
    (Join-Path $baseLibPackageDir "lib\net9.0\BaseLib.dll"),
    (Join-Path $baseLibPackageDir "Content\BaseLib.json"),
    (Join-Path $baseLibPackageDir "Content\BaseLib.pck")
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

foreach ($file in $baseLibPackageFiles) {
    if (-not (Test-Path -LiteralPath $file -PathType Leaf)) {
        throw "BaseLib package artifact missing: $file"
    }
}

New-Item -ItemType Directory -Force -Path $baseLibOutputDir | Out-Null
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

Copy-Item -LiteralPath (Join-Path $baseLibPackageDir "lib\net9.0\BaseLib.dll") -Destination (Join-Path $baseLibOutputDir "BaseLib.dll") -Force
Copy-Item -LiteralPath (Join-Path $baseLibPackageDir "Content\BaseLib.json") -Destination (Join-Path $baseLibOutputDir "BaseLib.json") -Force
Copy-Item -LiteralPath (Join-Path $baseLibPackageDir "Content\BaseLib.pck") -Destination (Join-Path $baseLibOutputDir "BaseLib.pck") -Force
Copy-Item -LiteralPath (Join-Path $managedOutputDir "FlandreMod.dll") -Destination (Join-Path $modOutputDir "flandremod.dll") -Force
Copy-Item -LiteralPath (Join-Path $managedOutputDir "FlandreMod.pdb") -Destination (Join-Path $modOutputDir "flandremod.pdb") -Force
Copy-Item -LiteralPath $manifestPath -Destination (Join-Path $modOutputDir "mod_manifest.json") -Force
Copy-Item -LiteralPath $pckPath -Destination (Join-Path $modOutputDir "flandremod.pck") -Force

$installedPckPath = Join-Path $modOutputDir "flandremod.pck"
$installedPck = Get-Item -LiteralPath $installedPckPath
$sourcePck = Get-Item -LiteralPath $pckPath
$verification = Get-InstallVerificationSummary -ProjectRoot $projectAbs -ManagedOutputDir $managedOutputDir -ModOutputDir $modOutputDir -SourcePckPath $pckPath -InstalledPckPath $installedPckPath -RequiredRepoFiles $requiredRepoFiles

[pscustomobject]@{
    ModDir = $modOutputDir
    Configuration = $Configuration
    SourcePckLength = $sourcePck.Length
    InstalledPckLength = $installedPck.Length
    InstalledPckLastWriteTime = $installedPck.LastWriteTime
    ManagedOutputDir = $managedOutputDir
    BaseLibDir = $baseLibOutputDir
    BaseLibVersion = $baseLibPackageVersion
} | Format-List

""
"Install verification:"
$verification | ConvertTo-Json -Depth 5
