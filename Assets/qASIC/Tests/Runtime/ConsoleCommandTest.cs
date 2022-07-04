using UnityEngine;
using NUnit.Framework;
using qASIC.Console;
using System;
using qASIC.InputManagement;
using qASIC.InputManagement.Map;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using System.Collections;

namespace qASIC.Tests.Runtime
{
    public class ConsoleCommandTest : MonoBehaviour
    {
        [Test]
        public void AudioParameter()
        {
            _RunCommand("audioparameter", "test");
            _RunCommand("audioparameter", "test", "0");
        }

        [Test]
        public void Clear()
        {
            _RunCommand("clear");
            Assert.IsTrue(_GetLastLog().Type == GameConsoleLog.LogType.Clear);
        }

        [Test]
        public void ClearDebug() =>
            _RunCommand("cleardebugdisplayer");

        [Test]
        public void DebugDisplayer()
        {
            _RunCommand("debugdisplayer");
            _RunCommand("debugdisplayer", "true");
        }

        [Test]
        public void Echo() =>
            _RunCommand("echo", "test message");

        [Test]
        public void FOV()
        {
            _RunCommand("fov");
            _RunCommand("fov", "60");
        }

        [Test]
        public void Help()
        {
            _RunCommand("help");
            _RunCommand("help", "help");
            _RunCommand("help", "0");
        }

        [Test]
        public void Input()
        {
#if !ENABLE_LEGACY_INPUT_MANAGER
            throw new Exception("This test requires the old Input Handler!");
#else
            InputMap map = Resources.Load<InputMap>("qASIC/Tests/Map");
            InputManager.LoadMap(map);

            _RunCommand("changeinput", "Action0", "Return");
            Assert.IsTrue(InputManager.GetKeyCode("Action0", 0) == KeyCode.Return);
            _RunCommand("changeinput", "Action1", "0", "Return");
            Assert.IsTrue(InputManager.GetKeyCode("Action1", 0) == KeyCode.Return);
            _RunCommand("changeinput", "Group1", "Action0", "Return");
            Assert.IsTrue(InputManager.GetKeyCode("Group1", "Action0", 0) == KeyCode.Return);
            _RunCommand("changeinput", "Group1", "Action1", "0", "Return");
            Assert.IsTrue(InputManager.GetKeyCode("Group1", "Action1", 0) == KeyCode.Return);
#endif
        }

        [Test]
        public void InputList() =>
            _RunCommand("inputlist");

        [Test]
        public void Options()
        {
            //No tests yet
        }

        bool _waitForSceneLoad;

        [UnityTest]
        public IEnumerator Scene()
        {
            string cmd = "scene";

            SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) => _SceneCommandHandleSceneLoad();

            //Print
            _RunCommand(cmd);

            //Reload
            _WaitForSceneCommand(cmd, "reload");
            while (_waitForSceneLoad) yield return null;

            //Load with name
            _WaitForSceneCommand(cmd, SceneManager.GetActiveScene().name);
            while (_waitForSceneLoad) yield return null;

            //Load with index
            _WaitForSceneCommand(cmd, SceneManager.GetActiveScene().buildIndex.ToString());
            while (_waitForSceneLoad) yield return null;

            SceneManager.sceneLoaded -= (Scene scene, LoadSceneMode mode) => _SceneCommandHandleSceneLoad();
        }

        void _SceneCommandHandleSceneLoad() =>
            _waitForSceneLoad = false;

        void _WaitForSceneCommand(params string[] args)
        {
            _RunCommand(args);
            _waitForSceneLoad = true;
        }

        [Test]
        public void SceneList() =>
            _RunCommand("scenelist");

        [Test]
        public void SettingsList() =>
            _RunCommand("settinglist");

        [Test]
        public void Specification() =>
            _RunCommand("specification");

        [Test]
        public void TimeScale() =>
            _RunCommand("timescale 1");

        [Test]
        public void Version() =>
            _RunCommand("version");

        //Test command only appears in the editor, so it won't be possible to
        //run this test successfully in a standalone build
#if UNITY_EDITOR
        [Test]
        public void OtherAssemblyCommand()
        {
            _RunCommand("test");
            Assert.AreNotEqual(GameConsoleController.Constants.CommandNotFoundMessage, _GetLastLog().Message);
        }
#endif

        GameConsoleLog _GetLastLog() =>
            GameConsoleController.logs[GameConsoleController.logs.Count - 1];

        void _RunCommand(params string[] args) =>
            _RunCommand(true, args);

        void _RunCommand(bool log, params string[] args)
        {
            string cmd = string.Join(" ", args);
            GameConsoleController.RunCommand(cmd);
            Debug.Log(cmd);
            if (log)
                Debug.Log(_GetLastLog().Message);
        }
    }
}