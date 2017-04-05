@echo off
clean ^
  && dotnet restore ^
  && dotnet build src\Remote.Linq ^
  && dotnet build src\Remote.Linq.Newtonsoft.Json ^
  && dotnet build test\Remote.Linq.Tests ^
  && dotnet test test\Remote.Linq.Tests\Remote.Linq.Tests.csproj ^
  && dotnet pack src\Remote.Linq --output "..\..\artifacts" --configuration Debug --include-symbols --version-suffix 026 ^
  && dotnet pack src\Remote.Linq.Newtonsoft.Json --output "..\..\artifacts" --configuration Debug --include-symbols --version-suffix 026