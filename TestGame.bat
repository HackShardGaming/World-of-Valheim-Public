
SET SERVER_DIR=C:\Games\Valheim_servers\server1
SET GAME_DIR=C:\Program Files (x86)\Steam\steamapps\common\Valheim

PUSHD %SERVER_DIR%
start cmd /k start_headless_server.bat
POPD

PUSHD %GAME_DIR%
CALL valheim.exe -windows-mode exclusive -console
POPD
