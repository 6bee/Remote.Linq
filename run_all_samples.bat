@echo off
echo BUILD FRAMEWORK
dotnet build "%~dp0Remote.Linq.sln" -p GeneratePackageOnBuild=true > nul
echo RUN SAMPLES
for /r "%~dp0samples\" %%f in (DemoStartUp.csproj) do @if exist "%%f" dotnet run --project "%%f" --non-interactive
echo.
echo ALL DONE.
pause