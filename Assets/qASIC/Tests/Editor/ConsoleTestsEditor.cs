using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using System;
using qASIC.Console;
using qASIC.Console.Commands;

namespace qASIC.Tests.Editor
{
    public class ConsoleTestsEditor
    {
        [Test]
        public void CommandSort()
        {
            string testCommand = "test \"\" \"tes\"t1\" \"test2\" \"test3\"";
            string[] expectedResult = new string[] { "test", "", "tes\"t1", "test2", "test3" };

            List<string> args = GameConsoleController.SortCommand(testCommand);

            Assert.AreEqual(5, args.Count);

            for (int i = 0; i < 5; i++)
                Assert.AreEqual(expectedResult[i], args[i]);
        }

        [Test]
        public void CommandArgumentCount()
        {
            GameConsoleEchoCommand command = new GameConsoleEchoCommand();
            List<string> args = new List<string>(new string[3]);

            Assert.IsTrue(command.CheckForArgumentCount(args, 1) == false, GetLastLog().Message);
            Assert.IsTrue(command.CheckForArgumentCount(args, 2) == true, GetLastLog().Message);
        }

        GameConsoleLog GetLastLog() =>
            GameConsoleController.logs[GameConsoleController.logs.Count - 1];
    }
}