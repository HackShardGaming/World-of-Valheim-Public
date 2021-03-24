@echo off
REM This is a script to copy the built DLL to a valheim client/server 
REM  for testing. Chenge the path to what you need for testing.
REM  
REM Note: git will ignore changes
REM

REM Predetermine Paths
set CUR_DIR=%~dp0
set MOD_NAME=ValheimOnline
set MOD_DIR=%CUR_DIR%\%MOD_NAME%
set MOD_DLL=%MOD_DIR%\bin\Debug\%MOD_NAME%.dll
set PLUGIN_DIR=BepInEx\plugins

REM Development specific variables
set SERVER_DIR=C:\Games\Valheim_servers\server1
set PLAYER_DIR=C:\Program Files (x86)\Steam\steamapps\common\Valheim

REM Commence with the copying

REM Send the mod to a server for testing.
if exist "%SERVER_DIR%\%PLUGIN_DIR%" (
	echo "%MOD_DLL% -> %SERVER_DIR%\%PLUGIN_DIR%\%MOD_NAME%.dll"
	copy "%MOD_DLL%" "%SERVER_DIR%\%PLUGIN_DIR%\%MOD_NAME%.dll"
) else (
	echo "WARNING: Output Directory Does not exist."
)

REM Send the mod to the client for testing.
if exist "%PLAYER_DIR%\%PLUGIN_DIR%" (
	echo "%MOD_DLL% -> %PLAYER_DIR%\%PLUGIN_DIR%\%MOD_NAME%.dll"
	copy "%MOD_DLL%" "%PLAYER_DIR%\%PLUGIN_DIR%\%MOD_NAME%.dll"
) else (
	echo "WARNING: Output Directory Does not exist."
)
