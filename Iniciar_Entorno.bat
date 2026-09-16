@echo off
setlocal
title TourismTracking - Modo Desarrollo Completo
color 0A

cd /d "%~dp0"

echo ========================================================
echo Iniciando Entorno de Desarrollo - TourismTracking
echo ========================================================
echo.

:: 1. Iniciar Backend (ASP.NET Core) en una nueva ventana
echo [1/3] Levantando Backend (.NET Core)...
start "Backend - API" /D "%~dp0aspnet-core\src\TourismTracking.HttpApi.Host" cmd /k "dotnet run"

:: 2. Iniciar Frontend (Angular) en otra ventana
echo [2/3] Levantando Frontend (Angular ng serve)...
start "Frontend - Angular" /D "%~dp0angular" cmd /k "npm start"

:: 3. Esperar activamente a que los servicios esten listos
echo.
echo [3/3] Sincronizando servicios antes de abrir el navegador...
python "%~dp0angular\wait_ready.py"

:: 4. Abrir el navegador
start http://localhost:4200

echo.
echo ========================================================
echo  Todo listo! Puedes cerrar esta ventana.
echo  Para detener los servidores, cierra las 2 ventanas negras.
echo ========================================================
echo.
pause
