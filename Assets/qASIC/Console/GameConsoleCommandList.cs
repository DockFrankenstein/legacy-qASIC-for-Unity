using qASIC.Console.Commands;
using System.Collections.Generic;
using System;
using System.Reflection;
using qASIC.Tools;

namespace qASIC.Console
{
    public class GameConsoleCommandList
    {
        public static bool TryGettingCommandByName(string commandName, out GameConsoleCommand command)
        {
            command = null;
            for (int i = 0; i < Commands.Count; i++)
            {
                if (!AliasExists(Commands[i], commandName) || !Commands[i].Active) continue;

                command = Commands[i];
                return true;
            }
            return false;
        }

        private static bool AliasExists(GameConsoleCommand command, string targetName)
        {
            if (command.CommandName == targetName) return true;
            if (command.Aliases == null) return false;
            for (int i = 0; i < command.Aliases.Length; i++)
                if (command.Aliases[i] == targetName) 
                    return true;
            return false;
        }

        public static List<GameConsoleCommand> UpdateList()
        {
            List<Type> types = TypeFinder.FindAllTypes<GameConsoleCommand>();
            _commands.Clear();
            for (int i = 0; i < types.Count; i++)
            {
                ConstructorInfo constructor = types[i].GetConstructor(Type.EmptyTypes);
                _commands.Add((GameConsoleCommand)constructor.Invoke(null));
            }
            return _commands;
        }

        private static List<GameConsoleCommand> _commands = new List<GameConsoleCommand>();
        public static List<GameConsoleCommand> Commands 
        {
            get
            {
                if (_commands.Count == 0) UpdateList();
                return _commands;
            }
        }
    }
}