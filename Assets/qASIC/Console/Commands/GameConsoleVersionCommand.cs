using System.Collections.Generic;
using UnityEngine;
using qASIC.Console.Tools;
using qASIC.Tools;

namespace qASIC.Console.Commands
{
    public class GameConsoleVersionCommand : GameConsoleCommand
    {
        public override bool Active { get => GameConsoleController.GetConfig().versionCommand; }
        public override string CommandName { get; } = "version";
        public override string Description { get; } = "displays version";
        public override string Help { get; } = "displays version";
        public override string[] Aliases { get; } = new string[] { "info", "about" };

        int toolCount = 0;

        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 0)) return;
            string log = Application.productName;
            GameConsoleConfig config = GameConsoleController.GetConfig();

            toolCount = 0;

            if (config.displayGameVersion) log += $" v{Application.version}";
            log += CreateToolText(config.displayUnityVerion, $"Unity v{Application.unityVersion}");
            log += CreateToolText(config.displayQasicVersion, $"qASIC v{Info.Version}");

            Log(log, "info");
        }

        string CreateToolText(bool use, string text)
        {
            string result = string.Empty;
            if (!use) return result;

            if (toolCount == 0) result += " made with ";
            else result += " and ";

            result += text;
            toolCount++;
            return result;
        }
    }
}