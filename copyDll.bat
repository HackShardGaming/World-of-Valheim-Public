set CUR_DIR=%~dp0

set MOD_NAME=ValheimOnline
set MOD_DIR=%CUR_DIR%\%MOD_NAME%

set SERVER_DIR=C:\Games\Valheim_servers\server1
set PLAYER_DIR=C:\Program Files (x86)\Steam\steamapps\common\Valheim

set MOD_DLL=%MOD_DIR%\bin\Debug\%MOD_NAME%.dll

set PLUGIN_DIR=BepInEx\plugins

echo "%MOD_DLL% -> %SERVER_DIR%\%PLUGIN_DIR%\%MOD_NAME%.dll"
copy "%MOD_DLL%" "%SERVER_DIR%\%PLUGIN_DIR%\%MOD_NAME%.dll"


echo "%MOD_DLL% -> %PLAYER_DIR%\%PLUGIN_DIR%\%MOD_NAME%.dll"
copy "%MOD_DLL%" "%PLAYER_DIR%\%PLUGIN_DIR%\%MOD_NAME%.dll"


REM copy %MOD_DIR%\bin\Debug\%MOD_NAME%.dll %PLUGIN_OUT%
REM "C:\Program Files\7-Zip\7z.exe" a %PLUGIN_OUT%\%MOD_NAME%.zip %PLUGIN_OUT%\%MOD_NAME%.dll

REM pause