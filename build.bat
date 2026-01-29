@echo off
setlocal
set SCRIPT_DIR=%~dp0
cd /d "%SCRIPT_DIR%"
set APP_NAME=ObfuTool
set RID=win-x64
set CONFIG=Release
dotnet publish -c %CONFIG% -r %RID% --self-contained true /p:PublishSingleFile=true
if errorlevel 1 goto error
set PUBDIR=%SCRIPT_DIR%bin\%CONFIG%\net8.0-windows\%RID%\publish
set DIST=%SCRIPT_DIR%dist
if not exist "%DIST%" mkdir "%DIST%"
robocopy "%PUBDIR%" "%DIST%\portable" /MIR >nul
copy /y "%PUBDIR%\%APP_NAME%.exe" "%DIST%\%APP_NAME%.exe" >nul
powershell -NoProfile -Command "Compress-Archive -Path '%DIST%\portable\*' -DestinationPath '%DIST%\ObfuTool_Portable.zip' -Force"
echo Portable package: %DIST%\ObfuTool_Portable.zip
exit /b 0
:error
echo Build failed
exit /b 1
