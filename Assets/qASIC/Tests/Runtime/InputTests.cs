using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using qASIC.InputManagement;
using qASIC.InputManagement.Map;
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
        public void GetAction()
        {
            Assert.IsTrue(InputManager.TryGetInputAction("Group0", "Action0", out InputAction action));
            Assert.IsNotNull(action);
        }

        [Test]
        public void GetAxis()
        {
            Assert.IsTrue(InputManager.TryGetInputAxis("Group0", "Axis0", out InputAxis axis));
            Assert.IsNotNull(axis);
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
            InputManager.GetInputDown(new InputActionReference("Group0", "Action0"));
            InputManager.GetInput(new InputActionReference("Group0", "Action0"));
            InputManager.GetInputUp(new InputActionReference("Group0", "Action0"));
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

        [Test]
        public void GetMapAxis()
        {
            InputManager.GetMapAxis("Axis0");
            InputManager.GetMapAxis("Group0", "Axis0");
            InputManager.GetMapAxisRaw("Axis0");
            InputManager.GetMapAxisRaw("Group0", "Axis0");
        }

        [Test]
        public void CreateAxis()
        {
            InputManager.CreateAxis("Action0", "Action1");
            InputManager.CreateAxis(KeyCode.A, KeyCode.B);

            Assert.AreEqual(0f, InputManager.CreateAxis(false, false));
            Assert.AreEqual(-1f, InputManager.CreateAxis(false, true));
            Assert.AreEqual(1f, InputManager.CreateAxis(true, false));
            Assert.AreEqual(0f, InputManager.CreateAxis(true, true));
        }
    }
}