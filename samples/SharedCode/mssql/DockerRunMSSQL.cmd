docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=sa(!)Password" -p 1433:1433 -d --name mssql-2019 mcr.microsoft.com/mssql/server:2019-latest
pause