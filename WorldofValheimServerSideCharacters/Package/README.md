Please Note: This mod is a BepInEx Plugin? and thus requires this in order to be used.

Welcome to the World of Valheim - Server Side Characters Plugin.

This is a fork of the Valheim Online Plugin? by AluvielDev.

**We have been given permissions by AluvielDev for the release of his modified assets. He also changed the plugin permission on the Valheim-Online page with the following "You are allowed to modify my files and release bug fixes or improve on the features so long as you credit me as the original creator.".

This fork has all the main requested Core Feature of Valheim Online which is Character Saving being server side.  That is the only feature that this mod enables.  If you are looking for some Zones PVP enforcement check out my World of Valheim - Zones plugin

Core Features:

Server-Side Characters: When a client connects to the server, the server will look and see if it has an existing character file stored for this player.  If it does not it will then create a brand new character file for the client.  It will then send the character file to the client and ask periodically for updates about that character.  If the user leaves the server the character file on the server will be updated, however, the clients character on the client will not.

*NEW IN 0.3.0*
New Feature: Starting in 0.3.0 saved character names are now dynamic.  Meaning, if you change which character you login to the server with, it will load THAT character file for you instead of the other one. 

Backwards Compatible: Upon logging into the server for the first time (using 0.3.0), the server will check for an old character file "current.voc" and will rename this character file to "ActualCharacterName.wov".  Meaning, if I log into my server as "HackShardGaming" my old "current.voc" will be renamed to "HackShardGaming.wov".  The character name specific character files are still stored inside the proper steamid folder as well just like before.



Client Side Console Commands (F5 Screen)

!save
?Ask the server to save your progress!

All of the following commands requires the user to be an approved admin (adminlists.txt)  They cannot use these commands otherwise!
!save-all
?Ask the server to request a progress update from ALL connected users
!shutdown-server
?Shuts the server down gracefully. Also asks for a progress update from ALL connected users before shutting down.



Client Side Config Variables:

?>> AllowCharacterSave: This option is purely for preserving character files.  It will add the ability for the client to save there character upon logout locally as well as on the server.  However, this will NOT allow the user to modify the character data and log back into the server and have it update the server.  You will still download your character file upon every single login.


>> AllowSinglePlayer:  This option will allow a player to play single player again.  Note: If you log into a server using your single player character file it will overwrite this file.


Server Side Config Variables:

?>> CharacterSavePath:  This is where  we are going to store the character files. NOTE: Please make this a location you have access to on your server machine


>> DefaultCharacterPath: This is where we are going to store the default character file.  NOTE: Please make this a location you have access to on your server machine.


>> SaveInterval: How often to request an update from the client. Defaults to every 2 minutes (120 seconds)

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