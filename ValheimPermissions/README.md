If you include ValheimPermissions as a Dependency into your plugin you can tie into it using the following commands:

*NEW RELEASE FEATURE IN VERSION: 1.1.0*
Users have always been able to type !valheimpermissions (COMMAND) in there F5 console window in order to attempt to run a command, However, as of our latest update 1.1.0 You can now type any command listed below DIRECTLY into the chat window.  The server will catch this command and then process it properly for you!  It will also return the results to you.

Server Owners?


IMPORTANT NOTE: Anywhere it says (STEAMID) You can either enter the players SteamID or the players Character Name. You will need to replace any Spaces in the players name with a / (This plugin will convert it back to spaces for you)

User Level Commands:

!user add (STEAMID) (GROUP_NAME)
Adds the User to the Group defined.
NOTE: You can only be in one Group at a time!

!user add permission (STEAMID) (PERMISSION_NODE)
Adds the permission node to the User.
Please note: Wildcards are acceptable.
Example: I add in HackShardGaming.WoV-SSC.* and the required permission node is HackShardGaming.WoV-SSC.SaveAll.
Result: I will have access to this command.
Example2: I add in HackShardGaming.WoV-SSC.* and the required permission node is HackShardGaming.WoV-SSCSaveAll (Note the missing period)
Result: I do not have access to this command.

!user del permission (STEAMID) (PERMISSION_NODE)
Deletes the permission node from the user

!user check permission (STEAMID)
List all permissions accessible by the User.
And: List all permissions accessible by the Group the user is in.

!user check permission(STEAMID) (PERMISSION_NODE)
Check the Users access to the requested Permission_Node

Group Level Commands

!group add (GROUP_NAME)
Will create the Group (GROUP_NAME)

!group del (GROUP_NAME)
Will delete the Group (GROUP_NAME)
NOTE: This will also remove all permission nodes associated with (GROUP_NAME)
NOTE: This will also remove the group from all users. (Will not affect a users direct permission list

!group add permission (GROUP_NAME) (PERMISSION_NODE)
Adds the permission node to the Group.
Note: Refer to !user add permission for more details

!group del permission (GROUP_NAME) (PERMISSION_NODE)
Deletes the permission node from the Group.

!group check permission(GROUP_NAME)
List all permissions owned by the Group

!group check permission(GROUP_NAME) (PERMISSION_NODE)
Will check the Group access to the requested Permission_Node

*** MOD DEVELOPERS ***?


The following section will show a list of available functions provided by ValheimPermissions. All the following functions are "public" functions accessible if you include ValheimPermissions as a dependency

User Level Requests:

ValheimPermissions.ValheimDB.ShowUserPermissions(SteamID)
Important Note: You need to send the SteamID in STRING format instead of the default LONG format. use SteamID.ToString() before sending.
Results: Will return a string[] of all the permission nodes the user has access to.
Or: will return one string[] result: (No.Permissions.Found) if there is no permissions.
NOTE: Results will be returned to you in an string[] format! Code accordingly!

ValheimPermissions.ValheimDB.CheckUserPermission(SteamID, PERMISSION_NODE)
Important Note: You need to send the SteamID in STRING format instead of the default LONG format. use SteamID.ToString() before sending.
Results: Will first check the users group for a permission node and if they have it return true
Or: If false above. will check the user for the permission node and if they have it return true
Or: return false if neither have it.
NOTE: Results will be returned to you in BOOL format (true/false)

ValheimPermissions.ValheimDB.AddUserPermission(SteamID, PERMISSION_NODE)
Important Note: You need to send the SteamID in STRING format instead of the default LONG format. use SteamID.ToString() before sending.
Results: Will return false if the permission already exists
Or: WIll return true if the add was successful
NOTE: Results will be returned to you in BOOL format (true/false)

[b]ValheimPermissions.ValheimDB.DelUserPermission(SteamID, PERMISSION_NODE)
Important Note: You need to send the SteamID in STRING format instead of the default LONG format. use SteamID.ToString() before sending.
Results: Will return false if the user does not already have access to the permission node
Or: WIll return true if the deletion was successful
NOTE: Results will be returned to you in BOOL format (true/false)

ValheimPermissions.ValheimDB.AddUserToGroup(SteamID, Group_Name)
Important Note: You need to send the SteamID in STRING format instead of the default LONG format. use SteamID.ToString() before sending.
Results: Will first see if the user is already in a group. If so return "false user"
Or: will check if the group exists. If not return "false group"
Or: return true and add the users new group.
NOTE: Results will be returned to you in STRING format as described above.

ValheimPermissions.ValheimDB.DelUserGroup(SteamID)


ValheimPermissions.ValheimDB.GetGroup(SteamID)
]code]Results: Will return the group that the user is in
Or: If they are not in a group it will return "Default"
NOTE: Results will be returned to you in string format

GroupLevel Requests:

ValheimPermissions.ValheimDB.AddGroup(GROUP_NAME)
Result: Will return TRUE or FALSE if the group was created or not.
NOTE: Results will be returned to you in BOOL format (true/false)

ValheimPermissions.ValheimDB.DelGroup(GROUP_NAME)
Results: Will return false if the group does not already exist
Or: Will remove all users & permissions associated with the group
And: Will remove the Group itself.
And: Will return true
NOTE: Results will be returned to you in BOOL format (true/false)

ValheimPermissions.ValheimDB.AddGroupPermission(GROUP_NAME, PERMISSION_NODE)
Results: Will return "false group" if the group does not exist
Or: Will return "false exists" if the permission already exists
Or: Will return "true" and will add the permission
NOTE: Results will be returned to you in STRING format as described above.

ValheimPermissions.ValheimDB.DelGroupPermission(GROUP_NAME, PERMISSION_NODE)
[code]Results: Will return false if the Group does not already have access to the permission node
Or: Will return true if the deletion was successful.
Note: Results will be returned to you in BOOL format (true/false)

ValheimPermissions.ValheimDB.PermissionScanGroup(GROUP_NAME, PERMISSION_NODE)
[code]Results: Will return true if the group has permission to that node or higher.
Or: will return false if the group does not have permission to that node
NOTE: Results will be returned to you in BOOL format (true/false)

ValheimPermissions.ValheimDB.ShowGroupPermissions(GROUP_NAME,)
Results: Will return a string[] of all the permission nodes the group has access to.
Or: will return one string[] result: (No.Permissions.Found) if there is no permissions.
NOTE: Results will be returned to you in an string[] format! Code accordingly!