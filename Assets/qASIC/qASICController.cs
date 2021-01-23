using qASIC.InputManagment;
using qASIC.Console;
using System.Collections.Generic;
using UnityEngine;

namespace qASIC
{
    public class qASICController : MonoBehaviour
    {
        private static bool hasStarted = false;

        private void Awake()
        {
            if (!hasStarted)
            {
                InputManager.LoadKeys();
                LogInput();
            }
        }

        private void LogInput()
        {
            Console.Commands.GameConsoleInputCommand.Print();
        }
    }
}