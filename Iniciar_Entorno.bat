@echo off
setlocal
title TourismTracking - Modo Desarrollo Completo
color 0A

cd /d "%~dp0"

echo ========================================================
echo Iniciando Entorno de Desarrollo - TourismTracking
echo ========================================================
echo.

:: 1. Iniciar instancia SQL Server LocalDB si está disponible
echo [1/5] Verificando SQL Server LocalDB (MSSQLLocalDB)...
set "LOCALDB_CMD=sqllocaldb"
where sqllocaldb >nul 2>&1
if errorlevel 1 (
  for /f "delims=" %%F in ('dir /b /s "C:\Program Files\Microsoft SQL Server\SqlLocalDB.exe" 2^>nul') do set "LOCALDB_CMD=%%F"
)
"%LOCALDB_CMD%" start MSSQLLocalDB >nul 2>&1

:: 2. Instalar dependencias de Angular si faltan (clonacion limpia)
echo [2/5] Verificando dependencias de Angular...
if not exist "%~dp0angular\node_modules" (
  echo Instalando paquetes npm en angular...
  pushd "%~dp0angular"
  call npm install
  if errorlevel 1 (
    popd
    echo Error al instalar dependencias de Angular.
    pause
    exit /b 1
  )
  popd
)

:: 3. Aplicar migraciones y seed idempotente (DbMigrator)
echo [3/5] Aplicando migraciones y datos iniciales (DbMigrator)...
pushd "%~dp0aspnet-core\src\TourismTracking.DbMigrator"
dotnet run
if errorlevel 1 (
  popd
  echo Error al ejecutar DbMigrator. Verifica que SQL Server LocalDB este instalado.
  pause
  exit /b 1
)
popd

:: 4. Iniciar Backend (ASP.NET Core) en una nueva ventana
echo [4/5] Levantando Backend (.NET Core)...
start "Backend - API" /D "%~dp0aspnet-core\src\TourismTracking.HttpApi.Host" cmd /k "dotnet run"

:: 5. Iniciar Frontend (Angular) en otra ventana
echo [5/5] Levantando Frontend (Angular ng serve)...
start "Frontend - Angular" /D "%~dp0angular" cmd /k "npm start"

:: Esperar activamente a que los servicios esten listos
echo.
echo Sincronizando servicios antes de abrir el navegador...
where python >nul 2>&1
if not errorlevel 1 (
  python "%~dp0angular\wait_ready.py"
)

:: Abrir el navegador
start http://localhost:4200

echo.
echo ========================================================
echo  Todo listo! Puedes cerrar esta ventana.
echo  Para detener los servidores, cierra las 2 ventanas negras.
echo ========================================================
echo.
pause
