using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace ValheimPermissions
{
    public static class Dedicated_Server
    {
        // Server side Console command processing.
        public static void ProcessServerSideCommand(string text)
        {
            string[] results = text.Split(' ');
            // User Command
            if (results[0].ToLower().Equals($"!user"))
            {
                // Add Sub-Command (Can be a Group name or Permission name)
                if (results[1].ToLower().Equals($"add"))
                {
                    // if the next word is permission lets add the permission
                    if (results[2].ToLower().Equals($"permission"))
                    {
                        Debug.Log("Running permission add command");
                        Util.Dedicated_Commands.ServerSideConsole.AddUserPermission(long.Parse(results[3]), results[4]);
                    }
                    // if the next word is a group then lets add the group
                    else
                        Util.Dedicated_Commands.ServerSideConsole.AddUserToGroup(long.Parse(results[2]), results[3]);
                    return;
                }
                // Delete Sub-Command
                if (results[1].ToLower().Equals($"del"))
                    if (results[2].ToLower().Equals($"permission"))
                    {
                        Util.Dedicated_Commands.ServerSideConsole.DeleteUserPermission(long.Parse(results[3]), results[4]);
                    }
                // Check Sub-Command
                if (results[1].ToLower().Equals($"check"))
                {
                    // Check which group the user is in
                    if (results[2].ToLower().Equals($"group"))
                    {
                        Debug.Log($"Requesting group name for the user: {results[3]}!");
                        Util.Dedicated_Commands.ServerSideConsole.CheckGroup(long.Parse(results[3]));
                    }
                    // check A permission or list all permissions
                    if (results[2].ToLower().Equals($"permission"))
                    {
                        // If there is no permission requested show ALL permissions
                        if (results.Count() == 4)
                        {
                            Debug.Log($"Attempting to lookup all permissions for the user {results[3]}!");
                            Util.Dedicated_Commands.ServerSideConsole.ShowUserPermissions(long.Parse(results[3]));
                        }
                        // Show the requested permission
                        else
                        {
                            Debug.Log($"Attempting to lookup permission: {results[4]} for the user {results[3]}!");
                            Util.Dedicated_Commands.ServerSideConsole.CheckUserPermission(long.Parse(results[3]), results[4]);
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
                    Debug.Log($"Attempting to create the group {results[2]}!");
                    Util.Dedicated_Commands.ServerSideConsole.AddGroup(results[2]);
                }
                // Delete a group
                if (results[1].ToLower().Equals($"del"))
                {
                    if (results[2].ToLower().Equals($"permission"))
                    {
                        Util.Dedicated_Commands.ServerSideConsole.DelGroupPermission(results[3], results[4]);
                    }
                    else
                    {
                        Debug.Log($"Attempting to delete the group {results[2]}!");
                        Util.Dedicated_Commands.ServerSideConsole.DelGroup(results[2]);
                    }
                }
                // Add Sub-Command
                if (results[1].ToLower().Equals($"add"))
                {
                    // Add a permission to the selected group
                    if (results[2].ToLower().Equals($"permission"))
                    {
                        Debug.Log($"Attempting to add permission: {results[4]} to the group {results[3]}!");
                        Util.Dedicated_Commands.ServerSideConsole.AddGroupPermission(results[3], results[4]);
                    }
                }
                // Check Sub-Command
                if (results[1].ToLower().Equals($"check"))
                {
                    // Check a permission or list all permissions owned by a group
                    if (results[2].ToLower().Equals($"permission"))
                    {
                        // If no permission requested return all permissions owned by this group
                        if (results.Count() == 4)
                        {
                            Debug.Log($"Attempting to lookup all permissions for the group {results[3]}!");
                            Util.Dedicated_Commands.ServerSideConsole.ShowGroupPermissions(results[3]);
                        }
                        // Check the requested permission against the group
                        else
                        {
                            Debug.Log($"Attempting to lookup permission: {results[4]} for the group {results[3]}!");
                            Util.Dedicated_Commands.ServerSideConsole.CheckGroupPermission(results[3], results[4]);
                        }
                    }
                }
            }
        }
    }
}
