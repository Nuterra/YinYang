@ECHO OFF

echo Killing existing server
taskkill /IM YinYang.exe /T /F 2>nul

echo Starting new server
start "" /D ..\YinYang\bin\Debug "../YinYang/bin/Debug/YinYang.exe"
ping -n 3 127.0.0.1 >nul

echo Starting Script %1
casperjs %1

taskkill /IM YinYang.exe /T /F 2>nul