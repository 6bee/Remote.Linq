sqlcmd -S . -U sa -P "sa(!)Password" -i "%~dp0\RemoteQueryableDemoDB.sql"
pause