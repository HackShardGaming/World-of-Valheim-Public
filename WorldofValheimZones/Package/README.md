Please Note: This mod is a BepInEx Plugin? and thus requires this in order to be used.

Welcome to the World of Valheim - Zones Plugin.

This is a fork of the Valheim Online Plugin? by AluvielDev.

**We have been given permissions by AluvielDev for the release of his modified assets. He also changed the plugin permission on the Valheim-Online page with the following "You are allowed to modify my files and release bug fixes or improve on the features so long as you credit me as the original creator.".

This mod takes two of the features of Valheim-Online (PVP FORCED ON & SAFE-ZONES) and builds on it.  Adding an entire Zones config file and giving the option to enable/disable pvp & shared positioning or even just allowing the client to take over in certain "Zones"

If you are looking for the Server Sided Characters portion of his plugin to go with this please check out my other plugin World of Valheim - SSC
Core Features:
Server-Side Zones configuration file: When a client connects to the server it is sent the current zones configuration.


The arrival notification upon logging into a server has been removed.  This has no business in a PVP server as it will give away your location.

Required Mods:
We now require my ValheimPermissions in order to run.  Our plugin now uses a permission system rather then straight isAdmin in order to execute the client side commands listed below.
?
Compatibility Issue
Better Ward?

Custom Zone Configurations

?I personally recommend that if you are planning on using any of the following features to make a new zonetype specifically for these features in the areas you wish to put them in rather then just using "wilderness" / "safe" / "battle"

Each line of your Zone_Configurations.txt should look something like this

ZoneType | 00000000000000000 11111111111111111 22222222222222222 33333333333333333 | option1 option2 option3 option4

Breakdown: We do a Split on every | in our array.  Therefore you can enter multiple configuration options per line.

First split is the ZoneType.  This needs to match an existing ZoneType in your zones.txt (NOT ZONE NAMES!)  If you do not match up there will be an error but it will still load the file.

Second split is the Admins List.  Any users steamid that is found in this list will override any of the options that you select.  Therefore, if you for example want an admin only area you can supply everyone here and use pushaway in the options list.

Third split is the actual options you wish to implement.  Below is a list of actual available options that you can use for your server.  Note this list will change with future patches to include additional features..

NoChest: Prevent the user from accessing chests
NoDoors: Prevent the user from opening doors.
NoBuilding: Prevent anyone / anything from damaging buildings.
NoPickup: Prevent the user from picking up items.
NoItemDrop: Prevent the user from dropping items.
PushAway: Prevent the user from entering the area. (Please not if your sprint is fast enough you can still push in but it will push you away just like it does if you fall off the map).
PeriodicDamage(int): Deals (INT) damage periodically to the user.
PeriodicDamage(type,int) Deals (INT) damage of the damage type: (type) periodically to the user.
Damage types include: Fire, Frost, Lightning, Poison, Pierce, Blunt,, Slash, and Damage
PeriodicHeal(int) Heals the user for (INT) Periodically
DamageMultiplierToMobs(int): Deals (INT) * 100 Percent damage to Mobs (Includes Mob on Mob!) (1 = 100%)
DamageMultiplierToPlayers(int): Deals (INT) * 100 Percent damage to Players (This currently does not work in PVP) (1 = 100%)
DamageMultiplierToTrees(int): Deals (INT) * 100 Percent damage to Trees! (This includes trees falling on trees!) (1 = 100%)


Client Side Console Commands (F5 Screen)
[/b][/size]
WARNING: adding a zone the wrong way CAN and WILL break your zones file.  Make sure you know which "ZoneTypes" that you actually have!
NOTE: We will be adding more checks into this in the future. For now it will write whatever you type there. SO.. if you type the wrong thing it WILL NOT load that zone and will have a dead line in your zones.txt!

!getcoords
    This will show your current coords to you


All of the following commands requires the user to be an approved admin (adminlists.txt)  They cannot use these commands otherwise!

(Permission Required: HackShardGaming.WoV-Zones.Reload)
!reload-zones
?Reload the servers zones file.  Also sends all the new zones list to all connected users!

(Permission Required: HackShardGaming.WoV-Zones.Add)
!addzone [Name] [ZoneType] [Priority] [Shape(circle/square)] [x] [y] [Radius]
?Adds a zone into the current server and reloads the servers zones file.  Also sends the new zones list to all connected users! **Please note: X, Y, and Radius need to be formatted in 0.0 (note the one decimal place)**

!addzone [Name] [ZoneType [Priority] [Shape(circle/square)] [Radius]
    Adds a zone using our current coords into the current server and reloads the servers zones file.  Also sends the new zones list to all connected users! **Please note: Radius needs to be formatted in 0.0 (note the one decimal place) The X and Y will be automatically generated according to where you are currently located in the game.**





You must install this plugin on both the server & client in order to login to the server.
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
>>The default ZonePath is BepInEx/config/WoV/zones.txt
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


Example of a working Zone_Configuration.txt



////////////////////////////////////////////////////////////////////////////////////////////////////////////
//// This is a demonstration section for the Zone Configurations.                                       ////
//// This file will be formated in the following way                                                    ////
//// ZONETYPE | STEAMIDS | CONFIG OPTIONS                                                               ////
////                                                                                                    ////
//// ZoneType: needs to be the ZoneType of the zone you are in.  You should create a new one for each   ////
////specific "Config Section" you wish to have.  Such as a NoBuild zonetype in WoV-Zones            ////
////                                                                                                    ////
//// SteamIDS: This value can be null,  If you provide a list of steamid's then those specific users    ////
////will override all settings in the CONFIG OPTIONS.  For example, ADMIN ZONES!                    ////
////                                                                                                    ////
////  Config Options: Below is a list of available config options.                                      ////
////NoChest: Prevent the user from accessing chests                                                 ////
////NoDoors: Prevent the user from opening doors.                                                   ////
////NoBuilding: Prevent anyone / anything from damaging buildings.                                  ////
////NoPickup: Prevent the user from picking up items.                                               ////
////NoItemDrop: Prevent the user from dropping items.                                               ////
////PushAway: Prevent the user from entering the area. (Please not if your sprint is fast enough////
////you can still push in but it will push you away just like it does if you fall off the map). ////
////PeriodicDamage(int): Deals (INT) damage periodically to the user.                               ////
////PeriodicDamage(type,int) Deals (INT) damage of the damage type: (type) periodically to the user.////
////Damage types include: Fire, Frost, Lightning, Poison, Pierce, Blunt,, Slash, and Damage     ////
////PeriodicHeal(int) Heals the user for (INT) Periodically                                         ////
////DamageMultiplierToMobs(int): Deals (INT) * 100 Percent damage to Mobs (Includes Mob             ////
////on Mob!) (1 = 100%)                                                                         ////
////DamageMultiplierToPlayers(int): Deals (INT) * 100 Percent damage to Players (This currently     ////
////does not work in PVP) (1 = 100%)                                                            ////
////DamageMultiplierToTrees(int): Deals (INT) * 100 Percent damage to Trees! (This includes         ////
////trees falling on trees!) (1 = 100%)                                                         ////
////////////////////////////////////////////////////////////////////////////////////////////////////////////
battle | 00000000000000000 | PeriodicDamage(lighting,1) NoItemDrop NoPickup NoBuilding NoBuildDamage NoDoors NoChest






How does this work??



Valheim Zones accomplishes enforcing zones by using an RPC between the client and the server.  The server will tell the client where the zones are and the client will then enforce these zones via this plugin.

The new Configurations section accomplishes its features by checking which zone you currently occupy.  It then checks to see if there is a configurable option for that zone in the Zones_Configuration.txt file.  If there is it will force the client into these options, However, if the user is in the Admin list section of the file then it will allow that user to override all configurations that have been applied for that zone.


Bug List/Compatibility Issues:

This mod will not work with the original version of Valheim Online as it uses the same patches that Valheim Online uses.  If you wish to have server side characters please download my other modified mod World of Valheim - SSC


ToDo List:
- [X] Initial Release
- [X] Add command for /reload-zones
- [X] Add commands for /addzone 
- [X] Add command for /getcoords
- [  ] Add command for /removezone
- [  ] Add command for /addtype
- [  ] Add command for /removetype
- [X] Add additional features to zone. Example: Healing, No Damage to Structures, Act like a ward and do not allow editing.