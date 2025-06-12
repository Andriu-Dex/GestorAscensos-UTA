@echo off
echo Iniciando Sistema de Gestion de Ascensos...

echo Iniciando API...
start "SGA API" cmd /k "cd /d %~dp0SGA.Api && dotnet run"

timeout /t 3

echo Iniciando Cliente Blazor...
start "SGA Cliente" cmd /k "cd /d %~dp0SGA.BlazorClient && dotnet run"

echo.
echo Ambos proyectos iniciados en ventanas separadas.
echo API: https://localhost:7126
echo Cliente: Revisa la ventana del cliente para ver el puerto
pause
