using System.Collections;
using System.Collections.Generic;
using UnityEngine.TestTools;
using UnityEngine;
using NUnit.Framework;
using qASIC.Console;
using qASIC.Tools;
using System;
using qASIC.Console.Commands;

namespace qASIC.Tests.Runtime
{
    public class ConsoleTests : MonoBehaviour
    {
        static GameConsoleLog testLog = new GameConsoleLog("[Test] This is a test", DateTime.Now, Color.green, GameConsoleLog.LogType.Game);

        [Test]
        public void Log()
        {
            GameConsoleController.Log(testLog);

            if (GetLastLog() != testLog)
                throw new Exception("Console log is different");
        }

        [Test]
        public void Commands()
        {
            foreach (var command in GameConsoleCommandList.Commands)
            {
                GameConsoleController.RunCommand(command.CommandName);

                if (GetLastLog().Message.StartsWith($"Command '{command.CommandName}' execution failed"))
                    throw new Exception("Command execution failed!");

                for (int i = 0; i < command.Aliases.Length; i++)
                {
                    GameConsoleController.RunCommand(command.Aliases[i]);

                    if (GetLastLog().Message.StartsWith("Command not found"))
                        throw new Exception($"Command '{command.CommandName}' alias '{command.Aliases[i]}' doesn't work");

                    if (GetLastLog().Message.StartsWith("Command execution failed"))
                        throw new Exception($"Command '{command.CommandName}' execution failed!");
                }
            }
        }

        [Test]
        public void CommandSort()
        {
            string testCommand = "test \"\" \"tes\"t1\" \"test2\" test3";
            string[] expectedResult = new string[] { "test", "", "tes\"t1", "test2", "test3" };

            List<string> args = GameConsoleController.SortCommand(testCommand);

            if (args.Count != 5)
                throw new Exception("Incorrect length!");

            for (int i = 0; i < 5; i++)
                if (args[i] != expectedResult[i])
                    throw new Exception($"Arg i:{i} c:{args} is supposed to be equal {expectedResult[i]}");
        }

        GameConsoleLog GetLastLog() =>
            GameConsoleController.logs[GameConsoleController.logs.Count - 1];
    }
}