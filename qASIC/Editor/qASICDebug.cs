#if UNITY_EDITOR
using qASIC.AudioManagment;
using qASIC.Console;
using UnityEngine;
using UnityEditor;

namespace qASIC.Tools.Debug
{
    public class qASICDebug : EditorWindow
    {
        [MenuItem("Window/qASIC/Debug")]
        static void Init()
        {
            qASICDebug window = (qASICDebug)GetWindow(typeof(qASICDebug), false, "qASIC Debug", true);
            window.Show();
        }

        Texture2D CreateColorTexture(int width, int height, Color color)
        {
            Texture2D texture = new Texture2D(width, height);
            Color[] blackTexturePixels = texture.GetPixels();
            for (int i = 0; i < blackTexturePixels.Length; i++)
                blackTexturePixels[i] = color;
            texture.alphaIsTransparency = true;
            texture.SetPixels(blackTexturePixels);
            texture.Apply();
            return texture;
        }

        string consoleInput;
        Vector2 scroll;
        Vector2 audioScroll;
        bool resetPrefsWindow;

        enum DebugWindow { Info, Creation, Audio, Console, Other };
        DebugWindow window = DebugWindow.Info;

        Texture2D tintedTexture { get => CreateColorTexture(1, 1, new Color(0f, 0f, 0f, 0.2f)); }

        void OnGUI()
        {
            GUIStyle headerStyle = new GUIStyle("Label")
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 24,
                margin = new RectOffset(0, 0, 16, 0),
            };

            GUIStyle menuButtonStyle = new GUIStyle("Button")
            {
                padding = new RectOffset(10, 10, 10, 10),
                margin = new RectOffset(0, 0, 0, 0),
            };
            menuButtonStyle.normal.background = CreateColorTexture(1, 1, new Color(0f, 0f, 0f, 0f));
            menuButtonStyle.normal.scaledBackgrounds = new Texture2D[0];

            GUIStyle menuSelectedButtonStyle = new GUIStyle(menuButtonStyle);
            menuSelectedButtonStyle.normal.background = tintedTexture;

            GUIStyle menuStyle = new GUIStyle();
            menuStyle.normal.background = tintedTexture;

            GUIStyle contentStyle = new GUIStyle()
            {
                padding = new RectOffset(10, 10, 10, 10),
            };

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(menuStyle, GUILayout.Width(64), GUILayout.Height(Screen.height));
            if (GUILayout.Button("Info", window == DebugWindow.Info ? menuSelectedButtonStyle : menuButtonStyle)) window = DebugWindow.Info;
            if (GUILayout.Button("Creation menu", window == DebugWindow.Creation ? menuSelectedButtonStyle : menuButtonStyle)) window = DebugWindow.Creation;
            if (GUILayout.Button("Audio", window == DebugWindow.Audio ? menuSelectedButtonStyle : menuButtonStyle)) window = DebugWindow.Audio;
            if (GUILayout.Button("Console", window == DebugWindow.Console ? menuSelectedButtonStyle : menuButtonStyle)) window = DebugWindow.Console;
            if (GUILayout.Button("Other", window == DebugWindow.Other ? menuSelectedButtonStyle : menuButtonStyle)) window = DebugWindow.Other;
            GUILayout.EndVertical();

            //Content
            GUILayout.BeginVertical(contentStyle);
            scroll = GUILayout.BeginScrollView(scroll);

            //Menus
            switch(window)
            {
                default:
                    DisplayInfo(headerStyle);
                    break;
                case DebugWindow.Creation:
                    DisplayCreate(headerStyle);
                    break;
                case DebugWindow.Audio:
                    DisplayAudio(headerStyle);
                    break;
                case DebugWindow.Console:
                    DisplayConsole(headerStyle);
                    break;
                case DebugWindow.Other:
                    DisplayOther(headerStyle);
                    break;
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();


            GUILayout.EndHorizontal();
        }

        void DisplayInfo(GUIStyle headerStyle)
        {
            GUIStyle tintedBox = new GUIStyle("Label");
            tintedBox.normal.background = tintedTexture;

            GUILayout.Label("Versions", headerStyle);

            GUILayout.BeginVertical(tintedBox);
            GUILayout.Label($"qASIC: {Info.Version}");
            GUILayout.Label($"Game Console: {Info.Version}");
            GUILayout.Label($"Info Displayer: {Info.Version}");
            GUILayout.Label($"Options System: {Info.Version}");
            GUILayout.Label($"Input Management: {Info.Version}");
            GUILayout.Label($"Audio Management: {Info.Version}");
            GUILayout.Label($"File Management: {Info.Version}");
            GUILayout.EndVertical();


            GUILayout.Label("Specs", headerStyle);

            GUILayout.BeginVertical(tintedBox);
            GUILayout.Label($"CPU: {SystemInfo.processorType}");
            GUILayout.Label($"CPU Threads: {SystemInfo.processorCount}");
            GUILayout.Label($"GPU: {SystemInfo.graphicsDeviceName}");
            GUILayout.Label($"RAM: {SystemInfo.systemMemorySize}MB");
            GUILayout.Label($"OS: {SystemInfo.operatingSystem}");
            GUILayout.Label($"Unity Engine: {Application.unityVersion}");
            GUILayout.EndVertical();
        }

        void DisplayCreate(GUIStyle headerStyle)
        {
            GUILayout.Label("Creation Menu", headerStyle);
            if (GUILayout.Button("Create Info Displayer")) EditorApplication.ExecuteMenuItem("GameObject/qASIC/Displayer");
            if (GUILayout.Button("Create Game Console")) EditorApplication.ExecuteMenuItem("GameObject/qASIC/Game console");
            if (GUILayout.Button("Create Audio Manager")) EditorApplication.ExecuteMenuItem("GameObject/qASIC/Audio manager");
        }

        void DisplayAudio(GUIStyle headerStyle)
        {
            GUIStyle listItemStyle = new GUIStyle("Label");
            GUIStyle secondListItemStyle = new GUIStyle(listItemStyle);
            secondListItemStyle.normal.background = tintedTexture;

            GUIStyle scrollStyle = new GUIStyle();
            scrollStyle.normal.background = tintedTexture;

            GUIStyle centeredLabelStyle = new GUIStyle("Label")
            {
                alignment = TextAnchor.MiddleCenter,
            };
            
            GUILayout.Label("Audio", headerStyle);
            if (AudioManager.singleton == null)
            {
                GUILayout.Label(EditorApplication.isPlaying ? "There is no audio manager in the scene" : "Offline", centeredLabelStyle);
                return;
            }

            GUILayout.Label($"Paused: {AudioManager.Paused}");
            GUILayout.Label($"Channel count: {AudioManager.ChannelCount}");

            audioScroll = GUILayout.BeginScrollView(audioScroll, scrollStyle, GUILayout.Height(128));
            switch (Application.isPlaying && AudioManager.singleton != null)
            {
                default:
                    break;
                case true:
                    int i = 0;
                    foreach (var channel in AudioManager.singleton.channels)
                    {
                        GUILayout.Label(channel.Key, i % 2 == 0 ? secondListItemStyle : listItemStyle, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                        i++;
                    }
                    break;
            }
            GUILayout.EndScrollView();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Pause All") && AudioManager.singleton != null) AudioManager.PauseAll();
            if (GUILayout.Button("Un Pause All") && AudioManager.singleton != null) AudioManager.UnPauseAll();
            if (GUILayout.Button("Stop All") && AudioManager.singleton != null) AudioManager.StopAll();
            GUILayout.EndHorizontal();
        }

        void DisplayConsole(GUIStyle headerStyle)
        {
            GUIStyle consoleStyle = new GUIStyle()
            {
                normal = new GUIStyleState()
                {
                    background = CreateColorTexture(1, 1, Color.black),
                    textColor = Color.white,
                },
                alignment = TextAnchor.LowerLeft,
                clipping = TextClipping.Clip,
            };

            GUILayout.Label("Console", headerStyle);
            GUILayout.Label(Application.isPlaying ? GameConsoleController.LogsToString(9) : "Console is offline", consoleStyle, GUILayout.Height(128));
            GUILayout.BeginHorizontal();
            consoleInput = GUILayout.TextField(consoleInput);
            if (GUILayout.Button("Run", GUILayout.Width(32)) && Application.isPlaying)
            {
                GameConsoleController.RunCommand(consoleInput);
                consoleInput = string.Empty;
            }
            GUILayout.EndHorizontal();
        }

        void DisplayOther(GUIStyle headerStyle)
        {
            GUILayout.Label("Other", headerStyle);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Unlock cursor")) Cursor.lockState = CursorLockMode.None;
            if (GUILayout.Button("Lock cursor")) Cursor.lockState = CursorLockMode.Locked;
            if (GUILayout.Button("Confine")) Cursor.lockState = CursorLockMode.Confined;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            switch (resetPrefsWindow)
            {
                default:
                    if (GUILayout.Button("Reset Player Prefs")) resetPrefsWindow = true;
                    break;
                case true:
                    if (GUILayout.Button("Cancel")) resetPrefsWindow = false;
                    if (GUILayout.Button("Confirm"))
                    {
                        PlayerPrefs.DeleteAll();
                        resetPrefsWindow = false;
                    }
                    break;
            }
            GUILayout.EndHorizontal();
        }
    }
}
#endif