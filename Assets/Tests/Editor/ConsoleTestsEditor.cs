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
            string testCommand = "test \"\" \"tes\"t1\" \"test2\" test3";
            string[] expectedResult = new string[] { "test", "", "tes\"t1", "test2", "test3" };

            List<string> args = GameConsoleController.SortCommand(testCommand);

            if (args.Count != 5)
                throw new Exception("Incorrect length!");

            for (int i = 0; i < 5; i++)
                if (args[i] != expectedResult[i])
                    throw new Exception($"Arg i:{i} c:{args} is supposed to be equal {expectedResult[i]}");
        }

        [Test]
        public void CommandArgumentCount()
        {
            GameConsoleEchoCommand command = new GameConsoleEchoCommand();
            List<string> args = new List<string>(3);

            Assert.IsTrue(command.CheckForArgumentCount(args, 1) == false);
            Assert.IsTrue(command.CheckForArgumentCount(args, 2) == true);
        }
    }
}