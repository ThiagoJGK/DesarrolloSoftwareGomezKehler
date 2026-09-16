@echo off
setlocal
title TourismTracking - Modo Ultraliviano
color 0B

cd /d "%~dp0"

echo =======================================================================
echo     Iniciando Entorno - TourismTracking (MODO ULTRALIVIANO)
echo     Ahorro de RAM: ~3.5 GB a 4.0 GB ^| Maximo Rendimiento
echo =======================================================================
echo.

:: 1. Iniciar Backend (.NET Core) en modo optimizado de bajo consumo
echo [1/3] Levantando Backend (.NET Core)...
start "Backend - API (Modo Ligero)" /D "%~dp0aspnet-core\src\TourismTracking.HttpApi.Host" cmd /k "dotnet run -- --lightweight"

:: 2. Iniciar Frontend usando servidor estatico ultraliviano (~20 MB RAM)
echo [2/3] Levantando Frontend (Servidor Ultraliviano)...
start "Frontend - Angular (Modo Ligero)" /D "%~dp0angular" cmd /k "python serve_lightweight.py"

:: 3. Esperar activamente a que ambos servicios esten listos y respondiendo
echo.
echo [3/3] Sincronizando servicios antes de abrir el navegador...
python "%~dp0angular\wait_ready.py"

:: 4. Abrir el navegador exactamente cuando el sistema ya esta online
start http://localhost:4200

echo.
echo =======================================================================
echo  La aplicacion ya esta abierta y funcionando.
echo  Para detenerla cuando termines, cierra las 2 ventanas de comandos.
echo =======================================================================
echo.
pause
