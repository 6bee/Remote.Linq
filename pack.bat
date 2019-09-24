@echo off
set configuration=Debug
set version-suffix="alpha-001"
clean ^
  && dotnet restore ^
  && dotnet build src\Remote.Linq                           --configuration %configuration% --version-suffix "%version-suffix%" ^
  && dotnet build src\Remote.Linq.EntityFramework           --configuration %configuration% --version-suffix "%version-suffix%" ^
  && dotnet build src\Remote.Linq.EntityFrameworkCore       --configuration %configuration% --version-suffix "%version-suffix%" ^
  && dotnet build src\Remote.Linq.Newtonsoft.Json           --configuration %configuration% --version-suffix "%version-suffix%" ^
  && dotnet test test\Remote.Linq.Tests                     --configuration %configuration% ^
  && dotnet test test\Remote.Linq.EntityFrameworkCore.Tests --configuration %configuration% ^
  && dotnet test test\Remote.Linq.EntityFramework.Tests     --configuration %configuration% ^
  && dotnet pack src\Remote.Linq                            --configuration %configuration% --version-suffix "%version-suffix%" --include-symbols --include-source --output "artifacts" ^
  && dotnet pack src\Remote.Linq.Newtonsoft.Json            --configuration %configuration% --version-suffix "%version-suffix%" --include-symbols --include-source --output "artifacts" ^
  && dotnet pack src\Remote.Linq.EntityFramework            --configuration %configuration% --version-suffix "%version-suffix%" --include-symbols --include-source --output "artifacts" ^
  && dotnet pack src\Remote.Linq.EntityFrameworkCore        --configuration %configuration% --version-suffix "%version-suffix%" --include-symbols --include-source --output "artifacts"
