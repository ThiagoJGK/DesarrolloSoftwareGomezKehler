# tests/test_nuget_config.ps1 - Contract tests for M1
$ErrorActionPreference = 'Stop'
$repoRoot = (git rev-parse --show-toplevel).Trim()
$nugetConfigFile = Join-Path $repoRoot 'aspnet-core/NuGet.Config'
$solutionFile = Join-Path $repoRoot 'aspnet-core/TourismTracking.sln'

Write-Output "[TEST] Iniciando verificacion de M1: NuGet.Config..."

# 1. Test-NuGetConfig-StructureAndOfficialFeed
if (-not (Test-Path -LiteralPath $nugetConfigFile)) {
    Write-Error "Fallo: No existe el archivo $nugetConfigFile"
    exit 1
}

[xml]$xml = Get-Content -LiteralPath $nugetConfigFile -Raw
$packageSources = $xml.configuration.packageSources
if ($null -eq $packageSources) {
    Write-Error "Fallo: No se encontro el nodo <packageSources> en NuGet.Config"
    exit 1
}

$hasClear = $packageSources.SelectSingleNode("clear")
if ($null -eq $hasClear) {
    Write-Error "Fallo: Falta la directiva de aislamiento <clear /> en <packageSources>"
    exit 1
}

$officialFeed = $packageSources.SelectSingleNode("add[@key='nuget.org']")
if ($null -eq $officialFeed -or $officialFeed.value -ne 'https://api.nuget.org/v3/index.json') {
    Write-Error "Fallo: No se encontro el feed oficial 'nuget.org' con valor 'https://api.nuget.org/v3/index.json'"
    exit 1
}

Write-Output "[PASS] Test-NuGetConfig-StructureAndOfficialFeed OK"

# 2. Test-NuGetConfig-IsolatedRestore
Write-Output "[TEST] Ejecutando dotnet restore con --configfile..."
$restoreOutput = & dotnet restore $solutionFile --configfile $nugetConfigFile 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Error "Fallo: dotnet restore fallo con codigo de salida $LASTEXITCODE`n$($restoreOutput -join "`n")"
    exit 1
}

Write-Output "[PASS] Test-NuGetConfig-IsolatedRestore OK"
exit 0
