using UnityEditor;
using UnityEngine;
using qASIC.Input.Devices;

using UInput = UnityEngine.Input;

namespace qASIC.Input.DebugTools
{
    public class InputDebug : EditorWindow
    {
        [MenuItem("Window/qASIC/Input/Input Debug")]
        public static InputDebug OpenWindow()
        {
            InputDebug window = GetEditorWindow();
            window.minSize = new Vector2(300f, 300f);
            window.Show();
            return window;
        }

        public static InputDebug GetEditorWindow() =>
            (InputDebug)GetWindow(typeof(InputDebug), false, "Input Debug");

        private void OnGUI()
        {
            if (GUILayout.Button("Reload Device Manager"))
                DeviceManager.Reload();

            EditorGUILayout.Space();
            GUILayout.Label("UIM Joysticks", EditorStyles.whiteLabel);
            EditorGUI.indentLevel++;
            string[] joystickNames = UInput.GetJoystickNames();
            for (int i = 0; i < joystickNames.Length; i++)
            {
                EditorGUILayout.LabelField(joystickNames[i]);
            }
            EditorGUI.indentLevel--;
        }
    }
}