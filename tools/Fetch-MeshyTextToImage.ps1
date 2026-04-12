param(
    [string[]]$TaskId,
    [string]$OutDir = "assets_tmp/meshy/images",
    [string]$ApiKey = $env:MESHY_API_KEY,
    [int]$PageSize = 20,
    [switch]$UseRecentList
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Require-ApiKey {
    param([string]$Key)
    if ([string]::IsNullOrWhiteSpace($Key)) {
        throw "Meshy API key is missing. Set MESHY_API_KEY or pass -ApiKey."
    }
}

function New-Headers {
    param([string]$Key)
    return @{
        "Authorization" = "Bearer $Key"
        "Accept" = "application/json"
    }
}

function Safe-FileName {
    param([string]$Name)
    $invalid = [System.IO.Path]::GetInvalidFileNameChars()
    $chars = $Name.ToCharArray() | ForEach-Object {
        if ($invalid -contains $_) { "_" } else { $_ }
    }
    return (-join $chars)
}

function Resolve-TaskIds {
    param(
        [string[]]$ExplicitIds,
        [hashtable]$Headers,
        [int]$Size
    )

    if ($ExplicitIds -and $ExplicitIds.Count -gt 0) {
        return $ExplicitIds
    }

    if (-not $UseRecentList) {
        throw "No task ids provided. Pass -TaskId <id...> or add -UseRecentList."
    }

    $uri = "https://api.meshy.ai/openapi/v1/text-to-image?page_size=$Size&sort_by=-created_at"
    $list = Invoke-RestMethod -Method Get -Uri $uri -Headers $Headers
    if (-not $list) {
        throw "No tasks returned from Meshy text-to-image list endpoint."
    }

    return @($list | Where-Object { $_.status -eq "SUCCEEDED" } | ForEach-Object { $_.id })
}

function Backup-IfExists {
    param(
        [string]$Path,
        [string]$ProjectRoot
    )
    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        return
    }

    $backupRoot = Join-Path $ProjectRoot "assets_tmp/backups/images"
    & "$ProjectRoot\tools\Backup-ImageAsset.ps1" -SourcePath $Path -ProjectRoot $ProjectRoot -BackupRoot $backupRoot | Out-Null
}

Require-ApiKey -Key $ApiKey
$headers = New-Headers -Key $ApiKey
$projectRoot = (Get-Location).Path
$destRoot = Join-Path $projectRoot $OutDir
New-Item -ItemType Directory -Path $destRoot -Force | Out-Null

$taskIds = Resolve-TaskIds -ExplicitIds $TaskId -Headers $headers -Size $PageSize
if (-not $taskIds -or $taskIds.Count -eq 0) {
    throw "No SUCCEEDED text-to-image tasks found."
}

$results = @()
foreach ($id in $taskIds) {
    $uri = "https://api.meshy.ai/openapi/v1/text-to-image/$id"
    $task = Invoke-RestMethod -Method Get -Uri $uri -Headers $headers

    if ($task.status -ne "SUCCEEDED") {
        Write-Output "Skip $id (status=$($task.status))"
        continue
    }

    if (-not $task.image_urls -or $task.image_urls.Count -eq 0) {
        Write-Output "Skip $id (no image_urls)"
        continue
    }

    $index = 0
    foreach ($imageUrl in $task.image_urls) {
        $index++
        $ext = ".png"
        try {
            $uriObj = [System.Uri]$imageUrl
            $pathExt = [System.IO.Path]::GetExtension($uriObj.AbsolutePath)
            if (-not [string]::IsNullOrWhiteSpace($pathExt)) {
                $ext = $pathExt
            }
        } catch {
            # keep default
        }

        $baseName = "{0}_{1:D2}" -f (Safe-FileName -Name $id), $index
        $outPath = Join-Path $destRoot ($baseName + $ext)
        Backup-IfExists -Path $outPath -ProjectRoot $projectRoot
        Invoke-WebRequest -Uri $imageUrl -OutFile $outPath

        $results += [PSCustomObject]@{
            task_id = $id
            index = $index
            path = $outPath
            prompt = $task.prompt
            created_at = $task.created_at
        }
    }
}

if ($results.Count -eq 0) {
    throw "No images were downloaded."
}

$results | Format-Table -AutoSize | Out-String | Write-Output
