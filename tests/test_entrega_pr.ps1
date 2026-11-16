# tests/test_entrega_pr.ps1 - Contract tests for M2
$ErrorActionPreference = 'Stop'
$repoRoot = (git rev-parse --show-toplevel).Trim()
$guideFile = Join-Path $repoRoot 'docs/ENTREGA_PR_GUIA.md'

Write-Output "[TEST] Iniciando verificacion de M2: Guia de Entrega PR..."

if (-not (Test-Path -LiteralPath $guideFile)) {
    Write-Error "Fallo: No existe el documento $guideFile"
    exit 1
}

$content = Get-Content -LiteralPath $guideFile -Raw

$requiredTerms = @(
    'Settings',
    'Collaborators',
    'Write',
    'dev',
    'prod',
    'Reviewer'
)

foreach ($term in $requiredTerms) {
    if ($content -notmatch [regex]::Escape($term)) {
        Write-Error "Fallo: La guia no contiene el termino o seccion obligatoria '$term'"
        exit 1
    }
}

Write-Output "[PASS] Test-DeliveryPR-ChecklistAndIntegrity OK"
exit 0
