using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using System;
using qASIC.Input;
using qASIC.Input.Map;
using qASIC.Input.Map.Internal;

namespace qASIC.Tests.Editor
{
    public class InputTestsEditor
    {
        static InputMap map;
        static InputMapWindow window;

        [SetUp]
        public void Setup()
        {
            map = Resources.Load<InputMap>("qASIC/Tests/Map");
            window = InputMapWindow.GetEditorWindow();
            InputMapWindow.OpenWindow();
            InputMapWindow.OpenMap(map);
        }

        void _ModifyMap()
        {
            //InputMapWindow.Map.groups[0].actions[0]. = "TEMP TEST MODIFICATION";
            //InputMapWindow.SetMapDirty();
        }

        //bool _IsMapModified() =>
        //    InputMapWindow.Map.groups[0].actions[0].actionName == "TEMP TEST MODIFICATION";

        [Test]
        public void OpenMap()
        {
            InputMapWindow.OpenMap(map);
        }

        [Test]
        public void SetWindowTitle() =>
            window.SetWindowTitle();

        [Test]
        public void ReloadTrees() =>
            window.ReloadTrees();

        [Test]
        public void ResetEditor() =>
            window.ResetEditor();

        [Test]
        public void SetMapDirty()
        {
            //InputMapWindow.SetMapDirty();
            //Assert.IsTrue(InputMapWindow.IsDirty);
        }

        //[Test]
        //public void Save() =>
        //    InputMapWindow.Save();

        //[Test]
        //public void DiscardMapChanges()
        //{
        //    _ModifyMap();
        //    window.DiscardMapChanges();
        //    Assert.IsFalse(_IsMapModified());
        //}

        [Test]
        public void DeleteUnmodifiedOnClose()
        {
            window.Close();
            Assert.IsFalse(System.IO.File.Exists(InputMapWindow.GetUnmodifiedMapLocation()));

            //Finishing
            window = InputMapWindow.GetEditorWindow();
        }

        [TearDown]
        public void Teardown()
        {
            window.DiscardMapChanges();
            //InputMapWindow.CloseMap();
            window.Close();
        }
    }
}