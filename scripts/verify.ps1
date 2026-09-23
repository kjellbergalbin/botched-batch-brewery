$ErrorActionPreference = "Stop"

if (-not $env:GODOT4) {
    throw "Set GODOT4 to the Godot 4.7.2 .NET executable."
}

function Invoke-GodotGate {
    param(
        [Parameter(Mandatory = $true)][string]$Label,
        [Parameter(ValueFromRemainingArguments = $true)][string[]]$Arguments
    )

    Write-Host "==> $Label"
    $output = & $env:GODOT4 @Arguments 2>&1
    $output | ForEach-Object { Write-Host $_ }

    if ($LASTEXITCODE -ne 0) {
        throw "$Label exited with code $LASTEXITCODE."
    }

    if ($output -match "SCRIPT ERROR:|^ERROR:") {
        throw "$Label reported an engine or script error."
    }
}

Write-Host "==> Dependency restore"
dotnet restore BotchedBatchBrewery.sln --locked-mode --nologo

Write-Host "==> C# format"
dotnet format BotchedBatchBrewery.sln --verify-no-changes --no-restore

Write-Host "==> C# build"
dotnet build BotchedBatchBrewery.sln --no-restore --nologo

Invoke-GodotGate -Label "Godot import" -Arguments @("--headless", "--editor", "--path", ".", "--import", "--quit")
Invoke-GodotGate -Label "Godot main-scene smoke" -Arguments @("--headless", "--path", ".", "--quit-after", "5")

Write-Host "All verification gates passed."
