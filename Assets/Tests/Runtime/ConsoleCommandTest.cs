using UnityEngine;
using NUnit.Framework;
using qASIC.Console;
using System;
using qASIC.InputManagement;
using qASIC.InputManagement.Map;
using UnityEngine.SceneManagement;

namespace qASIC.Tests.Runtime
{
    public class ConsoleCommandTest : MonoBehaviour
    {
        [Test]
        public void AudioParameter()
        {
            RunCommand("audioparameter", "test");
            RunCommand("audioparameter", "test", "0");
        }

        [Test]
        public void Clear()
        {
            RunCommand("clear");
            Assert.IsTrue(GetLastLog().Type == GameConsoleLog.LogType.Clear);
        }

        [Test]
        public void ClearDebug() =>
            RunCommand("cleardebugdisplayer");

        [Test]
        public void DebugDisplayer()
        {
            RunCommand("debugdisplayer");
            RunCommand("debugdisplayer", "true");
        }

        [Test]
        public void Echo() =>
            RunCommand("echo", "test message");

        [Test]
        public void FOV()
        {
            RunCommand("fov");
            RunCommand("fov", "60");
        }

        [Test]
        public void Help()
        {
            RunCommand("help");
            RunCommand("help", "help");
            RunCommand("help", "0");
        }

        [Test]
        public void Input()
        {
            InputMap map = Resources.Load<InputMap>("qASIC/Tests/Map");
            InputManager.LoadMap(map);

            RunCommand("changeinput", "Action0", "Return");
            Assert.IsTrue(InputManager.GetKeyCode("Action0", 0) == KeyCode.Return);
            RunCommand("changeinput", "Action1", "0", "Return");
            Assert.IsTrue(InputManager.GetKeyCode("Action1", 0) == KeyCode.Return);
            RunCommand("changeinput", "Group1", "Action0", "Return");
            Assert.IsTrue(InputManager.GetKeyCode("Group1", "Action0", 0) == KeyCode.Return);
            RunCommand("changeinput", "Group1", "Action1", "0", "Return");
            Assert.IsTrue(InputManager.GetKeyCode("Group1", "Action1", 0) == KeyCode.Return);
        }

        [Test]
        public void InputList() =>
            RunCommand("inputlist");

        [Test]
        public void Options()
        {
            //No tests yet
        }

        [Test]
        public void Scene()
        {
            string cmd = "scene";

            //TODO: test scene reloading
            RunCommand(cmd);

            RunCommand(cmd, "reload");
            WaitForSceneLoad(() => 
            {
                RunCommand(cmd, SceneManager.GetActiveScene().name);
                WaitForSceneLoad(() =>
                {
                    RunCommand(cmd, SceneManager.GetActiveScene().buildIndex.ToString());
                });
            });
        }
        public void WaitForSceneLoad(Action onLoad)
        {
            SceneManager.sceneLoaded -= (Scene scene, LoadSceneMode mode) => WaitForSceneLoad(onLoad);
            onLoad?.Invoke();
        }

        [Test]
        public void SceneList() =>
            RunCommand("scenelist");

        [Test]
        public void SettingsList() =>
            RunCommand("settinglist");

        [Test]
        public void Specification() =>
            RunCommand("specification");

        [Test]
        public void TimeScale() =>
            RunCommand("timescale 1");

        [Test]
        public void Version() =>
            RunCommand("version");

        GameConsoleLog GetLastLog() =>
            GameConsoleController.logs[GameConsoleController.logs.Count - 1];

        void RunCommand(params string[] args) =>
            RunCommand(true, args);

        void RunCommand(bool log, params string[] args)
        {
            string cmd = string.Join(" ", args);
            GameConsoleController.RunCommand(cmd);
            Debug.Log(cmd);
            if (log)
                Debug.Log(GetLastLog().Message);
        }
    }
}