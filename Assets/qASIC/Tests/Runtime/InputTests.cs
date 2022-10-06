using NUnit.Framework;
using qASIC.Input;
using qASIC.Input.Map;
using UnityEngine;

namespace qASIC.Tests.Runtime
{
    public class InputTests
    {
        static InputMap map;

        bool _disableLoading;
        bool _disableSaving;

        [SetUp]
        public void SetUp()
        {
            _disableLoading = InputManager.DisableLoading;
            _disableSaving = InputManager.DisableSaving;

            InputManager.DisableLoading = true;
            InputManager.DisableSaving = true;

            map = Resources.Load<InputMap>("qASIC/Tests/Map");
            InputManager.LoadMap(map);

            Assert.IsTrue(InputManager.MapLoaded);
        }

        [TearDown]
        public void TearDown()
        {
            InputManager.DisableLoading = _disableLoading;
            InputManager.DisableSaving = _disableSaving;

            InputManager.LoadMap(null);
        }

        [Test]
        public void GetInput()
        {
            InputManager.GetInputDown("Group0", "Action0");
            InputManager.GetInput("Group0", "Action0");
            InputManager.GetInputUp("Group0", "Action0");
            InputManager.GetInputDown("Action0");
            InputManager.GetInput("Action0");
            InputManager.GetInputUp("Action0");
        }

        [Test]
        public void GetKeyCode()
        {
            //Assert.AreEqual(KeyCode.A, InputManager.GetKeyCode("Action1", 0));
            //Assert.AreEqual(KeyCode.A, InputManager.GetKeyCode("Group0", "Action1", 0));
        }

        [Test]
        public void ChangeInput()
        {
            //InputManager.ChangeInput("Action0", 0, KeyCode.B);
            //Assert.AreEqual(KeyCode.B, InputManager.GetKeyCode("Action0", 0));
            //InputManager.ChangeInput("Group0", "Action0", 0, KeyCode.C);
            //Assert.AreEqual(KeyCode.C, InputManager.GetKeyCode("Group0", "Action0", 0));
        }
    }
}