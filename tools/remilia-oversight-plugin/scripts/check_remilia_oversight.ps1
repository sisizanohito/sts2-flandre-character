param(
    [string]$Owner = "Remilia",
    [string]$ToolName = "",
    [string]$NextActor = "Gibbs"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Get-HookPayload {
    $raw = [Console]::In.ReadToEnd()
    if ([string]::IsNullOrWhiteSpace($raw)) {
        return $null
    }

    try {
        return $raw | ConvertFrom-Json -Depth 16
    }
    catch {
        return $raw
    }
}

function Get-ActionText {
    param(
        [object]$Payload,
        [string]$FallbackToolName
    )

    if ($Payload -is [string]) {
        $trimmed = $Payload.Trim()
        if ($trimmed.Length -gt 0) {
            return $trimmed
        }
    }

    if ($null -eq $Payload) {
        return $FallbackToolName
    }

    foreach ($key in @("tool_name", "toolName", "name")) {
        if ($Payload.PSObject.Properties.Name -contains $key -and $Payload.$key) {
            return [string]$Payload.$key
        }
    }

    if ($Payload.PSObject.Properties.Name -contains "tool_input" -and $Payload.tool_input) {
        return "$FallbackToolName :: $($Payload.tool_input | ConvertTo-Json -Compress -Depth 8)"
    }

    if ($Payload.PSObject.Properties.Name -contains "arguments" -and $Payload.arguments) {
        return "$FallbackToolName :: $($Payload.arguments | ConvertTo-Json -Compress -Depth 8)"
    }

    return $FallbackToolName
}

function Get-ReasonText {
    param(
        [string]$Tool
    )

    switch -Regex ($Tool) {
        "^shell_command$" { return "Shell execution can bypass seat boundaries. Confirm remit before continuing." }
        "^apply_patch$" { return "Direct file edits can cross issue scope. Require oversight before write-through." }
        "^git_publish$" { return "Git publishing changes branch or PR state. Oversight should confirm intent and scope." }
        "^github_write$" { return "GitHub write actions affect external state. Confirm authority before posting or mutating." }
        default { return "Risky tool activity detected. Oversight check is required before proceeding." }
    }
}

$payload = Get-HookPayload
$action = Get-ActionText -Payload $payload -FallbackToolName $ToolName
$reason = Get-ReasonText -Tool $ToolName
$decision = if ($ToolName -in @("shell_command", "apply_patch", "git_publish", "github_write")) { "not_allowed" } else { "allowed" }

Write-Output "owner: $Owner"
Write-Output "remilia_action: $decision"
Write-Output "reason: $reason :: $action"
Write-Output "next_actor: $NextActor"
