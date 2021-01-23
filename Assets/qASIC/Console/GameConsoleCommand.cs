using System.Collections.Generic;

namespace qASIC.Console.Commands
{
    public interface GameConsoleCommand
    {
        void Run(List<string> args);

        string commandName { get; set; }
        string description { get; set; }
    }
}