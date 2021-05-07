Please Note: This mod is a BepInEx Plugin? and thus requires this in order to be used.

Welcome to the World of Valheim - Zones Plugin.

This is a fork of the Valheim Online Plugin? by AluvielDev.

**We have been given permissions by AluvielDev for the release of his modified assets. He also changed the plugin permission on the Valheim-Online page with the following "You are allowed to modify my files and release bug fixes or improve on the features so long as you credit me as the original creator.".

This mod takes two of the features of Valheim-Online (PVP FORCED ON & SAFE-ZONES) and builds on it.  Adding an entire Zones config file and giving the option to enable/disable pvp & shared positioning or even just allowing the client to take over in certain "Zones"

If you are looking for the Server Sided Characters portion of his plugin to go with this please check out my other plugin World of Valheim - SSC
Core Features:
Server-Side Zones configuration file: When a client connects to the server it is sent the current zones configuration.


The arrival notification upon logging into a server has been removed.  This has no business in a PVP server as it will give away your location.


** NEW FEATURES IN 0.6.1 **
You can now use ValheimPermissions to give override permissions to the following features of this plugin:

Ward Protection: Ward Protection added by this plugin include Dropping items / Picking up items / and Damaging structures while inside a warded zone (If Enabled)  If you would like an admin to be able to override this function give them the permission node HackShardGaming.WoV-Zones.Wards.Override and have them relog back into the game.

Zone Configuration: Zone Configurations can be overriden by adding the users steamid into the zones file.  However, We now also have a new option where you can add the permission node HackShardGaming.WoV-Zones.Override.ZONETYPE and it will also accomplish this.  

** NEW FEATURES IN 0.6.0 SCROLL DOWN FOR MORE INFORMATION **

Required Mods:
We now require my ValheimPermissions in order to run.  Our plugin now uses a permission system rather then straight isAdmin in order to execute the client side commands listed below.
?
Compatibility Issue
Better Ward?

UPDATE: IF YOU USED ANY VERSION FROM 0.5.0 TO 0.5.10 YOU WILL NEED TO LOOK AT THE NEW ZONES.TXT FORMAT!  ZONES_CONFIGURATION.TXT HAS NOW BEEN MOVED TO ZONES.TXT FOR LESS LOAD ON THE SERVER!



Client Side Configuration Default:

[Biome]


## Should we announce changing PVP in a Biome Announcement? true or false
# Setting type: Boolean
# Default value: true
BiomePVPAnnouncement = true


[Colors]


## What color should our 'Now Entering' message be if the zone type has PVP on
# Setting type: String
# Default value: Red
PVPColor = Red


## What color should our 'Now Entering' message be if the zone type has PVE off
# Setting type: String
# Default value: White
PVEColor = White


## What color should our 'Now Entering' message be if the zone type has No PVP Enforcement
# Setting type: String
# Default value: Yellow
NonEnforcedColor = Yellow


[WorldofValheimZones]


## Nexus ID to make Nexus Update Happy!
# Setting type: Int32
# Default value: 891
NexusID = 891



Server Side Configuration Default:
## Settings file was created by plugin WorldofValheimZones v0.6.0
## Plugin GUID: HackShardGaming.WorldofValheimZones

[Death]


## ** NEW FEATURE 0.6.0 ** SERVER ONLY: Should we prevent a user from losing items/skills on death globally?
# Setting type: Boolean
# Default value: false
NoItemLoss = false


## ** NEW FEATURE 0.6.0 **  SERVER ONLY: How fast should the clients respawn?
# Setting type: Single
# Default value: 10
RespawnTimer = 1

[Ward]


## SERVER ONLY: Protect buildings from being damaged inside Warded Areas?
# Setting type: Boolean
# Default value: false
Building_ProtectDamage = false


## SERVER ONLY: Protect Picking up items in Warded Areas?
# Setting type: Boolean
# Default value: false
Item_Pickup = false


## SERVER ONLY: Protect Dropping items in Warded Areas?
# Setting type: Boolean
# Default value: false
Item_Drop = false


[WorldofValheimZones]


## Nexus ID to make Nexus Update Happy!
# Setting type: Int32
# Default value: 891
NexusID = 891


## SERVER ONLY: The file path to the zone file. If it does not exist, it will be created with a default zone.
# Setting type: String
# Default value: XXXXXXXXXXXX\BepInEx\config\WoV\Zones.txt
ZonePath = XXXXXXXXXXX\BepInEx\config\WoV\Zones.txt


## SERVER ONLY: Location of the ZonesPermissions file.
# Setting type: String
# Default value: XXXXXXXXXXXXX\BepInEx\config\WoV\Zone_Configuration.txt
ZoneConfigurationPath = XXXXXXXXXXXX\BepInEx\config\WoV\Zone_Configuration.txt


Custom Zone Configurations

?I personally recommend that if you are planning on using any of the following features to make a new zonetype specifically for these features in the areas you wish to put them in rather then just using "wilderness" / "safe" / "battle"

Each new line in your zones.txt should look something like this

Configuration: ZoneType | 00000000000000000 11111111111111111 22222222222222222 33333333333333333 | option1 option2 option3 option4

Breakdown: We do a Split on every | in our array.  Therefore you can enter multiple configuration options per line.

First we will convert the : to a | in our plugin itself.  Then we will ignore the first array which is the word Configuration:

Second split is the ZoneType.  This needs to match an existing ZoneType in your zones.txt (NOT ZONE NAMES!)  If you do not match up there will be an error but it will still load the file.

Third split is the Admins List.  Any users steamid that is found in this list will override any of the options that you select.  Therefore, if you for example want an admin only area you can supply everyone here and use pushaway in the options list.

Fourth split is the actual options you wish to implement.  Below is a list of actual available options that you can use for your server.  Note this list will change with future patches to include additional features..

NoChest: Prevent the user from accessing chests                                                 
NoDoors: Prevent the user from opening doors.                                                   
NoBuilding: Prevent a user from building  destroying using the hammer.                         
NoBuildDamage: Prevent anyone  anything from damaging buildings.
NoItemPickup: Prevent the user from picking up items.                                           
NoItemDrop: Prevent the user from dropping items.   
NoItemLoss: Prevent the user from losing there items upon death.                                           
NoTerrain: Prevent modification of Terrain.
NoPickaxe: Prevent the Pickaxe from being used. (Note: NoTerrain will also prevent this)
PushAway: Prevent the user from entering the area. (Please not if your sprint is fast enough
you can still push in but it will push you away just like it does if you fall off the map).
PeriodicDamage(int): Deals (INT) damage periodically to the user.                               
PeriodicDamage(type,int) Deals (INT) damage of the damage type: (type) periodically to the user.
Damage types include: Fire, Frost, Lightning, Poison, Pierce, Blunt,, Slash, and Damage 
PeriodicHeal(int) Heals the user for (INT) Periodically                                         
DamageMultiplierToMobs(int): Deals (INT) * 100 Percent damage to Mobs (Includes Mob             
on Mob!) (1 = 100%)                                                                     
DamageMultiplierToPlayers(int): Deals (INT) * 100 Percent damage to Players (This currently     
does not work in PVP) (1 = 100%)                                                        
DamageMultiplierToTrees(int): Deals (INT) * 100 Percent damage to Trees! (This includes         
trees falling on trees!) (1 = 100%)


Client Side Console Commands (F5 Screen)

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

# Zone Configuration File
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
Type: wilderness true false
Type: safe false true true true
Type: battle true true false true


# [ZoneConfigurations]
# ZoneTypeName: Name of an existing Zone Type
# AdminIDs: List of SteamID's of all the Admins you want to override all Configurations except damage mods in the area.
# Configuration:  List of all options you wish to put in this area.  Below is a list of available options.
## NoChest: Prevent the user from accessing chests
## NoDoors: Prevent the user from opening doors.
## NoBuilding: Prevent a user from building / destroying using the hammer.
## NoBuildDamage: Prevent anyone / anything from damaging buildings.
## NoItemPickup: Prevent the user from picking up items.
## NoItemDrop: Prevent the user from dropping items.
## NoItemLoss: Prevent the user from losing there items (TombStome) on death.
## NoTerrain: Prevent modification of Terrain.
## NoPickaxe: Prevent the Pickaxe from being used. (Note: NoTerrain will also prevent this)
## PushAway: Prevent the user from entering the area. (Please not if your sprint is fast enough
## you can still push in but it will push you away just like it does if you fall off the map).
## PeriodicDamage(int): Deals (INT) damage periodically to the user.
## PeriodicDamage(type,int) Deals (INT) damage of the damage type: (type) periodically to the user.
## Damage types include: Fire, Frost, Lightning, Poison, Pierce, Blunt,, Slash, and Damage
## PeriodicHeal(int) Heals the user for (INT) Periodically
## DamageMultiplierToMobs(int): Deals (INT) * 100 Percent damage to Mobs (Includes Mob
## on Mob!) (1 = 100%)
## DamageMultiplierToPlayers(int): Deals (INT) * 100 Percent damage to Players (This currently
## does not work in PVP) (1 = 100%)
## DamageMultiplierToTrees(int): Deals (INT) * 100 Percent damage to Trees! (This includes
## trees falling on trees!) (1 = 100%) 
#
# Configuration: [ZoneTypeName] | [AdminIDs] | [Configuration]
Configuration: wilderness | null | none
Configuration: safe | null | none
Configuration: battle | null | none




# [Zones]
# Name: Name for the area.
# ZoneType : Name the zonetype for the zone
# Priority : Lower the number, higher the precidence
# Shape: What type of zone shape you desire
#   - Circle: A circle zone that is centered on x,y and goes out radius.
#   - Square: A box zone that is centered on x,y and goes out radius.


# [Name] [ZoneType] [Priority] [Shape(circle/square)] [x] [y] [r]
DefaultSafeZone safe 5 circle 0.0 0.0 50.0
DefaultBattle battle 4 square 50.0 50.0 20.0
Poni!? safe 1 circle 0.0 0.0 2.0


Please note: If you do not have the Zone Type: Wilderness it will cause an exception.  As a precaution we have added a default Wilderness that will get loaded in the instance of no Wilderness.

Also note: If you do not have any ZONES themselves it will cause an exception.  As another precaution we have added a default zone that will spawn at 20000, 20000 if there is no existing zones in the zones file.

However, if the zones_configuration file is empty that is perfectly fine.  It will just keep the defaults for the existing zones.
The Defaults for the zones is:
?Admins: null
?Configuration: none




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
- [  ] Add command for /addconfiguration
- [  ] Add command for /removeconfiguration
- [  ] Add command for /updateconfiguration
- [X] Add additional features to zone. Example: Healing, No Damage to Structures, Act like a ward and do not allow editing.

Github Repository:
https://github.com/HackShardGaming/World-of-Valheim-Public/tree/master/WorldofValheimZones