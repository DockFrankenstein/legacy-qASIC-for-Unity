using qASIC.Console.Commands;

namespace qASIC.Console
{
    public class GameConsoleCommandList
    {
        public static bool TryGettingCommandByName(string commandName, out GameConsoleCommand command)
        {
            command = null;
            for (int i = 0; i < commands.Length; i++)
            {
                if (commands[i].commandName == commandName)
                {
                    command = commands[i];
                    return true;
                }
            }
            return false;
        }

        public static GameConsoleCommand[] commands = new GameConsoleCommand[]
        {
            new GameConsoleHelp() { commandName = "help", description = "displays help" },
            new GameConsoleEcho() { commandName = "echo", description = "creates a new log containing a message" },
            new GameConsoleOptionCommand() { commandName = "changeoption", description = "changes basic options" },
            new GameConsoleConfigManager() { commandName = "config", description = "control config files" },
            new GameConsoleInputCommand() { commandName = "input", description = "change, print input" },
            new GameConsoleSceneCommand() { commandName = "scene", description = "Log, load scene" },
        };
    }
}