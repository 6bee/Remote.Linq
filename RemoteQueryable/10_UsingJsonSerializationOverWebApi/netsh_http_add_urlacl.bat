@echo off
netsh http add urlacl url=http://+:8089/ user=%USERDOMAIN%\%USERNAME%
netsh http show urlacl url=http://+:8089/
pause