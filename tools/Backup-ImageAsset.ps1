param(
    [Parameter(Mandatory = $true)]
    [string]$SourcePath,
    [string]$ProjectRoot = (Get-Location).Path,
    [string]$BackupRoot = "assets_tmp/backups/images"
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
$sourceAbs = Resolve-AbsolutePath -Path $SourcePath

if (-not (Test-Path -LiteralPath $sourceAbs -PathType Leaf)) {
    throw "Source image not found: $sourceAbs"
}

if (-not $sourceAbs.StartsWith($projectAbs, [System.StringComparison]::OrdinalIgnoreCase)) {
    throw "Source image must be under project root.`nproject=$projectAbs`nsource=$sourceAbs"
}

$relativePath = $sourceAbs.Substring($projectAbs.Length).TrimStart('\', '/')
$timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
$targetDir = Resolve-AbsolutePath -Path (Join-Path $BackupRoot $timestamp)
$targetPath = Join-Path $targetDir $relativePath
$targetParent = Split-Path -Path $targetPath -Parent

New-Item -ItemType Directory -Force -Path $targetParent | Out-Null
Copy-Item -LiteralPath $sourceAbs -Destination $targetPath -Force

Write-Output "Backed up '$relativePath' -> '$targetPath'"
