
/* Disabled until Fixed
using static WorldofValheimServerSideCharacters.Console.CMethods;
using static WorldofValheimServerSideCharacters.Console.CUtils;


namespace WorldofValheimServerSideCharacters.Console
{
    class Console
    {
        public static Console instance;

        public List<Command> commands = new List<Command>();

        public List<string> ModList = new List<string>();

        public static Console Instance
        {
            get
            {
                return Console.instance;
            }
        }
        public void ObtainModList()
        {
            ModList = CUtils.LoadModList(Utils.GetSaveDataPath() + "/modlist.txt");
        }

        public Console()
        {
            Debug.Log("Loading Console");
            instance = this;
            if (WorldofValheimServerSideCharacters.ServerMode)
            {
                ObtainModList();
                Debug.Log("Server Side commands have been loaded.");
                RegisterServerCommands();
                return;
            }
            Debug.Log("Client Side commands have been loaded.");
            RegisterClientCommands();
        }

        public void RegisterClientCommands()
        {
            commands.Add(new Command("!help", "!help - List of all commands", Help, false));
            commands.Add(new Command("!version", "!version - prints server version", PrintVersion, false));
        }
        public void RegisterServerCommands()
        {
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
                                    Debug.Log(command.Hint);
                                    return;
                                }
                                Debug.Log($"Something went wrong with args[0]: {args[0]}");
                            }
                        }
                    }
                }
                Debug.Log("Invalid command to get all commands please use: !help");
            }
        }

    }
}
*/