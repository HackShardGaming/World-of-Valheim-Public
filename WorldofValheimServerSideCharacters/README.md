Core Features:
Server-Side Characters: When a client connects to the server for the first time the server will create a brand new character file for this client.  It then sends this character to the client and will ask periodically for updates about that character.  If the user leaves the server it will also update the character from the client as well.  If you exit and log into a different server you will create another server side character for that server.

Client Side Config Variables:
?>> AllowCharacterSave: This option is purely for preserving character files.  It will add the ability for the client to save there character upon logout locally as well as on the server.  However, this will NOT allow the user to modify the character data and log back into the server and have it update the server.  You will still download your character file upon every single login.

>> AllowSinglePlayer:  This option will allow a player to play single player again.  Note: If you log into a server using your single player character file it will overwrite this file.

Server Side Config Variables:
?>> CharacterSavePath:  This is where  we are going to store the character files. NOTE: Please make this a location you have access to on your server machine

>> DefaultCharacterPath: This is where we are going to store the default character file.  NOTE: Please make this a location you have access to on your server machine.

>> SaveInterval: How often to request an update from the client. Defaults to every 2 minutes (120 seconds)

Default Character Feature:
This mod also allows you to modify the default character file that it creates and it will send this modified default character file to the clients upon there initial login.  This allows server owners to give every single player a specific set of gear to start the game with.