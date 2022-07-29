@echo OFF
dotnet publish -c Release -r ubuntu.20.04-x64  -o bin/Publish --self-contained true
if %ERRORLEVEL% NEQ 0 EXIT /B 1 
docker build . -t joebart457/web-compiler-server:0.0.1