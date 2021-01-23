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
            AssignList();
            command = null;
            for (int i = 0; i < commands.Count; i++)
                if (commands[i].commandName == commandName)
                { command = commands[i]; return true; }
            return false;
        }

        public static void AssignList()
        {
            List<Type> types = FindAllDerivedTypes<GameConsoleCommand>();
            commands = new List<GameConsoleCommand>();
            for (int i = 0; i < types.Count; i++)
            {
                ConstructorInfo constructor = types[i].GetConstructor(Type.EmptyTypes);
                commands.Add((GameConsoleCommand)constructor.Invoke(null));
            }
        }

        public static List<Type> FindAllDerivedTypes<T>()
        { return FindAllDerivedTypes<T>(Assembly.GetAssembly(typeof(T))); }

        public static List<Type> FindAllDerivedTypes<T>(Assembly assembly)
        {
            var derivedType = typeof(T);
            return assembly.GetTypes().Where(t => t != derivedType && derivedType.IsAssignableFrom(t)).ToList();
        }

        public static List<GameConsoleCommand> commands = new List<GameConsoleCommand>();
        /*public static GameConsoleCommand[] commands = new GameConsoleCommand[]
        {
            new GameConsoleHelp(),
            new GameConsoleEcho(),
            new GameConsoleOptionCommand(),
            new GameConsoleInputCommand(),
            new GameConsoleSceneCommand(),
        };*/
    }
}