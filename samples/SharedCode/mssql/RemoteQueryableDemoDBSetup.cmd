sqlcmd -S . -U sa -P "sa(!)Password" -i "%~dp0RemoteQueryableDemoDB.sql"
pause