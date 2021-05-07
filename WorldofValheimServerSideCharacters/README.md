Please Note: This mod is a BepInEx Plugin? and thus requires this in order to be used.

Welcome to the World of Valheim - Server Side Characters Plugin.

This is a fork of the Valheim Online Plugin? by AluvielDev.

**We have been given permissions by AluvielDev for the release of his modified assets. He also changed the plugin permission on the Valheim-Online page with the following "You are allowed to modify my files and release bug fixes or improve on the features so long as you credit me as the original creator.".

This fork has all the main requested Core Feature of Valheim Online which is Character Saving being server side.  That is the only feature that this mod enables.  If you are looking for some Zones PVP enforcement check out my World of Valheim - Zones plugin

Core Features:

Server-Side Characters: When a client connects to the server, the server will look and see if it has an existing character file stored for this player.  If it does not it will then create a brand new character file for the client.  It will then send the character file to the client and ask periodically for updates about that character.  If the user leaves the server the character file on the server will be updated, however, the clients character on the client will not.

*NEW IN 0.6.0* 
New Feature: Character Backups!

*NEW IN 0.5.0*
New Feature: Single player has been reintroduced! You can now properly play single player without any problems from this plugin.  As a side note, if you enable ExportCharacter you can make a duplicate of your server side character on your local computer.  This can be useful for saving your character. 

Config Option AllowSinglePlayer has been removed as it is no longer required.


*NEW IN 0.4.0*
New Feature: Plugin now has a Permissions system.  And thus requires another mod in order to run now! Please download: Valheim Permissions

*NEW IN 0.3.0*
New Feature: Starting in 0.3.0 saved character names are now dynamic.  Meaning, if you change which character you login to the server with, it will load THAT character file for you instead of the other one. 

Backwards Compatible: Upon logging into the server for the first time (using 0.3.0), the server will check for an old character file "current.voc" and will rename this character file to "ActualCharacterName.wov".  Meaning, if I log into my server as "HackShardGaming" my old "current.voc" will be renamed to "HackShardGaming.wov".  The character name specific character files are still stored inside the proper steamid folder as well just like before.



Client Side Console Commands (F5 Screen)

!save
?Ask the server to save your progress!

All of the following commands requires the user to be an approved admin (adminlists.txt)  They cannot use these commands otherwise!

(REQUIRED PERMISSION: HackShardGaming.WoV-SSC.SaveAll)
!save-all
?Ask the server to request a progress update from ALL connected users

(REQUIRED PERMISSION: HackShardGaming.WoV-SSC.ShutdownServer)
!shutdown-server
?Shuts the server down gracefully. Also asks for a progress update from ALL connected users before shutting down.

(REQUIRED PERMISSION: HackShardGaming.WoV-SSC.ReloadDefault)
!ReloadDefault
    Asks the server to reload the Default character file.


Client Side Config Variables:

?>> ExportCharacter: Export character from server for single player use and/or retain character. (WARNING: LOCAL CHARACTER FILE WILL BE OVERWRITEN!!)


Server Side Config Variables:

>> AllowMultipleCharacters:  Should we allow clients to save multiple different character files on the server (TRUE) or use one consistent file (FALSE)

?>> CharacterSavePath:  This is where  we are going to store the character files. NOTE: Please make this a location you have access to on your server machine


>> DefaultCharacterPath: This is where we are going to store the default character file.  NOTE: Please make this a location you have access to on your server machine.


>> SaveInterval: How often to request an update from the client. Defaults to every 2 minutes (120 seconds)

>> ShutdownDelay: How long should we wait after !shutdown-server before actually killing the server. (I suggest anything from 15 seconds or more!)

[Backups]

>> MaxBackups: How many backups maximum would you like to have?

>> BackupInterval: How often (in minutes) should we run a backup?  Default is every 10 minutes! (For big server owners I would suggest every few hours.  I personally used to go with 6 hours so 360.

Installation




You must install this plugin on both the server & client in order to login to the server.


Server Side:

?>>Download WorldofValheimServerSideCharacters.zip
>>Place WorldofValheimServerSideCharacters.dll in your BepInEx/plugins folder
>>Start the server
>>Shutdown the server
>>Go to your BepInEx/config folder
>>open WorldofValheimServerSideCharacters.cfg
>>Change the CharacterSavePath & DefaultCharacterPath to a place that you have access to. (Yes this works with GPortal etc)
>>Restart the server once again


Client Side:

>?Download WorldofValheimServerSideCharacters.zip
>Place WorldofValheimServerSideCharacters.dll in your BepInEx/plugins folder
>Done. Start game.


Default Character Feature:

This mod also allows you to modify the default character file that it creates and it will send this modified default character file to the clients upon their initial login.  This allows server owners to give every single player a specific set of gear to start the game with.

Github Repository:
https://github.com/HackShardGaming/World-of-Valheim-Public/tree/master/WorldofValheimServerSideCharacters