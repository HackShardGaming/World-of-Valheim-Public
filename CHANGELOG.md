Version 0.3.0
Revamping all zones management systems. ALL zones be moved to one file (Zones.txt). This way, you can have multiple types of zones. This adds the ability to change the default "Wilderness" zone to your liking, aka, PVP on & Sharing off, PVP off & Sharing on, No enforcement at all, ETC.
Added in a few new console Here is a list
!zone-reload - Reload zones from zones.txt file
!shutdown - Shutdown the server SAFELY. (This is the HIGHLY recommended way to shutdown your server as it will request a save from all online characters before shutting down)
!save-all - Save all the characters currently connected to the server.
!help - List of all commands
!version - prints server version
Client commands will come at a later date.
Please note: The !shutdown command MUST be executed to force a save of all characters. If your server crashes or restarts without executing this anyone who is online at the time of the shutdown WILL experience a rollback in some way.
Version 0.2.9
Bug Fix: Logout button should no longer exit the game again.
Version 0.2.8
Bug Fix: Resolved PVPEnforcement Issues.
Version 0.2.4
Resolving an issue where if a user uses ALT+F4 the client will not send the quit RPC call to the server
Adding in support for MultiVerse, also supports any other fastlogout addons.
Version 0.2.3
Implemented BattleZones
Created a config variable ServerForcePVP. This variable will either Force the client to PVP or force the client to PVE
Introducing the capability to have a modified "Default Character" so that server owners can supply a default set of gear on every new player connected.
Version 0.2.2
New Config Variable: AllowCharacterSave. This setting will allow the local player to save it's character file on its computer. It will not, however, allow the user to play locally, modify the character, reupload it to the server.
New Config Variable: AllowSinglePlayer. AllowCharacterSave must be TRUE for this function to work. It readds the single player button to the game. (WARNING: ERRORS IN LOG WILL OCCUR) (WARNING: THE SELECTED LOCAL CHARACTER FILE WILL BE OVERWRITEN UPON LOGGING INTO THE SERVER. USE THIS WITH CARE!)
As a side note, the reason I have released this patch is due to SERVER ADMIN requests. It will allow a user to download there character file. Modify it. send it to the SERVER OWNER. who can then modify that users server side character by reuploading it. It will NOT allow an external user to modify there OWN server side character!
Version 0.2.1
Resolved a Game Breaking bug that was preventing users from creating new characters on a server.
Removing both variables introduced in 0.2 as we are reworking this section.
ServerPvpEnforced: This option will either Force PVP on (TRUE) or allow the client to modify its own PvP toggle (FALSE)
PVPSharePosition: IF ServerPvPEnforced is (TRUE) then This variable will be called. Force your position to be shared (TRUE). Force your position not to be shared (FALSE).
Version 0.2
﻿Introduced a config variable: ServerPvPMode. TRUE (PVP is on) FALSE (PVP Is off) This variable is currently client side and has plans to be moved to the server. (DEFAULTS TO FALSE)
﻿Introduced a config variable: ServerPvPToggle: This variable is supposed to allow the client to toggle pvp on or off itself. Currently this is not working! Please leave it FALSE (DEFAULTS TO FALSE)
Version 0.1
Initial Release