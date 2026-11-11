# tests/run_all_contracts.ps1 - Suite global de contratos
$ErrorActionPreference = 'Continue'
$repoRoot = (git rev-parse --show-toplevel).Trim()

$testFiles = @(
    'tests/test_nuget_config.ps1',
    'tests/test_entrega_pr.ps1',
    'tests/test_e2e_restore.ps1'
)

$anyFail = $false

foreach ($tf in $testFiles) {
    $fullPath = Join-Path $repoRoot $tf
    if (Test-Path -LiteralPath $fullPath) {
        Write-Output "==> Ejecutando $tf..."
        & powershell -NoProfile -ExecutionPolicy Bypass -File $fullPath
        if ($LASTEXITCODE -ne 0) {
            $anyFail = $true
            Write-Warning "Fallo en $tf (ExitCode: $LASTEXITCODE)"
        }
    } else {
        Write-Warning "No existe $tf todavia."
        $anyFail = $true
    }
}

if ($anyFail) {
    Write-Output "CONTRATOS_FAIL"
    exit 1
} else {
    Write-Output "CONTRATOS_OK"
    exit 0
}
