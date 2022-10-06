using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using qASIC.SettingsSystem;
using UnityEngine;

namespace qASIC.Tests.Runtime
{
    public class OptionsTest
    {
        bool _disableLoading;
        bool _disableSaving;

        [OneTimeSetUp]
        public void Setup()
        {
            _disableLoading = OptionsController.DisableLoading;
            _disableSaving = OptionsController.DisableSaving;

            OptionsController.DisableLoading = true;
            OptionsController.DisableSaving = true;

            OptionsController.OverrideEnabled(true);
            OptionsController.Initialize();
        }

        void _ChangeOption(string optionName, object value) =>
            Assert.AreEqual(1, OptionsController.ChangeOption(optionName, value, true, false));

        [Test]
        public void Resolution() =>
            _ChangeOption("resolution", "1920x1080");

        [Test]
        public void Fullscreen() =>
            _ChangeOption("fullscreen", false);

        [Test]
        public void FullscreenMode() =>
            _ChangeOption("resolution", FullScreenMode.Windowed);

        [Test]
        public void Framelimit() =>
            _ChangeOption("framelimit", 60);

        [Test]
        public void VsyncBool() =>
            _ChangeOption("vsync", true);

        [Test]
        public void VsyncInt() =>
            _ChangeOption("vsync", 1);

        [Test]
        public void TryGetOptionValue()
        {
            object expectedValue = "1920x1080";
            OptionsController.ChangeOption("resolution", expectedValue, true, false);

            Assert.IsTrue(OptionsController.TryGetOptionValue("resolution", out object value));
            Assert.AreEqual(expectedValue, value);
        }

        [Test]
        public void GetSettingsList()
        {
            Debug.Log(OptionsController.GetSettingsList().Count);
        }



        [OneTimeTearDown]
        public void Teardown()
        {
            OptionsController.DisableLoading = _disableLoading;
            OptionsController.DisableSaving = _disableSaving;
        }
    }
}