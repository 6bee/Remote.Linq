for /d /r . %%d in (packages,bin,obj) do @if exist "%%d" rd /s /q "%%d"
del /s /q "*.csproj.user" "packages.config"