﻿using UnityEngine;
using NUnit.Framework;
using qASIC.Console;
using System;

namespace qASIC.Tests.Runtime
{
    public class ConsoleControllerTests : MonoBehaviour
    {
        static GameConsoleLog testLog = new GameConsoleLog("[Test] This is a test", DateTime.Now, Color.green, GameConsoleLog.LogType.Game);

        [Test]
        public void Log()
        {
            GameConsoleController.Log(testLog);

            Assert.IsTrue(GetLastLog() == testLog);
        }

        [Test]
        public void CommandException() =>
            RunCommand("test exception");

        void RunCommand(params string[] args) =>
            RunCommand(true, args);

        void RunCommand(bool log, params string[] args)
        {
            GameConsoleController.RunCommand(string.Join(" ", args));
            if (log)
                Debug.Log(GetLastLog().Message);
        }

        GameConsoleLog GetLastLog() =>
            GameConsoleController.logs[GameConsoleController.logs.Count - 1];
    }
}