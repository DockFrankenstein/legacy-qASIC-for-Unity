using System.Collections.Generic;
using qASIC.Console.Tools;
using UnityEngine;

namespace qASIC.Console.Commands
{
    public class GameConsoleSpecsCommand : GameConsoleCommand
    {
        public override bool Active { get => GameConsoleController.GetConfig().specsCommand; }
        public override string CommandName { get; } = "specification";
        public override string Description { get; } = "displays system specs";
        public override string Help { get; } = "displays system specs";
        public override string[] Aliases { get; } = new string[] { "hardware", "hardware info", "specs", "hardware specification", "hardware specs" };

        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 0)) return;

            string log = "Hardware specification: ";
            GameConsoleConfig config = GameConsoleController.GetConfig();

            if (config.displayCpu) log += $"\nProcessor: {SystemInfo.processorType}";
            if (config.displayCpuThreads) log += $"\nThread count: {SystemInfo.processorCount}";
            if (config.displayGpu) log += $"\nGraphics card: {SystemInfo.graphicsDeviceName}";
            if (config.displayMemory) log += $"\nSystem memory: {SystemInfo.systemMemorySize}MB";
            if (config.displaySystem) log += $"\nOperating system: {SystemInfo.operatingSystem}";

            Log(log, "info");
        }
    }
}