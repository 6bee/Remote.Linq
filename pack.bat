@echo off
set configuration=Debug
clean ^
  && dotnet test test\Remote.Linq.Tests                     --configuration %configuration% ^
  && dotnet test test\Remote.Linq.Async.Queryable.Tests     --configuration %configuration% ^
  && dotnet test test\Remote.Linq.EntityFrameworkCore.Tests --configuration %configuration% ^
  && dotnet test test\Remote.Linq.EntityFramework.Tests     --configuration %configuration% ^
  && dotnet pack src\Remote.Linq                            --configuration %configuration% ^
  && dotnet pack src\Remote.Linq.Async.Queryable            --configuration %configuration% ^
  && dotnet pack src\Remote.Linq.EntityFrameworkCore        --configuration %configuration% ^
  && dotnet pack src\Remote.Linq.EntityFramework            --configuration %configuration% ^
  && dotnet pack src\Remote.Linq.Newtonsoft.Json            --configuration %configuration% ^
  && dotnet pack src\Remote.Linq.protobuf-net               --configuration %configuration%
