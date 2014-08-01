@ECHO OFF
%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\msbuild "%~dp0\Remote.Linq.msbuild" /v:minimal /maxcpucount /nodeReuse:false /property:VisualStudioVersion=12.0 %*
pause