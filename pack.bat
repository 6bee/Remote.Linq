@echo off
set configuration=Debug
set version-suffix="001"
clean ^
  && dotnet restore ^
  && dotnet build src\Remote.Linq --configuration %configuration% ^
  && dotnet build src\Remote.Linq.Newtonsoft.Json --configuration %configuration% ^
  && dotnet build test\Remote.Linq.Tests --configuration %configuration% ^
  && dotnet test test\Remote.Linq.Tests\Remote.Linq.Tests.csproj --configuration %configuration% ^
  && dotnet pack src\Remote.Linq --output "..\..\artifacts" --configuration %configuration% --include-symbols --version-suffix %version-suffix% ^
  && dotnet pack src\Remote.Linq.Newtonsoft.Json --output "..\..\artifacts" --configuration %configuration% --include-symbols --version-suffix %version-suffix%