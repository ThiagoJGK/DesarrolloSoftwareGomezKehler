# tests/test_e2e_restore.ps1 - Contract tests for M3
$ErrorActionPreference = 'Stop'
$repoRoot = (git rev-parse --show-toplevel).Trim()
$scriptFile = Join-Path $repoRoot 'scripts/verify_clean_restore.ps1'

Write-Output "[TEST] Iniciando verificacion de M3: Script E2E de Restauracion Limpia..."

if (-not (Test-Path -LiteralPath $scriptFile)) {
    Write-Error "Fallo: No existe el script $scriptFile"
    exit 1
}

$output = & powershell -NoProfile -ExecutionPolicy Bypass -File $scriptFile 2>&1
$code = $LASTEXITCODE

if ($code -ne 0) {
    Write-Error "Fallo: verify_clean_restore.ps1 retorno codigo $code`n$($output -join "`n")"
    exit 1
}

if (($output -join "`n") -notmatch 'RESTORE_AND_BUILD_SUCCESS') {
    Write-Error "Fallo: El script no emitio la cadena de exito canocica 'RESTORE_AND_BUILD_SUCCESS'"
    exit 1
}

Write-Output "[PASS] Test-E2E-CleanRestoreAndBuild OK"
exit 0
