@echo off
set configuration=Release
clean ^
  && dotnet restore ^
  && dotnet build src\Remote.Linq                           --configuration %configuration% ^
  && dotnet build src\Remote.Linq.EntityFramework           --configuration %configuration% ^
  && dotnet build src\Remote.Linq.EntityFrameworkCore       --configuration %configuration% ^
  && dotnet build src\Remote.Linq.Newtonsoft.Json           --configuration %configuration% ^
  && dotnet test test\Remote.Linq.Tests                     --configuration %configuration% ^
  && dotnet test test\Remote.Linq.EntityFrameworkCore.Tests --configuration %configuration% ^
  && dotnet test test\Remote.Linq.EntityFramework.Tests     --configuration %configuration% ^
  && dotnet pack src\Remote.Linq                            --configuration %configuration% --include-symbols --include-source --output "artifacts" ^
  && dotnet pack src\Remote.Linq.Newtonsoft.Json            --configuration %configuration% --include-symbols --include-source --output "artifacts" ^
  && dotnet pack src\Remote.Linq.EntityFramework            --configuration %configuration% --include-symbols --include-source --output "artifacts" ^
  && dotnet pack src\Remote.Linq.EntityFrameworkCore        --configuration %configuration% --include-symbols --include-source --output "artifacts"
