# Script de PowerShell para conversión automática de HTML a PDF usando Google Chrome
$ErrorActionPreference = "Stop"

# 1. Definir Rutas
$htmlPath = "c:\Users\thiag\Desktop\Files\Facu\FACU 2024\Desarrollo de Software\Proyecto Final\Documentacion\Manual_Maestro_Exequiel.html"
$pdfPath = "c:\Users\thiag\Desktop\Files\Facu\FACU 2024\Desarrollo de Software\Proyecto Final\Documentacion\Originales\Manual_Maestro_Exequiel.pdf"
$downloadsFolder = "C:\Users\thiag\Downloads\Entregable_Exequiel"

Write-Output "--- Iniciando conversión de HTML a PDF ---"

# 2. Localizar Google Chrome
$chromePaths = @(
    "C:\Program Files\Google\Chrome\Application\chrome.exe",
    "C:\Program Files (x86)\Google\Chrome\Application\chrome.exe"
)

$chromeExe = $null
foreach ($path in $chromePaths) {
    if (Test-Path $path) {
        $chromeExe = $path
        break
    }
}

if ($null -eq $chromeExe) {
    # Intentar buscar por el registro de Windows
    try {
        $regPath = Get-ItemProperty -Path "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe" -ErrorAction SilentlyContinue
        if ($null -ne $regPath -and (Test-Path $regPath.Path)) {
            $chromeExe = $regPath.Path
        }
    } catch {}
}

if ($null -eq $chromeExe) {
    Write-Error "No se pudo localizar el ejecutable de Google Chrome en este sistema. Por favor, instala Chrome para continuar."
    exit 1
}

Write-Output "Google Chrome encontrado en: $chromeExe"

# 3. Ejecutar Conversión Headless de Chrome
Write-Output "Generando PDF de alta definición..."

$arguments = @(
    "--headless=new",
    "--no-sandbox",
    "--disable-gpu",
    "--print-to-pdf=`"$pdfPath`"",
    "--include-background",
    "`"$htmlPath`""
)

$process = Start-Process -FilePath $chromeExe -ArgumentList $arguments -PassThru -Wait

if ($process.ExitCode -ne 0) {
    Write-Error "La ejecución de Google Chrome falló con el código de salida: $($process.ExitCode)"
    exit 1
}

if (-not (Test-Path $pdfPath)) {
    Write-Error "No se generó el archivo PDF esperado."
    exit 1
}

Write-Output "¡PDF generado con éxito en el repositorio: $pdfPath!"

# 4. Copiar al Entregable de Descargas para Exequiel
if (Test-Path $downloadsFolder) {
    $targetDownloadsPdf = Join-Path $downloadsFolder "Manual_Maestro_Exequiel.pdf"
    $targetDownloadsHtml = Join-Path $downloadsFolder "Manual_Maestro_Exequiel.html"
    
    # Copiar PDF
    Copy-Item -Path $pdfPath -Destination $targetDownloadsPdf -Force
    Write-Output "¡PDF copiado con éxito a Descargas: $targetDownloadsPdf!"
    
    # Copiar HTML (el todo-en-uno visual)
    Copy-Item -Path $htmlPath -Destination $targetDownloadsHtml -Force
    Write-Output "¡HTML copiado con éxito a Descargas: $targetDownloadsHtml!"
} else {
    Write-Warning "La carpeta de descargas de Exequiel ($downloadsFolder) no existe, no se copió el entregable allí."
}

Write-Output "--- Proceso completado exitosamente ---"
