using UnityEngine;

namespace qASIC.Input.UIM
{
    [CreateAssetMenu(fileName = "NewUIMPlatformAxisMapper", menuName = "qASIC/Input/UIM Platform Axis Mapper", order = 21)]
    public class UIMAxisMapperPlatform : ScriptableObject
    {
        public ButtonMapping[] axes = new ButtonMapping[]
        {
            new ButtonMapping(GamepadButton.A, UIMGamepadKey.JoystickButton0),
            new ButtonMapping(GamepadButton.B, UIMGamepadKey.JoystickButton1),
            new ButtonMapping(GamepadButton.X, UIMGamepadKey.JoystickButton2),
            new ButtonMapping(GamepadButton.Y, UIMGamepadKey.JoystickButton3),
            new ButtonMapping(GamepadButton.LeftBumper, UIMGamepadKey.JoystickButton4),
            new ButtonMapping(GamepadButton.RightBumper, UIMGamepadKey.JoystickButton5),
            new ButtonMapping(GamepadButton.LeftTrigger, "gamepad_{0}_axis_8", false),
            new ButtonMapping(GamepadButton.RightTrigger, "gamepad_{0}_axis_9", false),
            new ButtonMapping(GamepadButton.LeftStickButton, UIMGamepadKey.JoystickButton8),
            new ButtonMapping(GamepadButton.RightStickButton, UIMGamepadKey.JoystickButton9),
            new ButtonMapping(GamepadButton.LeftStickUp, "gamepad_{0}_axis_1", true),
            new ButtonMapping(GamepadButton.LeftStickRight, "gamepad_{0}_axis_0", false),
            new ButtonMapping(GamepadButton.LeftStickDown, "gamepad_{0}_axis_1", false),
            new ButtonMapping(GamepadButton.LeftStickLeft, "gamepad_{0}_axis_0", true),
            new ButtonMapping(GamepadButton.RightStickUp, "gamepad_{0}_axis_4", true),
            new ButtonMapping(GamepadButton.RightStickRight, "gamepad_{0}_axis_3", false),
            new ButtonMapping(GamepadButton.RightStickDown, "gamepad_{0}_axis_4", false),
            new ButtonMapping(GamepadButton.RightStickLeft, "gamepad_{0}_axis_3", true),
            new ButtonMapping(GamepadButton.DPadUp, "gamepad_{0}_axis_6", false),
            new ButtonMapping(GamepadButton.DPadRight, "gamepad_{0}_axis_5", false),
            new ButtonMapping(GamepadButton.DPadDown, "gamepad_{0}_axis_6", true),
            new ButtonMapping(GamepadButton.DPadLeft, "gamepad_{0}_axis_5", true),
            new ButtonMapping(GamepadButton.Back, UIMGamepadKey.JoystickButton6),
            new ButtonMapping(GamepadButton.Start, UIMGamepadKey.JoystickButton7),
        };

        [System.Serializable]
        public struct ButtonMapping
        {
            public GamepadButton button;
            public UIMInputType type;

            //Axis
            public string axisName;
            public bool negative;

            //Button
            public UIMGamepadKey uimKey;

            public ButtonMapping(GamepadButton button, string axisName, bool negative)
            {
                this.button = button;
                type = UIMInputType.Axis;

                this.axisName = axisName;
                this.negative = negative;
                uimKey = default;
            }

            public ButtonMapping(GamepadButton button, UIMGamepadKey key)
            {
                this.button = button;
                type = UIMInputType.Button;

                axisName = string.Empty;
                negative = false;
                uimKey = key;
            }
        }

        public enum UIMInputType
        {
            Button,
            Axis,
        }

        public enum UIMGamepadKey
        {
            None = 0,
            JoystickButton0 = 350,
            JoystickButton1 = 351,
            JoystickButton2 = 352,
            JoystickButton3 = 353,
            JoystickButton4 = 354,
            JoystickButton5 = 355,
            JoystickButton6 = 356,
            JoystickButton7 = 357,
            JoystickButton8 = 358,
            JoystickButton9 = 359,
            JoystickButton10 = 360,
            JoystickButton11 = 361,
            JoystickButton12 = 362,
            JoystickButton13 = 363,
            JoystickButton14 = 364,
            JoystickButton15 = 365,
            JoystickButton16 = 366,
            JoystickButton17 = 367,
            JoystickButton18 = 368,
            JoystickButton19 = 369,
        }
    }
}
