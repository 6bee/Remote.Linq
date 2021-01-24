@echo off
for /r "%~dp0samples\" %%f in (DemoStartUp.csproj) do @if exist "%%f" dotnet run -p "%%f"
echo.
echo ALL DONE.
pause