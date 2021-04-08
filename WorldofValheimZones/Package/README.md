Please Note: This mod is a BepInEx Plugin? and thus requires this in order to be used.

Welcome to the World of Valheim - Zones Plugin.

This is a fork of the Valheim Online Plugin? by AluvielDev.

**We have been given permissions by AluvielDev for the release of his modified assets. He also changed the plugin permission on the Valheim-Online page with the following "You are allowed to modify my files and release bug fixes or improve on the features so long as you credit me as the original creator.".

This mod takes two of the features of Valheim-Online (PVP FORCED ON & SAFE-ZONES) and builds on it.  Adding an entire Zones config file and giving the option to enable/disable pvp & shared positioning or even just allowing the client to take over in certain "Zones"

If you are looking for the Server Sided Characters portion of his plugin to go with this please check out my other plugin World of Valheim - SSC
Core Features:
Server-Side Zones configuration file: When a client connects to the server it is sent the current zones configuration.


The arrival notification upon logging into a server has been removed.  This has no business in a PVP server as it will give away your location.

Patch Notes:
Version 0.2.0
>> Resolved issue involving logging into the wilderness not loading the wilderness settings.

Installation



You must install this plugin on both the server & client in order to login to the server.


Server Side:

?>>Download WorldofValheimZones.dll
>>Place WorldofValheimZones.dll in your BepInEx/plugins folder
>>Start the server
>>Shutdown the server
>>Go to your BepInEx/config folder
>>open WorldofValheimZones.cfg and turn EnforceZone to true to enable this plugin
>>Change the ZonePath to a place that you have access to. (Yes this works with GPortal etc)
>>Restart the server once again
>>Shutdown the server
>>Configure the Zones.txt file the way you like.


Client Side:

>?Download WorldofValheimZones.dll
>Place WorldofValheimZones.dll in your BepInEx/plugins folder
>Done. Start game.



Example of a working Zones.txt

?# Zone Configuration File
# Here you will specify the zone types and zones they occupy.

# [ZoneType]
# ZoneTypeName: Unique name for the zone type (zones will use)
# PVP_Mode: PVP state for the Zone.
# PVP_Enforce: Force PVP mode on the users.
# Position_Show: Should we show our position in the zone?
# Position_Enforce: Force position on the users.
#
# Type: [ZoneTypeName] [PVP_Mode] [PVP_Enforce] [Position_Show] [Position_Enforce]


# wilderness is the default zone for everywhere


# wilderness will enforce PVP and disable sharing of your position
Type: wilderness true true false true


# safe will enforce PVP to off and will enable sharing your position
Type: safe false true true true


# battle will enforce PVP to on and will disable sharing of your position
Type: battle true true false true


# noenforce will not enforce PVP or position sharing. This gives the client 100% control
Type: noenforce false false false false


# [Zones]
# Name: Name for the area.
# ZoneType : Name the zonetype for the zone
# Priority : Lower the number, higher the precedence
# Shape: What type of zone shape you desire
#   - Circle: A circle zone that is centered on x,y and goes out radius.
#   - Square: A box zone that is centered on x,y and goes out radius.




# [Name] [ZoneType] [Priority] [Shape(circle/square)] [x] [y] [r]


#DefaultSafeZone is a circular zone located at 0,0. It extends 50 vectors in all directions  It also has a priority levelof 5
DefaultSafeZone safe 5 circle 0.0 0.0 50.0


#DefaultBattle is a square zone located at 50,50. Since this zone is square it will extend 20 Vectors. So if you are within 30,30 and 70,70 you will be within the zone.  It also has a priority level of 4
DefaultBattle battle 4 square 50.0 50.0 20.0


#Poni!? is a circular zone extending out 2 vectors in all directions.  It also has a priority level of 1 (THE HIGHEST)!
Poni!? safe 1 circle 0.0 0.0 2.0







How does this work??



Valheim Zones accomplishes enforcing zones by using an RPC between the client and the server.  The server will tell the client where the zones are and the client will then enforce these zones via this plugin.


Bug List/Compatibility Issues:

This mod will not work with the original version of Valheim Online as it uses the same patches that Valheim Online uses.  If you wish to have server side characters please download my other modified mod World of Valheim - SSC


ToDo List:
- [X] Initial Release
- [X] Add commands for /addzone /removezone
- [X] Add additional features to zone. Example: Healing, No Damage to Structures, Act like a ward and do not allow editing.