using System.Collections.Generic;
using System;
using System.Reflection;
using qASIC.Tools;

namespace qASIC.Console.Commands
{
    public class GameConsoleCommandList
    {
        private static bool _disableInitialization;
        public static bool DisableInitialization => _disableInitialization;

        private static bool _initialized = false;
        public static bool Initialized => _initialized;

        public static void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            qDebug.Log("Initializing command list...", "init");

            _disableInitialization = ProjectSettings.ConsoleProjectSettings.Instance.startArgsDisableCommandInitialization
                && Array.IndexOf(Environment.GetCommandLineArgs(), "-qASIC-console-disable-commandlistinitialization") != -1;

            UpdateList();

            qDebug.Log($"Successfully finished console command list initialization! Command count: {Commands.Count}", "init");
        }

        public static bool TryGettingCommandByName(string commandName, out GameConsoleCommand command)
        {
            command = null;
            for (int i = 0; i < Commands.Count; i++)
            {
                if (!CompareName(Commands[i], commandName) || !Commands[i].Active) continue;

                command = Commands[i];
                return true;
            }
            return false;
        }

        private static bool CompareName(GameConsoleCommand command, string targetName)
        {
            if (command.CommandName.Trim().ToLower() == targetName) return true;

            if (command.Aliases == null) return false;
            for (int i = 0; i < command.Aliases.Length; i++)
                if (command.Aliases[i].Trim().ToLower() == targetName) 
                    return true;

            return false;
        }

        public static List<GameConsoleCommand> UpdateList()
        {
            _commands = new List<GameConsoleCommand>();

            if (DisableInitialization)
                return _commands;

            List<Type> types = TypeFinder.FindAllTypes<GameConsoleCommand>();
            for (int i = 0; i < types.Count; i++)
            {
                ConstructorInfo constructor = types[i].GetConstructor(Type.EmptyTypes);
                if (constructor == null || constructor.IsAbstract) continue;
                GameConsoleCommand command = (GameConsoleCommand)constructor.Invoke(null);
                if (command.Active)
                    _commands.Add(command);
            }
            return _commands;
        }

        private static List<GameConsoleCommand> _commands;
        public static List<GameConsoleCommand> Commands 
        {
            get
            {
                if (_commands == null)
                    UpdateList();

                return _commands;
            }
        }
    }
}