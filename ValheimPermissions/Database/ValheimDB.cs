using System;
using System.Linq;
using System.Collections.Generic;
using LiteDB;
using HarmonyLib;

namespace ValheimPermissions
{
    public class ValheimDB
    {
        public static string DatabaseLocation = "";
        public class User
        {
            public string SteamID { get; set; }
            public string Group_Name { get; set; }
        }
        public class Group
        {
            public string Group_Name { get; set; }
        }
        public class Group_Permission
        {
            public string Group_Name { get; set; }
            public string permission { get; set; }
        }
        public class User_Permission
        {
            public string SteamID { get; set; }
            public string permission { get; set; }
        }
        
        
        // Execution: ValheimPermissions.ValheimDB.AddGroup(GROUP_NAME)
        // Result: Will return TRUE or FALSE if the group was created or not.
        // NOTE: Results will be returned to you in BOOL format (true/false)
        public static bool AddGroup(string group_name)
        {
            
            using (var db = new LiteDatabase(DatabaseLocation))
            {
                var groups = db.GetCollection<Group>("Group");
                var group = new Group
                {
                    Group_Name = group_name
                };
                var exists = groups.FindOne(Query.EQ("Group_Name", group_name));
                if (exists != null)
                {
                    return false;
                }
                else
                {
                    groups.Insert(group);
                    return true;
                }
            }
        }
        // Execution ValheimPermissions.ValheimDB.DelGroup(GROUP_NAME)
        // Results: Will return false if the group does not exist
        // Or: Will remove all users & permissions associated with the group
        // And: Will remove the Group itself
        // And: Will return true;
        // NOTE: Results will be returned to you in BOOL format (true/false).
        public static bool DelGroup(string group_name)
        {
            using (var db = new LiteDatabase(DatabaseLocation))
            {
                var groups = db.GetCollection<Group>("Group");
                var users = db.GetCollection<User>("User");
                var permission = db.GetCollection<Group_Permission>("Group_Permission");
                var exists = groups.FindOne(Query.EQ("Group_Name", group_name));
                if (exists == null)
                {
                    return false;
                }
                else
                {
                    var resultpermission = db.Execute($"DELETE Group_Permission WHERE Group_Name='{group_name}'");
                    var resultsusers = db.Execute($"DELETE User WHERE Group_Name='{group_name}'");
                    var resultsgroup = db.Execute($"DELETE Group WHERE Group_Name='{group_name}'");
                    return true;
                }
            }
        }
        // Execution ValheimPermissions.ValheimDB.AddGroupPermission(GROUP_NAME, PERMISSION_NODE)
        // Results: Will return "false group" if the group does not exist
        // Or: Will return "false exists" if the permission already exists
        // Or: WIll return "true" if the add was successful
        // NOTE: Results will be returned to you in STRING format as described above.
        public static string AddGroupPermission(string group, string permission)
        {
            permission = permission.ToLower();
            using (var db = new LiteDatabase(DatabaseLocation))
            {
                var groups = db.GetCollection<Group>("Group");
                var permissions = db.GetCollection<Group_Permission>("Group_Permission");
                var _permission = new Group_Permission
                {
                    Group_Name = group,
                    permission = permission
                };
                var gexists = groups.FindOne(Query.EQ("Group_Name", group));
                if (gexists == null)
                {
                    return "false group";
                }
                var pexists = permissions.FindOne(Query.And(Query.EQ("Group_Name", group), Query.EQ("permission", permission)));
                if (pexists != null)
                {
                    return $"false exists";
                }
                permissions.Insert(_permission);
                return "true";
            }
        }
        // Execution ValheimPermissions.ValheimDB.DelGroupPermission(GROUP_NAME, PERMISSION_NODE)
        // Results: Will return false if the Group does not already have access to the permission node
        // Or: WIll return true if the deletion was successful
        // NOTE: Results will be returned to you in BOOL format (true/false)
        public static bool DelGroupPermission(string Group_Name, string permission)
        {
            permission = permission.ToLower();
            using (var db = new LiteDatabase(DatabaseLocation))
            {
                var permissions = db.GetCollection<Group_Permission>("Group_Permission");
                var pexists = permissions.FindOne(Query.And(Query.EQ("Group_Name", Group_Name), Query.EQ("permission", permission)));
                if (pexists != null)
                {
                    var result = db.Execute($"DELETE Group_Permission WHERE Group_Name='{Group_Name}' and permission='{permission}'");
                    return true;
                }
                return false;
            }
        }
        // Execution ValheimPermissions.ValheimDB.AddUserPermission(STEAMID, PERMISSION_NODE)
        // Important Note: You need to send the SteamID in STRING format instead of the default LONG format. use SteamID.ToString() before sending.
        // Results: Will return false if the permission already exists
        // Or: WIll return true if the add was successful
        // NOTE: Results will be returned to you in BOOL format (true/false)
        public static bool AddUserPermission(string SteamID, string permission)
        {
            permission = permission.ToLower();
            using (var db = new LiteDatabase(DatabaseLocation))
            {
                var permissions = db.GetCollection<User_Permission>("User_Permission");
                var _permission = new User_Permission
                {
                    SteamID = SteamID.ToString(),
                    permission = permission.ToLower()
                };
                var pexists = permissions.FindOne(Query.And(Query.EQ("SteamID", SteamID), Query.EQ("permission", permission)));
                if (pexists != null)
                {
                    return false;
                }
                permissions.Insert(_permission);
                return true;
            }
        }

        // Execution ValheimPermissions.ValheimDB.DelUserPermission(STEAMID, PERMISSION_NODE)
        // Important Note: You need to send the SteamID in STRING format instead of the default LONG format. use SteamID.ToString() before sending.
        // Results: Will return false if the user does not already have access to the permission node
        // Or: WIll return true if the deletion was successful
        // NOTE: Results will be returned to you in BOOL format (true/false)
        public static bool DelUserPermission(string SteamID, string permission)
        {
            permission = permission.ToLower();
            using (var db = new LiteDatabase(DatabaseLocation))
            {
                var permissions = db.GetCollection<User_Permission>("User_Permission");
                var pexists = permissions.FindOne(Query.And(Query.EQ("SteamID", SteamID), Query.EQ("permission", permission)));
                if (pexists != null)
                {
                    db.Execute($"DELETE User_Permission WHERE SteamID='{SteamID}' and permission='{permission}'");
                    return true;
                }
                return false;
            }
        }
        // Execution ValheimPermissions.ValheimDB.PermissionScanGroup(GROUP_NAME, PERMISSION_NODE)
        // Results: Will return true if the group has permission to that node or higher.
        // Or: will return false if the group does not have permission to that node
        // NOTE: Results will be returned to you in BOOL format (true/false)
        public static bool AbsolutePermissionScanGroup(string group, string permission)
        {
            permission = permission.ToLower();

            using (var db = new LiteDatabase(DatabaseLocation))
            {
                var Permissions = db.GetCollection<User_Permission>("Group_Permission");
                var pexists = Permissions.FindOne(Query.And(Query.EQ("Group_Name", group), Query.EQ("permission", permission)));
                if (pexists != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public static bool PermissionScanGroup(string group, string permission)
        {
            permission = permission.ToLower();
            string[] permissionsplit = permission.Split('.');
            using (var db = new LiteDatabase(DatabaseLocation))
            {
                var Permissions = db.GetCollection<Group_Permission>("Group_Permission");
                int i = 0;
                string lookuppermission = "";
                var pexists = Permissions.FindOne(Query.And(Query.EQ("Group_Name", group), Query.EQ("permission", "*")));
                if (pexists != null)
                {
                    return true;
                }
                while (i < permissionsplit.Length)
                {
                    if (i == 0)
                    {
                        lookuppermission = permissionsplit[i].ToLower();
                    }
                    else
                    {
                        lookuppermission = lookuppermission + "." + permissionsplit[i].ToLower();
                    }
                    if (i + 1 != permissionsplit.Length)
                    {
                        pexists = Permissions.FindOne(Query.And(Query.EQ("Group_Name", group), Query.EQ("permission", lookuppermission + ".*")));
                    }
                    else
                    {
                        pexists = Permissions.FindOne(Query.And(Query.EQ("Group_Name", group), Query.EQ("permission", lookuppermission)));
                    }

                    if (pexists != null)
                    {
                        return true;
                    }
                    if (i + 1 == permissionsplit.Length)
                    {
                        return false;
                    }
                    i++;
                }
            }
            return false;
        }
            // Execution ValheimPermission.ValheimDB.ShowUserPermissions(SteamID)
            // Important Note: You need to send the SteamID in STRING format instead of the default LONG format. use SteamID.ToString() before sending.
            // Results: Will return a string[] of all the permission nodes the user has access to.
            // Or: will return one array result (false) if there is no permissions.
            // NOTE: Results will be returned to you in an string[] format! Code accordingly!
        public static string[] ShowUserPermissions(string SteamID)
        {
            string[] nullresult = { "No.Permissions.Found" };
            using (var db = new LiteDatabase(DatabaseLocation))
            {
                var Permissions = db.GetCollection<User_Permission>("User_Permission");
                var count = Permissions.Count(Query.EQ("SteamID", SteamID));
                string[] returnme = new string[0];
                int i = 0;
                if (Util.IsAdmin(long.Parse(SteamID)))
                {
                    returnme = new string[count+1];
                    returnme[0] = "*";
                }
                else
                {
                    returnme = new string[count];
                    i = 0;
                }
                foreach (var item in Permissions.Find(Query.EQ("SteamID", SteamID)).OrderBy(x => x.permission))
                {
                    returnme[i++] = item.permission;
                }
                if ((count > 0) || (returnme.Length > 0))
                {
                    return returnme;
                }
                    return nullresult;
            }
        }

        // Execution ValheimPermission.ValheimDB.ShowGroupPermissions(SteamID)
        // Important Note: You need to send the SteamID in STRING format instead of the default LONG format. use SteamID.ToString() before sending.
        // Results: Will return a string[] of all the permission nodes the user has access to.
        // Or: will return one array result (false) if there is no permissions.
        // NOTE: Results will be returned to you in an string[] format! Code accordingly!
        public static string[] ShowGroupPermissions(string Group_Name)
        {
            string[] nullresult = { "No.Permissions.Found" };
            using (var db = new LiteDatabase(DatabaseLocation))
            {
                var Permissions = db.GetCollection<Group_Permission>("Group_Permission");
                var count = Permissions.Count(Query.EQ("Group_Name", Group_Name));
                string[] returnme = new string[count];
                int i = 0;
                foreach (var item in Permissions.Find(Query.EQ("Group_Name", Group_Name)).OrderBy(x => x.permission))
                {
                    returnme[i++] = item.permission;
                }
                if (count > 0)
                {
                    return returnme;
                }
                return nullresult;
            }
        }
        // Execution ValheimPermissions.ValheimDB.AbsoluteScanUserPermission(SteamID, PERMISSION_NODE)
        // Important Note: You need to send the SteamID in STRING format instead of the default LONG format. use SteamID.ToString() before sending.
        // Results: Will ONLY return true if the user has THIS exact permission node!.
        // NOTE: Results will be returned to you in BOOL format (true/false)
        public static bool AbsoluteScanUserPermission(string SteamID, string permission)
        {
            permission = permission.ToLower();
            
            using (var db = new LiteDatabase(DatabaseLocation))
            {
                var Permissions = db.GetCollection<User_Permission>("User_Permission");
                var pexists = Permissions.FindOne(Query.And(Query.EQ("SteamID", SteamID.ToString()), Query.EQ("permission", permission)));
                if (pexists != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        // Execution ValheimPermissions.ValheimDB.ScanUserPermission(SteamID, PERMISSION_NODE)
        // Important Note: You need to send the SteamID in STRING format instead of the default LONG format. use SteamID.ToString() before sending.
        // Results: Will return true if the user has permission to that node or higher.
        // Or: will return false if the user does not have permission to that node
        // NOTE: Results will be returned to you in BOOL format (true/false)
        public static bool ScanUserPermission(string SteamID, string permission)
        {
            permission = permission.ToLower();
            string[] permissionsplit = permission.Split('.');
            using (var db = new LiteDatabase(DatabaseLocation))
            {
                var Permissions = db.GetCollection<User_Permission>("User_Permission");
                int i = 0;
                string lookuppermission = "";
                if (Util.IsAdmin(long.Parse(SteamID)))
                {
                    return true;
                }
                var pexists = Permissions.FindOne(Query.And(Query.EQ("SteamID", SteamID.ToString()), Query.EQ("permission", lookuppermission + "*")));
                if (pexists != null)
                {
                    return true;
                }
                while (i < permissionsplit.Length)
                {
                    if (i == 0)
                    {
                        lookuppermission = permissionsplit[i].ToLower();
                    }
                    else
                    {
                        lookuppermission = lookuppermission + "." + permissionsplit[i].ToLower();
                    }
                    if (i + 1 != permissionsplit.Length)
                    {
                        
                        pexists = Permissions.FindOne(Query.And(Query.EQ("SteamID", SteamID.ToString()), Query.EQ("permission", lookuppermission + ".*")));
                    }
                    else
                    {
                        pexists = Permissions.FindOne(Query.And(Query.EQ("SteamID", SteamID.ToString()), Query.EQ("permission", lookuppermission)));
                    }

                    if (pexists != null)
                    {
                        return true;
                    }
                    if (i + 1 == permissionsplit.Length)
                    {
                        return false;
                    }
                    i++;
                }
            }
            return false;
        }
        // Execution ValheimPermissions.ValheimDB.GetGroup(SteamID)
        // Important Note: You need to send the SteamID in STRING format instead of the default LONG format. use SteamID.ToString() before sending.
        // Results: Will return the group that the user is in.
        // Or: If that user is not in a group. return "Default"
        // NOTE: Results will be returned to you in string format as described above.
        public static string GetGroup(string SteamID)
        {
            using (var db = new LiteDatabase(DatabaseLocation))
            {
                var users = db.GetCollection<User>("User");
                var result = users.Find(x => x.SteamID == SteamID).FirstOrDefault();
                if (result == null)
                {
                    return "Default";
                }
                else
                {
                    return result.Group_Name;
                }
            }
        }
        // Execution ValheimPermissions.ValheimDB.CheckUserPermission(SteamID, Permission)
        // Important Note: You need to send the SteamID in STRING format instead of the default LONG format. use SteamID.ToString() before sending.
        // Results: Will first check the users group for a permission node and if they have it return true
        // Or: If false above. will check the user for the permission node and if they have it return true
        // Or: return false if neither have it.
        // NOTE: Results will be returned to you in BOOL format (true/false)
        public static bool CheckUserPermission(string SteamID, string permission)
        {
            permission = permission.ToLower();
            if (PermissionScanGroup(GetGroup(SteamID), permission))
                return true;
            else
            {
                if (ScanUserPermission(SteamID, permission))
                    return true;
                else 
                    return false;
            }
        }
        public static bool CheckUserAbsolutePermission(string SteamID, string permission)
        {
            permission = permission.ToLower();
            if (AbsolutePermissionScanGroup(GetGroup(SteamID), permission))
                return true;
            else
            {
                if (AbsoluteScanUserPermission(SteamID, permission))
                    return true;
                else
                    return false;
            }
        }
        // Execution ValheimPermissions.ValheimDB.AddUserToGroup(SteamID, Group_Name)
        // Important Note: You need to send the SteamID in STRING format instead of the default LONG format. use SteamID.ToString() before sending.
        // Results: Will first see if the user is already in a group. If so return "false user"
        // Or: will check if the group exists. If not return "false group"
        // Or: return true and add the users new group.
        // NOTE: Results will be returned to you in STRING format as described above.
        public static string AddUserToGroup(string SteamID, string group)
        {
            using (var db = new LiteDatabase(DatabaseLocation))
            {
                var groups = db.GetCollection<Group>("Group");
                var users = db.GetCollection<User>("User");
                var user = new User
                {
                    SteamID = SteamID,
                    Group_Name = group
                };
                var uexists = users.FindOne(Query.EQ("SteamID", SteamID));
                if (uexists != null)
                {
                    return "false user";
                }
                var gexists = groups.FindOne(Query.EQ("Group_Name", group));
                if (gexists == null)
                {
                    return "false group";
                }
                users.Insert(user);
                return "true";
            }
        }
        // Execution ValheimPermissions.ValheimDB.DelUserGroup(STEAMID)
        // Results: Will return false if the User is not in a group
        // Or: Will remove the user from the group they are currently in..
        // NOTE: Results will be returned to you in BOOL format (true/false).
        public static bool DelUserGroup(string steamid)
        {
            using (var db = new LiteDatabase(DatabaseLocation))
            {
                var users = db.GetCollection<User>("User");
                var exists = users.FindOne(Query.EQ("SteamID", steamid));
                if (exists == null)
                {
                    return false;
                }
                else
                {
                    var resultsusers = db.Execute($"DELETE User WHERE SteamID='{steamid}'");
                    return true;
                }
            }
        }

    }
}
