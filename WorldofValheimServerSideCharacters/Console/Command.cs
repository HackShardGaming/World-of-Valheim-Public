/* Disabling until Fixed
namespace WorldofValheimServerSideCharacters.Console
{
    // Main command handler class for the plugin
    class Command
    {
        private readonly string commandName;
        private readonly string argHint;
        private readonly bool adminCmd;
        private readonly Method method;

        public Command(string commandName, string argHint, Method method, bool adminCmd)
        {
            this.commandName = commandName;
            this.argHint = argHint;
            this.method = method;
            this.adminCmd = adminCmd;
        }

        public delegate bool Method(string[] args);
        public string Name { get => commandName; }
        public string Hint { get => argHint; }
        public bool AdminCmd { get => adminCmd; }

        public bool Run(string[] args)
        {
            return (bool)method.DynamicInvoke(new object[] { args });
        }

    }
}
*/