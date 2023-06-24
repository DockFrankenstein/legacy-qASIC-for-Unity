#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using qASIC.EditorTools;
using System.Linq;
using System;

using MapperAxis = qASIC.Input.UIM.UIMAxisMapperPlatform.ButtonMapping;

namespace qASIC.Input.UIM.Internal
{
    [CustomEditor(typeof(UIMAxisMapperPlatform))]
    public class UIMAxisMapperPlatformInspector : Editor, IHasCustomMenu
    {
        UIMAxisMapperPlatform script;
        GUIContent negativeLabelContent;
        GUIStyle wrappedLabelStyle;

        SerializedProperty p_axis;

        private static bool? _overrideRuntimeEditBlock = null;
        private const string overrideRuntimeEditBlockSessionStateName = "qASIC_UIM_Mapper_runtime_edit_override";

        public static bool OverrideRuntimeEditBlock
        {
            get
            {
                if (_overrideRuntimeEditBlock == null)
                    _overrideRuntimeEditBlock = SessionState.GetBool(overrideRuntimeEditBlockSessionStateName, false);

                return _overrideRuntimeEditBlock ?? false;
            }
            set
            {
                _overrideRuntimeEditBlock = value;
                SessionState.SetBool(overrideRuntimeEditBlockSessionStateName, value);
            }
        }

        private void OnEnable()
        {
            script = (UIMAxisMapperPlatform)target;
            negativeLabelContent = new GUIContent("negative");

            p_axis = serializedObject.FindProperty(nameof(UIMAxisMapperPlatform.axes));

            RegenerateArray();
        }

        public void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem("Regenerate array", false, RegenerateArray);
            menu.AddItem("Toggle runtime edit block override", OverrideRuntimeEditBlock, () => OverrideRuntimeEditBlock = !OverrideRuntimeEditBlock);
        }

        MapperAxis[] _axisButtons = new MapperAxis[]
        {
            new MapperAxis(GamepadButton.A, UIMAxisMapperPlatform.UIMGamepadKey.JoystickButton0),
            new MapperAxis(GamepadButton.B, UIMAxisMapperPlatform.UIMGamepadKey.JoystickButton1),
            new MapperAxis(GamepadButton.X, UIMAxisMapperPlatform.UIMGamepadKey.JoystickButton2),
            new MapperAxis(GamepadButton.Y, UIMAxisMapperPlatform.UIMGamepadKey.JoystickButton3),
            new MapperAxis(GamepadButton.LeftBumper, UIMAxisMapperPlatform.UIMGamepadKey.JoystickButton4),
            new MapperAxis(GamepadButton.RightBumper, UIMAxisMapperPlatform.UIMGamepadKey.JoystickButton5),
            new MapperAxis(GamepadButton.LeftTrigger, "gamepad_{0}_axis_8", false),
            new MapperAxis(GamepadButton.RightTrigger, "gamepad_{0}_axis_9", false),
            new MapperAxis(GamepadButton.LeftStickButton, UIMAxisMapperPlatform.UIMGamepadKey.JoystickButton8),
            new MapperAxis(GamepadButton.RightStickButton, UIMAxisMapperPlatform.UIMGamepadKey.JoystickButton9),
            new MapperAxis(GamepadButton.LeftStickUp, "gamepad_{0}_axis_1", true),
            new MapperAxis(GamepadButton.LeftStickRight, "gamepad_{0}_axis_0", false),
            new MapperAxis(GamepadButton.LeftStickDown, "gamepad_{0}_axis_1", false),
            new MapperAxis(GamepadButton.LeftStickLeft, "gamepad_{0}_axis_0", true),
            new MapperAxis(GamepadButton.RightStickUp, "gamepad_{0}_axis_4", true),
            new MapperAxis(GamepadButton.RightStickRight, "gamepad_{0}_axis_3", false),
            new MapperAxis(GamepadButton.RightStickDown, "gamepad_{0}_axis_4", false),
            new MapperAxis(GamepadButton.RightStickLeft, "gamepad_{0}_axis_3", true),
            new MapperAxis(GamepadButton.DPadUp, "gamepad_{0}_axis_6", false),
            new MapperAxis(GamepadButton.DPadRight, "gamepad_{0}_axis_5", false),
            new MapperAxis(GamepadButton.DPadDown, "gamepad_{0}_axis_6", true),
            new MapperAxis(GamepadButton.DPadLeft, "gamepad_{0}_axis_5", true),
            new MapperAxis(GamepadButton.Back, UIMAxisMapperPlatform.UIMGamepadKey.JoystickButton6),
            new MapperAxis(GamepadButton.Start, UIMAxisMapperPlatform.UIMGamepadKey.JoystickButton7),
        };

        public override void OnInspectorGUI()
        {
            if (wrappedLabelStyle == null)
            {
                wrappedLabelStyle = new GUIStyle(EditorStyles.label);
                wrappedLabelStyle.wordWrap = true;
            }

            bool isLocked = Application.isPlaying && !OverrideRuntimeEditBlock;

            if (isLocked)
            {
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.LabelField("Changed will not be applied until exiting the play mode.", wrappedLabelStyle);
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Change anyways"))
                            OverrideRuntimeEditBlock = true;
                    }
                }

                EditorGUILayout.Space();
            }

            using (new EditorGUI.DisabledScope(isLocked))
            {
                for (int i = 0; i < p_axis.arraySize; i++)
                {
                    using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
                    {
                        SerializedProperty item = p_axis.GetArrayElementAtIndex(i);
                        EditorGUILayout.PrefixLabel(((GamepadButton)item.FindPropertyRelative(nameof(MapperAxis.button)).intValue).ToString());

                        SerializedProperty typeProperty = item.FindPropertyRelative(nameof(MapperAxis.type));

                        EditorGUILayout.PropertyField(typeProperty, GUIContent.none, GUILayout.Width(60f));

                        switch ((UIMAxisMapperPlatform.UIMInputType)typeProperty.intValue)
                        {
                            case UIMAxisMapperPlatform.UIMInputType.Button:
                                EditorGUILayout.PropertyField(item.FindPropertyRelative(nameof(MapperAxis.uimKey)), GUIContent.none);
                                break;
                            case UIMAxisMapperPlatform.UIMInputType.Axis:
                                EditorGUILayout.PropertyField(item.FindPropertyRelative(nameof(MapperAxis.axisName)), GUIContent.none);

                                EditorStyles.label.CalcMinMaxWidth(negativeLabelContent, out float negativeLabelWidth, out _);
                                EditorGUILayout.LabelField("negative", GUILayout.Width(negativeLabelWidth));

                                EditorGUILayout.PropertyField(item.FindPropertyRelative(nameof(MapperAxis.negative)), GUIContent.none, GUILayout.Width(16f));
                                break;
                        }
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        void RegenerateArray()
        {
            UIMAxisMapperPlatform.ButtonMapping[] axes = script.axes;

            int axisCount = _axisButtons.Length;
            script.axes = new UIMAxisMapperPlatform.ButtonMapping[axisCount];

            for (int i = 0; i < axisCount; i++)
            {
                script.axes[i] = _axisButtons[i];

                UIMAxisMapperPlatform.ButtonMapping[] targetAxes = axes.Where(x => x.button == _axisButtons[i].button).ToArray();
                if (targetAxes.Length == 0)
                    continue;

                script.axes[i] = targetAxes[0];
            }
        }
    }
}
#endif