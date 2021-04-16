using System;
using System.Linq;
using LiteDB;
using HarmonyLib;

namespace ValheimPermissions
{
    public class ValheimDB
    {
        public class User
        {
            public string SteamID { get; set; }
            public string Group_Name { get; set; }
        }
        public class Group
        {
            public string Group_Name { get; set; }
        }
        public class Permission
        {
            public class Group
            {
                public string Group_Name { get; set; }
                public string permission { get; set; }
            }
            public class User
            {
                public string SteamID { get; set; }
                public string permission { get; set; }
            }
        
        }
        // Execution: ValheimPermissions.ValheimDB.AddGroup(GROUP_NAME)
        // Result: Will return TRUE or FALSE if the group was created or not.
        // NOTE: Results will be returned to you in BOOL format (true/false)
        public static bool AddGroup(string group_name)
        {
            using (var db = new LiteDatabase(@"HsG-Database.db"))
            {
                var groups = db.GetCollection<Group>("Group");
                var group = new Group
                {
                    Group_Name = group_name
                };
                var exists = groups.FindOne(Query.EQ("Group_Name", group_name));
                if (exists != null) {
                    return false;
                }
                else
                {
                    groups.Insert(group);
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
            using (var db = new LiteDatabase(@"HsG-Database.db"))
            {
                var groups = db.GetCollection<Group>("Group");
                var permissions = db.GetCollection<Permission.Group>("Permission");
                var _permission = new Permission.Group
                {
                    Group_Name = group,
                    permission = permission
                };
                var gexists = groups.FindOne(Query.EQ("Group_Name", group));
                if (gexists == null)
                {
                    return "false group";
                }
                var pexists = permissions.FindOne(Query.And(Query.Contains("Group_Name", group), Query.Contains("permission", permission)));
                if (pexists != null)
                {
                    return $"false exists";
                }
                permissions.Insert(_permission);
                return "true";
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
            using (var db = new LiteDatabase(@"HsG-Database.db"))
            {
                var permissions = db.GetCollection<Permission.User>("Permission");
                var _permission = new Permission.User
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
        // Execution ValheimPermissions.ValheimDB.PermissionScanGroup(GROUP_NAME, PERMISSION_NODE)
        // Results: Will return true if the group has permission to that node or higher.
        // Or: will return false if the group does not have permission to that node
        // NOTE: Results will be returned to you in BOOL format (true/false)
        public static bool PermissionScanGroup(string group, string permission)
        {
            permission = permission.ToLower();
            string[] permissionsplit = permission.Split('.');
            using (var db = new LiteDatabase(@"HsG-Database.db"))
            {
                var Permissions = db.GetCollection<Permission.Group>("Permission");
                int i = 0;
                string lookuppermission = "";
                var pexists = Permissions.FindOne(Query.And(Query.EQ("Group_Name", group), Query.EQ("permission", lookuppermission + "*")));
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
                    i = i + 1;
                }
            }
            return false;
        }
        // Execution ValheimPermissions.ValheimDB.PermissionScanUser(SteamID, PERMISSION_NODE)
        // Important Note: You need to send the SteamID in STRING format instead of the default LONG format. use SteamID.ToString() before sending.
        // Results: Will return true if the user has permission to that node or higher.
        // Or: will return false if the user does not have permission to that node
        // NOTE: Results will be returned to you in BOOL format (true/false)
        public static bool PermissionScanUser(string SteamID, string permission)
        {
            permission = permission.ToLower();
            string[] permissionsplit = permission.Split('.');
            using (var db = new LiteDatabase(@"HsG-Database.db"))
            {
                if (Util.isAdmin(long.Parse(SteamID)))
                    return true;
                var Permissions = db.GetCollection<Permission.User>("Permission");
                int i = 0;
                string lookuppermission = "";
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
                    i = i + 1;
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
            using (var db = new LiteDatabase(@"HsG-Database.db"))
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
        // Execution ValheimPermissions.ValheimDB.CheckUserPermission(SteamID)
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
                if (PermissionScanUser(SteamID, permission))
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
            using (var db = new LiteDatabase(@"HsG-Database.db"))
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

    }
}
