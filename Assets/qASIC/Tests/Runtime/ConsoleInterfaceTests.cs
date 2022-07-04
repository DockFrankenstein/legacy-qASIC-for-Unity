using UnityEngine;
using NUnit.Framework;
using qASIC.Console;
using System;

namespace qASIC.Tests.Runtime
{
    public class ConsoleInterfaceTests : MonoBehaviour
    {
        GameConsoleInterface console;

        [OneTimeSetUp]
        public void Setup()
        {
            console = new GameObject("Console").AddComponent<GameConsoleInterface>();
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Destroy(console);
        }

        [Test]
        public void LogRefresh()
        {
            string log = "Log refresh test";

            GameConsoleController.Log(new GameConsoleLog("", DateTime.Now, Color.white, GameConsoleLog.LogType.Clear));
            GameConsoleController.Log(log, Color.white);

            Assert.IsTrue(console.Content.Contains(log), console.Content);
        }
    }
}