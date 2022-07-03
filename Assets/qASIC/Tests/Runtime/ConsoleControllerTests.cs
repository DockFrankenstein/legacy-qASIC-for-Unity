using UnityEngine;
using NUnit.Framework;
using qASIC.Console;
using System;
using qASIC.Console.Internal;

namespace qASIC.Tests.Runtime
{
    public class ConsoleControllerTests : MonoBehaviour
    {
        static GameConsoleLog testLog = new GameConsoleLog("[Test] This is a test", DateTime.Now, Color.green, GameConsoleLog.LogType.Game);

        [OneTimeSetUp]
        public void Setup()
        {
            GameConsoleController.AssignConfig(Resources.Load<GameConsoleConfig>("qASIC/Tests/Console Config"));
        }

        [Test]
        public void Log()
        {
            GameConsoleController.Log(testLog);

            Assert.IsTrue(GetLastLog() == testLog);
        }

        [Test]
        public void CommandException() =>
            _RunCommand("test exception");

        void _RunCommand(params string[] args) =>
            _RunCommand(true, args);

        void _RunCommand(bool log, params string[] args)
        {
            GameConsoleController.RunCommand(string.Join(" ", args));
            if (log)
                Debug.Log(GetLastLog().Message);
        }

        [Test]
        public void IgnoreUnityConsoleLog()
        {
            GameConsoleController.Log(testLog);
            Assert.AreEqual(GetLastLog(), testLog);
        }

        GameConsoleLog GetLastLog() =>
            GameConsoleController.logs[GameConsoleController.logs.Count - 1];
    }
}