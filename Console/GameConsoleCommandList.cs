﻿using qASIC.Console.Commands;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace qASIC.Console
{
    public class GameConsoleCommandList
    {
        public static bool TryGettingCommandByName(string commandName, out GameConsoleCommand command)
        {
            command = null;
            for (int i = 0; i < Commands.Count; i++)
            {
                if (!AliasExists(Commands[i], commandName)) continue;
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
            List<Type> types = FindAllTypes();
            _commands.Clear();
            for (int i = 0; i < types.Count; i++)
            {
                ConstructorInfo constructor = types[i].GetConstructor(Type.EmptyTypes);
                _commands.Add((GameConsoleCommand)constructor.Invoke(null));
            }
            return _commands;
        }

        public static List<Type> FindAllTypes()
        {
            var derivedType = typeof(GameConsoleCommand);
            return Assembly.GetAssembly(typeof(GameConsoleCommand)).GetTypes().Where(t => t != derivedType && derivedType.IsAssignableFrom(t)).ToList();
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