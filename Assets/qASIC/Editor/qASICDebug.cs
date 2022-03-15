#if UNITY_EDITOR
using qASIC.Console;
using UnityEngine;
using UnityEditor;
using qASIC.Internal;

using static UnityEngine.GUILayout;

namespace qASIC.Tools.Debug
{
    public class qASICDebug : EditorWindow
    {
        public Texture icon;

        [MenuItem("Window/qASIC/Debug")]
        static void Init()
        {
            qASICDebug window = (qASICDebug)GetWindow(typeof(qASICDebug), false, "qASIC Debug", true);
            window.titleContent.image = window.icon;
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

        enum DebugWindow { Info, Audio, Console, Other };
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

            BeginHorizontal();

            BeginVertical(menuStyle, Width(100), Height(Screen.height));
            if (Button("Info", window == DebugWindow.Info ? menuSelectedButtonStyle : menuButtonStyle)) window = DebugWindow.Info;
            if (Button("Audio", window == DebugWindow.Audio ? menuSelectedButtonStyle : menuButtonStyle)) window = DebugWindow.Audio;
            if (Button("Console", window == DebugWindow.Console ? menuSelectedButtonStyle : menuButtonStyle)) window = DebugWindow.Console;
            if (Button("Other", window == DebugWindow.Other ? menuSelectedButtonStyle : menuButtonStyle)) window = DebugWindow.Other;
            EndVertical();

            //Content
            BeginVertical(contentStyle);
            scroll = BeginScrollView(scroll);

            //Menus
            switch(window)
            {
                default:
                    DisplayInfo(headerStyle);
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

            EndScrollView();
            EndVertical();


            EndHorizontal();
        }

        void DisplayInfo(GUIStyle headerStyle)
        {
            GUIStyle tintedBox = new GUIStyle("Label")
            {
                margin = new RectOffset(),
                padding = new RectOffset(),
            };
            tintedBox.normal.background = tintedTexture;

            GUIStyle tittleStyle = new GUIStyle("Label")
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 48,
            };

            GUIStyle logoStyle = new GUIStyle()
            {
                margin = new RectOffset(4, 4, 4, 4),
            };

            GUIStyle linkStyle = new GUIStyle("Label")
            {
                alignment = TextAnchor.MiddleCenter,
                normal = new GUIStyleState()
                {
                    textColor = new Color(0f, 0.7f, 1f),
                },
            };
            linkStyle.hover = linkStyle.normal;

            BeginHorizontal();
            FlexibleSpace();
            BeginVertical(tintedBox);

            BeginHorizontal(logoStyle);
            Label(icon, Height(100), Width(100));
            Label($"qASIC v{Info.Version}", tittleStyle, Height(100));
            EndHorizontal();

            BeginHorizontal(tintedBox);
            if (Button("Website", linkStyle)) Application.OpenURL("https://qasictools.com");
            if (Button("Docs", linkStyle)) Application.OpenURL("https://docs.qasictools.com");
            if (Button("Github", linkStyle)) Application.OpenURL("https://github.com/DockFrankenstein/qASIC");
            EndHorizontal();

            EndVertical();
            FlexibleSpace();
            EndHorizontal();

            Label("Versions", headerStyle);

            BeginVertical(tintedBox);
            Label($"qASIC: {Info.Version}");
            Label($"Game Console: {Info.Version}");
            Label($"Info Displayer: {Info.Version}");
            Label($"Options System: {Info.Version}");
            Label($"Input Management: {Info.Version}");
            Label($"Audio Management: {Info.Version}");
            Label($"File Management: {Info.Version}");
            EndVertical();


            Label("Specs", headerStyle);

            BeginVertical(tintedBox);
            Label($"CPU: {SystemInfo.processorType}");
            Label($"CPU Threads: {SystemInfo.processorCount}");
            Label($"GPU: {SystemInfo.graphicsDeviceName}");
            Label($"RAM: {SystemInfo.systemMemorySize}MB");
            Label($"OS: {SystemInfo.operatingSystem}");
            Label($"Unity Engine: {Application.unityVersion}");
            EndVertical();
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
            
            Label("Audio", headerStyle);

            if (!AudioManagement.AudioManager.Enabled) return;

            if (AudioManagement.AudioManager.Singleton == null)
            {
                Label(EditorApplication.isPlaying ? "There is no Audio Manager in the scene" : "Offline", centeredLabelStyle);
                if (EditorApplication.isPlaying && Button("Generate Audio Manager")) new GameObject("Audio Manager").AddComponent<AudioManagement.AudioManager>();
                return;
            }

            Label($"Paused: {AudioManagement.AudioManager.Paused}");
            Label($"Channel count: {AudioManagement.AudioManager.ChannelCount}");

            audioScroll = BeginScrollView(audioScroll, scrollStyle, Height(128));
            switch (Application.isPlaying && AudioManagement.AudioManager.Singleton != null)
            {
                default:
                    break;
                case true:
                    int i = 0;
                    foreach (var channel in AudioManagement.AudioManager.Singleton.channels)
                    {
                        Label(channel.Key, i % 2 == 0 ? secondListItemStyle : listItemStyle, Height(EditorGUIUtility.singleLineHeight));
                        i++;
                    }
                    break;
            }
            EndScrollView();

            BeginHorizontal();
            if (Button("Pause All") && AudioManagement.AudioManager.Singleton != null) AudioManagement.AudioManager.PauseAll();
            if (Button("Un Pause All") && AudioManagement.AudioManager.Singleton != null) AudioManagement.AudioManager.UnPauseAll();
            if (Button("Stop All") && AudioManagement.AudioManager.Singleton != null) AudioManagement.AudioManager.StopAll();
            EndHorizontal();
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

            Label("Console", headerStyle);
            Label(Application.isPlaying ? GameConsoleController.LogsToString(9) : "Console is offline", consoleStyle, Height(128));
            BeginHorizontal();
            consoleInput = TextField(consoleInput);
            if (Button("Run", Width(32)) && Application.isPlaying)
            {
                GameConsoleController.RunCommand(consoleInput);
                consoleInput = string.Empty;
            }
            EndHorizontal();
        }

        void DisplayOther(GUIStyle headerStyle)
        {
            Label("Other", headerStyle);

            BeginHorizontal();
            if (Button("Unlock cursor")) Cursor.lockState = CursorLockMode.None;
            if (Button("Lock cursor")) Cursor.lockState = CursorLockMode.Locked;
            if (Button("Confine")) Cursor.lockState = CursorLockMode.Confined;
            EndHorizontal();

            BeginHorizontal();
            switch (resetPrefsWindow)
            {
                default:
                    if (Button("Reset Player Prefs")) resetPrefsWindow = true;
                    break;
                case true:
                    if (Button("Cancel")) resetPrefsWindow = false;
                    if (Button("Confirm"))
                    {
                        PlayerPrefs.DeleteAll();
                        resetPrefsWindow = false;
                    }
                    break;
            }
            EndHorizontal();
        }
    }
}
#endif