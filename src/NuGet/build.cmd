@ECHO OFF
%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\msbuild "%~dp0\NuGet.proj" /v:minimal /maxcpucount /nodeReuse:false %*
pause