@echo off
echo Building ObfuTool...
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
if %errorlevel% neq 0 (
    echo Build failed!
    exit /b %errorlevel%
)
echo Creating distribution package...
if not exist dist mkdir dist
copy /Y bin\Release\net8.0-windows\win-x64\publish\ObfuTool.exe dist\ObfuTool.exe
if %errorlevel% neq 0 (
    echo Copy failed!
    exit /b %errorlevel%
)
echo Creating Zip...
powershell -Command "Compress-Archive -Path 'bin\Release\net8.0-windows\win-x64\publish\*' -DestinationPath 'dist\ObfuTool_Portable.zip' -Force"
echo Portable package: %CD%\dist\ObfuTool_Portable.zip
echo Done.
