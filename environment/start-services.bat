@echo off
cd /d "%~dp0"
docker-compose up -d
if %ERRORLEVEL% neq 0 (
    echo Cannot start containers
    pause
    exit /b %ERRORLEVEL%
)
echo Containers started
