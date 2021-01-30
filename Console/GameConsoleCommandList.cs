using qASIC.Console.Commands;
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
            List<GameConsoleCommand> _commands = GetList();
            command = null;
            for (int i = 0; i < _commands.Count; i++)
            {
                if (AliasExists(_commands[i], commandName))
                {
                    command = _commands[i];
                    return true;
                }
            }
            return false;
        }

        private static bool AliasExists(GameConsoleCommand command, string targetName)
        {
            if (command.commandName == targetName) return true;
            if (command.aliases != null)
                for (int i = 0; i < command.aliases.Length; i++)
                    if (command.aliases[i] == targetName)
                        return true;
            return false;
        }

        public static List<GameConsoleCommand> GetList()
        {
            List<Type> types = FindAllDerivedTypes<GameConsoleCommand>();
            commands = new List<GameConsoleCommand>();
            for (int i = 0; i < types.Count; i++)
            {
                ConstructorInfo constructor = types[i].GetConstructor(Type.EmptyTypes);
                commands.Add((GameConsoleCommand)constructor.Invoke(null));
            }
            return commands;
        }

        public static List<Type> FindAllDerivedTypes<T>()
        { return FindAllDerivedTypes<T>(Assembly.GetAssembly(typeof(T))); }

        public static List<Type> FindAllDerivedTypes<T>(Assembly assembly)
        {
            var derivedType = typeof(T);
            return assembly.GetTypes().Where(t => t != derivedType && derivedType.IsAssignableFrom(t)).ToList();
        }

        private static List<GameConsoleCommand> commands = new List<GameConsoleCommand>();
    }
}