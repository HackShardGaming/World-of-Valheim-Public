If you include ValheimPermissions as a Dependancy into your plugin you can tie into it using the following commands:


For the Server owners this is a list of the available commands for the console that have been created and there descriptions:

User Level Commands:
		!user add (STEAMID) (GROUP_NAME)
		// Adds a user to the Group defined.  Please note at this time you can only be in ONE group at a time.  Maybe a future change.
		
		!user add permission (STEAMID) (PERMISSION_NODE)
		// Adds the permission_node to the user.
		// Note: * is a (ALL) option for any access level AFTER the *.
		// Example: ValheimPermissions.user.* would give access to a command that requires ValheimPermissions.user.create or ValheimPermissions.user.add but not ValheimPermissions.useradd
		
		!user check group (STEAMID)
		// Will display which group the provided user is in.
		
		!user check permission (STEAMID) (PERMISSION_NODE)
		// This will check the users permission against the provided permission_node
		
		!user check permission (STEAMID)
		// If you do not provide a permission_node this will list ALL available permissions for the provided user.  It will also display all permissions for the group they are in. (DEFAULT GROUP IS "Default")
		
Group Level Commands:
		!group create (GROUP_NAME)
		// Creates the group provided
		
		!group add permission (GROUP_NAME) (PERMISSION_NODE)
		// Adds the permission_node to the group.
		// Note: * is a (ALL) option for any access level AFTER the *.
		// Example: ValheimPermissions.user.* would give access to a command that requires ValheimPermissions.user.create or ValheimPermissions.user.add but not ValheimPermissions.useradd
		
		!group check permission (GROUP_NAME) (PERMISSION_NODE)
		// This will check the groups permission against the provided permission_node
		
		!group check permission (GROUOP_NAME)
		// This will show you all permissions for the provided group.
		
		


This is the information for Mod Developers that wish to incorperate my mod into there project
User Level Requests:

		ValheimPermission.ValheimDB.ShowUserPermissions(SteamID)
        // Important Note: You need to send the SteamID in STRING format instead of the default LONG format. use SteamID.ToString() before sending.
        // Results: Will return a string[] of all the permission nodes the user has access to.
        // Or: will return one array result (false) if there is no permissions.
        // NOTE: Results will be returned to you in an string[] format! Code accordingly!
		
		ValheimPermissions.ValheimDB.PermissionScanUser(SteamID, PERMISSION_NODE)
        // Important Note: You need to send the SteamID in STRING format instead of the default LONG format. use SteamID.ToString() before sending.
		// Important Note: This will ONLY return true if the USER themself has access to the permission. Use CheckUserPermission to see if the group has access as well.
        // Results: Will return true if the user has permission to that node or higher.
        // Or: will return false if the user does not have permission to that node
        // NOTE: Results will be returned to you in BOOL format (true/false)
		
		ValheimPermissions.ValheimDB.CheckUserPermission(SteamID)
        // Important Note: You need to send the SteamID in STRING format instead of the default LONG format. use SteamID.ToString() before sending.
        // Results: Will first check the users group for a permission node and if they have it return true
        // Or: If false above. will check the user for the permission node and if they have it return true
        // Or: return false if neither have it.
        // NOTE: Results will be returned to you in BOOL format (true/false)
		
		ValheimPermissions.ValheimDB.AddUserPermission(STEAMID, PERMISSION_NODE)
        // Important Note: You need to send the SteamID in STRING format instead of the default LONG format. use SteamID.ToString() before sending.
        // Results: Will return false if the permission already exists
        // Or: WIll return true if the add was successful
        // NOTE: Results will be returned to you in BOOL format (true/false)
		
		ValheimPermissions.ValheimDB.AddUserToGroup(SteamID, Group_Name)
        // Important Note: You need to send the SteamID in STRING format instead of the default LONG format. use SteamID.ToString() before sending.
        // Results: Will first see if the user is already in a group. If so return "false user"
        // Or: will check if the group exists. If not return "false group"
        // Or: return true and add the users new group.
        // NOTE: Results will be returned to you in STRING format as described above.
		
		ValheimPermissions.ValheimDB.GetGroup(SteamID)
        // Important Note: You need to send the SteamID in STRING format instead of the default LONG format. use SteamID.ToString() before sending.
        // Results: Will return the group that the user is in.
        // Or: If that user is not in a group. return "Default"
        // NOTE: Results will be returned to you in string format as described above.

Group Level Requests:		
        ValheimPermissions.ValheimDB.AddGroup(GROUP_NAME)
        // Result: Will return TRUE or FALSE if the group was created or not.
        // NOTE: Results will be returned to you in BOOL format (true/false)
		
		ValheimPermissions.ValheimDB.AddGroupPermission(GROUP_NAME, PERMISSION_NODE)
        // Results: Will return "false group" if the group does not exist
        // Or: Will return "false exists" if the permission already exists
        // Or: WIll return "true" if the add was successful
        // NOTE: Results will be returned to you in STRING format as described above.
		
		ValheimPermissions.ValheimDB.PermissionScanGroup(GROUP_NAME, PERMISSION_NODE)
        // Results: Will return true if the group has permission to that node or higher.
        // Or: will return false if the group does not have permission to that node
        // NOTE: Results will be returned to you in BOOL format (true/false)
		
		ValheimPermission.ValheimDB.ShowGroupPermissions(SteamID)
        // Important Note: You need to send the SteamID in STRING format instead of the default LONG format. use SteamID.ToString() before sending.
        // Results: Will return a string[] of all the permission nodes the user has access to.
        // Or: will return one array result (false) if there is no permissions.
        // NOTE: Results will be returned to you in an string[] format! Code accordingly!
		
		


Current Progress that needs to be completed:


Alphabatize the results from getting user permissions.

Update User Group
Delete User Group
Delete User Permission
Delete Group Permission
Add a currency base? (For plugins that use lots of "Currency" so the user doesn't have to hold them)
		
		