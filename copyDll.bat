@echo off
set CUR_DIR=%~dp0

set MOD_NAME=ValheimOnline
set MOD_DIR=%CUR_DIR%\%MOD_NAME%

set SERVER_DIR=C:\Games\Valheim_servers\server1
set PLAYER_DIR=C:\Program Files (x86)\Steam\steamapps\common\Valheim

set MOD_DLL=%MOD_DIR%\bin\Debug\%MOD_NAME%.dll

set PLUGIN_DIR=BepInEx\plugins

REM Send the mod to a server for testing.
if exist "%SERVER_DIR%\%PLUGIN_DIR%\%MOD_NAME%.dll" (
	echo "%MOD_DLL% -> %SERVER_DIR%\%PLUGIN_DIR%\%MOD_NAME%.dll"
	copy "%MOD_DLL%" "%SERVER_DIR%\%PLUGIN_DIR%\%MOD_NAME%.dll"
) else (
  echo "WARNING: Output Directory Does not exist."
)

REM Send the mod to the client for testing.
if exist "%PLAYER_DIR%\%PLUGIN_DIR%\%MOD_NAME%.dll" (
	echo "%MOD_DLL% -> %PLAYER_DIR%\%PLUGIN_DIR%\%MOD_NAME%.dll"
	copy "%MOD_DLL%" "%PLAYER_DIR%\%PLUGIN_DIR%\%MOD_NAME%.dll"
) else (
  echo "WARNING: Output Directory Does not exist."
)
