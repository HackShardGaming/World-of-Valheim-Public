using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace ValheimPermissions
{
    public static class RPC
    {
        public static void ProcessClientSideCommand_RPC(long sender, string text)
        {

        }
        public static void ProcessClientSideCommand(long sender, string text)
        {
            ZNetPeer peer = ZNet.instance.GetPeer(sender);
            string peerSteamID = ((ZSteamSocket)peer.m_socket).GetPeerID().m_SteamID.ToString(); // Get the SteamID from peer.
            string[] results = text.Split(' ');
            long l = 0;
            /// Did we receive a !? Lets process!
            if (results[0].ToLower().StartsWith($"!"))
            {
                Debug.Log($"User: {peerSteamID} is attempting to run the following command {text} ");
            }
            string PermissionNode = "HackShardGaming.ValheimPermissions";
            // User Command
            if (results[0].ToLower().Equals($"!user"))
            {
                // Add Sub-Command (Can be a Group name or Permission name)
                if (results[1].ToLower().Equals($"add"))
                {
                    // if the next word is permission lets add the permission
                    if (results[2].ToLower().Equals($"permission"))
                    {
                        // Creating Permission Node HackShardGaming.ValheimPermissions.User.Add.Permission
                        PermissionNode = PermissionNode + ".User.Add.Permission";
                        if (ValheimDB.CheckUserPermission(peerSteamID, PermissionNode))
                        {

                            if (long.TryParse(results[3], out l))
                            {
                                Util.RoutedBroadcast(sender, $"Attempting to add the Permission Node: {results[4]} to the User: {results[3]}");
                                Util.Dedicated_Commands.ClientSideCommands.AddUserPermission(sender, long.Parse(results[3]), results[4]);
                                return;
                            }
                            // Convert a Player Name back to the SteamID (put a / for spaces)
                            string User = Util.GetPeerSteamID(results[3]);
                            if (User != "00000000000000000")
                            {
                                Util.RoutedBroadcast(sender, $"Attempting to add the Permission Node: {results[4]} to the User: {User}");
                                Util.Dedicated_Commands.ClientSideCommands.AddUserPermission(sender, long.Parse(User), results[4]);
                                return;
                            }
                            Util.RoutedBroadcast(sender, $"ERROR: The User: {results[3]} is not online!");
                            return;
                        }
                        else
                        {
                            Util.RoutedBroadcast(sender, $"ERROR: You do not have permission to access this command. (Needed Permission Node: {PermissionNode}");
                            return;
                        }
                    }
                    // if the next word is a group then lets add the group
                    // Creating Permission Node HackShardGaming.ValheimPermissions.User.Add.Group
                    PermissionNode = PermissionNode + ".User.Add.Group";
                    if (ValheimDB.CheckUserPermission(peerSteamID, PermissionNode))
                    {
                        // Convert a Player Name back to the SteamID (put a / for spaces)
                        if (long.TryParse(results[2], out l))
                        {
                            Util.RoutedBroadcast(sender, $"Attempting to add the User: {results[2]} to the following Group: {results[3]}");
                            Util.Dedicated_Commands.ClientSideCommands.AddUserToGroup(sender, long.Parse(results[2]), results[3]);
                            return;
                        }
                        string User = Util.GetPeerSteamID(results[2]);
                        if (User != "00000000000000000")
                        {
                            Util.RoutedBroadcast(sender, $"Attempting to add the User: {User} to the following Group: {results[3]}");
                            Util.Dedicated_Commands.ClientSideCommands.AddUserToGroup(sender, long.Parse(User), results[3]);
                            return;
                        }
                        Util.RoutedBroadcast(sender, $"ERROR: The User: {results[2]} is not online!");
                        return;
                    }
                    else
                    {
                        Util.RoutedBroadcast(sender, $"ERROR: You do not have permission to access this command. (Needed Permission Node: {PermissionNode}");
                        return;
                    }
                }
                // Delete Sub-Command
                if (results[1].ToLower().Equals($"del"))
                    if (results[2].ToLower().Equals($"permission"))
                    {
                        // Creating Permission Node HackShardGaming.User.Del.Permission
                        PermissionNode = PermissionNode + ".User.Del.Permission";
                        if (ValheimDB.CheckUserPermission(peerSteamID, PermissionNode))
                        {
                            if (long.TryParse(results[3], out l))
                            {
                                Util.RoutedBroadcast(sender, $"Attempting to delete the Permission Node: {results[4]} from the User: {results[3]}");
                                Util.Dedicated_Commands.ClientSideCommands.DeleteUserPermission(sender, long.Parse(results[3]), results[4]);
                                return;
                            }
                            // Convert a Player Name back to the SteamID (put a / for spaces)
                            string User = Util.GetPeerSteamID(results[3]);
                            if (User != "00000000000000000")
                            {
                                Util.RoutedBroadcast(sender, $"Attempting to delete the Permission Node: {results[4]} from the User: {User}");
                                Util.Dedicated_Commands.ClientSideCommands.DeleteUserPermission(sender, long.Parse(User), results[4]);
                                return;
                            }
                            Util.RoutedBroadcast(sender, $"ERROR: The User: {results[3]} is not online!");
                            return;
                        }
                        else
                        {
                            Util.RoutedBroadcast(sender, $"ERROR: You do not have permission to access this command. (Needed Permission Node: {PermissionNode}");
                            return;
                        }
                    }
                // Check Sub-Command
                if (results[1].ToLower().Equals($"check"))
                {
                    // Check which group the user is in
                    if (results[2].ToLower().Equals($"group"))
                    {
                        PermissionNode = PermissionNode + ".User.Check.Group";
                        if (ValheimDB.CheckUserPermission(peerSteamID, PermissionNode))
                        {
                            if (long.TryParse(results[3], out l))
                            {
                                Util.RoutedBroadcast(sender, $"Requested the Group Name for the User: {results[3]}");
                                Util.Dedicated_Commands.ClientSideCommands.CheckGroup(sender, long.Parse(results[3])); 
                                return;
                            }
                            // Convert a Player Name back to the SteamID (put a / for spaces)
                            string User = Util.GetPeerSteamID(results[3]);
                            if (User != "00000000000000000")
                            {
                                Util.RoutedBroadcast(sender, $"Requested the Group Name for the User: {User}");
                                Util.Dedicated_Commands.ClientSideCommands.CheckGroup(sender, long.Parse(User));
                                return;
                            }
                            Util.RoutedBroadcast(sender, $"ERROR: The User: {results[3]} is not online!");
                            return;
                        }
                        else
                        {
                            Util.RoutedBroadcast(sender, $"ERROR: You do not have permission to access this command. (Needed Permission Node: {PermissionNode}");
                            return;
                        }
                    }
                    // check A permission or list all permissions
                    if (results[2].ToLower().Equals($"permission"))
                    {
                        PermissionNode = PermissionNode + ".User.Check.Permission";
                        if (ValheimDB.CheckUserPermission(peerSteamID, PermissionNode))
                        {
                            
                            // If there is no permission requested show ALL permissions
                            if (results.Count() == 4)
                            {
                                if (long.TryParse(results[3], out l))
                                {
                                    Util.RoutedBroadcast(sender, $"Attempting to lookup ALL permissions for the User: {results[3]}");
                                    Util.Dedicated_Commands.ClientSideCommands.ShowUserPermissions(sender, long.Parse(results[3]));
                                    return;
                                }
                                // Convert a Player Name back to the SteamID (put a / for spaces)
                                string User = Util.GetPeerSteamID(results[3]);
                                if (User != "00000000000000000")
                                {
                                    Util.RoutedBroadcast(sender, $"Attempting to lookup ALL permissions for the User: {User}");
                                    Util.Dedicated_Commands.ClientSideCommands.ShowUserPermissions(sender, long.Parse(User));
                                    return;
                                }
                                Util.RoutedBroadcast(sender, $"ERROR: The User: {results[3]} is not online!");
                                return;
                            }
                            // Show the requested permission
                            else
                            {
                                // Convert a Player Name back to the SteamID (put a / for spaces)
                                if (long.TryParse(results[3], out l))
                                {
                                    Util.RoutedBroadcast(sender, $"Attempting to check the User: {results[3]} against the Permission Node: {results[4]}");
                                    Util.Dedicated_Commands.ClientSideCommands.CheckUserPermission(sender, long.Parse(results[3]), results[4]);
                                    return;
                                }
                                string User = Util.GetPeerSteamID(results[3]);
                                if (User != "00000000000000000")
                                {
                                    Util.RoutedBroadcast(sender, $"Attempting to check the User: {User} against the Permission Node: {results[4]}");
                                    Util.Dedicated_Commands.ClientSideCommands.CheckUserPermission(sender, long.Parse(User), results[4]);
                                    return;
                                }
                                Util.RoutedBroadcast(sender, $"ERROR: The User: {results[3]} is not online!");
                                return;
                            }
                        }
                        else
                        {
                            Util.RoutedBroadcast(sender, $"ERROR: You do not have permission to access this command. (Needed Permission Node: {PermissionNode}");
                            return;
                        }
                    }
                }
            }
            // Group Command
            if (results[0].ToLower().Equals($"!group"))
            {
                // Create a group
                if (results[1].ToLower().Equals($"create"))
                {
                    PermissionNode = PermissionNode + ".Group.Create";
                    if (ValheimDB.CheckUserPermission(peerSteamID, PermissionNode))
                    {
                        Util.RoutedBroadcast(sender, $"Attempting to create the Group {results[2]}");
                        Util.Dedicated_Commands.ClientSideCommands.AddGroup(sender, results[2]);
                        return;
                    }
                    else
                    {
                        Util.RoutedBroadcast(sender, $"ERROR: You do not have permission to access this command. (Needed Permission Node: {PermissionNode}");
                        return;
                    }
                }
                // Delete a group
                if (results[1].ToLower().Equals($"del"))
                {
                    if (results[2].ToLower().Equals($"permission"))
                    {
                        PermissionNode = PermissionNode + ".Group.Del.Permission";
                        if (ValheimDB.CheckUserPermission(peerSteamID, PermissionNode))
                        {
                            Util.RoutedBroadcast(sender, $"Attempting to delete the Permission Node: {results[4]} from the Group: {results[3]}");
                            Util.Dedicated_Commands.ClientSideCommands.DelGroupPermission(sender, results[3], results[4]);
                            return;
                        }
                        else
                        {
                            Util.RoutedBroadcast(sender, $"ERROR: You do not have permission to access this command. (Needed Permission Node: {PermissionNode}");
                            return;
                        }
                    }
                    else
                    {
                        PermissionNode = PermissionNode + ".Group.Del.Group";
                        if (ValheimDB.CheckUserPermission(peerSteamID, PermissionNode))
                        {
                            Util.RoutedBroadcast(sender, $"Attempting to delete the group {results[2]}!");
                            Util.Dedicated_Commands.ClientSideCommands.DelGroup(sender, results[2]);
                            return;
                        }
                        else
                        {
                            Util.RoutedBroadcast(sender, $"ERROR: You do not have permission to access this command. (Needed Permission Node: {PermissionNode}");
                            return;
                        }
                    }
                }
                // Add Sub-Command
                if (results[1].ToLower().Equals($"add"))
                {
                    // Add a permission to the selected group
                    if (results[2].ToLower().Equals($"permission"))
                    {
                        PermissionNode = PermissionNode + ".Group.Add.Permission";
                        if (ValheimDB.CheckUserPermission(peerSteamID, PermissionNode))
                        {
                            Util.RoutedBroadcast(sender, $"Attempting to add the Permission Node: {results[4]} to the Group: {results[3]}");
                            Util.Dedicated_Commands.ClientSideCommands.AddGroupPermission(sender, results[3], results[4]);
                            return;
                        }
                        else
                        {
                            Util.RoutedBroadcast(sender, $"ERROR: You do not have permission to access this command. (Needed Permission Node: {PermissionNode}");
                            return;
                        }
                    }
                }
                // Check Sub-Command
                if (results[1].ToLower().Equals($"check"))
                {
                    // Check a permission or list all permissions owned by a group
                    if (results[2].ToLower().Equals($"permission"))
                    {
                        PermissionNode = PermissionNode + ".Group.Check.Permission";
                        if (ValheimDB.CheckUserPermission(peerSteamID, PermissionNode))
                        {
                            // If no permission requested return all permissions owned by this group
                            if (results.Count() == 4)
                            {
                                Util.RoutedBroadcast(sender, $"Attempting to lookup ALL Permission Nodes for the Group: {results[3]}");
                                Util.Dedicated_Commands.ClientSideCommands.ShowGroupPermissions(sender, results[3]);
                                return;
                            }
                            // Check the requested permission against the group
                            else
                            {
                                Util.RoutedBroadcast(sender, $"Attempting to check the Group: {results[3]} against the Permission Node: {results[3]}");
                                Util.Dedicated_Commands.ClientSideCommands.CheckGroupPermission(sender, results[3], results[4]);
                                return;
                            }
                        }
                        else
                        {
                            Util.RoutedBroadcast(sender, $"ERROR: You do not have permission to access this command. (Needed Permission Node: {PermissionNode}");
                            return;
                        }
                    }
                }
            }
            return;
        }
    }
}
