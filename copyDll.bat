@echo off
REM This is a script to copy the built DLL to a valheim client/server 
REM  for testing. Chenge the path to what you need for testing.
REM  
REM Note: git will ignore changes
REM
ECHO "Running CopyDll Script"

REM Input Args
set APP_NAME=%1
set CONFIG=%2

REM Predetermine Paths
set CUR_DIR=%~dp0
set MOD_NAME=%APP_NAME%
set MOD_DIR=%CUR_DIR%\%MOD_NAME%
set MOD_DLL=%MOD_DIR%\bin\%CONFIG%\%MOD_NAME%.dll
set PLUGIN_DIR=BepInEx\plugins
set OUTPUT_DIR=%MOD_DIR%\Output

REM Development specific variables
set SERVER_DIR=C:\Games\Valheim_servers\server1
set PLAYER_DIR=C:\Program Files (x86)\Steam\steamapps\common\Valheim

set "MOD_VER="
    FOR /F "tokens=2 delims==" %%a in ('
        wmic datafile where name^="%MOD_DLL:\=\\%" get Version /value 
    ') do set "MOD_VER=%%a"

echo "VERSION: %MOD_VER%"
echo.

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

if "%CONFIG%" == "Release" (
    echo "Release Mode: Creating release packages (version: %MOD_VER%)"

	if not exist "%OUTPUT_DIR%" (
		echo "Creating output directory"
		mkdir "%OUTPUT_DIR%"
	)

	"C:\Program Files\7-Zip\7z.exe" a %OUTPUT_DIR%\%MOD_NAME%-v%MOD_VER%.zip "%MOD_DLL%"
)
