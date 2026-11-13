param(
    [string]$SolutionPath = "aspnet-core/TourismTracking.sln",
    [string]$ConfigPath = "aspnet-core/NuGet.Config"
)

$repoRoot = Split-Path -Parent $PSScriptRoot
if (-not (Test-Path -Path $ConfigPath -PathType Leaf)) {
    $fallbackConfig = Join-Path $repoRoot $ConfigPath
    if (Test-Path -Path $fallbackConfig -PathType Leaf) {
        $ConfigPath = $fallbackConfig
    }
}
if (-not (Test-Path -Path $SolutionPath -PathType Leaf)) {
    $fallbackSolution = Join-Path $repoRoot $SolutionPath
    if (Test-Path -Path $fallbackSolution -PathType Leaf) {
        $SolutionPath = $fallbackSolution
    }
}

Write-Host "[1/4] Validating NuGet.Config XML and package sources: $ConfigPath"
if (-not (Test-Path -Path $ConfigPath -PathType Leaf)) {
    Write-Host "ERROR: NuGet.Config not found at '$ConfigPath'."
    exit 1
}

try {
    $rawXml = Get-Content -Path $ConfigPath -Raw -Encoding UTF8 -ErrorAction Stop
    [xml]$nugetXml = $rawXml
}
catch {
    Write-Host "ERROR: Invalid XML in '$ConfigPath': $_"
    exit 1
}

$packageSources = $nugetXml.SelectSingleNode("/configuration/packageSources")
if ($null -eq $packageSources) {
    Write-Host "ERROR: Missing <configuration>/<packageSources> node in '$ConfigPath'."
    exit 1
}

$clearNode = $packageSources.SelectSingleNode("clear")
if ($null -eq $clearNode) {
    Write-Host "ERROR: Missing <clear /> node inside <packageSources> in '$ConfigPath'."
    exit 1
}

$officialFeedUrl = "https://api.nuget.org/v3/index.json"
$addNodes = $packageSources.SelectNodes("add")
$hasOfficialFeed = $false
foreach ($node in $addNodes) {
    if ($node.GetAttribute("value") -eq $officialFeedUrl) {
        $hasOfficialFeed = $true
        break
    }
}

if (-not $hasOfficialFeed) {
    Write-Host "ERROR: Official NuGet v3 feed ('$officialFeedUrl') not found in '$ConfigPath'."
    exit 1
}

Write-Host "[2/4] Running isolated dotnet restore on '$SolutionPath' using '$ConfigPath'..."
& dotnet restore $SolutionPath --configfile $ConfigPath
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: dotnet restore failed with exit code $LASTEXITCODE."
    exit 2
}

Write-Host "[3/4] Running clean dotnet build (Release, --no-restore) on '$SolutionPath'..."
& dotnet build $SolutionPath -c Release --no-restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: dotnet build failed with exit code $LASTEXITCODE."
    exit 3
}

Write-Host "[4/4] Checking git branch alignment (dev vs teamwork-lean/correcciones-docente)..."
$gitCmd = Get-Command git -ErrorAction SilentlyContinue
if ($null -ne $gitCmd) {
    $devHash = & git -C $repoRoot rev-parse --verify dev 2>$null
    $devOk = $LASTEXITCODE -eq 0
    $tlHash = & git -C $repoRoot rev-parse --verify teamwork-lean/correcciones-docente 2>$null
    $tlOk = $LASTEXITCODE -eq 0
    if ($devOk -and $tlOk -and $devHash -and $tlHash) {
        $diffCount = & git -C $repoRoot rev-list --left-right --count "dev...teamwork-lean/correcciones-docente" 2>$null
        Write-Host "Branch divergence (dev vs teamwork-lean/correcciones-docente): $diffCount"
    }
    else {
        Write-Host "One or both target branches (dev, teamwork-lean/correcciones-docente) not present locally; skipping diff check."
    }
}

Write-Output "RESTORE_AND_BUILD_SUCCESS"
exit 0
