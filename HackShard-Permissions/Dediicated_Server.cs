using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValheimPermissions
{
    public static class Dedicated_Server
    {
        public static void RunCommand(string text)
        {

            string[] results = text.Split(' ');
            if (results[0].ToLower().Equals($"!user"))
            {
                if (results[1].ToLower().Equals($"add"))
                {
                    if (results[2].ToLower().Equals($"permission"))
                        Util.Dedicated_Commands.AddUserPermission(long.Parse(results[3]), results[4]);
                    else
                        Util.Dedicated_Commands.AddUserToGroup(long.Parse(results[2]), results[3]);
                    return;
                }
                if (results[1].ToLower().Equals($"check"))
                {
                    if (results[2].ToLower().Equals($"group"))
                    {
                        Debug.Log($"Requesting group name for the user: {results[3]}!");
                        Util.Dedicated_Commands.CheckGroup(long.Parse(results[3]));
                    }
                    if (results[2].ToLower().Equals($"permission"))
                    {
                        Debug.Log($"Attempting to lookup permission: {results[4]} for the user {results[3]}!");
                        Util.Dedicated_Commands.CheckUserPermission(long.Parse(results[3]), results[4]);
                    }
                }
            }
            if (results[0].ToLower().Equals($"!group"))
            {
                if (results[1].ToLower().Equals($"create"))
                {
                    Debug.Log($"Attempting to create the group {results[2]}!");
                    Util.Dedicated_Commands.AddGroup(results[2]);
                }
                if (results[1].ToLower().Equals($"add"))
                {
                    if (results[2].ToLower().Equals($"permission"))
                    {
                        Debug.Log($"Attempting to add permission: {results[4]} to the group {results[3]}!");
                        Util.Dedicated_Commands.AddGroupPermission(results[3], results[4]);
                    }
                }
                if (results[1].ToLower().Equals($"check"))
                {
                    if (results[2].ToLower().Equals($"permission"))
                    {
                        Debug.Log($"Attempting to lookup permission: {results[4]} for the group {results[3]}!");
                        Util.Dedicated_Commands.CheckGroupPermission(results[3], results[4]);
                    }
                }
            }
        }
    }
}
