@echo off
for /r "%~dp0samples\" %%f in (DemoStartUp.csproj) do @if exist "%%f" echo. && echo. && echo "%%f" && dotnet run --project "%%f" --non-interactive
echo.
echo ALL DONE.
pause