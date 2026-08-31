@echo off
title Iniciar Desarrollo - TourismTracking
color 0A

echo ========================================================
echo Iniciando Entorno de Desarrollo - TourismTracking
echo ========================================================
echo.

:: 1. Iniciar Backend (ASP.NET Core) en una nueva ventana
echo [1/3] Iniciando Backend (.NET Core)...
start "Backend - API" cmd /k "cd /d "%~dp0aspnet-core\src\TourismTracking.HttpApi.Host" && dotnet run"

:: Esperar un par de segundos para asegurar que el backend levante
timeout /t 5 /nobreak >nul

:: 2. Iniciar Frontend (Angular) en otra ventana
echo [2/3] Iniciando Frontend (Angular)...
start "Frontend - Angular" cmd /k "cd /d "%~dp0angular" && npm start"

:: 3. Abrir el navegador
echo [3/3] Abriendo el navegador...
timeout /t 10 /nobreak >nul
start http://localhost:4200

echo.
echo Todo listo! Puedes cerrar esta ventana.
echo Para detener los servidores, cierra las ventanas negras (Backend y Frontend) que se abrieron.
pause
