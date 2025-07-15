@echo off
where dotnet >nul 2>nul || (
  echo .NET SDK not found. Please install it from https://dotnet.microsoft.com/en-us/download
  pause
  exit /b
)

dotnet publish -c Release -r win-x64 --self-contained true
copy /Y ".\bin\Release\net9.0-windows\win-x64\publish\SCAddonInstaller.exe" "%~dp0SCAddonInstaller.exe"

CertUtil -hashfile "%~dp0SCAddonInstaller.exe" SHA256 > "%~dp0SCAddonInstaller.exe.sha256.txt"
echo Build complete. The executable is located at "%~dp0SCAddonInstaller.exe".
type "%~dp0SCAddonInstaller.exe.sha256.txt"

pause
