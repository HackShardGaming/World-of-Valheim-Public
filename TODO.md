### Valheim Online Sprint #1 (V1.0 Release)

- [X] Rework original dll for a VS project.
- [X] Successfully compile.
- [X] Reproducability with the original dll.
- [X] Analyzed original project for new feature capabilities.
- [X] Release version 0.1 with removed PVP for people to use (Temporary).
- [X] Add PVP toggle capabilites for clients
- [X] Fix game breaking bug that prevents a new character from being created
- [X] Add PVP "Safe Zone" support back in
- [X] Release version 0.2.1 with Force PVP & Visibility capability.

- [ ] Introduce "Battle Zone" support
- [ ] Introduce local client save for user retention (Make me happy to have my character saved local)
- [ ] Client/Server sync for client save file to prevent accident DC and losing stuff (Encountered this many times)
- [ ] Add support for sync of configs and work with BepInEx for plugin support.
- [ ] Update TODO from feedback


- [X] Analyzed original project for new feature capabilities.
- [X] Release version 0.1 with removed PVP for people to use (Temporary).
- [X] Add PVP toggle capabilites for clients
- [X] Fix game breaking bug that prevents a new character from being created
- [X] Release version 0.2.1 with Force PVP & Visibility capability.
- [X] Add PVP "Safe Zone" support back in
- [x] Allow a player to save their server-side character locally.
- [x] Allow single-player (EARLY TESTING)
- [X] Introduce local client save for user retention (Make me happy to have my character saved local)
- [X] Introduce "Battle Zone" support
- [X] Customize starter gear?
- [ ] Mode to change visibility range
- [ ] Client/Server sync for client save file to prevent accident DC and losing stuff (Encountered this many times)
- [ ] Add support for sync of configs and work with BepInEx for plugin support.
- [ ] Update TODO from feedback
- [ ] Make PVP settings server only (RPC calls)
- [ ] Update TODO from feedback

CLI Capabilites:
- [ ] update-zones
- [ ] add-zone
- [ ] del-zone
- [ ] save (client-side)
- [ ] save-all (server-side)
- [ ] shutdown-now (server-side)

Zone Capabilites:
- [ ] Heal Zone
- [ ] User Tag visibility
- [ ] Ward/prevent destroying

CLI
/update-zones
/add-zone [Name] [ZoneType] [Priority] [Shape(circle/square)] [x] [y] [r]
/del-zone [Name]
/list-zones

/save