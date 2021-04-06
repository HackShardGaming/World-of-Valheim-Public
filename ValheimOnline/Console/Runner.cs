using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BepInEx;

using static ValheimOnline.Console.CMethods;
using static ValheimOnline.Console.CUtils;

namespace ValheimOnline.Console
{
    class Runner
    {
        public static Runner instance;

        public List<Command> commands = new List<Command>();

        public List<string> ModList = new List<string>();

        public static Runner Instance
        {
            get
            {
                return Runner.instance;
            }
        }
        public void ObtainModList()
        {
            ModList = CUtils.LoadModList(Utils.GetSaveDataPath() + "/modlist.txt");
        }

        public Runner()
        {
            Debug.Log("Loading Console");
            instance = this;
            if (ValheimOnline.ServerMode)
            {
                ObtainModList();
                RegisterServerCommands();
            }

            RegisterClientCommands();

        }

        public void RegisterClientCommands()
        {
            commands.Add(new Command("!help", "!help - List of all commands", Help, false));
            commands.Add(new Command("!version", "!version - prints server version", PrintVersion, false));
        }
        public void RegisterServerCommands()
        {
            commands.Add(new Command("!zone-reload", "!zone-reload - Reload zones from zones.txt file", ZoneReload, true));
            commands.Add(new Command("!shutdown", "!shutdown - Shutdown the server", Shutdown, true));
            commands.Add(new Command("!save-all", "!save-all - Save all the characters currently connected to the server", SaveAll, true));
        }

        public void RunCommand(string text, bool calledFromClient)
        {
            if (!text.IsNullOrWhiteSpace())
            {
                text = text.Trim();
                string[] args = text.Split(' ');
                if (args.Length > 0)
                {
                    Command command = commands.Find(c => c.Name.Equals(args[0].ToLower()));
                    if (!calledFromClient || command.AdminCmd)
                    {
                        if (command != null)
                        {
                            bool finished = command.Run((string[])args.Clone());
                            if (finished)
                            {
                                return;
                            }
                            else
                            {
                                if (!args[0].IsNullOrWhiteSpace())
                                {
                                    Print(command.Hint);
                                    return;
                                }
                                Print($"Something went wrong with args[0]: {args[0]}");
                            }
                        }
                    }
                }
                Print("Invalid command to get all commands please use: !help");
            }
        }

    }
}
